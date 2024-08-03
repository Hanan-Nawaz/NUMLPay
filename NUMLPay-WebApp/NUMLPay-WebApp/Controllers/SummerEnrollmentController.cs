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
    public class SummerEnrollmentController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        summerFeeService summerFeeService;
        apiService apiServices;
        summerEnrollmentService summerEnrollmentService;
        CampusService campusService;

        public SummerEnrollmentController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            summerFeeService = new summerFeeService(baseAddress);
            summerEnrollmentService = new summerEnrollmentService(baseAddress);
            campusService = new CampusService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add SummerEnrollment
        public async Task<ActionResult> addSummerEnrollment()
        {
            Admin admin = userAccessAdmin();

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }
            else
            {
                ViewBag.adminRoles = "none;";
                ViewBag.DeptId = admin.dept_id;
            }


            ViewBag.Display = "none;";
            return View();
        }

        public async Task<ActionResult> getSubject(int? DeptDdl)
        {
            List<SummerFeeView> subjects = await summerFeeService.GetActiveSummerFeesForYearAsync(Convert.ToInt32(DeptDdl));
            List<SelectListItem> subOptions = new List<SelectListItem>();

            if (subjects != null)
            {
                foreach (var sub in subjects)
                {
                    subOptions.Add(new SelectListItem { Value = sub.id.ToString(), Text = sub.subject_name.ToString() });
                }
            }

            return Json(subOptions, JsonRequestBehavior.AllowGet);
        }

        // Add SummerEnrollment Post
        [HttpPost]
        public async Task<ActionResult> addSummerEnrollment(SummerEnrollment summerEnrollment)
        {
            Admin admin = userAccessAdmin();

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                ViewBag.campusList = await campusService.addCampustoListAsync();
                admin.dept_id = Convert.ToInt32(Request.Form["deptDdl"]);
            }
            else
            {
                ViewBag.adminRoles = "none;";
                
            }

            summerEnrollment.added_by = admin.email_id;
            summerEnrollment.summer_fee_id = Convert.ToInt32(Request.Form["subDdl"]);

            HttpResponseMessage responseMessage = await apiServices.PostAsync($"SummerEnrollment/{admin.dept_id}", summerEnrollment);

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

        // View all SummerEnrollment
        public async Task<ActionResult> viewSummerEnrollments()
        {
            Admin admin = userAccessAdmin();

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                ViewBag.campusList = await campusService.addCampustoListAsync();
                admin.dept_id = Convert.ToInt32(Session["dept"]);
            }
            else
            {
                ViewBag.adminRoles = "none;";
                admin.dept_id = Convert.ToInt32(admin.dept_id);
            }

            List<SummerEnrollmentView> listSummerEnrollment = await summerEnrollmentService.getSummerEnrollmentsAsync(Convert.ToInt32(admin.dept_id));

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listSummerEnrollment);
        }

        public async Task<ActionResult> sessionGenerator(int dept)
        {
            Session["dept"] = dept;

            return RedirectToAction("viewSummerEnrollments");
        }


        // Delete SummerEnrollment
        public async Task<ActionResult> deleteSummerEnrollment(int? Id)
        {
            Admin admin = userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("viewSummerEnrollments");
            }
            else
            {

                HttpResponseMessage responseMessage = await apiServices.DeleteAsync($"SummerEnrollment/DeleteEnrollment/{Id}");

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

            List<SummerEnrollmentView> listSummerEnrollment = await summerEnrollmentService.getSummerEnrollmentsAsync(Convert.ToInt32(admin.dept_id));

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewSummerEnrollments", listSummerEnrollment);
        }
    }
}