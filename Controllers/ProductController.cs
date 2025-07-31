using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPortfolio.Models;
using testPortfolio.Services;

namespace testPortfolio.Controllers
{
    public class ProductController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ProductService _productService;
        private readonly PictureService _pictureService;

        public ProductController(ProductService productService,PictureService pictureService, IWebHostEnvironment environment)
        {
            _productService = productService;
            _pictureService = pictureService;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, List<IFormFile> images)
        {
            var pictures = new List<Picture>();

            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
                        var relativePath = "/uploads/" + fileName;
                        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        pictures.Add(new Picture
                        {
                            path = relativePath,
                            name = image.FileName,
                            dateAdded = DateTime.UtcNow,
                            isPublic = true
                        });
                    }
                }
            }

            product.images = pictures;
            product.dateAdded = DateTime.UtcNow;

            await _productService.CreateAsync(product);

            return RedirectToAction(nameof(Index), "BackOffice");
        }



        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _productService.GetAllAsync();
            return products;
        }

        public async Task<List<Product>> GetAllPublicAsync()
        {
            var products = await _productService.GetAllPublicAsync();
            return products;
        }


        public async Task<Product?> GetByIdAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return true;
        }

        public async Task UpdateAsync(Product product)
        {
            await _productService.UpdateAsync(product);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productService.CreateAsync(product);
            return product;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int productId,int PictureIndex)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var image = product.images.ElementAtOrDefault(PictureIndex);

            // Supprimer le fichier physique
            if (!string.IsNullOrEmpty(image.path))
            {
                var filePath = Path.Combine(_environment.WebRootPath, image.path.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            product.images.RemoveAt(PictureIndex);
            await _productService.UpdateAsync(product);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Product newProduct, List<IFormFile> images)
        {
            if (id != newProduct.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //// Mettre à jour les propriétés de base du produit
                    var existingProduct = await _productService.GetByIdAsync(id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    existingProduct.name = newProduct.name;
                    existingProduct.description = newProduct.description;
                    existingProduct.price = newProduct.price;
                    existingProduct.isPublic = newProduct.isPublic;

                    //// Gérer les nouvelles images
                    if (images != null && images.Count > 0)
                    {
                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
                                var relativePath = "/uploads/" + fileName;
                                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

                                if (!Directory.Exists(uploadPath))
                                {
                                    Directory.CreateDirectory(uploadPath);
                                }

                                var filePath = Path.Combine(uploadPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                var productImage = new Picture
                                {
                                    path = relativePath,
                                    productId = newProduct.Id
                                };
                                existingProduct.images.Add(productImage);
                            }
                        }
                    }
                    await _productService.UpdateAsync(existingProduct);
                    return RedirectToAction(nameof(Index), "BackOffice");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(newProduct.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index), "BackOffice");
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _productService.IsProductExistsAsync(id);
        }
    }
}