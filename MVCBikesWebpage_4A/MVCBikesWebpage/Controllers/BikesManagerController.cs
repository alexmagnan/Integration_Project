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
using System.IO;

namespace MVCBikesWebpage.Controllers
{
   
    public class BikesManagerController : Controller
    {
        private DBEntities db = new DBEntities();

        private IQueryable<Product> getProducts(){

            return (from row in db.Products
                           where row.ProductCategoryID >= 5 &&
                               row.ProductCategoryID <= 7
                           select row);
        }

        private IQueryable<ProductCategory> getProductCategories()
        {
            return (from row in db.ProductCategories
                   where row.ParentProductCategoryID == 1
                   select row);
        }
        private List<BikeListModel> getBikes(){
            var bikes = (from row in db.vProductAndDescriptions
             where
                 row.Culture == "en" && row.ProductCategoryID >= 5 && row.ProductCategoryID <= 7
             select new
             {
                 ProductModel = row.ProductModel,
                 Description = row.Description,
                 ProductModelID = row.ProductModelID
             }).Distinct();

            List<BikeListModel> list = new List<BikeListModel>();
            foreach (var item in bikes)
            {
                list.Add(new BikeListModel(item.ProductModel, item.Description, item.ProductModelID));
            }
            return list;
        }

        // GET: BikesManager
        public ActionResult Index()
        {
            
            return View(getProducts());
        }

        // GET: BikesManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
           
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: BikesManager/Create
        public ActionResult Create()
        {
            var productCategories = getProductCategories();

            var bikes = getBikes();

            
            ViewBag.ProductCategoryID = new SelectList(productCategories, "ProductCategoryID", "Name");
            ViewBag.ProductModelID = new SelectList(bikes, "ProductModelID", "ProductModel");
            return View();
        }

        // POST: BikesManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryID,ProductModelID,SellStartDate,SellEndDate,DiscontinuedDate,ThumbNailPhoto,ThumbnailPhotoFileName,rowguid,ModifiedDate")] Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                bool valid = true;
                product.rowguid = Guid.NewGuid();
                product.ModifiedDate = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd"));

                // Validate for uniquess of NAME
                var uniqueName = (from row in db.Products where row.Name == product.Name select row).Count();
                if(uniqueName != 0)
                {   valid = false;
                    ModelState.AddModelError("Name", "This name already exists !");
                }
                // Validate for uniquess of PRODUCT NUMBER
                var uniqueProdNo = (from row in db.Products where row.ProductNumber == product.ProductNumber select row).Count();
                if (uniqueProdNo != 0)
                {   valid = false;
                    ModelState.AddModelError("ProductNumber", "This Product Number already exists !");
                } 

                // Validate that list price cannot be lower or equal than standard cost
                if (product.ListPrice < product.StandardCost)
                {   valid = false;
                    ModelState.AddModelError("ListPrice", "List Price cannot be lower than Standard Cost !");
                }


                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));

                        // file is uploaded
                        file.SaveAs(path);

                        // save the image path path to the database or you can send image
                        // directly to database
                        // in-case if you want to store byte[] ie. for DB
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            product.ThumbNailPhoto = array;
                        }

                        product.ThumbnailPhotoFileName = file.FileName;
                    }
                    catch (Exception ex)
                    {
                        valid = false;
                        ModelState.AddModelError("ThumbNailPhoto", "Error uploading your image !");
                    }      
                }
                else
                {
                    string base64 = "R0lGODlhUAAxAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf//zP///yH5BAEAABAALAAAAABQADEAAAj/AP8JHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3Mixo8ePIEOKHEmypMmTKFOqXJkRBYqBLhfGZPnQ5ct/MxPmpMnQpsCZNm/CfBnTZ86gQ3HeRMoRadGlQpUqJfoUZ9KnVH9GxVhUKtCoVaWKnZrVK9SmVMPuVHvWrFisPjd+LbuW7tmvb8t6nJuXIFutfbH2lSt07ta/eeOy3clTYuGtjS8yjUy5suXLmDHHdRjWIGPGIjdDBA3YL2SQVY+mvQsVL16yqLOqfuyWtlHZbTv+nY176G67H38DTs068GrSkoMSN+62+fKQqrW2Xe6aem7CSaf6fq7ceevTmcOLEh9Pvrz58+jTq1/Pvr379+8DAgA7";
                    product.ThumbNailPhoto = Convert.FromBase64String(base64);
                }
                                   

                if (valid)
                {   db.Products.Add(product);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                    
            }

            var productCategories = getProductCategories();
            var bikes = getBikes();

            ViewBag.ProductCategoryID = new SelectList(productCategories, "ProductCategoryID", "Name");
            ViewBag.ProductModelID = new SelectList(bikes, "ProductModelID", "ProductModel");
            return View(product);
        }

        // GET: BikesManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
            ViewBag.ProductModelID = new SelectList(db.ProductModels, "ProductModelID", "Name", product.ProductModelID);
            return View(product);
        }

        // POST: BikesManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryID,ProductModelID,SellStartDate,SellEndDate,DiscontinuedDate,ThumbNailPhoto,ThumbnailPhotoFileName,rowguid,ModifiedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
            ViewBag.ProductModelID = new SelectList(db.ProductModels, "ProductModelID", "Name", product.ProductModelID);
            return View(product);
        }

        // GET: BikesManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: BikesManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
