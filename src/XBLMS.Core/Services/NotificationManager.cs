using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Models;
using XBLMS.Services;
using static XBLMS.Core.Utils.UnipushManager;
using Config = XBLMS.Core.Utils.UnipushManager.Config;

namespace XBLMS.Core.Services
{
    public partial class NotificationManager : INotificationManager
    {
        private readonly UnipushManager _pusher = null;

        public NotificationManager(Config config)
        {
            //_pusher = new UnipushManager(config);
        }

        public async Task SendExamPaperArrangedNotificationAsync(User user, ExamPaper paper)
        {
            await SendNotificationAsync(user.Cid, new MessageModel
            {
                Title = "单独推送标题-来自C#",
                Content = "这是一条来自C#发送的测试消息，单独推送给123456",
                ClickType = ClickType.payload,
                ClickObj = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                })
            });
        }

        public async Task SendExamPracticeArrangedNotificationAsync(User user, ExamPractice practice)
        {
            await SendNotificationAsync(user.Cid, new MessageModel
            {
                Title = "单独推送标题-来自C#",
                Content = "这是一条来自C#发送的测试消息，单独推送给123456",
                ClickType = ClickType.payload,
                ClickObj = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                })
            });
        }

        public async Task SendNotificationAsync(string cid, MessageModel message)
        {
            if (_pusher == null) return;
            // 单独推送给cid为123456的用户一条消息，点击后会调用系统浏览器打开https://www.baidu.com
            await _pusher.SinglePush(cid, message);
        }
    }
}
