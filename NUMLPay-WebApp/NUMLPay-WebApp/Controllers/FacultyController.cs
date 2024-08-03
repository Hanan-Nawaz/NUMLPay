using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4, 1 })]
    public class FacultyController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        apiService apiServices;

        public FacultyController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        //Add Faculty
        public async Task<ActionResult> addFaculty()
        {
            Admin admin = userAccessAdmin();

            await getAccessAccordingtoRole(admin);

            ViewBag.Display = "none;";
            return View();
        }

        //Add Faculty Post
        [HttpPost]
        public async Task<ActionResult> addFaculty(Faculty faculty)
        {
            Admin admin = userAccessAdmin();

            await getAccessAccordingtoRole(admin);

            faculty.added_by = admin.email_id;
            faculty.is_active = 1;
            DateTime now = DateTime.Now;
            faculty.date = now.Date;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Faculty", faculty);

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

        //View all Faculty
        public async Task<ActionResult> viewFaculties()
        {
            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            Admin admin = userAccessAdmin();

            List<FacultyView> listFaculty = await giveRealtiveData(admin.campus_id);

            return View(listFaculty);
        }

        //Update Faculty
        public async Task<ActionResult> updateFaculty(int Id)
        {
            userAccessAdmin();

            Faculty faculty = null;

            using (var response = await apiServices.GetAsync($"Faculty/GetbyId/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    faculty = JsonConvert.DeserializeObject<Faculty>(data);
                }
            }

            ViewBag.Display = "none;";

            int? selectedValue = faculty.is_active;
            int? selectedValuecampus = faculty.campus_id;

            ViewBag.campusList = await campusService.getSelectedCampus(selectedValuecampus);

            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(faculty);
        }

        //Update Faculty
        [HttpPost]
        public async Task<ActionResult> updateFaculty(Faculty faculty)
        {
            Admin admin = userAccessAdmin();
            
            ViewBag.campusList = await campusService.addCampustoListAsync();

            ViewBag.Display = "none;";

            var date = DateTime.Now;
            faculty.date = date.Date;

            faculty.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Faculty/updateFaculty/{faculty.id}", faculty);

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

            List<FacultyView> listFaculty = await giveRealtiveData(admin.campus_id);

            return View("viewFaculties", listFaculty);
        }


        //Delete Faculty
        public async Task<ActionResult> deleteFaculty(int? Id)
        {
            Admin admin = userAccessAdmin();

            List<FacultyView> listFaculty = await giveRealtiveData(admin.campus_id);

            if (!Id.HasValue)
            {
                return RedirectToAction("viewFaculties");
            }
            else
            {
                Faculty faculty = new Faculty();
                faculty.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Faculty/inActive/{Id}", faculty);

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

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewFaculties", listFaculty);
        }

        public async Task<List<FacultyView>> giveRealtiveData(int? campusId)
        {
            List<FacultyView> listFaculty = null;

            if (campusId.HasValue)
            {
                listFaculty = await facultyServices.getFacultybyCampusAsync((int) campusId);
            }
            else
            {
                listFaculty = await facultyServices.getfacultyAsync();
            }

            return listFaculty;

        }

        public async Task<bool> getAccessAccordingtoRole(Admin admin)
        {
            SelectList allCampusesSelectList = await campusService.addCampustoListAsync();

            if (admin.campus_id != null)
            {
                int campusId = (int)admin.campus_id;
                var selectedCampus = allCampusesSelectList
                .FirstOrDefault(item => int.Parse(item.Value) == campusId);
                var filteredCampuses = new List<SelectListItem> { selectedCampus };
                ViewBag.campusList = new SelectList(filteredCampuses, "Value", "Text");
            }
            else
            {
                ViewBag.campusList = allCampusesSelectList;
            }

            return true;
        }
    }
}