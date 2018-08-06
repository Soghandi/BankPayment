# BankPayment

Persian Bank Payment Gateway

Easily connect to Iranian bank gateways without getting involved with their contractual protocol.
<br/>
<br/>
<b>
  Supported Banks:</b>
      <ul>
        <li>Saman</li>
        <li>Mellat (behpardakht)</li>
  </ul>
  
<br/>
<b>
  In Progress:</b>
      <ul>
        <li>Parsian</li>        
  </ul>
<br/>
<b>
  How to use:  
 </b>
 <br/>
1- clone project, build and run Adin.BankPayment in your server (Windows, Linux, Mac)
  <br/>
  build command:
<br/>

```
dotnet build Adin.BankPayment
```

 <br/>
 publish command:
 <br/>
 
 ```
dotnet publish .\Adin.BankPayment\Adin.BankPayment.csproj
```

<br/>
run command:
<br/>

```
dotnet .\Adin.BankPayment\bin\Debug\netcoreapp2.1\publish\Adin.BankPayment.dll
```

<br/>
server is ready!
<br/>
 
2- add connector dll to client project
 <br/>
```
Install-Package Adin.BankPayment.Connector
```
 <br/>
3- use it in your project (like sample project)

send request:

```
 public async Task<IActionResult> GotoBankPage()
 {
   var currentBaseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
   var model = new Connector.Model.PayInfoModel
   {
       Amount = 1000,
       BankCode = BankCodeEnum.Saman,
       CallbackUrl = currentBaseUrl + "/Home/Callback",
       Mobile = 989129475195,
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
```

callback:

```
  public async Task<IActionResult> CallBack()
  {
    Guid payId = Guid.Parse(Request.Query["id"]);
    string trackCode = Request.Query["trackCode"];
    bool status = bool.Parse(Request.Query["status"]);
    string message = Request.Query["message"];
    if (status == false)
    {
        int errorCode = int.Parse(Request.Query["errorCode"]);
        BankErrorCodeEnum errCode = (BankErrorCodeEnum)errorCode;
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
 ```
 <br/>
 <br/>
 <br/>
 enjoy it.
  

 
