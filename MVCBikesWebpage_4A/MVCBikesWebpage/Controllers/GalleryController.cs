using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVCBikesWebpage.Controllers
{
    public class GalleryController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: Gallery
        public ActionResult Index()
        {
            List<Gallery> galleryImages = db.Galleries.ToList();
            return View(galleryImages);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string page, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));

                    // file is uploaded
                    file.SaveAs(path);

                    db.Galleries.Add(new Gallery() { FileName = "/images/" + Path.GetFileName(file.FileName), Page = page });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Image", "Error uploading your image !");
                }

                List<Gallery> galleryImages = db.Galleries.ToList();
                return View("Index", galleryImages);
            }
            else
            {
                ModelState.AddModelError("Image", "Select Image!");

                return View();
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View("Delete", model: db.Galleries.FirstOrDefault(g => g.Id == id).FileName);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            db.Galleries.Remove(db.Galleries.FirstOrDefault(g => g.Id == id));
            db.SaveChanges();

            List<Gallery> galleryImages = db.Galleries.ToList();
            return View("Index", galleryImages);
        }
    }
}