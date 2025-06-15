using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testPortfolio.Models;

namespace testPortfolio.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }


        [Route("/insert")]
        public async Task InsertAsyncDemo()
        {
            Product product = new Product();
            product.Description = "fds";
            product.Name = "fds";
            product.Price= "fds";
            product.IsPublic = true;
            _context.Products.Add(
                product
                );
            await _context.SaveChangesAsync();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","BackOffice");
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _context.Products
                //.Where(p => p.IsPublic)
                .OrderByDescending(p => p.DateAdded)
                .ToListAsync();
            return products;
        }

        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

    }
}