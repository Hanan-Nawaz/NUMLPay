using Microsoft.Extensions.Configuration;
using NUMLPay_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class usersTypeService
    {
        //check User is admin or Student 
        public Tuple<string, Users, Admin> checkUserType(HttpSessionStateBase session)
        {
            Admin admin = null;
            Users user = null;
            if (Convert.ToInt32(session["As"]) == 1)
            {
                user = session["loggedUser"] as Users;
                return Tuple.Create("1", user, admin);
            }
            else
            {
                admin = session["loggedUser"] as Admin;
                return Tuple.Create("2", user, admin);
            }
        }
    }
}