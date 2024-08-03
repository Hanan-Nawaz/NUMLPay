using NUMLPay_WebApp.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class CustomAuthorizationFilter : ActionFilterAttribute
    {
        private readonly usersTypeService _usersTypeService = new usersTypeService();

        private readonly bool _requireAdmin;
        private readonly bool _requireUser;
        private readonly int[] _requiredRole;

        public CustomAuthorizationFilter(bool requireAdmin = false, bool requireUser = false, int[] requiredRole = null)
        {
            _requireAdmin = requireAdmin;
            _requireUser = requireUser;
            _requiredRole = requiredRole ?? new int[0];
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;

            bool isAdmin = IsAuthorizedAdmin(session);
            bool isUser = IsAuthorizedUser(session);

            if (_requireAdmin && !isAdmin)
            {
                filterContext.Result = new RedirectResult("~/Home/loginAdmin");
            }
            else if (_requireUser && !isUser)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
            else if (_requiredRole.Length != 0 && !IsAuthorizedRole(session, _requiredRole))
            {
                filterContext.Result = new RedirectResult("~/Home/loginAdmin");
            }
        }

        private bool IsAuthorizedAdmin(HttpSessionStateBase session)
        {
            Tuple<string, Users, Admin> userTuple = _usersTypeService.checkUserType(session);
            return userTuple.Item3 != null;
        }

        private bool IsAuthorizedUser(HttpSessionStateBase session)
        {
            Tuple<string, Users, Admin> userTuple = _usersTypeService.checkUserType(session);
            return userTuple.Item2 != null;
        }


        private bool IsAuthorizedRole(HttpSessionStateBase session, params int[] requiredRoles)
        {
            Tuple<string, Users, Admin> userTuple = _usersTypeService.checkUserType(session);
            Admin admin = userTuple.Item3;

            if (admin != null)
            {
                foreach (int requiredRole in requiredRoles)
                {
                    if (admin.role == requiredRole)
                    {
                        return true; 
                    }
                }
            }

            return false; 
        }


    }
}
