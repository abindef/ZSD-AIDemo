<template>
  <div class="app-page-container">
    <div class="app-page-header">
      <el-breadcrumb separator-class="el-icon-arrow-right">
        <el-breadcrumb-item to="/home">首页</el-breadcrumb-item>
        <el-breadcrumb-item>{{ $route.meta.name }}</el-breadcrumb-item>
      </el-breadcrumb>
      <div class="flex-1" />
    </div>
    <div class="app-page-body" style="padding-top: 0">
      <inno-split-pane
        :min-percent="20"
        :default-percent="30"
        split="vertical"
        @resize="resize"
      >
        <template #paneL="{ size, full, onFull }">
          A{{ size }}
          <inno-svg-Icon
            :icon-class="full === 'paneL' ? 'restore' : 'enlarge'"
            class="icon"
            @click="onFull"
          />
          {{ full }}
        </template>
        <template #paneR>
          <inno-split-pane split="horizontal">
            <template #paneL="{ size, full, onFull }">
              B {{ size }}

              <inno-svg-Icon
                :icon-class="full === 'paneL' ? 'restore' : 'enlarge'"
                class="icon"
                @click="onFull"
              />
              {{ full }}
            </template>
            <template #paneR="{ size, full, onFull }">
              <inno-svg-Icon
                class="icon"
                :icon-class="full === 'paneR' ? 'restore' : 'enlarge'"
                @click="onFull"
              />
              C{{ size }}
              {{ full }}
            </template>
          </inno-split-pane>
        </template>
      </inno-split-pane>
    </div>
  </div>
</template>
<script lang="ts" setup>
const resize = (e) => {
  console.log(e);
};
</script>
