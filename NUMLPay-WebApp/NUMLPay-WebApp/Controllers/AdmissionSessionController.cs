using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 4 })]
    public class AdmissionSessionController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        usersTypeService usersTypeService;
        statusService StatusService;
        sessionService sessionServices;
        apiService apiServices;

        public AdmissionSessionController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            sessionServices = new sessionService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add Session
        public async Task<ActionResult> addSession()
        {
            userAccessAdmin();
            ViewBag.nameList = new SelectList(sessionServices.getSessionName(null), "Value", "Text");
            ViewBag.sessionYear = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");
            ViewBag.Display = "none;";
            return View();
        }

        // Add Session Post
        [HttpPost]
        public async Task<ActionResult> addSession(Session session)
        {
            Admin admin = userAccessAdmin();

            ViewBag.nameList = new SelectList(sessionServices.getSessionName(null), "Value", "Text");
            ViewBag.sessionYear = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");

            session.added_by = admin.email_id;
            session.is_active = 1;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Session", session);

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

        // View all Sessions
        public async Task<ActionResult> viewSessions()
        {
            userAccessAdmin();

            List<Session> listSessions = await sessionServices.getSessionAsync();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listSessions);
        }

        // Update Session
        public async Task<ActionResult> updateSession(int Id)
        {
            userAccessAdmin();
            Session session = await sessionServices.getSessionAsync(Id);
            ViewBag.Display = "none;";

            ViewBag.nameList = new SelectList(sessionServices.getSessionName(null), "Value", "Text");
            ViewBag.sessionYear = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");

            int? selectedValue = session.is_active;
            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(session);
        }

        // Update Session Post
        [HttpPost]
        public async Task<ActionResult> updateSession(Session session)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            ViewBag.nameList = new SelectList(sessionServices.getSessionName(null), "Value", "Text");
            ViewBag.sessionYear = new SelectList(sessionServices.getYearsFrom1990ToCurrent(), "Value", "Text");

            session.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"Session/updateSession/{session.id}", session);

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

            List<Session> listSessions = await sessionServices.getSessionAsync();
            return View("viewSessions", listSessions);
        }

        // Delete Session
        public async Task<ActionResult> deleteSession(int? Id)
        {
            userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewSessions");
            }
            else
            {
                Session session = new Session();
                session.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"Session/inActive/{Id}", session);

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

            List<Session> listSessions = await sessionServices.getSessionAsync();

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewSessions", listSessions);
        }
        
    }
}