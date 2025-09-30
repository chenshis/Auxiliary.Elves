using Auxiliary.Elves.Infrastructure.Config;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Client
{
    public class AuxElvesHttpClient
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public AuxElvesHttpClient(ILogger<AuxElvesHttpClient> logger, IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this._httpClient = httpClientFactory.CreateClient(nameof(AuxElvesHttpClient));
            this._httpClient.BaseAddress = new Uri(SystemConstant.ServerUrl);
        }

        public async Task<ApiResponse<TData>> PostAsync<TData>(string route)
        {
            return await PostAsync<int, TData>(route, 0);
        }

        public async Task<ApiResponse<TData>> PostAsync<TRequest, TData>(string route, TRequest request)
        {
            StringContent stringContent = null;
            if (request != null)
            {
                stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            }
            try
            {
                HttpResponseMessage responseMessage = await _httpClient.PostAsync(route, stringContent);
                var responseResult = GetResponseCodeResult<TData>(responseMessage);
                // 未授权 但是有token 则刷新token
                if (responseResult.Code == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    var refreshResponse = RefreshToken();
                    if (refreshResponse.Code != 0)
                    {
                        _logger.LogError($"Post请求异常：{refreshResponse.Msg}");
                        return responseResult;
                    }
                    responseMessage = await _httpClient.PostAsync(route, stringContent);
                    responseResult = GetResponseCodeResult<TData>(responseMessage);
                    if (responseResult.Code != 0)
                    {
                        _logger.LogError($"Post请求异常：{responseResult.Msg}");
                        responseResult.Code = 1;
                    }
                }
                return responseResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Post请求异常：{ex.Message}");
                var apiResponse = new ApiResponse<TData>();
                apiResponse.Code = 1;
                apiResponse.Msg = ex.Message;
                apiResponse.Data = default(TData);
                apiResponse.ServerTime = DateTime.Now.Ticks;
                return apiResponse;
            }
        }


        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        private ApiResponse<string> RefreshToken()
        {
            ApiResponse<string> apiResponse = null;
            var token = _httpClient.DefaultRequestHeaders.Authorization?.Parameter;
            if (string.IsNullOrWhiteSpace(token))
            {
                apiResponse = new ApiResponse<string>();
                apiResponse.Code = 1;
                apiResponse.Msg = SystemConstant.Unauthorized;
                apiResponse.Data = default(string);
                apiResponse.ServerTime = DateTime.Now.Ticks;
            }
            else
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
                try
                {
                    HttpResponseMessage responseMessage = _httpClient.PostAsync(SystemConstant.RefreshTokenRoute, stringContent).Result;
                    apiResponse = GetResponseCodeResult<string>(responseMessage);
                    if (apiResponse.Code == 0)
                    {
                        SetToken(apiResponse.Data.ToString());
                    }
                    else
                    {
                        apiResponse.Code = 1;
                        SetToken("");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Post请求异常：{ex.Message}");
                    apiResponse = new ApiResponse<string>();
                    apiResponse.Code = 1;
                    apiResponse.Msg = ex.Message;
                    apiResponse.Data = default(string);
                    apiResponse.ServerTime = DateTime.Now.Ticks;
                }

            }
            return apiResponse;
        }
        public AuxElvesHttpClient SetToken(string token)
        {
            this._httpClient.SetBearerToken(token);
            return this;
        }


        /// <summary>
        /// 获取响应状态返回结果
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <returns></returns>
        private ApiResponse<TData> GetResponseCodeResult<TData>(HttpResponseMessage responseMessage)
        {
            ApiResponse<TData> apiResponse = new ApiResponse<TData>();
            if (responseMessage.IsSuccessStatusCode)
            {
                try
                {
                    var stringResult = responseMessage.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<TData>>(stringResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"接口请求序列化异常：{ex.Message}");
                    apiResponse.Code = 1;
                    apiResponse.Msg = ex.Message;
                    apiResponse.Data = default(TData);
                    apiResponse.ServerTime = DateTime.Now.Ticks;
                }
            }
            else
            {
                _logger.LogError($"接口请求错误,错误代码{responseMessage.StatusCode}，错误原因{responseMessage.ReasonPhrase}");
                // 401 访问拒绝
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var authorization = _httpClient.DefaultRequestHeaders.Authorization?.ToString();
                    if (string.IsNullOrWhiteSpace(authorization))
                    {
                        apiResponse = new ApiResponse<TData>();
                        apiResponse.Code = 1;
                        apiResponse.Msg = SystemConstant.Unauthorized;
                        apiResponse.Data = default(TData);
                        apiResponse.ServerTime = DateTime.Now.Ticks;
                    }
                    else
                    {
                        // 赋值401状态 用于上层处理
                        apiResponse = new ApiResponse<TData>();
                        apiResponse.Code = (int)responseMessage.StatusCode;
                        apiResponse.Msg = SystemConstant.Unauthorized;
                        apiResponse.Data = default(TData);
                        apiResponse.ServerTime = DateTime.Now.Ticks;
                    }
                }
                else
                {
                    apiResponse = new ApiResponse<TData>();
                    apiResponse.Code = 1;
                    apiResponse.Msg = responseMessage.ReasonPhrase;
                    apiResponse.Data = default(TData);
                    apiResponse.ServerTime = DateTime.Now.Ticks;
                }
            }
            return apiResponse;
        }
    }
}