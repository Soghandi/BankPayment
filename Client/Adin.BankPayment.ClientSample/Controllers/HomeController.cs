using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Adin.BankPayment.ClientSample.Models;
using Adin.BankPayment.Connector;
using Adin.BankPayment.Connector.Enum;

namespace Adin.BankPayment.ClientSample.Controllers
{
    public class HomeController : Controller
    {
        ClientService client = new ClientService("http://localhost:57698", AppSetting.PaymentKey);

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GotoBankPage(int id)
        {
            var currentBaseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
            var model = new Connector.Model.PayInfoModel
            {
                Amount = 1000,
                BankCode = (BankCodeEnum)id,
                CallbackUrl = currentBaseUrl + "/Home/Callback",
                Mobile = 989354762696,
                PriceUnit = Connector.Enum.PriceUnitEnum.Rial,
                TrackCode = DateTime.Now.ToString("hhmmssfff")
            };
            var response = await client.RequestPay(model);
            if (response.Status == Connector.Enum.ApiStatusCodeEnum.Success)
            {
                return Redirect(response.Body.Url);
            }
            else
            {
                throw new Exception(response.Status + ":" + response.Body);
            }
        }

        public async Task<IActionResult> CallBack()
        {
    Guid payId = Guid.Parse(Request.Query["id"]);
    string trackCode = Request.Query["trackCode"];
    bool status = bool.Parse(Request.Query["status"]);
    string message = Request.Query["message"];
    if (status == false)
    {
        int errorCode = int.Parse(Request.Query["errorCode"]);
        ErrorCodeEnum errCode = (ErrorCodeEnum)errorCode;
        ViewBag.Result = message;
    }
    else
    {
        try
        {
            //Todo: Please deliver product to customer here


            var res = await client.Verify(payId);
            if (res.Status == Connector.Enum.ApiStatusCodeEnum.Success && res.Body.Status)
            {
                //Transaction Done
                ViewBag.Result = res.Body.Message;
            }
            else
            {
                //Transaction Failed
                ViewBag.Result = res.Body.Message + "<br/>" + "تا 24 ساعت دیگر مبلغ به حساب شما بازگردانده خواهد شد";
            }
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
