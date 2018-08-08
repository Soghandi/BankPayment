using Adin.BankPayment.Connector.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Adin.BankPayment.Connector
{
    public class ClientService
    {
        public ClientService(string baseUrl, string publicKey)
        {
            _baseUrl = baseUrl;
            _publicKey = publicKey;
        }
        private string _baseUrl;
        private string _publicKey;
        private string _token;
        private DateTime _tokenExpiration;

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

                string serializedModel = JsonConvert.SerializeObject(model);
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
                        Status = Enum.ApiStatusCodeEnum.Success
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new OutputModel<PayRequestResponseModel>
                    {
                        Status = Enum.ApiStatusCodeEnum.InvalidPublicKey
                    };
                }
                else
                {
                    return new OutputModel<PayRequestResponseModel>
                    {
                        Status = Enum.ApiStatusCodeEnum.BadRequest
                    };
                }
            }
        }



        private async Task FillToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var dict = new Dictionary<string, string>();
                dict.Add("publicKey", _publicKey);

                var pUrl = new Uri(string.Format("{0}/api/token", _baseUrl));

                var req = new HttpRequestMessage(HttpMethod.Post, pUrl) { Content = new FormUrlEncodedContent(dict) };
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

        public async Task<OutputModel<VerifyTransactionResponseModel>> Verify(Guid id)
        {
            if (_token == null || _tokenExpiration < DateTime.Now) await FillToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                var pUrl = new Uri(string.Format("{0}/api/PayInfo/Verify?publicKey={1}&id={2}", _baseUrl, _publicKey, id));
                var response = await client.GetAsync(pUrl);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var resModel = JsonConvert.DeserializeObject<VerifyTransactionResponseModel>(res);
                    return new OutputModel<VerifyTransactionResponseModel>
                    {

                        Body = resModel,
                        Status = Enum.ApiStatusCodeEnum.Success
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new OutputModel<VerifyTransactionResponseModel>
                    {
                        Status = Enum.ApiStatusCodeEnum.InvalidPublicKey,
                        Body = new VerifyTransactionResponseModel
                        {
                            Message = "عملیات با خطا مواجه شد"
                        }

                    };
                }
                else
                {
                    return new OutputModel<VerifyTransactionResponseModel>
                    {
                        Status = Enum.ApiStatusCodeEnum.BadRequest,
                        Body = new VerifyTransactionResponseModel
                        {
                            Message = "عملیات با خطا مواجه شد"
                        }
                    };
                }
            }
        }


    }
}
