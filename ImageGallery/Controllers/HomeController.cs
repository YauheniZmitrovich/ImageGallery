using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using ImageGallery.Models;
using Image = Domain.Entities.Image;

namespace ImageGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly IImageRepository _repository;

        public HomeController(IImageRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index() => View(InitializeImageHomeViewModel(1));

        public ViewResult List(int page = 1)=>View(InitializeImageHomeViewModel(page));
        

        private ImageHomeViewModel InitializeImageHomeViewModel(int page=1)
        {
            int PageSize = 6;
            return new ImageHomeViewModel
            {
                Images = _repository.Images
                    .OrderBy(image => image.Id)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),

                PagingInfo = new ImagePageInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = _repository.Images.Count()
                }
            };
        }

        #region Upload

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase image, string title)
        {
            if (image == null || title == null) return RedirectToAction("Index");

            var im = new Image
            {
                Data = new byte[image.ContentLength],

                MimeType = image.ContentType,

                Title = title
            };

            image.InputStream.Read(im.Data, 0, image.ContentLength);

            im.CompressedData = ResizeImage(im.Data);


            _repository.Save(im);


            return RedirectToAction("Index");
        }

        #endregion


        #region GetImage

        public FileResult GetImageFromAppData(string imagePath)
        {
            return File(Server.MapPath("~/App_Data/Images/" + imagePath), "image/png");
        }

        public FileResult GetBigImage(int id)
        {
            var image = _repository.GetImageById(id);

            if (image != null)
                return File(image.Data, image.MimeType);

            return GetDefaultLotImage();
        }

        public FileResult GetSmallImage(int id)
        {
            var image = _repository.GetImageById(id);

            if (image != null)
                return File(image.CompressedData, image.MimeType);

            return GetDefaultLotImage();
        }

        #endregion


        #region About, Contact

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

        #endregion


        #region Private

        private static byte[] ResizeImage(byte[] img)
        {
            var bigImg = new Bitmap(new MemoryStream(img));

            var smallImg = new Bitmap(bigImg, new Size(300, 300));

            var converter = new ImageConverter();

            return (byte[])converter.ConvertTo(smallImg, typeof(byte[]));
        }

        private FileResult GetDefaultLotImage()
        {
            var path = Server.MapPath("~/App_Data/Images/Default/Photo.jpg");

            return File(path, "image/png");
        }

        #endregion

    }
}
