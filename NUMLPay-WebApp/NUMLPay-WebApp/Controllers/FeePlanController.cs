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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 2, 4 })]
    public class FeePlanController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        usersTypeService usersTypeService;
        statusService StatusService;
        feePlanService feePlanService;
        apiService apiServices;

        public FeePlanController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            feePlanService = new feePlanService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add Fee Plan
        public async Task<ActionResult> addFeePlan()
        {
            userAccessAdmin();
            ViewBag.Display = "none;";
            return View();
        }

        // Add Fee Plan Post
        [HttpPost]
        public async Task<ActionResult> addFeePlan(FeePlan feePlan)
        {
            Admin admin = userAccessAdmin();

            feePlan.added_by = admin.email_id;
            feePlan.is_active = 1;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("FeePlan", feePlan);

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
        }

        // View all Fee Plans
        public async Task<ActionResult> viewFeePlans()
        {
            userAccessAdmin();

            List<FeePlan> listFeePlans = await feePlanService.getFeePlanAsync();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listFeePlans);
        }

        // Update Fee Plan
        public async Task<ActionResult> updateFeePlan(int Id)
        {
            userAccessAdmin();
            FeePlan feePlan = await feePlanService.getFeePlanAsync(Id);
            ViewBag.Display = "none;";

            int? selectedValue = feePlan.is_active;
            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(feePlan);
        }

        // Update Fee Plan Post
        [HttpPost]
        public async Task<ActionResult> updateFeePlan(FeePlan feePlan)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            int? selectedValue = feePlan.is_active;
            ViewBag.is_active = StatusService.getStatus(selectedValue);

            feePlan.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"FeePlan/updateFeePlan/{feePlan.id}", feePlan);

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

            List<FeePlan> listFeePlans = await feePlanService.getFeePlanAsync();
            return RedirectToAction("viewFeePlans", listFeePlans);
        }

        // Delete Fee Plan
        public async Task<ActionResult> deleteFeePlan(int? Id)
        {
            userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewFeePlans");
            }
            else
            {
                FeePlan feePlan = new FeePlan();
                feePlan.is_active = 2;
                HttpResponseMessage responseMessage = await apiServices.PutAsync($"FeePlan/inActive/{Id}", feePlan);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }
                else
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-success";
                    ViewBag.Display = "block;";
                }
            }

            List<FeePlan> listFeePlans = await feePlanService.getFeePlanAsync();

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewFeePlans", listFeePlans);
        }
    }

}