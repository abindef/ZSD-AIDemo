export interface Window {
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
