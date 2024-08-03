using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    public class SessionController : Controller
    {
        usersTypeService usersTypeService = new usersTypeService();

        public Admin userAccessAdmin()
        {
            Tuple<string, Users, Admin> userTuple = usersTypeService.checkUserType(Session);

                ViewBag.adminRole = userTuple.Item3.role.ToString();

                ViewBag.roleAssignmet = userTuple.Item1;

                return userTuple.Item3;
        }

        public Users userAccess()
        {
            Tuple<string, Users, Admin> userTuple = usersTypeService.checkUserType(Session);

            ViewBag.adminRole = usersTypeService.checkUserType(Session);

            ViewBag.roleAssignmet = userTuple.Item1;

            return userTuple.Item2;
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Home");
        }
    }
}