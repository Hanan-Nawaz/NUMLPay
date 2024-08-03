using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NUMLPay_Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
               name: "Api2param",
               routeTemplate: "api/{controller}/{Id}/{Password}",
               defaults: new { Id = RouteParameter.Optional, numlPassword = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
               name: "generateChallan",
               routeTemplate: "api/{controller}/{sessionId}/{feeFor}/{mode}/{shiftId}/{admissionSession}/{feePlan}",
               defaults: new { controller = RouteParameter.Optional, feeFor = RouteParameter.Optional, mode = RouteParameter.Optional, shiftId = RouteParameter.Optional, admissionSession = RouteParameter.Optional, feePlan = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
               name: "ApiparamMethod",
               routeTemplate: "api/{controller}/{campusId}/Degrees",
               defaults: new { Id = RouteParameter.Optional, numlPassword = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
              name: "Api3param",
              routeTemplate: "api/{controller}/{Method}/{Id}/{Password}",
              defaults: new { Id = RouteParameter.Optional, numlPassword = RouteParameter.Optional }
          );

            config.Routes.MapHttpRoute(
              name: "APISubFee",
              routeTemplate: "api/{controller}/{Method}/{id}/{Cursem}/{feeType}/{numlId}/{feeFor}",
              defaults: new { id = RouteParameter.Optional, Cursem = RouteParameter.Optional, feeType = RouteParameter.Optional, numlId = RouteParameter.Optional, feeFor = RouteParameter.Optional }
          );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


        }
    }
}
