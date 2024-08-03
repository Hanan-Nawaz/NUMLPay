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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 2 })]
    public class DescriptionController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        descriptionService descService;
        usersTypeService usersTypeService;
        statusService StatusService;
        apiService apiServices;

        public DescriptionController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            descService = new descriptionService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add Description
        public async Task<ActionResult> AddDescription()
        {
            userAccessAdmin();
            ViewBag.Display = "none;";
            return View();
        }

        // Add Description Post
        [HttpPost]
        public async Task<ActionResult> AddDescription(Description description)
        {
            Admin admin = userAccessAdmin();

            description.is_active = 1;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Description", description);


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

        // View all Descriptions
        public async Task<ActionResult> viewDescriptions()
        {
            userAccessAdmin();

            List<Description> listDescriptions = await descService.getDescAsync();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listDescriptions);
        }

        // Update Description
        public async Task<ActionResult> updateDescription(int Id)
        {
            userAccessAdmin();
            Description description = await descService.getDescriptionAsync(Id);
            ViewBag.Display = "none;";

            int? selectedValue = description.is_active;

            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(description);
        }

        // Update Description Post
        [HttpPost]
        public async Task<ActionResult> updateDescription(Description description)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Description/{description.id}", description);

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

            List<Description> listDescriptions = await descService.getDescAsync();
            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;
            return View("viewDescriptions", listDescriptions);
        }

        //Delete Campus
        public async Task<ActionResult> deleteDescription(int? Id)
        {
            userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewDescriptions");
            }
            else
            {

                Description desc = new Description();
                desc.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Description/inActive/{Id}", desc);

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

            List<Description> listDesc = await descService.getDescAsync();

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewDescriptions", listDesc);
        }
    }
}