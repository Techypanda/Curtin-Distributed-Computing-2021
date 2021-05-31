using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Business_Webservice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Users()
        {
            ViewBag.Message = "Your Users Page";
            return View();
        }
        public ActionResult Accounts()
        {
            ViewBag.Message = "Your Accounts Page";
            return View();
        }
        public ActionResult Transactions()
        {
            ViewBag.Message = "Your Transactions Page";
            return View();
        }
        public ActionResult XSS()
        {
            ViewBag.Message = "Stefan's XSS Page";
            return View();
        }
    }
}