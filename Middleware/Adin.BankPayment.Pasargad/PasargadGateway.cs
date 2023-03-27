using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Adin.BankPayment.Pasargad
{
    public class PasargadGateway
    {
        private readonly ILogger _logger;
        private readonly string _merchantCode;
        private readonly string _terminalCode;
        private readonly string _privateKey;
        private const string BaseUrl = "https://pep.shaparak.ir/Api/v1";

        public PasargadGateway(ILogger logger, string merchantCode, string terminalCode, string privateKey)
        {
            _logger = logger;
            _merchantCode = merchantCode;
            _terminalCode = terminalCode;
            _privateKey = privateKey;
        }

        public async Task<string> GetToken(string userTrackCode, int amount, string callbackUrl, string mobile, string merchantName, DateTime InvoiceDate)
        {
            try
            {
                var getTokenRequestModel = new GetTokenRequestModel()
                {
                    MerchantCode = _merchantCode,
                    TerminalCode = _terminalCode,
                    Amount = amount,
                    RedirectAddress = callbackUrl,
                    Mobile = mobile.Length > 9 ? mobile.Substring(mobile.Length - 10, 10) : string.Empty,
                    InvoiceNumber = userTrackCode,
                    Timestamp = InvoiceDate.ToString(),
                    InvoiceDate = InvoiceDate.ToString(),
                    MerchantName = merchantName
                };

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("Sign", GetSign(JsonSerializer.Serialize(getTokenRequestModel)));

                var content = new StringContent(JsonSerializer.Serialize(getTokenRequestModel),
                    Encoding.UTF8,
                    "application/json");

                var result = await httpClient.PostAsync($"{BaseUrl}/Payment/GetToken", content);
                if (result.IsSuccessStatusCode)
                {
                    var getTraceResult = JsonSerializer.Deserialize<GetTokenResponseModel>(await result.Content.ReadAsStringAsync());

                    var Success = getTraceResult.IsSuccess;
                    var Message = getTraceResult.Message;

                    if (Success)
                    {
                        return getTraceResult.Token;
                    }
                    else
                    {
                        _logger.LogWarning($"EfardaGateway - GetTraceId - Not successful - {Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EfardaGateway - GetTraceId - Failed");
            }
            return default;
        }

        public async Task<CheckTransactionResultResponseModel> CheckTransactionResult(CheckTransactionResultByReferenceIDRequestModel model)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("Sign", GetSign(JsonSerializer.Serialize(model)));

                var content = new StringContent(JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json");

                var result = await httpClient.PostAsync($"{BaseUrl}/Payment/CheckTransactionResult", content);
                if (result.IsSuccessStatusCode)
                {
                    var getTraceResult = JsonSerializer.Deserialize<CheckTransactionResultResponseModel>(await result.Content.ReadAsStringAsync());

                    return getTraceResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EfardaGateway - Verify - Failed");
            }

            return new CheckTransactionResultResponseModel()
            {
                IsSuccess = false
            };
        }

        public async Task<CheckTransactionResultResponseModel> CheckTransactionResult(CheckTransactionResultByInvoiceNumberRequestModel model)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("Sign", GetSign(JsonSerializer.Serialize(model)));

                var content = new StringContent(JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json");

                var result = await httpClient.PostAsync($"{BaseUrl}/Payment/CheckTransactionResult", content);
                if (result.IsSuccessStatusCode)
                {
                    var getTraceResult = JsonSerializer.Deserialize<CheckTransactionResultResponseModel>(await result.Content.ReadAsStringAsync());

                    return getTraceResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EfardaGateway - Verify - Failed");
            }

            return new CheckTransactionResultResponseModel()
            {
                IsSuccess = false
            };
        }

        public async Task<(bool IsSuccess, string Message)> Verify(string userTrackCode, int amount, DateTime InvoiceDate)
        {
            try
            {
                var verifyPaymentRequestModel = new VerifyPaymentRequestModel()
                {
                    MerchantCode = _merchantCode,
                    TerminalCode = _terminalCode,
                    Amount = amount,
                    InvoiceNumber = userTrackCode,
                    Timestamp = InvoiceDate.ToString(),
                    InvoiceDate = InvoiceDate.ToString(),
                };

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("Sign", GetSign(JsonSerializer.Serialize(verifyPaymentRequestModel)));

                var content = new StringContent(JsonSerializer.Serialize(verifyPaymentRequestModel),
                    Encoding.UTF8,
                    "application/json");

                var result = await httpClient.PostAsync($"{BaseUrl}/Payment/VerifyPayment", content);
                if (result.IsSuccessStatusCode)
                {
                    var getTraceResult = JsonSerializer.Deserialize<VerifyResponseModel>(await result.Content.ReadAsStringAsync());

                    bool Success = getTraceResult.IsSuccess;
                    string Message = getTraceResult.Message;

                    if (Success)
                    {
                        return (Success, Message);
                    }
                    else
                    {
                        _logger.LogWarning($"EfardaGateway - Verify - Not successful - Message: {Message}");
                        return (Success, Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EfardaGateway - Verify - Failed");
            }
            return (false, "Failed");
        }

        private string GetSign(string PostBodyJson)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_privateKey);

            byte[] signedData = rsa.SignData(Encoding.UTF8.GetBytes(PostBodyJson), new SHA1CryptoServiceProvider());

            string sign = Convert.ToBase64String(signedData);
            return sign;
        }
    }
}