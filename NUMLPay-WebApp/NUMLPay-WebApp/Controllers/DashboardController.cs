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
using NUMLPay_WebApp.ViewModel;

namespace NUMLPay_WebApp.Controllers
{
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

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4 })]
        //Main Dashboard
        public async Task<ActionResult> mainDashboard()
        {
            Admin admin = userAccessAdmin();
            var campuses = await GetCampus();
            ViewBag.campuses = campuses;

            var feePaidByCampusData = await GetDataForFeePaidByCampusChart();
            var studentsincampusData = await studentsincampus();

            ViewBag.FeePaidByCampusData = feePaidByCampusData;
            ViewBag.studentsincampusData = studentsincampusData;


            return View();
        }

        private async Task<List<FacultyModel>> GetCampus()
        {

            List<FacultyModel> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/getcampus");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FacultyModel>>(data);
                }
            }
            return fees;
        }


        private async Task<List<FeePaidByCampusModel>> studentsincampus()
        {
            List<FeePaidByCampusModel> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/studentsincampus");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FeePaidByCampusModel>>(data);
                }
            }
            return fees;
        }


        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3 })]
        //Department Dashboard
        public async Task<ActionResult> deptDashboard()
        {
           Admin admin = userAccessAdmin();
           var degrees = await GetDegrees(Convert.ToInt32(admin.dept_id));
            ViewBag.degrees = degrees;
            ViewBag.GetTotalStudents = await GetTotalStudents(Convert.ToInt32(admin.dept_id));
            ViewBag.GetTotalCeasedStudents = await GetTotalCeasedStudents(Convert.ToInt32(admin.dept_id));

            return View();
        }

        private async Task<List<FacultyModel>> GetDegrees(int id)
        {

            List<FacultyModel> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/getDegrees/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FacultyModel>>(data);
                }
            }
            return fees;
        }

        public async Task<List<StudentsBycategory>> GetTotalStudents(int? id)
        {
            List<StudentsBycategory> students = new List<StudentsBycategory>();

            var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/GetStudentsInDegree/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<List<StudentsBycategory>>(data);
            }
            return students;
        }

        public async Task<List<StudentsBycategory>> GetTotalCeasedStudents(int? id)
        {
            List<StudentsBycategory> students = new List<StudentsBycategory>();
            var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/GetCeasedStudentsInDegree/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<List<StudentsBycategory>>(data);
            }
            return students;

        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 2 })]
        //Accountant Dashboard
        public async Task<ActionResult> accountantDashboard()
        {
            Admin admin = userAccessAdmin();


                var feePaidByCampusData = await GetDataForFeePaidByCampusChart();
                var tuitionFeeData = await GetDataForTuitionFeeChart();
                var hostelFeeData = await GetDataForHostelFeeChart();
                var busFeeData = await GetDataForBusFeeChart();

                ViewBag.FeePaidByCampusData = feePaidByCampusData;
                ViewBag.TuitionFeeData = tuitionFeeData;
                ViewBag.HostelFeeData = hostelFeeData;
                ViewBag.BusFeeData = busFeeData;

                return View();
        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 1 })]
        //Accountant Dashboard
        public async Task<ActionResult> campusDashboard()
        {
            Admin admin = userAccessAdmin();


            var faculities = await GetFaculties(Convert.ToInt32(admin.campus_id));

            ViewBag.faculities = faculities;

            return View();
        }

        private async Task<List<FacultyModel>> GetFaculties(int id)
        {

            List<FacultyModel> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/getFaculties/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FacultyModel>>(data);
                }
            }
            return fees;
        }

        public async Task<ActionResult> GetFacultiesDept(int? id)
        {
            var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/getFacultiesdept/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var fees = JsonConvert.DeserializeObject<List<DepartmentModel>>(data);
                return Json(fees, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Handle error response
                return Json(null); // Or return an appropriate error response
            }
        }

        public async Task<ActionResult> GetHostelBusStudents(int? id)
        {
            var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/GetHostelBusStudents/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var fees = JsonConvert.DeserializeObject<List<StudentsBycategory>>(data);
                return Json(fees, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Handle error response
                return Json(null); // Or return an appropriate error response
            }
        }


        private async Task<List<FeePaidByCampusModel>> GetDataForFeePaidByCampusChart()
        {
            List<FeePaidByCampusModel> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/feepaidbycampus");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FeePaidByCampusModel>>(data);
                }
            }
            return fees;
        }

        private async Task<List<FeePaidByCategory>> GetDataForTuitionFeeChart()
        {
            List<FeePaidByCategory> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/feepaidbycategory/Tuition Fee");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FeePaidByCategory>>(data);
                }
            }
            return fees;
        }

        private async Task<List<FeePaidByCategory>> GetDataForHostelFeeChart()
        {
            List<FeePaidByCategory> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/feepaidbycategory/Hostel Fee");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FeePaidByCategory>>(data);
                }
            }
            return fees;
        }

        private async Task<List<FeePaidByCategory>> GetDataForBusFeeChart()
        {
            List<FeePaidByCategory> fees = null;

            {
                var response = await apiServices.GetAsync($"{baseAddress}dw/Dashboard/feepaidbycategory/Bus Fee");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fees = JsonConvert.DeserializeObject<List<FeePaidByCategory>>(data);
                }
            }
            return fees;
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