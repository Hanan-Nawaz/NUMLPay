using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4, 1 })]
    public class DeleteChallanController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        apiService apiServices;

        public DeleteChallanController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            apiServices = new apiService(baseAddress.ToString());
        }

        // Delete Challan
        public async Task<ActionResult> DeleteChallan()
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View();
        }

        // Delete Challan
        public async Task<ActionResult> DeleteChallans(int ChallanNumber)
        {
            Admin admin = userAccessAdmin();

            HttpResponseMessage responseMessage = await apiServices.GetAsync($"DeleteChallan/Delete/{ChallanNumber}");

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                TempData["AlertMessage"] = responseData;
                TempData["AlertType"] = "alert-success";
                TempData["Display"] = "block;";
            }
            else
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                TempData["AlertMessage"] = responseData;
                TempData["AlertType"] = "alert-danger";
                TempData["Display"] = "block;";
            }

            return RedirectToAction("DeleteChallan");
        }
    }
}