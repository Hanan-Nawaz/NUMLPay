using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using NUMLPay_WebApp.ModelFront;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NUMLPay_WebApp.ViewModel;
using Humanizer;
using Stripe;
using Stripe.Checkout;
using System.IO;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireUser: true)]
    public class MainController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        Uri domainAddress = new Uri(ConfigurationManager.AppSettings["DomainBaseUrl"]);
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

        public MainController()
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

        // Dashboard
        public async Task<ActionResult> Dashboard()
        {
           Users user = userAccess();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.Method = new SelectList(StatusService.getMethod(null), "value", "text");

            List<UnPaidFeeView> listFee = await getUnpaidFee(user.numl_id);

            return View(listFee);
        }

        //Print Challan
        public async Task<ActionResult> DownloadChallanPdf(int Id, int feeType, int fee_for)
        {
            Users user = userAccess();
            try
            {
                JoinedDataChallan challanData = await FetchChallanData(Id, feeType, fee_for);

                float number = challanData.total_fee + challanData.fine;

                int integerPart = (int)Math.Floor(number);
                string integerWords = integerPart.ToWords();

                string englishWords = $"{integerWords} only";


                var parameters = new ReportParameterCollection
                {
                    new ReportParameter("challanNumber", challanData.challan_no.ToString()),
                    new ReportParameter("issueDate", Convert.ToDateTime(challanData.issue_date).ToString("dd/MMM/yyyy")),
                    new ReportParameter("dueDate", Convert.ToDateTime(challanData.due_date).ToString("dd/MMM/yyyy")),
                    new ReportParameter("validDate", Convert.ToDateTime(challanData.valid_date).ToString("dd/MMM/yyyy")),
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
                reportViewer.LocalReport.ReportPath = Server.MapPath("~/Challans/unPaidChallan.rdlc");
                reportViewer.LocalReport.SetParameters(parameters);
                

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("UnPaidChallan", subChallanData));

                byte[] pdfBytes = reportViewer.LocalReport.Render("PDF");

                // Return the PDF file for download
                return File(pdfBytes, "application/pdf", challanData.name.ToString() + "_" + challanData.FeeFor + "_FeeChallan.pdf");
            }
            catch (Exception ex)
            {
                ViewBag.AlertMessage = "An error occurred while generating the report: " + ex.Message;
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = ViewBag.AlertMessage;
                TempData["Display"] = "block;";
                List<UnPaidFeeView> listFees = await getUnpaidFee(user.numl_id);

                return RedirectToAction("Dashboard", listFees);
            }
        }

      
        //Profile User
        public async Task<ActionResult> profileUser()
            {
                Users user = userAccess();

                Tuple<int, Users> userTuple = await getUserbyId(user.numl_id);
                Users userId = userTuple.Item2;

                return View(userId);
            }


        //Helper Method to get Users
        public async Task<Tuple<int, Users>> getUserbyId(string Id)
        {
            Users userId = new Users();
            userId.numl_id = HttpUtility.UrlDecode(Id);

            using (var response = await apiServices.PostAsync($"Users/GetUsers", userId))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    userId = JsonConvert.DeserializeObject<Users>(data);
                }
            }

            ViewBag.Display = "none;";

            int? selectedValueDept = userId.dept_id;

            Tuple<int?, List<SelectListItem>> selectedDept = await departmentServices.getSelectedDepartments(selectedValueDept);
            ViewBag.deptList = selectedDept.Item2;

            Tuple<int?, List<SelectListItem>> selectedFaculty = await facultyServices.getSelectedFaculties(selectedDept.Item1);
            ViewBag.facultyList = selectedFaculty.Item2;

            ViewBag.campusList = await campusService.getSelectedCampus(selectedFaculty.Item1);

            ViewBag.is_active = StatusService.getStatusUsers(userId.is_active);

            ViewBag.statusDegree = new SelectList(StatusService.getStatusDegree(userId.status_of_degree), "Value", "Text");

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(Convert.ToInt32(userId.admission_session));

            ViewBag.feePlan = await feeService.addFeePlantoListAsync(userId.fee_plan);

            Shift shift = new Shift();

            using (var response = await apiServices.GetAsync($"Shift/GetbyId/{userId.degree_id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    shift = JsonConvert.DeserializeObject<Shift>(data);
                }
            }

            ViewBag.academicLevels = levelService.getLevel(shift.academic_id);

            int selectedDeptID = (int)selectedValueDept;
            List<SelectListItem> listDegree = await degreeServices.getDegrees(selectedDeptID, shift.academic_id, shift.Id);
            ViewBag.degreeList = listDegree;

            EligibleFees stdFees = new EligibleFees();
            stdFees.std_numl_id = userId.numl_id;

            using (var response = await apiServices.PostAsync($"Users/GetEligibleFees", stdFees))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    stdFees = JsonConvert.DeserializeObject<EligibleFees>(data);
                }
            }

            ViewBag.busFee = StatusService.getFeesUsers(stdFees.bus_fee);

            ViewBag.semFee = StatusService.getFeesUsers(stdFees.semester_fee);

            ViewBag.hostelFee = StatusService.getFeesUsers(stdFees.hostel_fee);

            return Tuple.Create(stdFees.id, userId);
        }

        public async Task<ActionResult> OnlineMethod(int Id, int feeType, int fee_for)
        {
            Users user = userAccess();
            ViewBag.Display = "none;";

            JoinedDataChallan challanData = await FetchChallanData(Id, feeType, fee_for);
            int totalFee = Convert.ToInt32(challanData.total_fee);
            int Fine = Convert.ToInt32(challanData.fine);
            int totalFeewithFine = totalFee + Fine;

            Session["totalFee"] = totalFeewithFine;
            Session["StripeFeeId"] = Id;
            Session["degreeName"] = challanData.degree_name;

            // Set your Stripe secret key
            StripeConfiguration.ApiKey = "sk_test_26PHem9AhJZvU623DfE1x4sd";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                 {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "pkr",
                            UnitAmount = totalFeewithFine * 100, // Amount in cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = challanData.name + " ("+ (challanData.numl_id) + ")",
                                Description ="Due Date is " + Convert.ToDateTime(challanData.due_date).ToString("dd/MMM/yyyy") ,
                                Images = new List<string>
                                {
                                    "https://www.numl.edu.pk/images/logo/logo.png"
                                },
                                Metadata = new Dictionary<string, string>
                                {
                                    { "Student ID", challanData.numl_id },
                                    { "Category", challanData.FeeFor }
                                }
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = domainAddress + "Main/successPage", // success URL
                CancelUrl = domainAddress + "Main/Dashboard" // cancel URL
            };

            var service = new SessionService();
            var session = service.Create(options);

            // Redirect the user to the Stripe Checkout page
            return Redirect(session.Url);
        }

        //Pay by Bank 
        public async Task<ActionResult> payByBankMethod(int Id, int feeType, int fee_for)
        {
            Users user = userAccess();
            ViewBag.Display = "none;";

            JoinedDataChallan challanData = await FetchChallanData(Id, feeType, fee_for);

            Session["IdforBank"] = Id;

            ViewBag.challanNo = challanData.challan_no;
            ViewBag.name = challanData.name;
            ViewBag.numlId = challanData.numl_id;
            ViewBag.feePlan = challanData.fee_plan;
            ViewBag.degreeName = challanData.degree_name;
            ViewBag.challanType = challanData.FeeFor ?? "-";
            ViewBag.feeSem = challanData.feeSem;
            ViewBag.dueDate = Convert.ToDateTime(challanData.due_date).ToString("dd/MMM/yyyy");
            ViewBag.totalFee = challanData.total_fee;
            ViewBag.fine = challanData.fine;
            ViewBag.totalFeewithFine = (challanData.fine + challanData.total_fee);

            return View();
        }

        //Pay by Bank 
        [HttpPost]
        public async Task<ActionResult> payByBankMethod(ChallanVerification challanVerification, HttpPostedFileBase imageFile)
        {
            Users user = userAccess();
            ViewBag.Display = "none;";

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(imageFile.FileName);

                string uniqueFileName = $"{user.numl_id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

                string filePath = Path.Combine(Server.MapPath("~/ChallanImages/"), uniqueFileName);
           
                    imageFile.SaveAs(filePath);

                    challanVerification.image = "/ChallanImages/" + uniqueFileName;
                    challanVerification.fee_installment_id = Convert.ToInt32(Session["IdforBank"]);

                    HttpResponseMessage responseMessage = await apiServices.PostAsync("ChallanVerification", challanVerification);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        ViewBag.AlertMessage = responseData;
                        ViewBag.AlertType = "alert-success";
                        ViewBag.Display = "block;";

                         string body = $"Dear {user.name},\r\n\r\n" +
                        "We hope this email finds you well. We would like to inform you that your request for fee verification through NUMLPay has been successfully created.\r\n\r\n" +
                        "Here are the details of your request:\r\n\r\n" +
                        $"Student ID: {user.numl_id}\r\n" +
                        $"Payment Method: By Bank\r\n" +
                        "Please note that an administrative review of your request will be conducted, and you can expect a response within 7 working days. During this time, our team will thoroughly verify the provided information to ensure accuracy.\r\n\r\n" +
                        $"If you have any urgent inquiries or concerns, please don't hesitate to contact our support team at numlfeepay@gamil.com.\r\n\r\n" +
                        "Thank you for choosing NUMLPay for your fee payment needs. We appreciate your trust and look forward to serving you.\r\n\r\n" +
                        "Best regards,\r\n" +
                        "The NUMLPay Team";

                    eService.Send(user.email, "Request for Fee Verification - Confirmation NUMLPay", body);
                }
                else
                    {
                        string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        ViewBag.AlertMessage = responseData;
                        ViewBag.AlertType = "alert-danger";
                        ViewBag.Display = "block;";
                    }
                }
            else
            {
                string responseData = "Please add Paid Challan Image!";
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
            }

            ViewBag.Method = new SelectList(StatusService.getMethod(null), "value", "text");

            List<UnPaidFeeView> listFee = await getUnpaidFee(user.numl_id);
            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;
            return View("Dashboard", listFee);

        }

        //Pay by Bank 
        public async Task<ActionResult> successPage()
        {
            Users user = userAccess();
            ViewBag.Display = "none;";

            if(Convert.ToInt32(Session["StripeFeeId"]) != 0)
            {
                try
                {
                    string body = "Dear " + user.name + ",\r\n\r\nWe are delighted to inform you that your" +
                        " academic fee payment has been successfully processed through NUMLPay, our secure and user-friendly" +
                        " payment platform. This email serves as an official confirmation of your payment for the semester/term " +
                        "fee.\r\n\r\nPayment Details:\r\n\r\nStudent Name: " + user.name + "\r\nStudent ID: " + user.numl_id + "\r\n" +
                        "Degree Program: " + Session["degreeName"].ToString() + "\r\nFee Amount: RS " + Session["totalFee"].ToString() + "\r\nPayment Method: NUMLPay\r\nPayment Date: " +
                        "" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + "\r\n\r\nWith your fee now successfully paid, you can seamlessly continue with" +
                        " your academic pursuits, secure in the knowledge that your enrollment remains uninterrupted and that you have " +
                        "full access to the wide array of resources that NUML provides.\r\n\r\nShould you have any queries or require further " +
                        "assistance related to your payment or any other aspect, please do not hesitate to get in touch with our dedicated" +
                        " support team at numlfeepay@gmail.com. We are here to provide you with the assistance " +
                        "you may need.\r\n\r\nYour confidence in NUMLPay is greatly appreciated, and we eagerly anticipate the opportunity to " +
                        "contribute to your ongoing academic journey.\r\n\r\nBest regards,\r\n\r\nNUMLPay" +
                        "\r\n\r\nNote: This email is for informational purposes only. No further action is required on your part.\r\n";
                    eService.Send(user.email, "Confirmation of Payment - Your Fee is Paid through NUMLPay", body);

                    AccountBook aBook = new AccountBook();
                    aBook.challan_no = Convert.ToInt32(Session["StripeFeeId"]);
                    aBook.std_numl_id = user.numl_id;

                    HttpResponseMessage responseMessage = await apiServices.PostAsync($"AccountBook", aBook);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        ViewBag.Titles = "Thank You";
                        ViewBag.Message = responseData;
                        ViewBag.Class = "border-success";
                        ViewBag.Icon = "1";
                    }
                    else
                    {
                        string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        ViewBag.Titles = "Error!!";
                        ViewBag.Message = responseData;
                        ViewBag.Class = "border-danger";
                        ViewBag.Icon = "2";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                ViewBag.Titles = "Error!!";
                ViewBag.Message = "Fee not Paid";
                ViewBag.Class = "border-danger";
                ViewBag.Icon = "2";
            }

            return View();
        }

        //Fetch Main Chllan Data
        private async Task<JoinedDataChallan> FetchChallanData(int id, int feeType, int fee_for)
        {
            JoinedDataChallan listFee = null;

            HttpResponseMessage responseMessage = await apiServices.GetAsync($"GetChallan/Get/{id}/{feeType}/{fee_for}");
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

        private async Task<List<UnPaidFeeView>> getUnpaidFee(string id)
        {
            List<UnPaidFeeView> listFee = null;
            var response = await apiServices.GetAsync($"GenerateChallan/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                listFee = JsonConvert.DeserializeObject<List<UnPaidFeeView>>(data);
            }

            return listFee;
        }

    }
}