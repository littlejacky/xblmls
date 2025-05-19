using Android.App;
using Android.Content;
using Android.Nfc;
using Com.Igexin.Sdk;
using Com.Igexin.Sdk.Message;

namespace XBLMS.Maui
{
    /// <summary>
    /// 继承 GTIntentService 接收来自个推的消息，所有消息在主线程中回调，如果注册了该服务，则务必要在 AndroidManifest 中声明，否则无法接受消息
    /// </summary>
    [Service()]
    public class MauiIntentService : GTIntentService
    {
        public override void OnReceiveServicePid(Context? context, int pid)
        {
        }

        /**
        * 此方法用于接收和处理透传消息。透传消息个推只传递数据，不做任何处理，客户端接收到透传消息后需要自己去做后续动作处理，如通知栏展示、弹框等。
        * 如果开发者在客户端将透传消息创建了通知栏展示，建议将展示和点击回执上报给个推。
        */
        public override void OnReceiveMessageData(Context? context, Com.Igexin.Sdk.Message.GTTransmitMessage? msg)
        {
            byte[]? payload = msg?.GetPayload();
            String? data = Convert.ToString(payload);
            System.Diagnostics.Debug.WriteLine("receiver payload = " + data);//透传消息文本内容

            //taskid和messageid字段，是用于回执上报的必要参数。详情见下方文档“6.2 上报透传消息的展示和点击数据”
            String? taskid = msg?.TaskId;
            String? messageid = msg?.MessageId;

        }

        // 接收 cid
        public override void OnReceiveClientId(Context? context, String? clientid)
        {
            System.Diagnostics.Debug.WriteLine("onReceiveClientId -> " + "clientid = " + clientid);
        }

        // cid 离线上线通知
        public override void OnReceiveOnlineState(Context? context, Boolean online)
        {
        }

        // 各种事件处理回执
        public override void OnReceiveCommandResult(Context? context, GTCmdMessage? cmdMessage)
        {
        }

        // 通知到达，只有个推通道下发的通知会回调此方法
        public override void OnNotificationMessageArrived(Context? context, GTNotificationMessage? msg)
        {
        }

        // 通知点击，只有个推通道下发的通知会回调此方法
        public override void OnNotificationMessageClicked(Context? context, GTNotificationMessage? msg)
        {
        }
    }
}
