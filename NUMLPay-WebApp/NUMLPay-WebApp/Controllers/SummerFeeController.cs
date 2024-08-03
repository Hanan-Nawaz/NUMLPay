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
    public class SummerFeeController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        subjectService subjectService;
        summerFeeService summerFeeService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        sessionService sessionServices;
        departmentService deptService;
        apiService apiServices;

        public SummerFeeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            subjectService = new subjectService(baseAddress); 
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            summerFeeService = new summerFeeService(baseAddress);
            facultyServices = new facultyService(baseAddress);
            deptService = new departmentService(baseAddress);
            sessionServices = new sessionService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add SummerFee
        public async Task<ActionResult> addSummerFee()
        {
            Admin admin = userAccessAdmin();

            ViewBag.Subject = new SelectList(await subjectService.getActiveSubjectsAsync(Convert.ToInt32(admin.dept_id)), "id", "name");
            ViewBag.Session = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");


            ViewBag.Display = "none;";
            return View();
        }

        // Add SummerFee Post
        [HttpPost]
        public async Task<ActionResult> addSummerFee(SummerFee summerFee)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Subject = new SelectList(await subjectService.getActiveSubjectsAsync(Convert.ToInt32(admin.dept_id)), "id", "name");
            ViewBag.Session = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");

            summerFee.is_active = 1;
            summerFee.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("SummerFee", summerFee);

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

        // View all SummerFees
        public async Task<ActionResult> viewSummerFees()
        {
            Admin admin = userAccessAdmin();

            List<SummerFeeView> listSummerFees = await summerFeeService.getSummerFeesAsync(Convert.ToInt32(admin.dept_id));

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listSummerFees);
        }

        // Update SummerFee
        public async Task<ActionResult> updateSummerFee(int Id)
        {
            userAccessAdmin();
            SummerFee summerFee = await summerFeeService.getSummerFeeAsync(Id);
            ViewBag.Display = "none;";

            ViewBag.Subject = await subjectService.getSelectedSubject(summerFee.subject_id);
            ViewBag.Session = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");

            int? selectedValue = summerFee.is_active;

            ViewBag.IsActive = StatusService.getStatus(selectedValue);

            return View(summerFee);
        }

        // Update SummerFee Post
        [HttpPost]
        public async Task<ActionResult> updateSummerFee(SummerFee summerFee)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";
            summerFee.added_by = admin.added_by;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"SummerFee/updateSummerFee/{summerFee.id}", summerFee);

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

            List<SummerFeeView> listSummerFees = await summerFeeService.getSummerFeesAsync(Convert.ToInt32(admin.dept_id));
            return View("viewSummerFees", listSummerFees);
        }

        // Delete SummerFee
        public async Task<ActionResult> deleteSummerFee(int? Id)
        {
            Admin admin = userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewSummerFees");
            }
            else
            {
                SummerFee summerFee = new SummerFee();
                summerFee.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"SummerFee/inActive/{Id}", summerFee);

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

            List<SummerFeeView> listSummerFees = await summerFeeService.getSummerFeesAsync(Convert.ToInt32(admin.dept_id));

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewSummerFees", listSummerFees);
        }
    }
}
