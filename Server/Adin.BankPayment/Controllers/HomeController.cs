using Microsoft.AspNetCore.Mvc;

namespace Adin.BankPayment.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}