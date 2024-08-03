using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 2 })]
    public class ChallanController : SessionController
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

        public ChallanController()
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

        // Add Challan
        public async Task<ActionResult> addChallan()
        {
            userAccessAdmin();

            ViewBag.Display = "none;";
            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(null), "value", "text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);
            ViewBag.session = await sessionServices.addSessiontoListAsync(null);
            return View();
        }

        // Add Challan Post
        [HttpPost]
        public async Task<ActionResult> addChallan(Challans challan)
        {
            Admin admin = userAccessAdmin();

            challan.added_by = admin.email_id;
            challan.is_active = 1;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Challan", challan);

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

            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(null), "value", "text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);
            ViewBag.session = await sessionServices.addSessiontoListAsync(null);

            return View();
        }

        // View all Challans
        public async Task<ActionResult> viewChallans()
        {
            userAccessAdmin();

            List<dynamic> listChallans = await GetChallans();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listChallans);
        }

        // Update Challan
        public async Task<ActionResult> updateChallan(int Id)
        {
            userAccessAdmin();

            Challans challan = null;

            HttpResponseMessage responseMessage = await apiServices.GetAsync($"Challan/{Id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                challan = JsonConvert.DeserializeObject<Challans>(responseData);
            }

            DateTime dValid = Convert.ToDateTime(challan.valid_date);
            DateTime dDue = Convert.ToDateTime(challan.due_date);

            challan.valid_date = dValid.ToString("yyyy-MM-dd");
            challan.due_date = dDue.ToString("yyyy-MM-dd");

            int? selectedValue = challan.is_active;

            ViewBag.is_active = StatusService.getStatus(selectedValue);

            ViewBag.Display = "none;";
            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(challan.fee_for), "value", "text");

            ViewBag.Sessions = await sessionServices.addSessiontoListAsync(challan.session);
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(challan.admissison_session);

            return View(challan);
        }

        // Update Challan Post
        [HttpPost]
        public async Task<ActionResult> updateChallan(Challans challan)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            challan.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Challan/{challan.challan_id}", challan);

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

            ViewBag.feeFor = StatusService.getFeeFor(null);
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(challan.session);
            ViewBag.session = await sessionServices.addSessiontoListAsync(challan.session);

            List<dynamic> listChallans = await GetChallans();

            return View("viewChallans", listChallans);
        }

        // Delete Challan
        public async Task<ActionResult> deleteChallan(int? Id)
        {
            userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewChallans");
            }
            else
            {
                Challans challan = new Challans();
                challan.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Challan/inActive/{Id}", challan);

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

            List<dynamic> listChallans = await GetChallans();

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewChallans", listChallans);
        }

        private async Task<List<dynamic>> GetChallans()
        {
            List<dynamic> listChallans = null;
;            HttpResponseMessage responseMessage = await apiServices.GetAsync("Challan");
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listChallans = JsonConvert.DeserializeObject<List<dynamic>>(responseData);
            }
            return listChallans;
        }
    }
}