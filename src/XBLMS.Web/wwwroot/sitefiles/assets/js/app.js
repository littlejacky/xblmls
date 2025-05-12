var ACCESS_TOKEN_NAME = "xblms_user_access_token";
var $apiUrl = "/api/home";
var $rootUrl = "/app";

window.handleNotificationData = function (jsonDataString) {
  // jsonDataString 是一个包含通知数据的 JSON 字符串
  console.log("Received notification data in H5:", jsonDataString);
  try {
    const notificationData = JSON.parse(jsonDataString);
    // 在这里处理解析后的 notificationData 对象
    // 例如，根据数据显示通知内容，或者导航到特定页面等
    alert("收到推送消息: " + (notificationData.title || "无标题") + "\n内容: " + (notificationData.body || "无内容"));

    // 示例：如果数据中包含 'url' 字段，则尝试在 H5 内部跳转
    if (notificationData.url) {
      // window.location.href = notificationData.url; // 直接跳转
      // 或者更复杂的应用内路由逻辑
      console.log("Notification requests navigation to: ", notificationData.url);
    }

  } catch (error) {
    console.error("Error parsing notification data in H5:", error);
  }
};
