using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testPortfolio.Models;

namespace testPortfolio.Controllers
{
    public class BackOffice : Controller
    {
        private readonly ProductController _productController;
        public BackOffice(ProductController productController)
        {
            _productController = productController;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
           //var items = await _productController.GetAllAsync();


            return View();
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }



    }
}
