using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBikesWebpage;
using MVCBikesWebpage.Models;

namespace MVCBikesWebpage.Controllers
{
    public class BikesController : Controller
    {
        private DBEntities db = new DBEntities();

        private ActionResult GetFromVProductsAndDescriptions(int id)
        {
            var bikes = (from row in db.vProductAndDescriptions
                         where
                             row.Culture == "en" && row.ProductCategoryID == id
                             && row.SellEndDate == null
                         select new
                         {
                             ProductModel = row.ProductModel,
                             Description = row.Description,
                             ProductModelID = row.ProductModelID
                         }).Distinct();
            List<BikeListModel> list = new List<BikeListModel>();
            foreach (var item in bikes)
            {
                list.Add(new BikeListModel(item.ProductModel, item.Description, item.ProductModelID.Value)); 
            }

            return View(list);
        }

        // GET: Bikes
        public ActionResult Index()
        {
            var productCategories =  from row in db.ProductCategories where row.ParentProductCategoryID == 1 select row;
            List<string> galleryImages = db.Galleries.Where(g => g.Page == "Bikes").Select(g => g.FileName).ToList();
            return View(new GalleryGridViewModel() { Products = productCategories.ToList(), Gallery = galleryImages });
        }

        public ActionResult Mountain()
        {

            return GetFromVProductsAndDescriptions(5);
        }
        public ActionResult Road()
        {
            return GetFromVProductsAndDescriptions(6);
        }

        public ActionResult Touring()
        {
            return GetFromVProductsAndDescriptions(7);
        }

        public ActionResult Details(int id){

            var products = from row in db.Products where row.ProductModelID == id && row.SellEndDate == null select row;
            ViewBag.DetailName = (from row in db.vProductAndDescriptions join prod in db.Products on row.ProductModelID equals prod.ProductModelID where prod.ProductModelID == id select row.ProductModel).First();
            return View(products);
        }

    }

    public class GalleryGridViewModel
    {
        public List<string> Gallery { get; set; }

        public List<ProductCategory> Products { get; set; }
    }
}
