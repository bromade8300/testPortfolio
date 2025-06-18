using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPortfolio.Models;

namespace testPortfolio.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        [Route("/insert")]
        public async Task InsertAsyncDemo()
        {
            Product product = new Product();
            product.description = "fds";
            product.name = "fds";
            product.price= "fds";
            product.isPublic = true;
            _context.Products.Add(
                product
                );
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, List<IFormFile> images)
        {   
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

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
                            productId    = product.Id
                        };

                        _context.Pictures.Add(productImage);
                        await _context.SaveChangesAsync();
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _context.Products
                //.Where(p => p.IsPublic)
                .OrderByDescending(p => p.dateAdded)
                .Include(p => p.images)
                .ToListAsync();
            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.images)
                .FirstOrDefaultAsync(p => p.Id == id);
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}