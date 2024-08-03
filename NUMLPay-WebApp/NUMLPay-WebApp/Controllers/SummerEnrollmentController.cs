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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3 })]
    public class SummerEnrollmentController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        summerFeeService summerFeeService;
        apiService apiServices;
        summerEnrollmentService summerEnrollmentService;

        public SummerEnrollmentController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            summerFeeService = new summerFeeService(baseAddress);
            summerEnrollmentService = new summerEnrollmentService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add SummerEnrollment
        public async Task<ActionResult> addSummerEnrollment()
        {
            Admin admin = userAccessAdmin();

            ViewBag.Subjects = new SelectList(await summerFeeService.GetActiveSummerFeesForYearAsync(Convert.ToInt32(admin.dept_id)), "id", "subject_name");

            ViewBag.Display = "none;";
            return View();
        }

        // Add SummerEnrollment Post
        [HttpPost]
        public async Task<ActionResult> addSummerEnrollment(SummerEnrollment summerEnrollment)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Subjects = new SelectList(await summerFeeService.GetActiveSummerFeesForYearAsync(Convert.ToInt32(admin.dept_id)), "id", "subject_name");

            summerEnrollment.added_by = admin.email_id;

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

            List<SummerEnrollmentView> listSummerEnrollment = await summerEnrollmentService.getSummerEnrollmentsAsync(Convert.ToInt32(admin.dept_id));

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listSummerEnrollment);
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