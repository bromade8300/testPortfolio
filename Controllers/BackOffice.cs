using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testPortfolio.Models;

namespace testPortfolio.Controllers
{
    [Authorize]
    public class BackOffice : Controller
    {
        private readonly ProductController _productController;

        public BackOffice(ProductController productController)
        {
            _productController = productController;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productController.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> EditProducts()
        {
            var products = await _productController.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        public async Task<IActionResult> ProductEdit(int id)
        {
            var product = await _productController.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productController.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productController.DeleteAsync(id);
            TempData["SuccessMessage"] = "Produit supprimé avec succès";
            return RedirectToAction(nameof(EditProducts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productController.UpdateAsync(product);
                TempData["SuccessMessage"] = "Produit mis à jour avec succès";
                return RedirectToAction(nameof(EditProducts));
            }

            return View(product);
        }
    }
}
