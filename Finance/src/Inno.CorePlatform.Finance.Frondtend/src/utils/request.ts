import '../public-path';
import HttpRequest from '@inno/inno-mc-vue3/template/utils/HttpRequest';

const httpRequest = new HttpRequest({
  baseURL: window.backendUrl, //import.meta.env.BASE_URL,
  timeout: import.meta.env.VITE_APP_TIME_OUT,
  interceptors: {
    // 请求前拦截
    requestInterceptor: (config) => {
      // console.log('请求成功1');
      return config;
    },
    // 请求失败拦截
    requestInterceptorCatch: (err) => {
      // console.log('请求失败')
      return Promise.reject(err);
    },
    // 响应成功拦截
    responseInterceptor: (res) => {
      // console.log('响应成功')
      return res;
    },
    // 响应失败拦截
    responseInterceptorCatch: (error) => {
      // console.log('响应失败')
      return Promise.reject(error);
    }
  }
});
export default httpRequest.instance;
