using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Adin.BankPayment.Client
{
    public class ClientService
    {
        private string _baseUrl = "http://localhost:57698";

        public async Task<OutputModel<PayRequestResponseModel>> RequestPay(PayInfoModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

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

        public async Task<OutputModel<bool>> Verify(string publicKey, Guid id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var pUrl = new Uri(string.Format("{0}/api/PayInfo/Verify?publicKey={1}&id={2}", _baseUrl, publicKey, id));
                var response = await client.GetAsync(pUrl);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var resModel = JsonConvert.DeserializeObject<bool>(res);
                    return new OutputModel<bool>
                    {

                        Body = resModel,
                        Status = Enum.ApiStatusCodeEnum.Success
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new OutputModel<bool>
                    {
                        Status = Enum.ApiStatusCodeEnum.InvalidPublicKey
                    };
                }
                else
                {
                    return new OutputModel<bool>
                    {
                        Status = Enum.ApiStatusCodeEnum.BadRequest
                    };
                }
            }
        }


    }
}
