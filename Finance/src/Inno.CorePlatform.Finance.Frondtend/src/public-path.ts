import { Window } from '../types/global';

declare let window: Window;
let baseUrl = '';
if (window.__MICRO_APP_ENVIRONMENT__) {
  window.__webpack_public_path__ = window.__MICRO_APP_PUBLIC_PATH__;
  window.isOa = window.rawWindow.isOa;
  baseUrl = window.microApp.getData().baseUrl;
}
if (import.meta.env.MODE === 'development') {
  window.backendUrl =
    import.meta.env.VITE_APP_BACKEND_URL ||
    `${baseUrl}${import.meta.env.VITE_APP_BASE_URL}`; // 每个子应用修改为自己的后缀
  window.gatewayUrl = import.meta.env.VITE_APP_GATEWAY_URL || `${baseUrl}/`;
} else {
  window.backendUrl = `${baseUrl}${import.meta.env.VITE_APP_BASE_URL}`;
  window.gatewayUrl = `${baseUrl}/`;
}
