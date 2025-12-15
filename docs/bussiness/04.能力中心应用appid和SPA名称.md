## 容器应用[](https://ado.innostic.com/dc/CorePlatform/_wiki/wikis/CorePlatform.wiki/267/%E8%83%BD%E5%8A%9B%E4%B8%AD%E5%BF%83%E5%BA%94%E7%94%A8appid%E5%92%8CSPA%E5%90%8D%E7%A7%B0?anchor=%E5%AE%B9%E5%99%A8%E5%BA%94%E7%94%A8)

|应用|DaprAppId|ContainerAppName|ImageName|本地端口|备注|
|---|---|---|---|---|---|
|用户门户|user-portal|user-portal|user-portal|6110|
|用户中心Backend|uc-backend|uc-backend|uc-backend|6111|
|用户中心Webapi|uc-webapi|uc-webapi|uc-webapi|6112|
|主数据服务|mds|mds|mds|7110|
|权限中心Backend|pc-backend|pc-backend|pc-backend|6113|
|权限中心Webapi|pc-webapi|pc-webapi|pc-webapi|6114|
|首营中心Backend|fm-backend|fm-backend|fm-backend|6115|
|首营中心Webapi|fm-webapi|fm-webapi|fm-webapi|6116|
|首营中心ACL(java acl)|jacl-backend|jacl-backend|jacl-backend|7112|
|基础数据服务Backend|bds-backend|bds-backend|bds-backend|6117/36117|
|基础数据服务Webapi|bds-webapi|bds-webapi|bds-webapi|6118/36118|
|项目管理Backend|pm-backend|pm-backend|pm-backend|6119/35119|
|项目管理Webapi|pm-webapi|pm-webapi|pm-webapi|6200/35200|
|单号生成服务|cgs|cgs|cgs|7111/36111|
|api网关|api-gateway|api-gateway|api-gateway|6100/35100|
|api文档|api-docs|api-docs|api-docs|6101/35101|
|网关背景程序|gateway-daemon|gateway-daemon|gateway-daemon|6102/35102|
|管理后台|system-admin|system-admin|system-admin|7200/36200|
|测试应用|test-client|test-client|test-client|7300/36300|
|入库申请 Backend|sia-backend|sia-backend|sia-backend|6201/35120|
|入库申请 Webapi|sia-webapi|sia-webapi|sia-webapi|6202/35121|
|销售管理 Backend|sell-backend|sell-backend|sell-backend|6203/35122|
|销售管理 Webapi|sell-webapi|sell-webapi|sell-webapi|6204/35123|
|采购管理 Backend|purchase-backend|purchase-backend|purchase-backend|6205/35124|
|采购管理 Webapi|purchase-webapi|purchase-webapi|purchase-webapi|6206/35125|
|出库申请 Backend|soa-backend|soa-backend|soa-backend|6207/35126|
|出库申请 Webapi|soa-webapi|soa-webapi|soa-webapi|6208/35127|
|订货系统 Backend|ordering-backend|ordering-backend|ordering-backend|6209/35128|
|订货系统 Webapi|ordering-webapi|ordering-webapi|ordering-webapi|6210/35129|
|财务管理 Backend|finance-backend|finance-backend|finance-backend|6211/35130|
|财务管理 Webapi|finance-webapi|finance-webapi|finance-webapi|6212/35131|
|库存管理 Backend|inventory-backend|inventory-backend|inventory-backend|6213/35132|
|库存管理 Webapi|inventory-webapi|inventory-webapi|inventory-webapi|6214/35133|
|物流管理 Backend|logistics-backend|logistics-backend|logistics-backend|6215/35133|
|物流管理 Webapi|logistics-webapi|logistics-webapi|logistics-webapi|6216/35134|
|后台任务调度|background-job|background-job|background-job|6217/35135|
|集成中心 Backend|ic-backend|ic-backend|ic-backend|6218/35136|
|集成中心 Webapi|ic-webapi|ic-webapi|ic-webapi|6219/35137|
|采购子系统 Backend|puc-backend|puc-backend|puc-backend|6220/35138|
|采购子系统 Webapi|puc-webapi|puc-webapi|puc-webapi|6221/35139|
|系统后台管理 Backend|system-backend|system-backend|system-backend|6222/35140|
|待办消息中心 webapi|todo-center|todo-center|todo-center|6223/35141|

**注意**

在每个Dapr应用的根文件夹里，新建一个rundapr.ps1的文件，放入如下类似的脚本：

```
dapr run --app-id fm-backend --app-port 6115 --dapr-http-port 35115 -- java -jar target/first-market-service-0.0.1-SNAPSHOT.jar com.innostic.FirstMarketApplication
```

或者把以上说明放到README.md里面

|应用|ChildPath|备注|
|---|---|---|
|用户中心|uc|
|权限中心|pc|
|首营中心|fm|
|基础数据服务|bds|
||
||