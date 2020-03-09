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
	<li>Efarda (Ertebat Farda - efarda.ir)</li>
	<li>Parsian</li>
  </ul>
<br/>
<b>
  How to setup server:  
 </b>
 <br/>
clone project, build and run Adin.BankPayment in your server (Windows, Linux, Mac)
  <br/>
  build command:
<br/>

```
dotnet build Adin.BankPayment
```

<br/>
 add connectionstring to appsettings in Adin.Bankpayment project 
 <br/>
 
```
{
"ConnectionString": "Server=SERVERNAME;Database=DATABASENAME;User Id=DATABASEUSER;Password=PASSWORD;MultipleActiveResultSets=True"
}
```

 <br/>
 run below command to create tables with default values:
 <br/>
 
```
dotnet ef database update -p .\Adin.BankPayment.Domain\Adin.BankPayment.Domain.csproj -s .\Adin.BankPayment\Adin.BankPayment.csproj
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
after project started successfully you can easily add various applications with multiple bank gateways from swagger url:
http://localhost:5000/api-docs/index.html
<br/>
server configuration finish!
<br/>
<br/>
 <br/>
<b>
  How to setup client:  
 </b>
 <br/>

First add connector dll to client project
 <br/>
```
Install-Package Adin.BankPayment.Connector
```
 <br/>
use it in your project (like sample project)

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
            //Todo: deliver product to customer here


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
  

 
