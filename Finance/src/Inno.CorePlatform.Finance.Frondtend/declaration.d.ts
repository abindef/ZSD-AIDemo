// declare module '@inno/inno-mc';

declare module '@inno/inno-mc-vue3/lib/components/noRoute';
declare module '@inno/inno-mc-vue3/lib/components/redirect';
declare module '@inno/inno-mc-vue3/lib/crud';
declare module '@inno/inno-mc-vue3/lib/utils/filters';
declare module 'element-plus/dist/locale/zh-cn.mjs';

declare module '*.module.css';

declare module '*.module.scss';

declare module '*.vue' {
  import { defineComponent } from 'vue';
  const component: ReturnType<typeof defineComponent>;
  export default component;
}

interface Window {
  __MICRO_APP_BASE_ROUTE__: string;
  __MICRO_APP_ENVIRONMENT__: string;
  __MICRO_APP_PUBLIC_PATH__: string;
  __MICRO_APP_NAME__: string;
  gatewayUrl: string;
  backendUrl: string;
  token: any;
  isOa: any;
  rawWindow: any;
  microApp: any;
  gatewayUrl: any;
  [key: string]: any;
  // existLoading: boolean
  // lazy: NodeJS.Timer
  // unique: number
  // tokenRefreshing: boolean
  // requests: Function[]
  // eventSource: EventSource
}

// 添加 JSX 命名空间声明
namespace JSX {
  interface IntrinsicElements {
    [elemName: string]: any;
  }
  interface ElementChildrenAttribute {
    children: {};
  }
}

// 添加 Fragment 类型
declare global {
  namespace JSX {
    interface IntrinsicElements {
      fragment: any;
    }
  }
}