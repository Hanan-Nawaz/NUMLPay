using Newtonsoft.Json;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
    public class SubjectController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        subjectService subjectService;
        statusService StatusService;
        apiService apiServices;
        CampusService campusService;

        public SubjectController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            subjectService = new subjectService(baseAddress);
            StatusService = new statusService();
            apiServices = new apiService(baseAddress.ToString());
            campusService = new CampusService(baseAddress);
        }

        // Add Subject
        public async Task<ActionResult> addSubject()
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

        // Add Subject Post
        [HttpPost]
        public async Task<ActionResult> addSubject(Subjects subject)
        {
            Admin admin = userAccessAdmin();

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                ViewBag.campusList = await campusService.addCampustoListAsync();
                subject.dept_id = Convert.ToInt32(Request.Form["deptDdl"]);
            }
            else
            {
                ViewBag.adminRoles = "none;";
                subject.dept_id = admin.dept_id;
            }

            subject.is_active = 1;
            subject.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Subject", subject);

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

        // View all Subjects
        public async Task<ActionResult> viewSubjects()
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

            List<Subjects> listSubjects = await subjectService.getSubjectsAsync(admin.dept_id);

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listSubjects);
        }

        public async Task<ActionResult> sessionGenerator(int dept)
        {
            Session["dept"] = dept;

            return RedirectToAction("viewSubjects");
        }

        // Update Subject
        public async Task<ActionResult> updateSubject(int Id)
        {
           Admin admin = userAccessAdmin();

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
            }
            else
            {
                ViewBag.adminRoles = "none;";
            }

            Subjects subject = await subjectService.getSubjectAsync(Id);
            ViewBag.Display = "none;";

            int? selectedValue = subject.is_active;

            Session["deptId_Sub"] = subject.dept_id;

            ViewBag.IsActive = StatusService.getStatus(selectedValue);

            return View(subject);
        }

        // Update Subject Post
        [HttpPost]
        public async Task<ActionResult> updateSubject(Subjects subject)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";
            subject.added_by = admin.added_by;
            

            if (admin.role == 4)
            {
                subject.dept_id = Convert.ToInt32(Session["deptId_Sub"]);
                ViewBag.adminRoles = "block;";
            }
            else
            {
                subject.dept_id = admin.dept_id;
                ViewBag.adminRoles = "none;";
            }

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Subject/updateSubject/{subject.id}", subject);

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

            List<Subjects> listSubjects = await subjectService.getSubjectsAsync(admin.dept_id);
            return View("viewSubjects", listSubjects);
        }

        // Delete Subject
        public async Task<ActionResult> deleteSubject(int? Id)
        {
            Admin admin = userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewSubjects");
            }
            else
            {
                Subjects subject = new Subjects();
                subject.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Subject/inActive/{Id}", subject);

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

            List<Subjects> listSubjects = await subjectService.getSubjectsAsync(admin.dept_id);

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewSubjects", listSubjects);
        }
    }
}