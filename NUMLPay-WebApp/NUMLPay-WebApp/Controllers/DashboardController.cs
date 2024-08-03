using System;
using System.Configuration;
using BCrypt.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using NUMLPay_WebApp.Models;
using Newtonsoft.Json;
using System.Text;
using System.Web.Helpers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using NUMLPay_WebApp.Services;
using System.Web.Services;
using System.Web.ApplicationServices;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4, 1, 2, 3 })]
    public class DashboardController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService departmentServices;
        rolesService roleService;
        apiService apiServices;

        public DashboardController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            departmentServices = new departmentService(baseAddress);
            roleService = new rolesService();
            apiServices = new apiService(baseAddress.ToString());
        }

        //Main Dashboard
        public ActionResult mainDashboard()
        {
            userAccessAdmin();

            return View();
        }

        //Department Dashboard
        public ActionResult deptDashboard()
        {
           Admin admin = userAccessAdmin();
            
            return View();
        }

        // Update Admin
        public async Task<ActionResult> profileAdmin()
        {
            Admin admin = userAccessAdmin();

            using (var response = await apiServices.PostAsync($"Admin/GetAdmin", admin))
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

            List<SelectListItem> selectList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "Nil",
                    Selected = true
                }
            };


            if (selectedValueCampus == null)
            {
                ViewBag.campusList = selectList;
            }
            else
            {
                ViewBag.campusList = await campusService.getSelectedCampus(selectedValueCampus);
            }

            if (selectedValueFaculty == null)
            {
                ViewBag.facultyList = selectList;
            }
            else
            {
                Tuple<int?, List<SelectListItem>> selectedFaculty = await facultyServices.getSelectedFaculties(selectedValueFaculty);
                ViewBag.facultyList = selectedFaculty.Item2;
            }

            if (selectedValueFaculty == null)
            {
                ViewBag.deptList = selectList;
            }
            else
            {
                ViewBag.deptList = await departmentServices.getSelectedDept(selectedValueDept);
            }

            ViewBag.is_active = StatusService.getStatus(admin.is_active);

            ViewBag.roles = roleService.getAdminRoles();

            return View(admin);
        }

       
    }
}