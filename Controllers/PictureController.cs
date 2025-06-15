using Microsoft.AspNetCore.Mvc;
using testPortfolio.Models;

namespace testPortfolio.Controllers
{
    public class PictureController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PictureController(ApplicationDbContext context)
            {
                _context = context;
            }


        public async Task InsertAsync(Picture picture)
        {
                //var picture = new Picture
                //{
                //    name = "Example Picture",
                //    usage = PictureUsage.HomePage, 
                //    path = "/img/example.jpg",
                //    description = "This is an example picture.",
                //    dateAdded = DateTime.Now,
                //    isPublic = true
                //};
            _context.Pictures.Add(picture);
            await _context.SaveChangesAsync();
        }

    }
}
