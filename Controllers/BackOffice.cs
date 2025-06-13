using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace testPortfolio.Controllers
{
    public class BackOffice : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
