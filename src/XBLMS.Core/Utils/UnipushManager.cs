using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XBLMS.Core.Utils
{
    public class UnipushManager
    {
        /// <summary>
        /// 配置
        /// </summary>
        private Config _Config;

        /// <summary>
        /// 接口前缀
        /// </summary>
        private string _ApiBaseUrl = "";

        public UnipushManager(Config config)
        {
            if (string.IsNullOrEmpty(config.AppId))
            {
                throw new ArgumentNullException("AppId不能为空");
            }
            if (string.IsNullOrEmpty(config.AppKey))
            {
                throw new ArgumentNullException("AppKey不能为空");
            }
            if (string.IsNullOrEmpty(config.AppSecret))
            {
                throw new ArgumentNullException("AppSecret不能为空");
            }
            if (string.IsNullOrEmpty(config.MasterSecret))
            {
                throw new ArgumentNullException("MasterSecret不能为空");
            }
            _Config = config;
            _ApiBaseUrl = $"https://restapi.getui.com/v2/{config.AppId}";
        }

        #region 私有方法

        /// <summary>
        /// 获取32位guid
        /// </summary>
        /// <returns></returns>
        private string GetGuid() => Guid.NewGuid().ToString().Replace("-", "");

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string SHA256EncryptString(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.Create().ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        private async Task<BaseResponse<TokenResponse>> GetToken()
        {
            var url = $"{_ApiBaseUrl}/auth";
            var ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var sign = SHA256EncryptString($"{_Config.AppKey}{ts}{_Config.MasterSecret}");
            var body = new TokenRequest { AppKey = _Config.AppKey, Timestamp = ts.ToString(), Sign = sign };
            var result = await HttpTool.Post<BaseResponse<TokenResponse>>(url, JsonConvert.SerializeObject(body));
            return result;
        }

        /// <summary>
        /// 创建消息
        /// </summary>
        /// <returns></returns>
        private async Task<BaseResponse<CreateMessageResponse>> CreateMessage(MessageModel msg)
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token.Data.Token))
            {
                return new BaseResponse<CreateMessageResponse>
                {
                    Code = token.Code,
                    Msg = token.Msg,
                };
            }

            var url = $"{_ApiBaseUrl}/auth";

            var request = new CreateMessageRequest
            {
                RequestId = GetGuid(),
                PushMessage = new PushMessageModel
                {
                    Notification = new Notification
                    {
                        Title = msg.Title,
                        Body = msg.Content,
                        ClickType = msg.ClickType.ToString(),
                    }
                }
            };

            switch (msg.ClickType)
            {
                case ClickType.intent:
                    request.PushMessage.Notification.Intent = msg.ClickObj;
                    break;
                case ClickType.url:
                    request.PushMessage.Notification.Url = msg.ClickObj;
                    break;
                case ClickType.payload:
                    request.PushMessage.Notification.Payload = msg.ClickObj;
                    break;
                case ClickType.payload_custom:
                    request.PushMessage.Notification.Payload = msg.ClickObj;
                    break;
                default:
                    break;
            }

            var result = await HttpTool.Post<BaseResponse<CreateMessageResponse>>(url,
                                                                                  JsonConvert.SerializeObject(request),
                                                                                  new Dictionary<string, string> { { "token", token.Data.Token } });
            return result;
        }

        #endregion

        /// <summary>
        /// 全体推送
        /// </summary>
        /// <param name="msg">消息内容，注意content不能重复，否则会推送失败</param>
        /// <param name="settings">推送设置</param>
        /// <returns></returns>
        public async Task<BaseResponse<PushResponse>> AllPush(MessageModel msg, SettingsModel settings = null)
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token.Data.Token))
            {
                return new BaseResponse<PushResponse>
                {
                    Code = token.Code,
                    Msg = token.Msg,
                };
            }

            var request = new AllPushRequest
            {
                RequestId = GetGuid(),
                PushMessage = new PushMessageModel
                {
                    Notification = new Notification
                    {
                        Title = msg.Title,
                        Body = msg.Content,
                        ClickType = msg.ClickType.ToString(),
                    }
                }
            };
            switch (msg.ClickType)
            {
                case ClickType.intent:
                    request.PushMessage.Notification.Intent = msg.ClickObj;
                    break;
                case ClickType.url:
                    request.PushMessage.Notification.Url = msg.ClickObj;
                    break;
                case ClickType.payload:
                    request.PushMessage.Notification.Payload = msg.ClickObj;
                    break;
                case ClickType.payload_custom:
                    request.PushMessage.Notification.Payload = msg.ClickObj;
                    break;
                default:
                    break;
            }
            if (settings != null)
            {
                request.Settings = settings;
            }

            var url = $"{_ApiBaseUrl}/push/all";
            var result = await HttpTool.Post<BaseResponse<PushResponse>>(url,
                                                                         JsonConvert.SerializeObject(request),
                                                                         new Dictionary<string, string> { { "token", token.Data.Token } });
            return result;
        }

        /// <summary>
        /// 推送单一消息
        /// </summary>
        /// <param name="cid">指定用户的cid</param>
        /// <param name="msg">消息内容，注意content不能重复，否则会推送失败</param>
        /// <param name="settings">推送设置</param>
        /// <returns></returns>
        public async Task<BaseResponse<PushResponse>> SinglePush(string cid, MessageModel msg, SettingsModel settings = null)
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token.Data.Token))
            {

                return new BaseResponse<PushResponse>
                {
                    Code = token.Code,
                    Msg = token.Msg,
                };
            }

            var request = new SinglePushRequest
            {
                RequestId = GetGuid(),
                Audience = new AudienceModel { CId = new List<string> { cid } },
                PushMessage = new PushMessageModel
                {
                    Notification = new Notification
                    {
                        Title = msg.Title,
                        Body = msg.Content,
                        ClickType = msg.ClickType.ToString(),
                    }
                }
            };
            switch (msg.ClickType)
            {
                case ClickType.intent:
                    request.PushMessage.Notification.Intent = msg.ClickObj;
                    break;
                case ClickType.url:
                    request.PushMessage.Notification.Url = msg.ClickObj;
                    break;
                case ClickType.payload:
                    request.PushMessage.Notification.Payload = msg.ClickObj;
                    break;
                case ClickType.payload_custom:
                    request.PushMessage.Notification.Payload = msg.ClickObj;
                    break;
                default:
                    break;
            }
            if (settings != null)
            {
                request.Settings = settings;
            }

            var url = $"{_ApiBaseUrl}/push/single/cid";
            var result = await HttpTool.Post<BaseResponse<PushResponse>>(url,
                                                                         JsonConvert.SerializeObject(request),
                                                                         new Dictionary<string, string> { { "token", token.Data.Token } });
            return result;
        }

        /// <summary>
        /// 批量推送
        /// </summary>
        /// <param name="cids">批量推送用户的cid</param>
        /// <param name="msg">消息内容，注意content不能重复，否则会推送失败</param>
        /// <param name="isAsync">是否异步推送，如果异步推送的话无法直接获取推送结果</param>
        /// <param name="settings">推送设置</param>
        /// <returns></returns>
        public async Task<BaseResponse<PushResponse>> BatchPush(List<string> cids, MessageModel msg, bool isAsync = false, SettingsModel settings = null)
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token.Data.Token))
            {

                return new BaseResponse<PushResponse>
                {
                    Code = token.Code,
                    Msg = token.Msg,
                };
            }

            var msgResult = await CreateMessage(msg);
            if (msgResult == null || string.IsNullOrEmpty(msgResult.Data.TaskId))
            {
                return null;
            }

            var request = new BatchPushRequest
            {
                RequestId = GetGuid(),
                Audience = new AudienceModel { CId = cids },
                TaskId = msgResult.Data.TaskId,
                IsAsync = false, // 如果异步推送的话无法直接获取推送结果
            };
            if (settings != null)
            {
                request.Settings = settings;
            }

            var url = $"{_ApiBaseUrl}/push/list/cid";
            var result = await HttpTool.Post<BaseResponse<PushResponse>>(url,
                                                                         JsonConvert.SerializeObject(request),
                                                                         new Dictionary<string, string> { { "token", token.Data.Token } });
            return result;
        }

        /// <summary>
        /// UniPush应用配置
        /// </summary>
        public class Config
        {
            public string AppId { get; set; }
            public string AppKey { get; set; }
            public string AppSecret { get; set; }
            public string MasterSecret { get; set; }
        }

        /// <summary>
        /// 点击消息后的操作类型
        /// </summary>
        public enum ClickType
        {
            /// <summary>
            /// 打开应用内特定页面
            /// </summary>
            intent,

            /// <summary>
            /// 打开网页地址
            /// </summary>
            url,

            /// <summary>
            /// 自定义消息内容启动应用
            /// </summary>
            payload,

            /// <summary>
            /// 自定义消息内容不启动应用
            /// </summary>
            payload_custom,

            /// <summary>
            /// 打开应用首页
            /// </summary>
            startapp,

            /// <summary>
            /// 纯通知，无后续动作
            /// </summary>
            none,
        }

        /// <summary>
        /// Http请求类
        /// </summary>
        internal static class HttpTool
        {
            private static string _ContentType = "application/json";
            private static string _Charset = "utf-8";

            /// <summary>
            /// post请求
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="url"></param>
            /// <param name="json"></param>
            /// <param name="headers"></param>
            /// <returns></returns>
            public static async Task<T> Post<T>(string url, string json, Dictionary<string, string> headers = null)
            {
                using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) })
                {
                    var content = new StringContent(json, Encoding.GetEncoding(_Charset)) as HttpContent;
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_ContentType);

                    if (headers != null)
                    {
                        foreach (var kv in headers)
                        {
                            content.Headers.Add(kv.Key, kv.Value);
                        }
                    }

                    var resopense = await client.PostAsync(url, content);
                    var resultStr = await resopense.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(resultStr);

                    return result;
                }
            }
        }

        /// <summary>
        /// 推送目标用户
        /// </summary>
        public class AudienceModel
        {
            /// <summary>
            /// cid数组，推送单一消息时只能填一个cid
            /// </summary>
            [JsonProperty("cid")]
            public List<string> CId { get; set; }
        }

        /// <summary>
        /// 推送消息的Model
        /// </summary>
        public class MessageModel
        {
            /// <summary>
            /// 消息标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 消息内容
            /// </summary>
            public string Content { get; set; }

            /// <summary>
            /// 点击后执行的操作类型
            /// </summary>
            public ClickType ClickType { get; set; }

            /// <summary>
            /// 点击后执行的操作内容
            /// </summary>
            public string ClickObj { get; set; }
        }

        /// <summary>
        /// 个推通道消息内容
        /// </summary>
        public class PushMessageModel
        {
            /// <summary>
            /// 手机端通知展示时间段，格式为毫秒时间戳段，两个时间的时间差必须大于10分钟，例如："1590547347000-1590633747000"
            /// </summary>
            [JsonProperty("duration")]
            public string Duration { get; set; }

            /// <summary>
            /// 纯透传消息内容，安卓和iOS均支持，与notification、revoke 三选一，都填写时报错，长度 ≤ 3072
            /// </summary>
            [JsonProperty("transmission")]
            public string Transmission { get; set; }

            /// <summary>
            /// 通知消息内容，仅支持安卓系统，iOS系统不展示个推通知消息，与transmission、revoke三选一，都填写时报错
            /// </summary>
            [JsonProperty("notification")]
            public Notification Notification { get; set; }
        }

        /// <summary>
        /// 通知消息内容
        /// </summary>
        public class Notification
        {
            /// <summary>
            /// 通知消息标题，长度 ≤ 50
            /// </summary>
            [JsonProperty("title")]
            public string Title { get; set; }

            /// <summary>
            /// 通知消息内容，长度 ≤ 256
            /// </summary>
            [JsonProperty("body")]
            public string Body { get; set; }

            /// <summary>
            /// intent：打开应用内特定页面，
            /// url：打开网页地址，
            /// payload：自定义消息内容启动应用，
            /// payload_custom：自定义消息内容不启动应用，
            /// startapp：打开应用首页，
            /// none：纯通知，无后续动作
            /// </summary>
            [JsonProperty("click_type")]
            public string ClickType { get; set; }

            /// <summary>
            /// click_type为intent时必填,点击通知打开应用特定页面，长度 ≤ 4096;示例：intent:#Intent;component=你的包名/你要打开的 activity 全路径;S.parm1=value1;S.parm2=value2;end
            /// </summary>
            [JsonProperty("intent")]
            public string Intent { get; set; }

            /// <summary>
            /// click_type为url时必填,点击通知打开链接，长度 ≤ 1024
            /// </summary>
            [JsonProperty("url")]
            public string Url { get; set; }

            /// <summary>
            /// click_type为payload/payload_custom时必填,点击通知加自定义消息，长度 ≤ 3072
            /// </summary>
            [JsonProperty("payload")]
            public string Payload { get; set; }
        }

        /// <summary>
        /// 推送条件设置
        /// </summary>
        public class SettingsModel
        {
            /// <summary>
            /// 消息离线时间设置，单位毫秒，-1表示不设离线，-1 ～ 3 * 24 * 3600 * 1000(3天)之间 默认值1小时
            /// </summary>
            [JsonProperty("ttl")]
            public int TTL { get; set; }

            /// <summary>
            /// 厂商通道策略 默认值{"strategy":{"default":1}}
            /// </summary>
            /*
             * 此项为一个字典，key为uniPush支持的厂商编码，
             * key的可选值为：default、ios、st、hw、xm、vv、mz、op
             * value为：1、2、3、4
             * 1: 表示该消息在用户在线时推送个推通道，用户离线时推送厂商通道;
             * 2: 表示该消息只通过厂商通道策略下发，不考虑用户是否在线;
             * 3: 表示该消息只通过个推通道下发，不考虑用户是否在线；
             * 4: 表示该消息优先从厂商通道下发，若消息内容在厂商通道代发失败后会从个推通道下发。
             * 
             * **注意：要推送ios通道，需要在个推开发者中心上传ios证书，建议填写2或4，否则可能会有消息不展示的问题
             */
            [JsonProperty("strategy")]
            public Dictionary<string, int> Strategy { get; set; }
        }

        /// <summary>
        /// 全体推送请求参数
        /// </summary>
        public class AllPushRequest : BasePushRequest
        {
            /// <summary>
            /// 推送目标用户，写死填 all
            /// </summary>
            [JsonProperty("audience")]
            public new string Audience = "all";
        }

        /// <summary>
        /// 推送消息参数基类
        /// </summary>
        public class BasePushRequest
        {
            /// <summary>
            /// 请求唯一标识号，10-32位之间；如果request_id重复，会导致消息丢失
            /// </summary>
            [JsonProperty("request_id")]
            public string RequestId { get; set; }

            /// <summary>
            /// 推送条件设置
            /// </summary>
            [JsonProperty("settings")]
            public SettingsModel Settings { get; set; }

            /// <summary>
            /// 推送目标用户
            /// </summary>
            [JsonProperty("audience")]
            public virtual AudienceModel Audience { get; set; }

            /// <summary>
            /// 个推推送消息参数
            /// </summary>
            [JsonProperty("push_message")]
            public PushMessageModel PushMessage { get; set; }
        }

        /// <summary>
        /// 批量推送消息参数
        /// </summary>
        public class BatchPushRequest : BasePushRequest
        {
            /// <summary>
            /// 使用创建消息接口返回的taskId，可以多次使用
            /// </summary>
            [JsonProperty("taskid")]
            public string TaskId { get; set; }

            /// <summary>
            /// 是否异步推送，true是异步，false同步。异步推送不会返回data详情
            /// </summary>
            [JsonProperty("is_async")]
            public bool IsAsync { get; set; }
        }

        /// <summary>
        /// 创建消息基类
        /// </summary>
        public class CreateMessageRequest
        {
            /// <summary>
            /// 请求唯一标识号，10-32位之间；如果request_id重复，会导致消息丢失
            /// </summary>
            [JsonProperty("request_id")]
            public string RequestId { get; set; }

            /// <summary>
            /// 推送条件设置
            /// </summary>
            //[JsonProperty("settings")]
            //public string Settings { get; set; }

            /// <summary>
            /// 个推推送消息参数
            /// </summary>
            [JsonProperty("push_message")]
            public PushMessageModel PushMessage { get; set; }
        }

        /// <summary>
        /// 推送单一消息参数，注意：推送目标用户虽然是个数组，但是只能填一个cid
        /// </summary>
        public class SinglePushRequest : BasePushRequest
        {
        }

        /// <summary>
        /// 请求token参数
        /// </summary>
        internal class TokenRequest
        {
            [JsonProperty("sign")]
            public string Sign { get; set; }

            [JsonProperty("timestamp")]
            public string Timestamp { get; set; }

            [JsonProperty("appkey")]
            public string AppKey { get; set; }
        }

        /// <summary>
        /// 公共返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BaseResponse<T>
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("msg")]
            public string Msg { get; set; }

            [JsonProperty("data")]
            public T Data { get; set; }
        }

        /// <summary>
        /// 创建消息返回结果
        /// </summary>
        internal class CreateMessageResponse
        {
            [JsonProperty("taskid")]
            public string TaskId { get; set; }
        }

        /// <summary>
        /// 推送结果
        /// </summary>
        public class PushResponse
        {
            [JsonProperty("$taskid")]
            public Dictionary<string, string> TaskId { get; set; }
        }

        /// <summary>
        /// 获取token的结果
        /// </summary>
        internal class TokenResponse
        {
            [JsonProperty("token")]
            public string Token { get; set; }

            /// <summary>
            /// token过期时间，ms时间戳
            /// </summary>
            [JsonProperty("expire_time")]
            public string ExpireTime { get; set; }
        }
    }
}
