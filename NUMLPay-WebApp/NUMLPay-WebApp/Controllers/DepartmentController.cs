using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.ViewModel;
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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4, 1 })]
    public class DepartmentController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService departmentServices;
        apiService apiServices;

        public DepartmentController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            departmentServices = new departmentService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add Department
        public async Task<ActionResult> addDepartment()
        {
           Admin admin = userAccessAdmin();

            await getAccessAccordingtoRole(admin);

            ViewBag.Display = "none;";
            return View();
        }

        // Add Department Post
        [HttpPost]
        public async Task<ActionResult> addDepartment(Department department)
        {
            Admin admin = userAccessAdmin();

            await getAccessAccordingtoRole(admin);

            department.added_by = admin.email_id;
            department.is_active = 1;
            DateTime now = DateTime.Now;
            department.date = now.Date;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Department", department);

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

        // View all Departments
        public async Task<ActionResult> viewDepartments()
        {
            List<DeptView> listDepartment = null;
            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            Admin admin = userAccessAdmin();

            if(admin.role == 4)
            {
               listDepartment = await giveRealtiveData(null);

            }
            else
            {
                listDepartment = await giveRealtiveData(admin.campus_id);

            }


            return View(listDepartment);
        }

        // Update Department
        public async Task<ActionResult> updateDepartment(int Id)
        {
           userAccessAdmin();

            Department department = null;

            using (var response = await apiServices.GetAsync($"Department/GetbyId/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    department = JsonConvert.DeserializeObject<Department>(data);
                }
            }

            ViewBag.Display = "none;";

            int? selectedValue = department.is_active;
            int? selectedValueFaculty = department.faculty_id;

            Tuple<int?, List<SelectListItem>> selectedFaculty =  await facultyServices.getSelectedFaculties(selectedValueFaculty);
            ViewBag.facultyList = selectedFaculty.Item2;
            ViewBag.campusList = await campusService.getSelectedCampus(selectedFaculty.Item1);

            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(department);
        }

        // Update Department
        [HttpPost]
        public async Task<ActionResult> updateDepartment(Department department)
        {
          
            Admin admin = userAccessAdmin();

            ViewBag.campusList = await campusService.addCampustoListAsync();

            ViewBag.Display = "none;";

            var date = DateTime.Now;
            department.date = date.Date;

            department.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Department/updateDepartment/{department.id}", department);

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


            List<DeptView> listDepartment = await giveRealtiveData(admin.campus_id);

            return View("viewDepartments", listDepartment);
        }

        // Delete Department
        public async Task<ActionResult> deleteDepartment(int? Id)
        {
           Admin admin = userAccessAdmin();

            List<DeptView> listDepartment = await giveRealtiveData(admin.campus_id);

            if (!Id.HasValue)
            {
                return RedirectToAction("viewDepartments");
            }
            else
            {
                Department department = new Department();
                department.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Department/inActive/{Id}", department);

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

            return RedirectToAction("viewDepartments", listDepartment);
        }

        public async Task<List<DeptView>> giveRealtiveData(int? campusId)
        {
            List<DeptView> listDepartment = null;

            if (campusId.HasValue)
            {
                listDepartment = await departmentServices.getDepartmentAsync(campusId); ;
            }
            else
            {
                listDepartment = await departmentServices.getDepartmentAsync(null); ;
            }

            return listDepartment;

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
