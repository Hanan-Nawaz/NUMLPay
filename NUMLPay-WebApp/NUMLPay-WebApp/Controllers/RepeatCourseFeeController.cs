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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
    public class RepeatCourseFeeController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        sessionService sessionServices;
        apiService apiServices;
        facultyService facultyServices;
        departmentService departmentServices;
        CampusService campusService;
        subjectService subjectService;


        public RepeatCourseFeeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            sessionServices = new sessionService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
            facultyServices = new facultyService(baseAddress);
            departmentServices = new departmentService(baseAddress);
            campusService = new CampusService(baseAddress);
            subjectService = new subjectService(baseAddress);

        }

        public async Task<ActionResult> AddRepeatCourse()
        {
            Admin admin = userAccessAdmin();
            ViewBag.campusList = await campusService.addCampustoListAsync();

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
            }
            else
            {
                ViewBag.adminRoles = "none;";
                admin.dept_id = Convert.ToInt32(admin.dept_id);
                ViewBag.DeptId = admin.dept_id;
            }

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddRepeatCourse(UnpaidFees newChallan)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                admin.dept_id = Convert.ToInt32(Request.Form["deptDdl"]);
            }
            else
            {
                ViewBag.adminRoles = "none;";
                admin.dept_id = Convert.ToInt32(admin.dept_id);
                ViewBag.DeptId = admin.dept_id;
            }

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);
            newChallan.challan_id = Convert.ToInt32(Request.Form["subDdl"]);

            HttpResponseMessage responseMessage = await apiServices.PostAsync($"GenerateChallan/GenerateRepeatChallans/{admin.dept_id}", newChallan);

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

        public async Task<ActionResult> getSubject(int? DeptDdl)
        {
            List<Subjects> subjects = await subjectService.getActiveSubjectsAsync(Convert.ToInt32(DeptDdl));

            List<SelectListItem> subOptions = new List<SelectListItem>();

            if (subjects != null)
            {
                foreach (var sub in subjects)
                {
                    subOptions.Add(new SelectListItem { Value = sub.id.ToString(), Text = sub.name.ToString() });
                }
            }

            return Json(subOptions, JsonRequestBehavior.AllowGet);
        }
    }
}