import { ElMessage } from 'element-plus';

export default {
  mounted(el, binding) {
    const tableWrapper = el;

    let selectionStart = null;
    let selectionEnd = null;
    let isSelecting = false;

    // 创建高亮层
    const overlay = document.createElement('div');
    overlay.className = 'excel-selection-overlay';
    tableWrapper.appendChild(overlay);

    // 创建右键菜单
    const contextMenu = document.createElement('div');
    contextMenu.className = 'excel-context-menu';
    contextMenu.style.display = 'none';
    contextMenu.innerHTML = `
      <ul>
        <li data-action="copy-display">复制显示文字</li>
        <li data-action="copy-raw">复制原始字段值</li>
      </ul>
    `;
    document.body.appendChild(contextMenu);

    // 当前选中的单元格信息
    let currentCell = null;

    // 获取表格列的 prop（优先使用 store，其次尝试从 DOM 提取）
    function getColumnsProp() {
      const tableVm = el.__vueParentComponent?.ctx; // 获取组件上下文
      const columnInstances = tableVm?.store?.states?._columns?.value;
      if (Array.isArray(columnInstances)) {
        return columnInstances
          .map((col) => col.property || col.rawColumnKey || col.$attrs?.prop)
          .filter(Boolean);
      }

      // 如果失败，则从 DOM 表头提取 data-prop
      const headerCells = tableWrapper.querySelectorAll('.el-table__header th');
      return Array.from(headerCells)
        .map((th) => th.getAttribute('data-prop'))
        .filter(Boolean);
    }

    // 获取所有单元格信息
    function getTableCells() {
      const rows = tableWrapper.querySelectorAll('.el-table__body tr');
      const cells = [];

      rows.forEach((row, rowIndex) => {
        const cols = row.querySelectorAll('td');
        cols.forEach((cell, colIndex) => {
          cells.push({
            element: cell,
            rowIndex,
            colIndex,
            rect: cell.getBoundingClientRect()
          });
        });
      });

      return cells;
    }

    // 获取原始字段值（支持 el-input、el-select 等 v-model 绑定的值）
    function getRawValue(rowIndex, colIndex, props) {
      const tableVm = el.__vueParentComponent?.ctx;
      const rowData = tableVm.data[rowIndex];
      const prop = props[colIndex];
      // 优先取 rowData[prop]
      let value = rowData && prop ? rowData[prop] : '';
      // 如果单元格内有 el-input 或 el-select，尝试获取其 v-model 绑定值

      if (!value) {
        const cell = tableWrapper.querySelector(
          `.el-table__body tr:nth-child(${rowIndex + 1}) td:nth-child(${colIndex + 1})`
        );
        if (!cell) return '';
        const inputEl = cell.querySelector('.el-input__inner');
        if (inputEl) {
          value = inputEl.value;
        }
        const selectEl = cell.querySelector('.el-select__input');
        if (selectEl) {
          value = selectEl.value;
        }
      }
      return value;
    }

    // 鼠标按下开始选择
    function onMouseDown(e) {
      // 排除右键点击
      if (e.button === 2) return;

      const target = e.target.closest('td');
      if (!target) return;

      const cells = getTableCells();
      const clickedCell = cells.find((c) => c.element === target);

      if (clickedCell) {
        isSelecting = true;
        selectionStart = clickedCell;
        selectionEnd = clickedCell;
        updateOverlay();
        tableWrapper.classList.remove('excel-selection-dragging');
      }
    }

    // 鼠标移动更新范围
    function onMouseMove(e) {
      if (!isSelecting) return;

      const target = e.target.closest('td');
      if (!target) return;

      const cells = getTableCells();
      const currentCell = cells.find((c) => c.element === target);

      if (currentCell) {
        selectionEnd = currentCell;
        updateOverlay();
      }
    }

    // 鼠标释放结束选择
    function onMouseUp() {
      isSelecting = false;
      tableWrapper.classList.remove('excel-selection-dragging');
    }

    // 更新高亮框位置和大小
    function updateOverlay() {
      if (!selectionStart || !selectionEnd) return;

      const start = selectionStart;
      const end = selectionEnd;

      const topRow = Math.min(start.rowIndex, end.rowIndex);
      const bottomRow = Math.max(start.rowIndex, end.rowIndex);
      const leftCol = Math.min(start.colIndex, end.colIndex);
      const rightCol = Math.max(start.colIndex, end.colIndex);

      const first = getTableCells().find(
        (c) => c.rowIndex === topRow && c.colIndex === leftCol
      );
      const last = getTableCells().find(
        (c) => c.rowIndex === bottomRow && c.colIndex === rightCol
      );

      if (!first || !last) return;

      // 动态判断 overlay 的定位方式
      let top, left;
      if (getComputedStyle(overlay).position === 'absolute') {
        const wrapperRect = tableWrapper.getBoundingClientRect();
        top = first.rect.top - wrapperRect.top;
        left = first.rect.left - wrapperRect.left;
      } else {
        top = first.rect.top + window.scrollY;
        left = first.rect.left + window.scrollX;
      }
      const width = last.rect.right - first.rect.left;
      const height = last.rect.bottom - first.rect.top;

      overlay.style.top = `${top}px`;
      overlay.style.left = `${left}px`;
      overlay.style.width = `${width}px`;
      overlay.style.height = `${height}px`;
      overlay.style.display = 'block';
    }

    // 通用复制选区内容方法
    function copySelectionToClipboard({ type = 'raw' } = {}) {
      if (!selectionStart || !selectionEnd) return;
      const props = getColumnsProp();
      const startRow = Math.min(selectionStart.rowIndex, selectionEnd.rowIndex);
      const endRow = Math.max(selectionStart.rowIndex, selectionEnd.rowIndex);
      const startCol = Math.min(selectionStart.colIndex, selectionEnd.colIndex);
      const endCol = Math.max(selectionStart.colIndex, selectionEnd.colIndex);
      const copiedText = [];
      for (let r = startRow; r <= endRow; r++) {
        const rowValues = [];
        for (let c = startCol; c <= endCol; c++) {
          if (type === 'display') {
            const cell = tableWrapper.querySelector(
              `.el-table__body tr:nth-child(${r + 1}) td:nth-child(${c + 1})`
            );
            let value = cell ? cell.innerText.trim() : '';

            rowValues.push(value || getRawValue(r, c, props));
          } else {
            const value = getRawValue(r, c, props);
            rowValues.push(value);
          }
        }
        copiedText.push(rowValues.join('\t'));
      }
      const text = copiedText.join('\n');
      navigator.clipboard
        .writeText(text)
        .then(() => ElMessage.success('已复制到剪贴板'))
        .catch(() => ElMessage.error('复制失败，请手动复制'));
    }

    // Ctrl+C 复制事件
    function handleCopy(e) {
      if (!e.ctrlKey || e.key !== 'c') return;
      if (!selectionStart || !selectionEnd) return;
      copySelectionToClipboard({ type: 'raw' });
      e.preventDefault();
    }

    // 右键菜单事件
    function onContextMenu(e) {
      const target = e.target.closest('td');
      if (!target) return;

      const cells = getTableCells();
      const clickedCell = cells.find((c) => c.element === target);
      if (!clickedCell) return;

      currentCell = clickedCell;
      // const props = getColumnsProp();

      // const displayText = target.innerText.trim();
      // const rawValue = getRawValue(clickedCell.rowIndex, clickedCell.colIndex, props);

      e.preventDefault();

      const x = e.clientX;
      const y = e.clientY;

      contextMenu.style.left = `${x}px`;
      contextMenu.style.top = `${y}px`;
      contextMenu.style.display = 'block';

      // // 存储要复制的数据
      // contextMenu.dataset.displayText = displayText;
      // contextMenu.dataset.rawValue = rawValue;
    }

    // 菜单项点击处理
    function onMenuItemClick(e) {
      const target = e.target.closest('li');
      if (!target) return;

      const action = target.getAttribute('data-action');

      // 如果没有选中区域，则直接返回
      if (!selectionStart || !selectionEnd) {
        ElMessage.warning('请先选择单元格区域');
        return;
      }

      if (action === 'copy-display') {
        copySelectionToClipboard({ type: 'display' });
      } else if (action === 'copy-raw') {
        copySelectionToClipboard({ type: 'raw' });
      }

      contextMenu.style.display = 'none';
    }

    // 点击其他地方关闭菜单
    function onClickOutside(e) {
      if (!contextMenu.contains(e.target)) {
        contextMenu.style.display = 'none';
      }
    }

    // 禁用默认文字选中
    function disableSelection(e) {
      e.preventDefault();
    }

    // 绑定事件
    tableWrapper.addEventListener('mousedown', onMouseDown);
    tableWrapper.addEventListener('mousemove', onMouseMove);
    window.addEventListener('mouseup', onMouseUp);
    window.addEventListener('keydown', handleCopy);
    tableWrapper.addEventListener('selectstart', disableSelection);
    // 监听 copy 事件，覆盖默认复制行为
    // tableWrapper.addEventListener('copy', function (event) {
    //     if (!selectionStart || !selectionEnd) return;
    //     const props = getColumnsProp();
    //     const startRow = Math.min(selectionStart.rowIndex, selectionEnd.rowIndex);
    //     const endRow = Math.max(selectionStart.rowIndex, selectionEnd.rowIndex);
    //     const startCol = Math.min(selectionStart.colIndex, selectionEnd.colIndex);
    //     const endCol = Math.max(selectionStart.colIndex, selectionEnd.colIndex);

    //     const copiedText = [];
    //     for (let r = startRow; r <= endRow; r++) {
    //         const rowValues = [];
    //         for (let c = startCol; c <= endCol; c++) {
    //             const value = getRawValue(r, c, props);
    //             rowValues.push(value);
    //         }
    //         copiedText.push(rowValues.join('\t'));
    //     }
    //     event.clipboardData.setData('text/plain', copiedText.join('\n'));
    //     event.preventDefault();
    // });
    tableWrapper.addEventListener('contextmenu', onContextMenu);
    contextMenu.addEventListener('click', onMenuItemClick);
    document.addEventListener('click', onClickOutside);

    // ✅ 使用 __vueParentComponent 来绑定 beforeUnmount 生命周期
    const instance = el.__vueParentComponent;
    if (
      instance &&
      instance.proxy &&
      typeof instance.proxy.$onBeforeUnmount === 'function'
    ) {
      instance.proxy.$onBeforeUnmount(() => {
        if (typeof instance.hook === 'function') {
          instance.hook();
          delete instance.hook;
        }
      });
    }
  }
};
