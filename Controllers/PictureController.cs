using Microsoft.AspNetCore.Mvc;
using testPortfolio.Models;
using testPortfolio.Services;

namespace testPortfolio.Controllers
{
    public class PictureController : Controller
    {
        private readonly PictureService _pictureService;
        public PictureController(PictureService pictureService)
        {
            _pictureService = pictureService;
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
            await _pictureService.CreateAsync(picture);
        }

    }
}
