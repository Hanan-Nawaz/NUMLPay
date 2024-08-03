using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class rolesService
    {
        //All Roles
        public SelectList getRoles(int id)
        {
            List<SelectListItem> roleOptions = new List<SelectListItem>
            {
                
                
                new SelectListItem { Value = "3", Text = "Department Admin" },
                // Add more role options as needed
            };

            if(id == 4)
            {
                roleOptions.Add(new SelectListItem { Value = "1", Text = "Campus Admin" });
                roleOptions.Add(new SelectListItem { Value = "2", Text = "Accountant Admin" });
            }

            SelectList rolesList = new SelectList(roleOptions, "Value", "Text");
            return rolesList;
        }

        //All Roles
        public SelectList getAdminRoles()
        {
            List<SelectListItem> roleOptions = new List<SelectListItem>
            {   new SelectListItem { Value = "1", Text = "Campus Admin" },
                new SelectListItem { Value = "2", Text = "Accountant Admin" },
                new SelectListItem { Value = "3", Text = "Department Admin" },
                new SelectListItem { Value = "4", Text = "Super Admin" },
                // Add more role options as needed
            };

            SelectList rolesList = new SelectList(roleOptions, "Value", "Text");
            return rolesList;
        }
    }
}