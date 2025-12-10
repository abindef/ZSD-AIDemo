import './public-path';
import { run } from '@inno/inno-mc-vue3/template/main';
import { routes as routesTemplate } from '@inno/inno-mc-vue3/template/router';
import { routes } from './router';
import './style/index.scss';

window.USER_FUNCTION_URIS = ['/paymentAuto/All']
// 启动应用
run({
  appName: import.meta.env.VITE_APP_name,
  routes: [...routesTemplate, ...routes],
  InnoComponentConfig: {
    crud: {
      method: 'get',
      pageConfig: {
        // 分页参数按项目可以单独配置
        pageIndex: 'page',
        pageSize: 'limit'
      },
      resultKey: {
        // 后台返回字段key
        list: 'content',
        total: 'totalElements'
      }
    },
    file: {
      appId: 'admin',
      limitSize: 1024
    }
  }
});
