using Newtonsoft.Json;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4 })]
    public class CampusController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService deptService;
        apiService apiServices;

        public CampusController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            deptService = new departmentService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        //Add Campus
        public async Task<ActionResult> addCampus()
        {
            userAccessAdmin();
            ViewBag.Display = "none;";
            return View();
        }

        //Add Campus Post
        [HttpPost]
        public async Task<ActionResult> addCampus(Campus campus)
        {
            Admin admin = userAccessAdmin(); 

            campus.added_by = admin.email_id;
            campus.is_active = 1;
            DateTime now = DateTime.Now;
            campus.date = now.Date;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Campus", campus);

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

        //View all Campuses
        public async Task<ActionResult> viewCampuses()
        {
            userAccessAdmin();

            List<Campus> listCampuses = await campusService.getCampusAsync();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listCampuses);
        }

        //Update Campus
        public async Task<ActionResult> updateCampus(int Id)
        {
            userAccessAdmin();
            Campus campus = await campusService.getCampusAsync(Id);
            ViewBag.Display = "none;";

            int? selectedValue = campus.is_active;

            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(campus);
        }

        //Update Campus
        [HttpPost]
        public async Task<ActionResult> updateCampus(Campus campus)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            var date = DateTime.Now;
            campus.date = date.Date;

            campus.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Campus/updateCampus/{campus.Id}", campus);

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

            List<Campus> listCampuses = await campusService.getCampusAsync();
            return View("viewCampuses", listCampuses);

        }


        //Delete Campus
        public async Task<ActionResult> deleteCampus(int? Id)
        {
            userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewCampuses");
            }
            else
            {

                Campus campus = new Campus();
                campus.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Campus/inActive/{Id}", campus);

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

            List<Campus> listCampuses = await campusService.getCampusAsync();

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewCampuses", listCampuses);
        }
    }
}