using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XSSApplication.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult ReflectiveXSS() {
            ViewBag.Title = "Reflective XSS";
            return View();
        }

        public ActionResult PersistentXSS() {
            ViewBag.Title = "Persistent XSS";
            return View();
        }

        public ActionResult DOMXSS() {
            ViewBag.Title = "DOM XSS";
            return View();
        }

        public ActionResult FixedDOMXSS() {
            ViewBag.Title = "Fixed DOM XSS";
            return View();
        }
        public ActionResult FixedReflectiveXSS() {
            ViewBag.Title = "Fixed Reflective XSS";
            return View();
        }
        public ActionResult FixedPersistentXSS() {
            ViewBag.Title = "Fixed Persistent XSS";
            return View();
        }
    }
}
