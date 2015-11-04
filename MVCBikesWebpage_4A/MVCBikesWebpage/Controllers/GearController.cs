using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBikesWebpage;

namespace MVCBikesWebpage.Models
{
    public class GearController : Controller
    {
        private DBEntities db = new DBEntities();
        private ActionResult GetFromProductCategories(int id)
        {

            var clothing = (from row in db.ProductCategories
                            join prod in db.Products on row.ProductCategoryID equals prod.ProductCategoryID
                            where
                                row.ParentProductCategoryID == id && prod.SellEndDate == null
                            select row).Distinct();
            return View(clothing);
        }

        // GET: Gear
        public ActionResult Index()
        {
            var productCategories = from row in db.ProductCategories
                                    where row.ProductCategoryID >= 2 && row.ProductCategoryID <= 4
                                    select row;
            return View(productCategories);
        }

        public ActionResult Clothing()
        {
            return (GetFromProductCategories(3));
        }

        public ActionResult Accessories()
        {
            return (GetFromProductCategories(4));
        }
        public ActionResult Components()
        {
            return (GetFromProductCategories(2));
        }

        public ActionResult Details(int id)
        {
            var products = from row in db.Products where row.ProductCategoryID == id select row;

            ViewBag.DetailName = (from row in db.ProductCategories join prod in db.Products on row.ProductCategoryID equals prod.ProductCategoryID where prod.ProductCategoryID == id select row.Name).First();
            return View(products);
        }
    }
}
