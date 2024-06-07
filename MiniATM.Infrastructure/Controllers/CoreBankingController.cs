using Microsoft.AspNetCore.Mvc;

namespace MiniATM.Infrastructure.Controllers
{
    public class CoreBankingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
