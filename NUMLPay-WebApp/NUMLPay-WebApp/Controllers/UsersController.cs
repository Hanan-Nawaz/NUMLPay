using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4, 1, 3 })]
    public class UsersController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService departmentServices;
        academicLevelService levelService;
        sessionService sessionServices;
        feePlanService feeService;
        degreeService degreeServices;
        apiService apiServices;

        public UsersController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            departmentServices = new departmentService(baseAddress);
            levelService = new academicLevelService();
            sessionServices = new sessionService(baseAddress);
            feeService = new feePlanService(baseAddress);
            degreeServices = new degreeService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // View all Users
        public async Task<ActionResult> viewUsers()
        {
            Admin admin = userAccessAdmin();
            List<UsersView> listUsers = null;
            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";


            if(admin.role == 1)
            {
                listUsers = await giveRealtiveData(admin.campus_id, null);
            }
            else if (admin.role == 3)
            {
                listUsers = await giveRealtiveData(null, admin.dept_id);
            }
            else
            {
                listUsers = await giveRealtiveData(null, null);
            }

            return View(listUsers);
        }

        public async Task<ActionResult> viewUser(string Id)
        {
            Admin admin = userAccessAdmin();

            Tuple<int, Users> userTuple = await getUserbyId(Id);
            Users userId = userTuple.Item2;

            return View(userId);
        }

        public async Task<ActionResult> updateStatus()
        {
            Admin admin = userAccessAdmin();
            ViewBag.adminRoles = "none;";

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block";
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }
            else
            {
                ViewBag.adminRoles = "none;";
                ViewBag.DeptId = admin.dept_id;
            }

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            ViewBag.feeFor = new SelectList(StatusService.getFeeForTuition(null), "value", "text");

            ViewBag.academicLevels = levelService.getLevel(null);

            ViewBag.is_active = StatusService.getStatusUsers(null);

            ViewBag.statusDegree = new SelectList(StatusService.getStatusDegree(null), "Value", "Text");

            return View();
        }

        public async Task<ActionResult> UpdateUserStatus(int statDegree, int isActive, int sessionId, int shiftId, int deptId)
        {
            Admin admin = userAccessAdmin();
            ViewBag.adminRoles = "none;";

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block";
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }
            else
            {
                ViewBag.adminRoles = "none;";
            }

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            ViewBag.feeFor = new SelectList(StatusService.getFeeForTuition(null), "value", "text");

            ViewBag.academicLevels = levelService.getLevel(null);

            ViewBag.is_active = StatusService.getStatusUsers(null);

            ViewBag.statusDegree = new SelectList(StatusService.getStatusDegree(null), "Value", "Text");


            
                string url = $"Users/UpdateStatus/{statDegree}/{isActive}/{sessionId}/{shiftId}/{deptId}";

                HttpResponseMessage responseMessage = await apiServices.GetAsync(url); 
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
            


            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("updateStatus");
        }

        public async Task<ActionResult> updateUser(string Id)
        {
            Admin admin = userAccessAdmin();

            Tuple<int, Users> userTuple = await getUserbyId(Id);
            Users userId = userTuple.Item2;

            return View(userId);
        }

        [HttpPost]
        public async Task<ActionResult> updateUser(Users user, HttpPostedFileBase imageFile)
        {
            Admin admin = userAccessAdmin();

            Tuple<int, Users> userTuple = await getUserbyId(user.numl_id);
            Users userId = userTuple.Item2;

            int hostelfeeDdl = int.Parse(Request.Form["hostelDdl"]);
            int busfeeDdl = int.Parse(Request.Form["busDdl"]);
            int semfeeDdl = int.Parse(Request.Form["semDdl"]);

            user.verified_by = admin.email_id;

            if (hostelfeeDdl == 0 || busfeeDdl == 0 || semfeeDdl == 0)
            {
                ViewBag.AlertMessage = "Please Enter Valid Data";
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
                return View(user);
            }
            else
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(imageFile.FileName);

                    string uniqueFileName = $"{user.numl_id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

                    string filePath = Path.Combine(Server.MapPath("~/UserImage/"), uniqueFileName);

                    imageFile.SaveAs(filePath);

                    user.image = "/UserImage/" + uniqueFileName;
                }
                else
                {
                    user.image = userId.image;
                }

                if (user.status_of_degree == 1)
                {
                    HttpResponseMessage responseMessageSemester = await apiServices.GetAsync($"Session/GetSemester/{user.admission_session}");
                    if (responseMessageSemester.IsSuccessStatusCode)
                    {
                        string responseSemester = responseMessageSemester.Content.ReadAsStringAsync().Result;

                        user.semester = Convert.ToInt32(responseSemester);
                    }
                }

                EligibleFees eligibleFees = new EligibleFees();

                eligibleFees.hostel_fee = hostelfeeDdl;
                eligibleFees.semester_fee = semfeeDdl;
                eligibleFees.bus_fee = busfeeDdl;
                eligibleFees.std_numl_id = user.numl_id;
                eligibleFees.id = userTuple.Item1;

                HttpResponseMessage responseMessageFees = await apiServices.PutAsync($"Users/PutEligibleFees", eligibleFees);

                if (responseMessageFees.IsSuccessStatusCode)
                {
                    HttpResponseMessage responseMessage = await apiServices.PutAsync("Users/PutUsers", user);
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
                }
            }

            List<UsersView> listUsers = null;


            if (admin.role == 1)
            {
                listUsers = await giveRealtiveData(admin.campus_id, null);
            }
            else if (admin.role == 3)
            {
                listUsers = await giveRealtiveData(null, admin.dept_id);
            }
            else
            {
                listUsers = await giveRealtiveData(null, null);
            }

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewUsers", listUsers);
        }

        public async Task<Tuple<int, Users>> getUserbyId(string Id)
        {
            Users userId = new Users();
            userId.numl_id = HttpUtility.UrlDecode(Id);

            using (var response = await apiServices.PostAsync($"Users/GetUsers", userId))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    userId = JsonConvert.DeserializeObject<Users>(data);
                }
            }

            ViewBag.Display = "none;";

            int? selectedValueDept = userId.dept_id;

            Tuple<int?, List<SelectListItem>> selectedDept = await departmentServices.getSelectedDepartments(selectedValueDept);
            ViewBag.deptList = selectedDept.Item2;

            Tuple<int?, List<SelectListItem>> selectedFaculty = await facultyServices.getSelectedFaculties(selectedDept.Item1);
            ViewBag.facultyList = selectedFaculty.Item2;

            ViewBag.campusList = await campusService.getSelectedCampus(selectedFaculty.Item1);

            ViewBag.is_active = StatusService.getStatusUsers(userId.is_active);

            ViewBag.statusDegree = new SelectList(StatusService.getStatusDegree(userId.status_of_degree), "Value", "Text");

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(Convert.ToInt32(userId.admission_session));

            ViewBag.feePlan = await feeService.addFeePlantoListAsync(userId.fee_plan);

            Shift shift = new Shift();

            using (var response = await apiServices.GetAsync($"Shift/GetbyId/{userId.degree_id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    shift = JsonConvert.DeserializeObject<Shift>(data);
                }
            }

            ViewBag.academicLevels = levelService.getLevel(shift.academic_id);

            int selectedDeptID = (int)selectedValueDept;
            List<SelectListItem> listDegree = await degreeServices.getDegrees(selectedDeptID, shift.academic_id, shift.Id);
            ViewBag.degreeList = listDegree;

            EligibleFees stdFees = new EligibleFees();
            stdFees.std_numl_id = userId.numl_id;

            using (var response = await apiServices.PostAsync($"Users/GetEligibleFees", stdFees))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    stdFees = JsonConvert.DeserializeObject<EligibleFees>(data);
                }
            }

            ViewBag.busFee = StatusService.getFeesUsers(stdFees.bus_fee);

            ViewBag.semFee = StatusService.getFeesUsers(stdFees.semester_fee);

            ViewBag.hostelFee = StatusService.getFeesUsers(stdFees.hostel_fee);

            return Tuple.Create(stdFees.id, userId);
        }

       
        public async Task<List<UsersView>> giveRealtiveData(int? campusId, int? deptId)
        {
            List<UsersView> userList = null;

            if (campusId.HasValue)
            {
                var response = await apiServices.GetAsync($"{baseAddress}Users/GetByCampus/{campusId}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    userList = JsonConvert.DeserializeObject<List<UsersView>>(data);
                }
            }
            else if (deptId.HasValue)
            {
                var response = await apiServices.GetAsync($"{baseAddress}Users/GetByDept/{deptId}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    userList = JsonConvert.DeserializeObject<List<UsersView>>(data);
                }
            }
            else
            {
                var response = await apiServices.GetAsync($"{baseAddress}Users/Get");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    userList = JsonConvert.DeserializeObject<List<UsersView>>(data);
                }
            }

            return userList;
        }

        

    }
}