using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Adin.BankPayment.Efarda
{
    public class EfardaGateway
    {
        private readonly ILogger _logger;
        private readonly string UserName;
        private readonly string Password;
        private readonly string ServiceId;
        private const string BaseUrl = "https://pf.efarda.ir/pf/api";

        public EfardaGateway(ILogger logger, string userName, string password, string serviceId)
        {
            _logger = logger;
            UserName = userName;
            Password = password;
            ServiceId = serviceId;
        }

        public async Task<string> GetTraceId(string userTrackCode, string amount, string callbackUrl, string mobile)
        {
            try
            {
                EfardaGetTraceModel getTraceModel = new EfardaGetTraceModel()
                {
                    username = UserName,
                    password = Password,
                    amount = amount,
                    callBackUrl = callbackUrl,
                    mobile = mobile.Length > 9 ? mobile.Substring(mobile.Length - 10, 10) : string.Empty,
                    additionalData = userTrackCode
                };

                getTraceModel.serviceAmountList.Add(new EfardaServiceamountlist() { amount = amount, serviceId = ServiceId });

                HttpClient httpClient = new HttpClient();

                var content = new StringContent(JsonSerializer.Serialize(getTraceModel),
                    Encoding.UTF8,
                    "application/json");

                _logger.LogDebug("JsonSerializer.Serialize(getTraceModel):" + JsonSerializer.Serialize(getTraceModel));

                var result = await httpClient.PostAsync($"{BaseUrl}/ipg/getTraceId", content);
                if (result.IsSuccessStatusCode)
                {
                    EfardaGetTraceResultModel getTraceResult = JsonSerializer.Deserialize<EfardaGetTraceResultModel>(await result.Content.ReadAsStringAsync());

                    var (Success, Message) = EfardaErrors.GetResult(getTraceResult.result);

                    if (Success)
                    {
                        return getTraceResult.traceNumber;
                    }
                    else
                    {
                        _logger.LogWarning($"EfardaGateway - GetTraceId - Not successful - {Message} + result code :{getTraceResult?.result}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EfardaGateway - GetTraceId - Failed");
            }
            return default;
        }

        public async Task<bool> Verify(string bankTrackCode)
        {
            try
            {
                EfardaVerifyModel efardaVerifyModel = new EfardaVerifyModel()
                {
                    username = UserName,
                    password = Password,
                    traceNumber = bankTrackCode
                };

                HttpClient httpClient = new HttpClient();

                var content = new StringContent(JsonSerializer.Serialize(efardaVerifyModel),
                    Encoding.UTF8,
                    "application/json");

                var result = await httpClient.PostAsync($"{BaseUrl}/ipg/verify", content);
                if (result.IsSuccessStatusCode)
                {
                    EfardaVerifyResultModel getTraceResult = JsonSerializer.Deserialize<EfardaVerifyResultModel>(await result.Content.ReadAsStringAsync());

                    (bool Success, string Message) = EfardaErrors.GetResult(getTraceResult.result);

                    if (Success)
                    {
                        return Success;
                    }
                    else
                    {
                        _logger.LogWarning($"EfardaGateway - Verify - Not successful - bankTrackCode: {bankTrackCode} - Message: {Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EfardaGateway - Verify - Failed");
            }
            return false;
        }
    }
}