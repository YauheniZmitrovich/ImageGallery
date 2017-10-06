using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;

namespace ImageGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly IImageRepository _repository;

        public HomeController(IImageRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase image)
        {
            if (image == null) return RedirectToAction("Index");

            Image im = new Image();


            im.Data = new byte[image.ContentLength];

            im.MimeType = image.ContentType;

            image.InputStream.Read(im.Data, 0, image.ContentLength);


            _repository.Save(im);


            return RedirectToAction("Index");
        }

        public FileResult GetCarouselImage(string imagePath)
        {
            return File(Server.MapPath("~/App_Data/Images/" + imagePath), "image/png");
        }

        public FileResult GetImage(int id)
        {
            var image = _repository.GetImageById(id);

            if (image.Data != null && image.MimeType != null)
                return File(image.Data, image.MimeType);

            var path = Server.MapPath("~/App_Data/Images/Default/Photo.jpg");

            return File(path, "image/png");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
