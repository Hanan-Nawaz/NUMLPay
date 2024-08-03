using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    public class RepeatCourseFeeController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        sessionService sessionServices;
        apiService apiServices;

        public RepeatCourseFeeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            sessionServices = new sessionService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        public async Task<ActionResult> AddRepeatCourse()
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddRepeatCourse(UnpaidFees newChallan)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            HttpResponseMessage responseMessage = await apiServices.PostAsync($"GenerateChallan/GenerateRepeatChallans/{admin.dept_id}", newChallan);

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-success";
                ViewBag.Display = "block;";
            }
            else
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
            }

            return View();

            return View();
        }
    }
}