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
using System.Web.Services.Description;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireUser: true)]
    public class GenerateChallanController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        feePlanService fService;
        miscellaneousFeeService mFee;
        sessionService sessionServices;
        busRouteService busRouteService;
        installmentService installmentServices;
        apiService apiServices;

        public GenerateChallanController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            fService = new feePlanService(baseAddress);
            mFee = new miscellaneousFeeService(baseAddress);
            sessionServices = new sessionService(baseAddress);
            busRouteService = new busRouteService(baseAddress);
            installmentServices = new installmentService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        // Generate Challan
        public async Task<ActionResult> generateChallan()
        {
            Users user = userAccess();

            ViewBag.Display = "none;";
            ViewBag.gernerate = "none;";

            ViewBag.challanType = new SelectList(await fService.getFeeForUser(user), "value", "text");


            ViewBag.Session = await sessionServices.addSessiontoListAsyncWithEligibility(user.admission_session);
            ViewBag.routes = await busRouteService.GetSelectedBusRoute(null, user.dept_id);


            return View();
        }

        // Generate Challan
        [HttpPost]
        public async Task<ActionResult> generateChallan(UnpaidFees unpaidFee)
        {
           Users user = userAccess();

            ViewBag.Display = "none;";

            ViewBag.challanType = new SelectList(await fService.getFeeForUser(user), "value", "text");
            ViewBag.Session = await sessionServices.addSessiontoListAsyncWithEligibility(user.admission_session);
            ViewBag.routes = await busRouteService.GetSelectedBusRoute(null, user.dept_id);

            int sessionId = Convert.ToInt32(Request.Form["sessionDropdown"]);
            int feeFor = Convert.ToInt32(Request.Form["challanDropDown"]);
            int mode = Convert.ToInt32(Request.Form["installmentsDropDown"]);


            DateTime issueDateTime = DateTime.Now;
            string issueDate = issueDateTime.Date.ToString("yyyy-MM-dd");

            unpaidFee.fee_type = 1;
            unpaidFee.std_numl_id = user.numl_id;
            unpaidFee.semester = user.semester;
            unpaidFee.issue_date = issueDate;

            HttpResponseMessage responseMessage = null;

            if (feeFor == 2)
            {
                int route = 0;
                if (int.TryParse(Request.Form["routeDropdown"], out route)){
                    responseMessage = await apiServices.PostAsync($"GenerateChallan/{sessionId}/{feeFor}/{mode}/{user.degree_id}/{user.admission_session}/{user.fee_plan}/{route}", unpaidFee);
                }
                else
                {
                    string responseData = "Bus Route is required!";
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                    ViewBag.gernerate = "none;";
                }

            }
            else
            {
                responseMessage = await apiServices.PostAsync($"GenerateChallan/{sessionId}/{feeFor}/{mode}/{user.degree_id}/{user.admission_session}/{user.fee_plan}/{user.dept_id}", unpaidFee);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-success";
                ViewBag.Display = "block;";
                ViewBag.gernerate = "none;";
            }
            else
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
                ViewBag.gernerate = "none;";
            }

            return View();
        }

        // Generate Challan
        public async Task<ActionResult> generateMChallan()
        {
            userAccess();

            ViewBag.Display = "none;";
            ViewBag.gernerate = "none;";

            ViewBag.challanType = await mFee.getSelectedMiscellaneousFees(null);

            return View();
        }

        // Generate Challan
        [HttpPost]
        public async Task<ActionResult> generateMChallan(UnpaidFees unpaidFee)
        {
           Users user =  userAccess();

            ViewBag.Display = "none;";
            ViewBag.gernerate = "none;";

            ViewBag.challanType = await mFee.getSelectedMiscellaneousFees(null);

            int feeFor = Convert.ToInt32(Request.Form["challanDropDown"]);
            int method = Convert.ToInt32(Request.Form["paymentDropDown"]);

            DateTime issueDateTime = DateTime.Now;
            string issueDate = issueDateTime.Date.ToString("yyyy-MM-dd");

            unpaidFee.fee_type = 2;
            unpaidFee.std_numl_id = user.numl_id;
            unpaidFee.semester = user.semester;
            unpaidFee.fee_id = feeFor;
            unpaidFee.issue_date = issueDate;

            HttpResponseMessage responseMessage = await apiServices.PostAsync($"GenerateChallan", unpaidFee);

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-success";
                ViewBag.Display = "block;";
                ViewBag.gernerate = "none;";
            }
            else
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
                ViewBag.gernerate = "none;";
            }

            return View();
        }

        public async Task<ActionResult> getInstallments(int? fee_for)
        {
            SelectList installmentList = await installmentServices.addInstallmentToListAsync(Convert.ToInt32(fee_for));

            List<SelectListItem> installmentOptions = new List<SelectListItem>(installmentList.Items.Cast<SelectListItem>());

            return Json(installmentOptions, JsonRequestBehavior.AllowGet);
        }
    }
}