using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenLightSharp.Models;

namespace GreenLightSharp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Help()
        {
            ViewBag.Message = "How to use this app.";
            return View();
        }

        [HttpPost]
        public ActionResult AddBand(Show model)
        {
            //Create band with a given name
            //Go to AddMember view with bandId showing
            return View();
        }

        [HttpPost]
        public ActionResult Join(Member model)
        {
            //Goes to add a member page with given bandId
            return View();
        }

        public ActionResult AddMember(string bandID)
        {
            //Add Member page
            return View();
        }

        [HttpPost]
        public ActionResult AddMember(Member model)
        {
            //Add Member
            //Goes to show page
            return View();
        }

        public ActionResult ShowPage(Show model)
        {
            //Where everything is displayed and updated
            return View();
        }
    }
}