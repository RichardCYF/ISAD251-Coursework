﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISAD251_Coursework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Customer()
        {
            ViewBag.Message = "Customer Page";

            return View();
        }

        public ActionResult Admin()
        {
            ViewBag.Message = "Admin Page";

            return View();
        }
    }
}