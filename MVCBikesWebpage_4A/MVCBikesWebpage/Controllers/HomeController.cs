﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBikesWebpage;

namespace MVCBikesWebpage.Controllers
{
    public class HomeController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Home
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
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
