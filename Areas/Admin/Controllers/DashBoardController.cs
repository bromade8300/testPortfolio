using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace testPortfolio.Areas.Admin.Controllers

{

    [Area("Admin")]
    public class DashBoardController : Controller
    {
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }
    }
}
