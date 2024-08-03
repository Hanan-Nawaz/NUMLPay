using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json;
using NUMLPay_WebApp.CreateModel;
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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole:new int[] {3, 4})]
    public class ChallanVerificationController : SessionController
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

        public ChallanVerificationController()
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
        public async Task<ActionResult> viewUnverifiedChallans()
        {
            Admin admin = userAccessAdmin();
            ViewBag.Display = "none;";

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

            List<ChllanVerificationView> listUnverifiedFees = await getUnpaidFees(admin.dept_id);

            return View(listUnverifiedFees);
        }

        public async Task<List<ChllanVerificationView>> getUnpaidFees(int? id)
        {
            List<ChllanVerificationView> listBusRoutes = await getUnpaidFee(id); ;
            return listBusRoutes;
        }

        public async Task<ActionResult> sessionGenerator(int dept)
        {
            Session["dept"] = dept;

            return RedirectToAction("viewUnverifiedChallans");
        }

        //Verify Challan 
        public async Task<ActionResult> verifyChallan(int Id,int feeType)
        {
            Admin admin = userAccessAdmin();
            ViewBag.Display = "none;";

            JoinedDataChallan challanData = await FetchChallanData(Id, feeType);

            ViewBag.challanNo = challanData.challan_no;
            ViewBag.name = challanData.name;
            ViewBag.numlId = challanData.numl_id;
            ViewBag.feePlan = challanData.fee_plan;
            ViewBag.degreeName = challanData.degree_name;
            ViewBag.challanType = challanData.FeeFor;
            ViewBag.feeSem = challanData.feeSem;
            ViewBag.dueDate = Convert.ToDateTime(challanData.due_date).ToString("dd/MMM/yyyy");
            ViewBag.totalFee = challanData.total_fee;
            ViewBag.fine = challanData.fine;
            ViewBag.totalFeewithFine = (challanData.fine + challanData.total_fee);
            ViewBag.image = challanData.image;

            Session["Id"] = Id;
            Session["feeType"] = feeType;

            return View();
        }

        //Verify Challan POST
        [HttpPost]
        public async Task<ActionResult> verifyChallan()
        {
            Admin admin = userAccessAdmin();
            ViewBag.Display = "none;";

            if (admin.role == 4)
            {
                ViewBag.adminRoles = "block;";
            }
            else
            {
                ViewBag.adminRoles = "none;";
            }

            int Id = Convert.ToInt32(Session["Id"]);
            int feeType = Convert.ToInt32(Session["feeType"]);
            string reason = Request.Form["disapproved"].ToString();
            JoinedDataChallan challanData = await FetchChallanData(Id, feeType);


            CreateFeeAndAccountBook newFeeAndAccount = new CreateFeeAndAccountBook();
            newFeeAndAccount.Id = Id;
            newFeeAndAccount.verified_by = admin.email_id;
            newFeeAndAccount.status = Convert.ToInt32(Request.Form["decision"]);
            newFeeAndAccount.numlId = challanData.numl_id;


            if (newFeeAndAccount.status == 3 && reason == null) 
            {
                string responseData = "Please add reason of Rejection";
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
            }
            else{
                HttpResponseMessage responseMessage = await apiServices.PostAsync("AccountBook/CreateAccountBookbyBank", newFeeAndAccount);

                if (responseMessage.IsSuccessStatusCode)
                {
                    if(newFeeAndAccount.status == 3)
                    {
                        string body = $"Dear {challanData.name},\r\n\r\n" +
                          "We hope this email finds you well. We regret to inform you that your recent fee verification request through NUMLPay has been rejected.\r\n\r\n" +
                          "Here are the details of your rejected request:\r\n\r\n" +
                          $"Student ID: {challanData.numl_id}\r\n" +
                          $"Name: {challanData.name}\r\n" +
                          $"Degree: {challanData.degree_name}\r\n" +
                          $"Semester: {challanData.feeSem}\r\n" +
                          $"Challan Number: {challanData.challan_no}\r\n" +
                          $"Payment Method: By Bank\r\n\r\n" +
                          $"Reason of Rejectionb: {reason}\r\n\r\n" +
                          "We understand that this news may be disappointing. If you have any questions or concerns about the rejection, please feel free to reach out to our support team at: " +
                          "numlfeepay@gmail.com.\r\n\r\n" +
                          "We apologize for any inconvenience this may have caused. Thank you for choosing NUMLPay for your fee verification needs. You are Requested to Either Vist CoOrdinator Office or Update that request from Dashboard same way you Generate this one.\r\n\r\n" +
                          "Best regards,\r\n" +
                          "The NUMLPay Team\r\n\r\n" +
                          "Note: This email is for informational purposes only. No further action is required on your part. Please save this email, as it may be needed in the future.\r\n";


                        eService.Send(challanData.email, "Fee Verification Request Rejection - NUMLPay", body);
                    }
                    else
                    {
                        string body = $"Dear {challanData.name},\r\n\r\n" +
                             "We hope this email finds you well. We are pleased to inform you that your fee verification request through NUMLPay has been approved.\r\n\r\n" +
                             "Here are the details of your approved request:\r\n\r\n" +
                             $"Student ID: {challanData.numl_id}\r\n" +
                             $"Name: {challanData.name}\r\n" +
                             $"Degree: {challanData.degree_name}\r\n" +
                             $"Semester: {challanData.feeSem}\r\n" +
                             $"Challan Number: {challanData.challan_no}\r\n" +
                             $"Payment Method: By Bank\r\n\r\n" +
                             "With your fee verification request now successfully approved, you can continue with your academic pursuits with confidence. If you have any further questions or need assistance, please don't hesitate to reach out to our support team at: " +
                             "numlfeepay@gmail.com.\r\n\r\n" +
                             "Thank you for choosing NUMLPay for your fee verification needs. We look forward to serving you again in the future.\r\n\r\n" +
                             "Best regards,\r\n" +
                             "The NUMLPay Team\r\n\r\n" +
                             "Note: This email is for informational purposes only. No further action is required on your part. Please save this email for your reference.\r\n";

                        eService.Send(challanData.email, "Fee Verification Request Approval - NUMLPay", body);

                    }

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
            }

            List<ChllanVerificationView> listUnverifiedFees = await getUnpaidFee(admin.dept_id);

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;
            return View("viewUnverifiedChallans", listUnverifiedFees);
        }

        //Fetch Main Chllan Data
        private async Task<JoinedDataChallan> FetchChallanData(int id, int feeType)
        {
            JoinedDataChallan listFee = null;

            HttpResponseMessage responseMessage = await apiServices.GetAsync($"GetChallan/GetChallan/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listFee = JsonConvert.DeserializeObject<JoinedDataChallan>(responseData);
            }

            return listFee;
        }

        private async Task<List<ChllanVerificationView>> getUnpaidFee(int? id)
        {
            List<ChllanVerificationView> listUnverifiedFees = null;
            var response = await apiServices.GetAsync($"ChallanVerification/GetByDeptId/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                listUnverifiedFees = JsonConvert.DeserializeObject<List<ChllanVerificationView>>(data);
            }

            return listUnverifiedFees;
        }

    }
}