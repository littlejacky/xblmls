using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using XBLMS.Utils;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using OpenXmlPowerTools.HtmlToWml.CSS;

namespace XBLMS.Web.Controllers.Admin
{
    public partial class AnalysisController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            try
            {
                // 创建 HTTP 客户端
                var client = _httpClientFactory.CreateClient();

                // 构造 Guest Token 请求的 Payload
                var payload = new
                {
                    user = new
                    {
                        username = "ana", // 嵌入用户用户名
                        first_name = "Analyser",
                        last_name = "Ana",
                        role = new[] { "Gamma" } // 嵌入角色
                    },
                    resources = new[]
                    {
                        new
                        {
                            type = "dashboard",
                            id = _dashboardId // 仪表盘 ID
                        }
                    },
                    rls = new object[] { } // 可选：行级安全规则
                };

                // 将 Payload 序列化为 JSON
                var jsonPayload = TranslateUtils.JsonSerialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 设置请求头
                client.DefaultRequestHeaders.Add("X-CSRF-Token", await GetCsrfTokenAsync());
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAdminTokenAsync()}");

                // 发送 POST 请求到 Superset 的 Guest Token 端点
                var response = await client.PostAsync($"{_supersetUrl}/api/v1/security/guest_token/", content);

                // 确保请求成功
                response.EnsureSuccessStatusCode();

                // 解析响应
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = TranslateUtils.JsonDeserialize(responseContent);

                // 返回 Guest Token

                return new GetResult
                {
                    GuestToken = tokenResponse?.token
                };
            }
            catch (HttpRequestException ex)
            {
                // 处理 HTTP 请求错误
                return this.Error($"Failed to fetch guest token: {ex.Message}");
            }
            catch (JsonException ex)
            {
                // 处理 JSON 解析错误
                return this.Error($"Failed to parse Superset response: {ex.Message}");
            }
            catch (Exception ex)
            {
                // 处理其他错误
                return this.Error($"An error occurred: {ex.Message}");
            }
        }

        public async Task<string> GetAdminTokenAsync()
        {
            // 如果已有有效的 Admin Token，直接返回
            if (!string.IsNullOrEmpty(_adminToken))
            {
                return _adminToken;
            }

            // 尝试刷新令牌
            try
            {
                return await RefreshAdminTokenAsync();
            }
            catch
            {
                // 刷新失败，回退到登录
                return await LoginAsync();
            }
        }

        public async Task<string> RefreshAdminTokenAsync()
        {
            if (string.IsNullOrEmpty(_refreshToken))
            {
                throw new InvalidOperationException("No refresh token available, need to login.");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_refreshToken}");
            var response = await client.PostAsync($"{_supersetUrl}/api/v1/security/refresh", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to refresh token: {response.ReasonPhrase}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var refreshResponse = TranslateUtils.JsonDeserialize(responseContent);

            _adminToken = refreshResponse.AccessToken;
            _refreshToken = refreshResponse.RefreshToken;
            return _adminToken;
        }

        private async Task<string> LoginAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                username = _username,
                password = _password,
                provider = "db"
            };
            var jsonPayload = TranslateUtils.JsonSerialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_supersetUrl}/api/v1/security/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResponse = TranslateUtils.JsonDeserialize(responseContent);

            _adminToken = loginResponse.access_token;
            _refreshToken = loginResponse.refresh_token;
            return _adminToken;
        }

        public async Task<string> GetCsrfTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAdminTokenAsync()}");
            var response = await client.GetAsync($"{_supersetUrl}/api/v1/security/csrf_token/");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _adminToken = string.Empty;

                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAdminTokenAsync()}");

                response = await client.GetAsync($"{_supersetUrl}/api/v1/security/csrf_token/");
            }

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var csrfResponse = TranslateUtils.JsonDeserialize(responseContent);
            return csrfResponse.result;
        }
    }
}
