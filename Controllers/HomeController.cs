using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testPortfolio.Models;

namespace testPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductController _productController;
        public HomeController(ILogger<HomeController> logger, ProductController productController)
        {
            _logger = logger;
            _productController = productController;
        }

        public async Task<IActionResult> Index()
        {

            var products = await _productController.GetAllPublicAsync();

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Services()
        {
            // Récupérer les produits (exemple simplifié, à remplacer par la vraie récupération BDD si besoin)
            // var products = new List<Product> {
            //     new Product {
            //         name = "Portfolio React",
            //         description = "Un portfolio interactif moderne avec React et animations.",
            //         images = new List<Picture> { new Picture { path = "/img/1007429.jpg" } }
            //     },
            //     new Product {
            //         name = "Site Vitrine Artisan",
            //         description = "Site responsive pour un artisan, galerie photo et contact.",
            //         images = new List<Picture> { new Picture { path = "/img/2d2417ea-6c89-4960-a72f-a658c85f3d63.png" } }
            //     }
            // };

            var products = new List<Product>();
            products = await _productController.GetAllAsync();

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
