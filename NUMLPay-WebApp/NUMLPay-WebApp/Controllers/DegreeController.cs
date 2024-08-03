using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4, 1, 3 })]
    public class DegreeController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService departmentServices;
        academicLevelService levelService;
        apiService apiServices;

        public DegreeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            departmentServices = new departmentService(baseAddress);
            levelService = new academicLevelService();
            apiServices = new apiService(baseAddress.ToString());
        }

        //Add Degree
        public async Task<ActionResult> addDegree()
        {
           Admin admin = userAccessAdmin();

            await getAccessAccordingtoRole(admin);

            ViewBag.Display = "none;";

            return View();
        }

        //Add Degree Post
        [HttpPost]
        public async Task<ActionResult> addDegree(Degree newDegree)
        {
            Admin loggedAdmin = userAccessAdmin();

            await getAccessAccordingtoRole(loggedAdmin);

                var dateTime = DateTime.Now;
                var date = dateTime.Date;

                newDegree.added_by = loggedAdmin.email_id;
                newDegree.date = date;

                HttpResponseMessage responseMessage = await apiServices.PostAsync("Degree", newDegree);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-success";
                    ViewBag.Display = "block;";
                    newDegree = null;
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

        //Add Level and Shift
        public async Task<ActionResult> addLevelShift()
        {
            Admin admin = userAccessAdmin();

            await getAccessAccordingtoRole(admin);

            ViewBag.Display = "none;";

            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");
            ViewBag.shiftLevel = new SelectList(levelService.getShift(null), "Value", "Text");

            return View();
        }

        //Add Level and Shift Post
        [HttpPost]
        public async Task<ActionResult> addLevelShift(Shift newshift)
        {
            Admin loggedAdmin = userAccessAdmin();

            await getAccessAccordingtoRole(loggedAdmin);

            newshift.is_active = 1;

            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");
            ViewBag.shiftLevel = new SelectList(levelService.getShift(null), "Value", "Text");


            HttpResponseMessage responseMessage = await apiServices.PostAsync("Shift", newshift);

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


        // View all Degrees
        public async Task<ActionResult> viewDegrees()
        {
            Admin admin = userAccessAdmin();
            List<DegreeView> listDegree = null;
            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            if(admin.role == 1)
            {
                listDegree = await giveRealtiveData(admin.campus_id, null);
            }
            else if(admin.role == 3)
            {
                listDegree = await giveRealtiveData(null, admin.dept_id);
            }
            else
            {
                listDegree = await giveRealtiveData(null, null);
            }

            return View(listDegree);
        }

        // Update Degree
        public async Task<ActionResult> updateDegree(int Id)
        {
            userAccessAdmin();
            Shift degree = null;

            using (var response = await apiServices.GetAsync($"Shift/GetbyId/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    degree = JsonConvert.DeserializeObject<Shift>(data);
                }
            }

            ViewBag.Display = "none;";

            ViewBag.is_active = StatusService.getStatus(degree.is_active);
            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");

            return View(degree);
        }

        [HttpPost]
        public async Task<ActionResult> updateDegree(Shift degree)
        {
            Admin admin = userAccessAdmin();

            await getAccessAccordingtoRole(admin);
            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");

            ViewBag.Display = "none;";

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Shift/updateShift/{degree.Id}", degree);

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

            List<DegreeView> listDegree = null;

            if (admin.role == 1)
            {
                listDegree = await giveRealtiveData(admin.campus_id, null);
            }
            else if (admin.role == 2)
            {
                listDegree = await giveRealtiveData(null, admin.dept_id);
            }
            else
            {
                listDegree = await giveRealtiveData(null, null);
            }

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewDegrees", listDegree);
        }

        // Delete Degree
        public async Task<ActionResult> deleteDegree(int Id)
        {
            Admin admin = userAccessAdmin();

            List<DegreeView> listDegree = null;

            if (admin.role == 1)
            {
                listDegree = await giveRealtiveData(admin.campus_id, null);
            }
            else if (admin.role == 2)
            {
                listDegree = await giveRealtiveData(null, admin.dept_id);
            }
            else
            {
                listDegree = await giveRealtiveData(null, null);
            }

            if (Id == 0)
            {
                return RedirectToAction("viewDegrees");
            }
            else
            {
                Shift degree = new Shift();
                degree.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Degree/inActive/{Id}", degree);

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

            return RedirectToAction("viewDegrees", listDegree);
        }

        /********************/

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

            Tuple<int?, List<SelectListItem>> listFaculties = await facultyServices.getSelectedFaculties(null);
            List<SelectListItem> allFacultiesSelectListItem = listFaculties.Item2;
            SelectList allFacultiesSelectList = new SelectList(allFacultiesSelectListItem, "Value", "Text");

            if (admin.faculty_id != null)
            {
                int facultyId = (int)admin.faculty_id;
                var selectedFaculty = allFacultiesSelectList
                    .FirstOrDefault(item => int.Parse(item.Value) == facultyId);
                var filteredFaculties = new List<SelectListItem> { selectedFaculty };
                ViewBag.facultyList = new SelectList(filteredFaculties, "Value", "Text");
            }
           

            List<SelectListItem> allDeptsListItem = await departmentServices.getSelectedDept(null);
            SelectList allDepartmentsSelectList = new SelectList(allDeptsListItem, "Value", "Text");
            if (admin.dept_id != null)
            {
                int deptId = (int)admin.dept_id;
                var selectedDept = allDepartmentsSelectList
                    .FirstOrDefault(item => int.Parse(item.Value) == deptId);
                var filteredDepartments = new List<SelectListItem> { selectedDept };
                ViewBag.deptList = new SelectList(filteredDepartments, "Value", "Text");
            }
           

            return true;
        }

        public async Task<List<DegreeView>> giveRealtiveData(int? campusId, int? DeptId)
        {
            List<DegreeView> listDegree = null;

            if (campusId.HasValue)
            {
                var response = await apiServices.GetAsync($"Degree/GetDegreesByCampusId/{campusId}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listDegree = JsonConvert.DeserializeObject<List<DegreeView>>(data);
                }
            }
            else if (DeptId.HasValue)
            {
                var response = await apiServices.GetAsync($"Degree/GetDegreesByDeptId/{DeptId}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listDegree = JsonConvert.DeserializeObject<List<DegreeView>>(data);
                }
            }
            else
            {
                var response = await apiServices.GetAsync($"Degree/Get");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listDegree = JsonConvert.DeserializeObject<List<DegreeView>>(data);
                }
            }

            return listDegree;
        }

        //Get Degree on base of Dept and Academic Level
        public async Task<ActionResult> getDegrees(string deptId)
        {
            int deptIdValue = Convert.ToInt32(deptId);

            Degree degree = new Degree
            {
                dept_id = deptIdValue,
            };

            List<Degree> listDegree = null;
            HttpResponseMessage responseMessage = await apiServices.GetAsync($"Degree/GetByDept/{deptIdValue}");

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listDegree = JsonConvert.DeserializeObject<List<Degree>>(responseData);
            }

            SelectList degreeList = new SelectList(listDegree, "id", "name");
            var degreeOptions = degreeList.Select(f => new { Value = f.Value, Text = f.Text });

            return Json(degreeOptions, JsonRequestBehavior.AllowGet);
        }
    }
}