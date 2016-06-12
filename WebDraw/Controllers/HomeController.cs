using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDraw.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ID = User.Identity.GetUserId();
            if (ViewBag.Id == null)
                ViewBag.Id = "Guest";
            return View();
        }

    }
}