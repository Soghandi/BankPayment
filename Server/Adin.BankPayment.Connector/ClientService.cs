using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Connector.Model;
using Newtonsoft.Json;

namespace Adin.BankPayment.Connector
{
    public class ClientService
    {
        private readonly string _baseUrl;
        private readonly string _publicKey;
        private string _token;
        private DateTime _tokenExpiration;

        public ClientService(string baseUrl, string publicKey)
        {
            _baseUrl = baseUrl;
            _publicKey = publicKey;
        }

        private async Task FillToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var dict = new Dictionary<string, string> {{"publicKey", _publicKey}};

                var pUrl = new Uri($"{_baseUrl}/api/token");

                var req = new HttpRequestMessage(HttpMethod.Post, pUrl) {Content = new FormUrlEncodedContent(dict)};
                var response = await client.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var resModel = JsonConvert.DeserializeObject<TokenModel>(res);
                    _token = resModel.access_token;
                    _tokenExpiration = DateTime.Now.AddSeconds(resModel.expires_in - 30);
                }
            }
        }

        public async Task<OutputModel<PayRequestResponseModel>> RequestPay(PayInfoModel model)
        {
            if (_token == null || _tokenExpiration < DateTime.Now) await FillToken();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var serializedModel = JsonConvert.SerializeObject(model);
                var content = new StringContent(serializedModel,
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("/api/PayInfo/RequestPay", content);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var resModel = JsonConvert.DeserializeObject<PayRequestResponseModel>(res);
                    return new OutputModel<PayRequestResponseModel>
                    {
                        Body = resModel,
                        Status = ApiStatusCodeEnum.Success
                    };
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new OutputModel<PayRequestResponseModel>
                    {
                        Status = ApiStatusCodeEnum.InvalidPublicKey
                    };
                return new OutputModel<PayRequestResponseModel>
                {
                    Status = ApiStatusCodeEnum.BadRequest
                };
            }
        }

        public async Task<OutputModel<VerifyTransactionResponseModel>> Verify(Guid id)
        {
            if (_token == null || _tokenExpiration < DateTime.Now) await FillToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                var pUrl = new Uri($"{_baseUrl}/api/PayInfo/Verify?id={id}");
                var response = await client.GetAsync(pUrl);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var resModel = JsonConvert.DeserializeObject<VerifyTransactionResponseModel>(res);
                    return new OutputModel<VerifyTransactionResponseModel>
                    {
                        Body = resModel,
                        Status = ApiStatusCodeEnum.Success
                    };
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new OutputModel<VerifyTransactionResponseModel>
                    {
                        Status = ApiStatusCodeEnum.InvalidPublicKey,
                        Body = new VerifyTransactionResponseModel
                        {
                            Message = "Unauthorized - عملیات با خطا مواجه شد"
                        }
                    };
                return new OutputModel<VerifyTransactionResponseModel>
                {
                    Status = ApiStatusCodeEnum.BadRequest,
                    Body = new VerifyTransactionResponseModel
                    {
                        Message = "عملیات با خطا مواجه شد"
                    }
                };
            }
        }

        public async Task<OutputModel<CancelPaymentResponseModel>> CancelPayment(Guid id)
        {
            if (_token == null || _tokenExpiration < DateTime.Now) await FillToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                var pUrl = new Uri($"{_baseUrl}/api/PayInfo/CancelPayment?id={id}");
                var response = await client.GetAsync(pUrl);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var resModel = JsonConvert.DeserializeObject<CancelPaymentResponseModel>(res);
                    return new OutputModel<CancelPaymentResponseModel>
                    {
                        Body = resModel,
                        Status = ApiStatusCodeEnum.Success
                    };
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new OutputModel<CancelPaymentResponseModel>
                    {
                        Status = ApiStatusCodeEnum.InvalidPublicKey,
                        Body = new CancelPaymentResponseModel
                        {
                            Message = "عملیات با خطا مواجه شد"
                        }
                    };

                {
                    var res = await response.Content.ReadAsStringAsync();
                    var resModel = JsonConvert.DeserializeObject<CancelPaymentResponseModel>(res);
                    return new OutputModel<CancelPaymentResponseModel>
                    {
                        Status = ApiStatusCodeEnum.BadRequest,
                        Body = resModel
                    };
                }
            }
        }
    }
}