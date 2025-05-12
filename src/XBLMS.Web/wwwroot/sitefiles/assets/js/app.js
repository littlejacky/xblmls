var ACCESS_TOKEN_NAME = "xblms_user_access_token";
var $apiUrl = "/api/home";
var $rootUrl = "/app";

window.handleNotificationData = function (jsonDataString) {
  // jsonDataString ��һ������֪ͨ���ݵ� JSON �ַ���
  console.log("Received notification data in H5:", jsonDataString);
  try {
    const notificationData = JSON.parse(jsonDataString);
    // �����ﴦ�������� notificationData ����
    // ���磬����������ʾ֪ͨ���ݣ����ߵ������ض�ҳ���
    alert("�յ�������Ϣ: " + (notificationData.title || "�ޱ���") + "\n����: " + (notificationData.body || "������"));

    // ʾ������������а��� 'url' �ֶΣ������� H5 �ڲ���ת
    if (notificationData.url) {
      // window.location.href = notificationData.url; // ֱ����ת
      // ���߸����ӵ�Ӧ����·���߼�
      console.log("Notification requests navigation to: ", notificationData.url);
    }

  } catch (error) {
    console.error("Error parsing notification data in H5:", error);
  }
};
