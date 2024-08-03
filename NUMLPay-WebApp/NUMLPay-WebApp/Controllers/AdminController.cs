using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4, 1 })]
    public class AdminController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService departmentServices;
        apiService apiServices;
        rolesService roleService;

        public AdminController()
        {
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            departmentServices = new departmentService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
            roleService = new rolesService();
        }

        //Add Admin
        public async Task<ActionResult> addAdmin()
        {
           Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            SelectList allCampusesSelectList = await campusService.addCampustoListAsync();            

            if (admin.campus_id != null)
            {
                int campusId = (int) admin.campus_id;
                var selectedCampus = allCampusesSelectList
                .FirstOrDefault(item => int.Parse(item.Value) == campusId);
                var filteredCampuses = new List<SelectListItem> { selectedCampus };
                ViewBag.campusList = new SelectList(filteredCampuses, "Value", "Text");
            }
            else
            {
                ViewBag.campusList = allCampusesSelectList;
            }

            ViewBag.roles = roleService.getRoles();

            return View();
        }

        //Add Admin Post
        [HttpPost]
        public async Task<ActionResult> addAdmin(Admin admin)
        {    
            Admin loggedAdmin = userAccessAdmin();

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

            ViewBag.roles = roleService.getRoles();

            if (!ModelState.IsValid)
            {
                return View(admin);
            }
            else
            {
                admin.added_by = loggedAdmin.email_id;
                admin.is_active = 1;

                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(admin.password, salt);

                admin.password = hashedPassword;

                HttpResponseMessage responseMessage  = await apiServices.PostAsync("Admin", admin);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-success";
                    ViewBag.Display = "block;";
                    admin = null;
                }
                else
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }
            }

            return View();
        }

        // View all Admins
        public async Task<ActionResult> viewAdmins()
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            List<AdminView> listAdmin = await giveRealtiveData(admin.campus_id);

            return View(listAdmin);
        }

        // Update Admin
        public async Task<ActionResult> updateAdmin(string Id)
        {
            Admin admin = userAccessAdmin();

            Admin adminId = new Admin();
            adminId.email_id = HttpUtility.UrlDecode(Id);

            using (var response = await apiServices.PostAsync($"Admin/GetAdmin", adminId))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    admin = JsonConvert.DeserializeObject<Admin>(data);
                }
            }

            ViewBag.Display = "none;";

            int? selectedValueFaculty = admin.faculty_id;
            int? selectedValueCampus = admin.campus_id;
            int? selectedValueDept = admin.dept_id;

            Tuple<int?, List<SelectListItem>> selectedFaculty = await facultyServices.getSelectedFaculties(selectedValueFaculty);
            ViewBag.facultyList = selectedFaculty.Item2;

            ViewBag.campusList = await campusService.getSelectedCampus(selectedValueCampus);

            ViewBag.deptList = await departmentServices.getSelectedDept(selectedValueDept);

            ViewBag.is_active = StatusService.getStatus(admin.is_active);

            ViewBag.roles = roleService.getRoles();

            return View(admin);
        }

        [HttpPost]
        public async Task<ActionResult> updateAdmin(Admin admin)
        {
            Admin loggedInAdmin = userAccessAdmin();

            ViewBag.campusList = await campusService.addCampustoListAsync();

            ViewBag.Display = "none;";

            admin.added_by = loggedInAdmin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Admin/updateAdmin", admin);

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

            List<AdminView> listAdmin = await giveRealtiveData(loggedInAdmin.campus_id);

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewAdmins", listAdmin);
        }

        // Delete Admin
        public async Task<ActionResult> deleteAdmin(string Id)
        {
            Admin loggedInAdmin = userAccessAdmin();

            List<AdminView> listAdmin = await giveRealtiveData(loggedInAdmin.campus_id);

            if (Id == null)
            {
                return RedirectToAction("viewAdmins");
            }
            else
            {
                Admin admin = new Admin();
                admin.is_active = 2;
                admin.email_id = HttpUtility.UrlDecode(Id);

                HttpResponseMessage responseMessage = await apiServices.PostAsync($"Admin/inActive", admin);

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

            return RedirectToAction("viewAdmins", listAdmin);
        }

        public async Task<List<AdminView>> giveRealtiveData(int? campusId)
        {
            List<AdminView> listAdmin = null;

            if (campusId.HasValue)
            {
                var response = await apiServices.GetAsync($"{baseAddress}Admin/GetByCampus/{campusId}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listAdmin = JsonConvert.DeserializeObject<List<AdminView>>(data);
                }
            }
            else
            {
                var response = await apiServices.GetAsync($"{baseAddress}Admin");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listAdmin = JsonConvert.DeserializeObject<List<AdminView>>(data);
                }
            }
            return listAdmin;
        }

    }
}
