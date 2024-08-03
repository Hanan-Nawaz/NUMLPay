using Newtonsoft.Json;
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
    public class FineController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService deptService;
        sessionService sessionServices;
        apiService apiServices;

        public FineController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            deptService = new departmentService(baseAddress);
            sessionServices = new sessionService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add Fine
        public async Task<ActionResult> addFine()
        {
            userAccessAdmin();
            ViewBag.Display = "none;";

            ViewBag.feeFor = new SelectList(StatusService.getFineFor(null), "value", "text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            return View();
        }

        // Add Fine - POST
        [HttpPost]
        public async Task<ActionResult> addFine(Fines fine)
        {
            Admin admin = userAccessAdmin();

            ViewBag.feeFor = new SelectList(StatusService.getFineFor(null), "value", "text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Fine", fine);

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

        // View all Fines
        public async Task<ActionResult> viewFines()
        {
            userAccessAdmin();

            List<dynamic> listFines = await getData();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listFines);
        }

        // Update Fine
        public async Task<ActionResult> updateFine(int Id)
        {
            userAccessAdmin();
            Fines fine = null;

            using (var response = await apiServices.GetAsync($"Fine/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fine = JsonConvert.DeserializeObject<Fines>(data);
                }
            }

            ViewBag.feeFor = new SelectList(StatusService.getFineFor(fine.fine_for), "value", "text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(fine.session);

            ViewBag.Display = "none;";

            return View(fine);
        }

        // Update Fine - POST
        [HttpPost]
        public async Task<ActionResult> updateFine(Fines fine)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            ViewBag.feeFor = new SelectList(StatusService.getFineFor(fine.fine_for), "value", "text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(fine.session);

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Fine/{fine.id}", fine);

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

            List<dynamic> listFines = await getData();
            return View("viewFines", listFines);
        }


        public async Task<List<dynamic>> getData()
        {
            List<dynamic> listFines = null;

            using (var response = await apiServices.GetAsync($"Fine"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFines = JsonConvert.DeserializeObject<List<dynamic>>(data);
                }
            }

            return listFines;
        }
    }
}