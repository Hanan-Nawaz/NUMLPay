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
        CampusService campusService;

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
            campusService = new CampusService(baseAddress);
        }

        // Add SummerFee
        public async Task<ActionResult> addSummerFee()
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

            ViewBag.Session = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");


            ViewBag.Display = "none;";
            return View();
        }

        // Add SummerFee Post
        [HttpPost]
        public async Task<ActionResult> addSummerFee(SummerFee summerFee)
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

            ViewBag.Session = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");

            summerFee.is_active = 1;
            summerFee.added_by = admin.email_id;
            summerFee.subject_id = Convert.ToInt32(Request.Form["subDdl"]);

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

        // View all SummerFees
        public async Task<ActionResult> viewSummerFees()
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

            List<SummerFeeView> listSummerFees = await summerFeeService.getSummerFeesAsync(Convert.ToInt32(admin.dept_id));

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listSummerFees);
        }

        public async Task<ActionResult> sessionGenerator(int dept)
        {
            Session["dept"] = dept;

            return RedirectToAction("viewSummerFees");
        }

        // Update SummerFee
        public async Task<ActionResult> updateSummerFee(int Id)
        {
            userAccessAdmin();
            SummerFee summerFee = await summerFeeService.getSummerFeeAsync(Id);
            ViewBag.Display = "none;";

            ViewBag.Subject = await subjectService.getSelectedSubject(summerFee.subject_id);

            Session["subject_id_summer_fee"] = summerFee.subject_id;

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

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
            }
            else
            {
                ViewBag.adminRoles = "none;";
            }

            if (summerFee.subject_id == 0)
            {
                summerFee.subject_id = Convert.ToInt32(Session["subject_id_summer_fee"]);
            }

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
