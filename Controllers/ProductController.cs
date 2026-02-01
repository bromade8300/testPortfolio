using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPortfolio.Models;
using testPortfolio.Services;

namespace testPortfolio.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ProductService _productService;
        private readonly PictureService _pictureService;

        // Whitelist des types MIME autorisés pour les uploads
        private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public ProductController(ProductService productService, PictureService pictureService, IWebHostEnvironment environment)
        {
            _productService = productService;
            _pictureService = pictureService;
            _environment = environment;
        }

        private bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0 || file.Length > MaxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return false;

            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> images)
        {
            var pictures = new List<Picture>();

            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    if (!IsValidImageFile(image))
                    {
                        ModelState.AddModelError("images", $"Fichier invalide: {image.FileName}. Seuls les fichiers image (JPEG, PNG, GIF, WebP) de moins de 10 MB sont autorisés.");
                        continue;
                    }

                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    var fileName = $"{Guid.NewGuid()}{extension}";
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

            product.images = pictures;
            product.dateAdded = DateTime.UtcNow;

            await _productService.CreateAsync(product);

            TempData["SuccessMessage"] = $"Le produit « {product.name} » a été créé avec succès.";
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int productId, int PictureIndex)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var image = product.images?.ElementAtOrDefault(PictureIndex);
            if (image == null)
            {
                return NotFound();
            }

            // Supprimer le fichier physique avec protection contre Path Traversal
            if (!string.IsNullOrEmpty(image.path))
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, image.path.TrimStart('/')));

                // Vérifier que le chemin résolu est bien dans le dossier uploads (protection Path Traversal)
                if (filePath.StartsWith(uploadsPath, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            product.images.RemoveAt(PictureIndex);
            await _productService.UpdateAsync(product);

            return Ok();
        }

        [HttpPost]
        [Authorize]
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
                    var existingProduct = await _productService.GetByIdAsync(id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    existingProduct.name = newProduct.name;
                    existingProduct.description = newProduct.description;
                    existingProduct.price = newProduct.price;
                    existingProduct.isPublic = newProduct.isPublic;

                    // Gérer les nouvelles images avec validation
                    if (images != null && images.Count > 0)
                    {
                        foreach (var image in images)
                        {
                            if (!IsValidImageFile(image))
                            {
                                ModelState.AddModelError("images", $"Fichier invalide: {image.FileName}. Seuls les fichiers image (JPEG, PNG, GIF, WebP) de moins de 10 MB sont autorisés.");
                                continue;
                            }

                            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                            var fileName = $"{Guid.NewGuid()}{extension}";
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
                            existingProduct.images ??= new List<Picture>();
                            existingProduct.images.Add(productImage);
                        }
                    }
                    await _productService.UpdateAsync(existingProduct);
                    TempData["SuccessMessage"] = $"Le produit « {existingProduct.name} » a été mis à jour avec succès.";
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