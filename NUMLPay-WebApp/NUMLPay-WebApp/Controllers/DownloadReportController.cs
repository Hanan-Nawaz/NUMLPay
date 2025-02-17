﻿using Humanizer;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
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
    public class DownloadReportController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        academicLevelService levelService;
        CampusService campusService;
        sessionService sessionService;
        statusService StatusService;
        apiService apiServices;


        public DownloadReportController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            levelService = new academicLevelService();
            campusService = new CampusService(baseAddress);
            sessionService = new sessionService(baseAddress);
            StatusService = new statusService();
            apiServices = new apiService(baseAddress.ToString());
        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> downloadReport()
        {
            Admin admin = userAccessAdmin();
            List<ReportFee> reportfee = Session["data_table"] as List<ReportFee>;
            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }
            else
            {
                ViewBag.adminRoles = "none;";
                ViewBag.departmentId = admin.dept_id;
            }

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.departmentId = admin.dept_id;
            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");
            ViewBag.Session = await sessionService.addSessiontoListAsync(null);

            return View(reportfee);
        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> viewTuitionReport(int? shiftId, int? semester, int? session)
        {
            ViewBag.Display = "none;";

            Admin admin = userAccessAdmin();

                List<ReportFee> listFee = null;
                if (semester.HasValue)
                {
                    var response = await apiServices.GetAsync($"FeeReport/GetReportofTutionFee/{shiftId}/{semester}/{session}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                    }
                }
                else
                {
                    var response = await apiServices.GetAsync($"FeeReport/GetReportofTutionFee/{shiftId}/{session}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                    }
                }
            Session["data_table"] = listFee;

            return RedirectToAction("downloadReport");

        }


        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> downloadTuitionReport(int? shiftId, int? semester, int? session)
        {
            ViewBag.Display = "none;";

            Admin admin = userAccessAdmin();
            try
            {
                List<ReportFee> listFee = null;
                if (semester.HasValue)
                {
                    var response = await apiServices.GetAsync($"FeeReport/GetReportofTutionFee/{shiftId}/{semester}/{session}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                    }
                }
                else
                {
                    var response = await apiServices.GetAsync($"FeeReport/GetReportofTutionFee/{shiftId}/{session}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                    }
                }

                if (listFee.Count > 0)
                {
                    string sem = "";

                    if (semester.HasValue)
                    {
                        sem = listFee[0].degree_name + "_" + semester + "_semester";
                    }
                    else
                    {
                        sem = listFee[0].degree_name;
                    }

                    var parameters = new ReportParameterCollection
                {
                    new ReportParameter("deptName", " "),
                    new ReportParameter("date", DateTime.Now.ToString("dd/MMM/yyyy")),
                    new ReportParameter("semester", sem),
                };


                    var reportViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Unpaidfee.rdlc");
                    reportViewer.LocalReport.SetParameters(parameters);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportFee", listFee));
                    byte[] pdfBytes = reportViewer.LocalReport.Render("PDF");

                    Session["data_table"] = listFee;

                    return File(pdfBytes, "application/pdf", sem + "_FeeDeafaultersList.pdf");
                }
                else
                {
                    ViewBag.AlertMessage = "No Data Found.";
                    TempData["AlertType"] = "alert-danger";
                    TempData["AlertMessage"] = ViewBag.AlertMessage;
                    TempData["Display"] = "block;";

                    return RedirectToAction("downloadReport");
                }

                
            }
            catch (Exception ex)
            {
                ViewBag.AlertMessage = "An error occurred while generating the report: " + ex.Message;
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = ViewBag.AlertMessage;
                TempData["Display"] = "block;";

                return RedirectToAction("downloadReport");
            }

        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> downloadSummerReport()
        {
            Admin admin = userAccessAdmin();

            List<ReportFee> reportfee = Session["data_table"] as List<ReportFee>;


            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }
            else
            {
                ViewBag.adminRoles = "none;";
                ViewBag.departmentId = admin.dept_id;
            }

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.sessionYear = new SelectList(sessionService.getYearsFrom1990ToCurrent(), "Value", "Text");


            return View(reportfee);
        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> viewSummerFeeReport(int year, int? deptId)
        {
            ViewBag.Display = "none;";

            Admin admin = userAccessAdmin();

            List<ReportFee> listFee = null;
            var response = await apiServices.GetAsync($"FeeReport/SummerFee/{Convert.ToInt32(admin.dept_id)}/{year}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
            }

            Session["data_table"] = listFee;

            return RedirectToAction("downloadSummerReport");

        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> downloadSummerFeeReport(int year, int? deptId)
        {
            ViewBag.Display = "none;";

            Admin admin = userAccessAdmin();

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
                admin.dept_id = deptId;
            }
            else
            {
                ViewBag.adminRoles = "none;";
            }

            try
            {
                List<ReportFee> listFee = null;
                var response = await apiServices.GetAsync($"FeeReport/SummerFee/{Convert.ToInt32(admin.dept_id)}/{year}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                }

                string sem = "Summer_Semester";

                if(listFee.Count > 0)
                {


                var parameters = new ReportParameterCollection
                {
                    new ReportParameter("deptName", " "),
                    new ReportParameter("date", DateTime.Now.ToString("dd/MMM/yyyy")),
                    new ReportParameter("semester", sem),
                };

                var reportViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Unpaidfee.rdlc");
                reportViewer.LocalReport.SetParameters(parameters);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportFee", listFee));

                byte[] pdfBytes = reportViewer.LocalReport.Render("PDF");

                    Session["data_table"] = listFee;


                    return File(pdfBytes, "application/pdf", sem + "_FeeDeafaultersList.pdf");

                }
                else
                {
                    ViewBag.AlertMessage = "No Data Found.";
                    TempData["AlertType"] = "alert-danger";
                    TempData["AlertMessage"] = ViewBag.AlertMessage;
                    TempData["Display"] = "block;";

                    return RedirectToAction("downloadSummerReport");
                }
            }
            catch (Exception ex)
            {
                ViewBag.AlertMessage = "An error occurred while generating the report: " + ex.Message;
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = ViewBag.AlertMessage;
                TempData["Display"] = "block;";

                return RedirectToAction("downloadSummerReport");
            }

        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 1, 4 })]
        public async Task<ActionResult> downloadHMBReport()
        {
            Admin admin = userAccessAdmin();
            List<ReportFee> reportfee = Session["data_table"] as List<ReportFee>;

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.adminRoles = "none;";

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block";
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }
           

            ViewBag.feeFor = new SelectList(StatusService.getOtherFeeFor(null), "value", "text");
            ViewBag.Session = await sessionService.addSessiontoListAsync(null);

            return View(reportfee);
        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 1, 4 })]
        public async Task<ActionResult> viewHMBFeeReport(int? feefor, int? session, int? campus)
        {
            ViewBag.Display = "none;";

            Admin admin = userAccessAdmin();

            List<ReportFee> listFee = null;
            var response = await apiServices.GetAsync($"FeeReport/HMBFee/{Convert.ToInt32(admin.campus_id)}/{feefor}/{session}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
            }

            Session["data_table"] = listFee;

            return RedirectToAction("downloadHMBReport");

        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 1, 4 })]
        public async Task<ActionResult> downloadHMBFeeReport(int? feefor, int? session, int? campus)
        {
            ViewBag.Display = "none;"; 
            ViewBag.adminRoles = "none";

            Admin admin = userAccessAdmin();
            try
            {
                if (admin.role == 4)
                {
                    ViewBag.adminRoles = "block";
                    ViewBag.campusList = await campusService.addCampustoListAsync();

                    admin.campus_id = campus;


                }
                


                List<ReportFee> listFee = null;
                var response = await apiServices.GetAsync($"FeeReport/HMBFee/{Convert.ToInt32(admin.campus_id)}/{feefor}/{session}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                }



                string sem = "";

                if (feefor == 2)
                {
                    sem = "Bus Fee";
                }
                else if (feefor == 3)
                {
                    sem = "Hostel Fee";
                }
                else
                {
                    sem = "Mess Fee";
                }

                if (listFee != null && listFee.Count > 0)
                {


                    var parameters = new ReportParameterCollection
                {
                    new ReportParameter("deptName", " "),
                    new ReportParameter("date", DateTime.Now.ToString("dd/MMM/yyyy")),
                    new ReportParameter("semester", sem),
                };

                    var reportViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Unpaidfee.rdlc");
                    reportViewer.LocalReport.SetParameters(parameters);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportFee", listFee));

                    byte[] pdfBytes = reportViewer.LocalReport.Render("PDF");

                    Session["data_table"] = listFee;


                    return File(pdfBytes, "application/pdf", sem + "_FeeDeafaultersList.pdf");

                }
                else
                {
                    ViewBag.AlertMessage = "No Data Found.";
                    TempData["AlertType"] = "alert-danger";
                    TempData["AlertMessage"] = ViewBag.AlertMessage;
                    TempData["Display"] = "block;";

                    return RedirectToAction("downloadHMBReport");
                }
            }
            catch (Exception ex)
            {
                ViewBag.AlertMessage = "An error occurred while generating the report: " + ex.Message;
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = ViewBag.AlertMessage;
                TempData["Display"] = "block;";

                return RedirectToAction("downloadHMBReport");
            }

        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> downloadRepeatReport()
        {
            Admin admin = userAccessAdmin();
            List<ReportFee> reportfee = Session["data_table"] as List<ReportFee>;

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.adminRoles = "none;";

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block";
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }
            else
            {
                ViewBag.departmentId = admin.dept_id;
            }

            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");
            ViewBag.Session = await sessionService.addSessiontoListAsync(null);

            return View(reportfee);
        }


        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> viewRepeatReports(int? shiftId, int? semester, int? session)
        {
            ViewBag.Display = "none;";

            Admin admin = userAccessAdmin();

            List<ReportFee> listFee = null;
            if (semester.HasValue)
            {
                var response = await apiServices.GetAsync($"FeeReport/GetReportofRepeatFee/{shiftId}/{semester}/{session}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                }
            }
            else
            {
                var response = await apiServices.GetAsync($"FeeReport/GetReportofRepeatFee/{shiftId}/{session}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                }
            }

            Session["data_table"] = listFee;

            return RedirectToAction("downloadRepeatReport");

        }

        [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 3, 4 })]
        public async Task<ActionResult> downloadRepeatReports(int? shiftId, int? semester, int? session)
        {
            ViewBag.Display = "none;";
            ViewBag.adminRoles = "none;";

            Admin admin = userAccessAdmin();
            try
            {
                List<ReportFee> listFee = null;
                if (semester.HasValue)
                {
                    var response = await apiServices.GetAsync($"FeeReport/GetReportofRepeatFee/{shiftId}/{semester}/{session}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                    }
                }
                else
                {
                    var response = await apiServices.GetAsync($"FeeReport/GetReportofRepeatFee/{shiftId}/{session}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listFee = JsonConvert.DeserializeObject<List<ReportFee>>(data);
                    }
                }

                if (listFee.Count > 0)
                {
                    string sem = "";

                    if (semester.HasValue)
                    {
                        sem = listFee[0].degree_name + "_" + semester + "_semester";
                    }
                    else
                    {
                        sem = listFee[0].degree_name;
                    }

                    var parameters = new ReportParameterCollection
                {
                    new ReportParameter("deptName", " "),
                    new ReportParameter("date", DateTime.Now.ToString("dd/MMM/yyyy")),
                    new ReportParameter("semester", sem+" Repeat Courses"),
                };


                    var reportViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Unpaidfee.rdlc");
                    reportViewer.LocalReport.SetParameters(parameters);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportFee", listFee));
                    byte[] pdfBytes = reportViewer.LocalReport.Render("PDF");

                    Session["data_table"] = listFee;

                    return File(pdfBytes, "application/pdf", sem + "_FeeDeafaultersList.pdf");
                }
                else
                {
                    ViewBag.AlertMessage = "No Data Found.";
                    TempData["AlertType"] = "alert-danger";
                    TempData["AlertMessage"] = ViewBag.AlertMessage;
                    TempData["Display"] = "block;";

                    return RedirectToAction("downloadRepeatReport");
                }


            }
            catch (Exception ex)
            {
                ViewBag.AlertMessage = "An error occurred while generating the report: " + ex.Message;
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = ViewBag.AlertMessage;
                TempData["Display"] = "block;";

                return RedirectToAction("downloadRepeatReport");
            }

        }

    }
}