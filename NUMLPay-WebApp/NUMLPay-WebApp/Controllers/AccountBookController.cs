using Humanizer;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using NUMLPay_WebApp.ModelFront;
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
    [CustomAuthorizationFilter(requireUser: true)]
    public class AccountBookController : SessionController
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
        emailService eService;
        degreeService degreeServices;
        rolesService roleService;
        apiService apiServices;

        public AccountBookController()
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
            eService = new emailService();
            feeService = new feePlanService(baseAddress);
            degreeServices = new degreeService(baseAddress);
            roleService = new rolesService();
            apiServices = new apiService(baseAddress.ToString());
        }

        // View Account Book
        public async Task<ActionResult> viewAccountBook()
        {
            Users user = userAccess();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            List<UnPaidFeeView> listFees = await getAccountBook(user);

            return View(listFees);
        }

        //Print Challan
        public async Task<ActionResult> DownloadChallanPdf(int Id, int feeType)
        {
            Users user = userAccess();
            try
            {
                JoinedDataChallan challanData = await FetchChallanData(Id);

                float number = challanData.total_fee + challanData.fine;

                int integerPart = (int)Math.Floor(number);
                string integerWords = integerPart.ToWords();

                int decimalPart = (int)Math.Round((number - integerPart) * 100);
                string decimalWords = decimalPart.ToWords();

                string englishWords = $"{integerWords} only";


                var parameters = new ReportParameterCollection
                {
                    new ReportParameter("challanNumber", challanData.challan_no.ToString()),
                    new ReportParameter("issueDate", Convert.ToDateTime(challanData.issue_date).ToString("dd/MMM/yyyy")),
                    new ReportParameter("dueDate", Convert.ToDateTime(challanData.due_date).ToString("dd/MMM/yyyy")),
                    new ReportParameter("validDate", Convert.ToDateTime(challanData.paid_date).ToString("dd/MMM/yyyy")),
                    new ReportParameter("degreeName", challanData.degree_name.ToString()),
                    new ReportParameter("currentSession", challanData.Session.ToString()),
                    new ReportParameter("currentSemester", challanData.currentSem.ToString()),
                    new ReportParameter("systemId", challanData.numl_id.ToString()),
                    new ReportParameter("stdName", challanData.name.ToString()),
                    new ReportParameter("sonOf", challanData.father_name.ToString()),
                    new ReportParameter("feeSemester", challanData.feeSem.ToString()),
                    new ReportParameter("feeplan", challanData.fee_plan.ToString()),
                    new ReportParameter("installmentNo", challanData.installment_no.ToString()),
                    new ReportParameter("amountBeforeDueDate", challanData.total_fee.ToString()),
                    new ReportParameter("lateFine", challanData.fine.ToString()),
                    new ReportParameter("totalFee", number.ToString()),
                    new ReportParameter("feeFor", challanData.FeeFor.ToString()),
                    new ReportParameter("feeInEnglish", englishWords),
                };

                List<SubFeeView> subChallanData = await FetchSubChallanData(challanData.fee_id, feeType, challanData.feeSem, challanData.numl_id, challanData.fee_for);

                if (feeType == 6)
                {
                    subChallanData = new List<SubFeeView>();

                    SubFeeView subFeeView = new SubFeeView();
                    subFeeView.Description = "Repeat Course";
                    subFeeView.Amount = Convert.ToInt32(challanData.total_fee);

                    subChallanData.Add(subFeeView);
                }

                var reportViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = Server.MapPath("~/Challans/PaidChallan.rdlc");
                reportViewer.LocalReport.SetParameters(parameters);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("UnPaidChallan", subChallanData));

                byte[] pdfBytes = reportViewer.LocalReport.Render("PDF");

                // Return the PDF file for download
                return File(pdfBytes, "application/pdf", challanData.name.ToString() + "_" + challanData.FeeFor + "_FeePaidChallan.pdf");
            }
            catch (Exception ex)
            {
                ViewBag.AlertMessage = "An error occurred while generating the report: " + ex.Message;
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = ViewBag.AlertMessage;
                TempData["Display"] = "block;";
                List<UnPaidFeeView> listFees = await getAccountBook(user);

                return RedirectToAction("viewAccountBook",listFees);
            }
        }

        //Fetch Main Chllan Data
        private async Task<JoinedDataChallan> FetchChallanData(int id)
        {
            JoinedDataChallan listFee = null;

            HttpResponseMessage responseMessage = await apiServices.GetAsync($"AccountBook/GetPaidChallan/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listFee = JsonConvert.DeserializeObject<JoinedDataChallan>(responseData);
            }

            return listFee;
        }

        //Fetch Sub Chllan Data
        private async Task<List<SubFeeView>> FetchSubChallanData(int id, int feeType, int curSem, string numlId, int? feeFor)
        {
            List<SubFeeView> listFee = null;

            if (feeFor.HasValue)
            {
                HttpResponseMessage responseMessage = await apiServices.GetAsync($"GetChallan/{id}/{curSem}/{feeType}/{numlId}/{feeFor}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    listFee = JsonConvert.DeserializeObject<List<SubFeeView>>(responseData);
                }
            }
            else
            {
                HttpResponseMessage responseMessage = await apiServices.GetAsync($"GetChallan/{id}/{curSem}/{feeType}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    listFee = JsonConvert.DeserializeObject<List<SubFeeView>>(responseData);
                }
            }

            return listFee;
        }

        private async Task<List<UnPaidFeeView>> getAccountBook(Users user)
        {
            ViewBag.name = user.name;
            ViewBag.numlId = user.numl_id;

            if (user.is_active == 1)
            {
                ViewBag.is_active = "Active";
            }
            else
            {
                ViewBag.is_active = "InActive";
            }

            if (user.status_of_degree == 1)
            {
                ViewBag.statusOfDegree = "In-Complete";
            }
            else if (user.status_of_degree == 2)
            {
                ViewBag.statusOfDegree = "Complete";
            }
            else if (user.status_of_degree == 2)
            {
                ViewBag.statusOfDegree = "Freezed";
            }
            else
            {
                ViewBag.statusOfDegree = "Seized";
            }

            List<UnPaidFeeView> listFees = null;

            var response = await apiServices.GetAsync($"AccountBook/{user.numl_id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                listFees = JsonConvert.DeserializeObject<List<UnPaidFeeView>>(data);
            }

            return listFees;
        } 
    }
}