using System;
using System.Threading.Tasks;
using Adin.BankPayment.ClientSample.Models;
using Adin.BankPayment.Connector;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Connector.Model;
using Microsoft.AspNetCore.Mvc;

namespace Adin.BankPayment.ClientSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClientService client = new ClientService("http://localhost:57698", AppSetting.PaymentKey);

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GotoBankPage(int id)
        {
            var currentBaseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
            var model = new PayInfoModel
            {
                Amount = 1000,
                BankCode = (BankCodeEnum) id,
                CallbackUrl = currentBaseUrl + "/Home/Callback",
                Mobile = 989354762696,
                PriceUnit = PriceUnitEnum.Rial,
                TrackCode = DateTime.Now.ToString("hhmmssfff")
            };
            var response = await client.RequestPay(model);
            if (response.Status == ApiStatusCodeEnum.Success)
                return Redirect(response.Body.Url);
            throw new Exception(response.Status + ":" + response.Body);
        }

        public async Task<IActionResult> CallBack()
        {
            var payId = Guid.Parse(Request.Query["id"]);
            string trackCode = Request.Query["trackCode"];
            var status = bool.Parse(Request.Query["status"]);
            string message = Request.Query["message"];
            if (status == false)
            {
                var errorCode = int.Parse(Request.Query["errorCode"]);
                var errCode = (ErrorCodeEnum) errorCode;
                ViewBag.Result = message;
            }
            else
            {
                try
                {
                    //Todo: Please deliver product to customer here


                    var res = await client.Verify(payId);
                    if (res.Status == ApiStatusCodeEnum.Success && res.Body.Status)
                        //Transaction Done
                        ViewBag.Result = res.Body.Message;
                    else
                        //Transaction Failed
                        ViewBag.Result = res.Body.Message + "<br/>" +
                                         "تا 24 ساعت دیگر مبلغ به حساب شما بازگردانده خواهد شد";
                }
                catch
                {
                    //Reverse Transaction
                }
            }

            return View();
        }
    }
}