using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBikesWebpage.Controllers
{
    public class SearchController : Controller
    {
        private DBEntities db = new DBEntities();
        // GET: Search
        public ActionResult Index(string search)
        {
            var products = (from prod in db.Products
                           join
                               cate in db.ProductCategories on
                               prod.ProductCategoryID equals cate.ProductCategoryID
                           where prod.Name == search || prod.ProductModel.Name == search || cate.Name == search
                           select prod).Distinct();
            return View(products.ToList());
        }
    }
}