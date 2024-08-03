using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using NUMLPay_WebApp.Models;
using Newtonsoft.Json;
using System.Web.Razor.Parser.SyntaxTree;
using System.Text;
using NUMLPay_WebApp.Services;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using NUMLPay_WebApp.ViewModel;
using Humanizer;
using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using static Humanizer.In;
using System.Security.Policy;

namespace NUMLPay_WebApp.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        Uri domainAddress = new Uri(ConfigurationManager.AppSettings["DomainBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        departmentService deptService;
        academicLevelService levelService;
        sessionService sessionServices;
        feePlanService feeService;
        emailService emailService;
        apiService apiServices;

        public HomeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            deptService = new departmentService(baseAddress);
            levelService = new academicLevelService();
            sessionServices = new sessionService(baseAddress);
            feeService = new feePlanService(baseAddress);
            emailService = new emailService();
            apiServices = new apiService(baseAddress.ToString());
        }

        public async Task<ActionResult> Index()
        {

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.campusList = await campusService.addCampustoListAsync();
            return View();
        }

        public ActionResult sendEmail()
        {
            string name = Request.QueryString["name"];
            string email = Request.QueryString["Email"];
            string phone_num = Request.QueryString["PhoneNumber"];
            string message = Request.QueryString["Message"];

            if (name == "" || email == "" || phone_num == "" || message == "")
            {
                ViewBag.AlertMessage = "Please Fill all Data";
                TempData["AlertType"] = "alert-danger";
                TempData["AlertMessage"] = ViewBag.AlertMessage;
                TempData["Display"] = "block;";
                return RedirectToAction("Index");
            }
            else
            {
                string emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
                Regex regex = new Regex(emailPattern);
                if (regex.IsMatch(email))
                {
                    string body = $"Dear {name},\r\n\r\n" +
                        "Thank you for reaching out to NUMLPay. We have received your inquiry and will process it promptly. Please find below a copy of your message for your records:\r\n\r\n" +
                        "Here are the details of your request:\r\n\r\n" +
                        $"Name: {name}\r\n" +
                        $"Phone Number: {phone_num}\r\n" +
                        $"Email: {email}\r\n" +
                        $"Message: {message}\r\n" +
                        "Please note that an administrative review of your request will be conducted, and you can expect a response within 7 working days.\r\n\r\n" +
                        $"If you have any urgent inquiries or concerns, please don't hesitate to contact our support team at numlfeepay@gamil.com.\r\n\r\n" +
                        "Thank you for contacting NUMLPay. We appreciate your trust and look forward to serving you.\r\n\r\n" +
            "Best regards,\r\n" +
            "The NUMLPay Team";

                    emailService.Send(email, "Thank You for Contacting - NUMLPay", body);

                    string bodyAdmin = $"Dear Admin,\r\n\r\n" +
                            "A new contact form submission has been received. Here are the details:\r\n\r\n" +
                            $"Name: {name}\r\n" +
                            $"Phone Number: {phone_num}\r\n" +
                            $"Email: {email}\r\n" +
                            $"Message: {message}\r\n" +
                            "Please take appropriate action and respond to the sender as soon as possible.\r\n\r\n" +
                "Best regards,\r\n" +
                "The NUMLPay Team";

                    emailService.Send("numlfeepay@gmail.com", "New Contact Form Submission - NUMLPay", bodyAdmin);

                    ViewBag.AlertMessage = "Email Sent. Thank You for Contacting NUMLPay.";
                    TempData["AlertType"] = "alert-success";
                    TempData["AlertMessage"] = ViewBag.AlertMessage;
                    TempData["Display"] = "block;";

                    return RedirectToAction("Index");

                }
                else
                {
                    ViewBag.AlertMessage = "Please Enter Valid Email Address.";
                    TempData["AlertType"] = "alert-danger";
                    TempData["AlertMessage"] = ViewBag.AlertMessage;
                    TempData["Display"] = "block;";
                    return RedirectToAction("Index");
                }
            }



            return View();
        }


        public ActionResult Login()
        {
            ViewBag.Display = "none;";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(Users user)
        {
             Users loggedUser = new Users();

            HttpResponseMessage responseMessage = await apiServices.PostAsync("Users/Login", user);

            if (responseMessage.IsSuccessStatusCode)
             {
                string data = responseMessage.Content.ReadAsStringAsync().Result;

                 loggedUser = JsonConvert.DeserializeObject<Users>(data);

                 if (loggedUser != null)
                 {
                    Session["loggedUser"] = loggedUser;
                    Session["As"] = 1;
                    Session["Image"] = loggedUser.image;
                    return RedirectToAction("Dashboard", "Main");
                 }
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

        public ActionResult loginAdmin()
        {
            ViewBag.Display = "none;";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> loginAdmin(Admin admin)
        {
            Admin loggedUser;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("/Admin/Login", admin);

            if (responseMessage.IsSuccessStatusCode)
            {
                loggedUser = new Admin();
                string data = responseMessage.Content.ReadAsStringAsync().Result;

                loggedUser = JsonConvert.DeserializeObject<Admin>(data);

                if (loggedUser != null)
                {
                    Session["As"] = 2;
                    Session["loggedUser"] = loggedUser;
                    Session["Image"] = "/images/numl-logo.png";
                    if(loggedUser.role == 2)
                    {
                        return RedirectToAction("accountantDashboard", "Dashboard");
                    }
                    else if (loggedUser.role == 1)
                    {
                        return RedirectToAction("campusDashboard", "Dashboard");
                    }
                    else if (loggedUser.role == 3)
                    {
                        return RedirectToAction("deptDashboard", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("MainDashboard", "Dashboard");

                    }
                }
            }
            else
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";

                return View("loginAdmin");
            }

            return View();

        }

        public async Task<ActionResult> Register()
        {
            ViewBag.Display = "none;";
            ViewBag.campusList = await campusService.addCampustoListAsync();
            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);
            ViewBag.feePlan = await feeService.addFeePlantoListAsync(null);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(Users newUser, HttpPostedFileBase imageFile)
        {
            ViewBag.campusList = await campusService.addCampustoListAsync();
            ViewBag.academicLevels = new SelectList(levelService.getLevel(null), "Value", "Text");
            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);
            ViewBag.feePlan = await feeService.addFeePlantoListAsync(null);

            int hostelfeeDdl = int.Parse(Request.Form["hostelDdl"]);
            int busfeeDdl = int.Parse(Request.Form["busDdl"]);

            if(hostelfeeDdl == 0 || busfeeDdl == 0)
            {
                ViewBag.AlertMessage = "Please Enter Valid Data";
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
                return View(newUser);
            }
            else
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(imageFile.FileName);

                    string uniqueFileName = $"{newUser.numl_id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

                    string filePath = Path.Combine(Server.MapPath("~/UserImage/"), uniqueFileName);

                    HttpResponseMessage responseMessageSemester = await apiServices.GetAsync($"Session/GetSemester/{newUser.admission_session}");

                    if (responseMessageSemester.IsSuccessStatusCode)
                    {
                        string responseSemester = responseMessageSemester.Content.ReadAsStringAsync().Result;
                        imageFile.SaveAs(filePath);

                        newUser.image = "/UserImage/" + uniqueFileName;

                        newUser.semester = Convert.ToInt32(responseSemester);

                        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.password, salt);

                        newUser.password = hashedPassword;

                        newUser.status_of_degree = 1;
                        newUser.is_active = 2;

                        EligibleFees eligibleFees = new EligibleFees();

                        eligibleFees.hostel_fee = hostelfeeDdl;
                        eligibleFees.semester_fee = 1;
                        eligibleFees.bus_fee = busfeeDdl;
                        eligibleFees.std_numl_id = newUser.numl_id;

                        HttpResponseMessage responseMessage = await apiServices.PostAsync("Users", newUser);

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            HttpResponseMessage responseMessageFees = await apiServices.PostAsync("Users/EligibleFees", eligibleFees);

                            if (responseMessageFees.IsSuccessStatusCode)
                            {
                                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                                ViewBag.AlertMessage = responseData;
                                ViewBag.AlertType = "alert-success";
                                ViewBag.Display = "block;";
                            }
                        }
                        else
                        {
                            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                            ViewBag.AlertMessage = responseData;
                            ViewBag.AlertType = "alert-danger";
                            ViewBag.Display = "block;";
                        }
                    }

                    return View();
                }
                else
                {
                    ViewBag.AlertMessage = "Please Enter Valid Image";
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                    return View(newUser);
                }
            }
           
            }


        public async Task<ActionResult> forgotPasswordAdmin()
        {
            ViewBag.Display = "none;";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> forgotPasswordAdmin(Admin admin)
        {
            ViewBag.Display = "none;";

            string token = GenerateToken();

            admin.fp_token = token;
            admin.fp_token_expiry = DateTime.Now.AddMinutes(30);

            using (var response = await apiServices.PutAsync($"Admin/ForgotPassword", admin))
            {
                if (response.IsSuccessStatusCode)
                {
                    string resetPasswordLink = domainAddress + "Home/resetPasswordAdmin?token=" + admin.fp_token + "&email=" + admin.email_id;


                    string emailContent = "\n" +
                            "Dear Admin,\n" +
                            "\n\nA password reset request has been initiated for the NUMLPay system. To proceed with resetting the password, please click on the link provided below.\n\n" +
                            "\nReset Password Link: " + resetPasswordLink + "\n" +
                            "\nPlease note that this link will expire in 30 minutes for security reasons. If you did not request this password reset, you can safely ignore this email.\n" +
                            "\nIf you encounter any issues or have any concerns, please feel free to contact our support team.\n" +
                            "\nThank you for your attention to this matter.\n" +
                            "\nBest regards," +
                            "\nNUMLPay Team";


                    emailService.Send(admin.email_id, "Password Reset Request! Action Required - NUMLPay", emailContent);


                    ViewBag.AlertMessage = "Password Reset Email Sent! Check your inbox for instructions.";
                    ViewBag.AlertType = "alert-success";
                    ViewBag.Display = "block;";
                }
                else
                {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }
            }


            return View();
        }

        public async Task<ActionResult> resetPasswordAdmin(string token, string email)
        {
            ViewBag.Display = "none;";
            Session["token_fp"] = HttpUtility.UrlDecode(token);
            Session["email_fp"] = email;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> resetPasswordAdmin(Admin admin)
        {
            ViewBag.Display = "none;";

            //in name coming confirm password from view
            if(admin.password != admin.name)
            {
                ViewBag.AlertMessage = "Both Passwords doesn't match. Kindly Review.";
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
            }
            else
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(admin.password, salt);

                admin.email_id = Session["email_fp"].ToString();
                admin.fp_token = Session["token_fp"].ToString();
                admin.password = hashedPassword;

                using (var response = await apiServices.PutAsync($"Admin/ResetPassword", admin))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string emailContent =
                        "Dear Admin,\n" +
                        "\nThis is to confirm that the password for your NUMLPay account has been successfully reset.If you did not request this change, please contact our support team immediately.\n" +
                        "\nThank you,\n" +
                        "\nNUMLPay Team";

                        emailService.Send(admin.email_id, "Password Reset Confirmation - NUMLPay", emailContent);


                        ViewBag.AlertMessage = "Password Reset Successfully!";
                        ViewBag.AlertType = "alert-success";
                        ViewBag.Display = "block;";
                    }
                    else
                    {
                        string responseData = response.Content.ReadAsStringAsync().Result;
                        ViewBag.AlertMessage = responseData;
                        ViewBag.AlertType = "alert-danger";
                        ViewBag.Display = "block;";
                    }
                }
            }

           

            return View();
        }


        public async Task<ActionResult> forgotPasswordUser()
        {
            ViewBag.Display = "none;";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> forgotPasswordUser(Users user)
        {
            ViewBag.Display = "none;";


            string token = GenerateToken();

            user.fp_token = token;
            user.fp_token_expiry = DateTime.Now.AddMinutes(30).ToString();

            using (var response = await apiServices.PutAsync($"Users/ForgotPassword", user))
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    user.email = responseData.Trim('"');
                    string resetPasswordLink = domainAddress + "Home/resetPasswordUser?token=" + user.fp_token + "&email=" + user.email + "&numlId=" + user.numl_id;


                    string emailContent = "\n" +
                            "Dear User with NUML ID: " + user.numl_id + ",\n" +
                            "\n\nA password reset request has been initiated for the NUMLPay system. To proceed with resetting the password, please click on the link provided below.\n\n" +
                            "\nReset Password Link: " + resetPasswordLink + "\n" +
                            "\nPlease note that this link will expire in 30 minutes for security reasons. If you did not request this password reset, you can safely ignore this email.\n" +
                            "\nIf you encounter any issues or have any concerns, please feel free to contact our support team.\n" +
                            "\nThank you for your attention to this matter.\n" +
                            "\nBest regards," +
                            "\nNUMLPay Team";


                    emailService.Send(user.email, "Password Reset Request! Action Required - NUMLPay", emailContent);


                    ViewBag.AlertMessage = "Password Reset Email Sent! Check your inbox for instructions.";
                    ViewBag.AlertType = "alert-success";
                    ViewBag.Display = "block;";
                }
                else
                {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }
            }

            return View();
        }

        public async Task<ActionResult> resetPasswordUser(string token, string email, string numlId)
        {
            ViewBag.Display = "none;";
            Session["token_fp"] = HttpUtility.UrlDecode(token);
            Session["email_fp"] = email;
            Session["numlId_fp"] = numlId;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> resetPasswordUser(Users user)
        {
            ViewBag.Display = "none;";

            //in name coming confirm password from view
            if (user.password != user.name)
            {
                ViewBag.AlertMessage = "Both Passwords doesn't match. Kindly Review.";
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
            }
            else
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password, salt);

                user.numl_id = Session["numlId_fp"].ToString();
                user.email = Session["email_fp"].ToString();
                user.fp_token = Session["token_fp"].ToString();
                user.password = hashedPassword;

                using (var response = await apiServices.PutAsync($"Users/ResetPassword", user))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string emailContent =
                        "Dear User with NUML ID: " + user.numl_id + ",\n" +
                        "\nThis is to confirm that the password for your NUMLPay account has been successfully reset.If you did not request this change, please contact our support team immediately.\n" +
                        "\nThank you,\n" +
                        "\nNUMLPay Team";

                        emailService.Send(user.email, "Password Reset Confirmation - NUMLPay", emailContent);


                        ViewBag.AlertMessage = "Password Reset Successfully!";
                        ViewBag.AlertType = "alert-success";
                        ViewBag.Display = "block;";
                    }
                    else
                    {
                        string responseData = response.Content.ReadAsStringAsync().Result;
                        ViewBag.AlertMessage = responseData;
                        ViewBag.AlertType = "alert-danger";
                        ViewBag.Display = "block;";
                    }
                }
            }



            return View();
        }


        //Token Genertor for password reset
        public static string GenerateToken(int length = 32)
        {
            byte[] tokenData = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenData);
            }
            // Convert the byte array to a Base64 URL-safe string
            string base64Token = Convert.ToBase64String(tokenData)
                                            .Replace('+', '-')
                                            .Replace('/', '_')
                                            .Replace("=", "="); // Remove any padding characters
            return base64Token;
        }

        //Get Faculty on base of Campus
        public async Task<JsonResult> getFacultiesByCampusAsync(int? campusId)
        {
            if (campusId.HasValue)
            {
                List<Faculty> listFaculty = await facultyServices.getFacultybyCampusIdAsync(campusId.Value);
                var activeFaculties = listFaculty.Where(f => f.is_active == 1);
                SelectList facultyList = new SelectList(activeFaculties, "id", "name");
                var facultyOptions = facultyList.Select(f => new { Value = f.Value, Text = f.Text });

                return Json(facultyOptions, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        //Get Dept on base of Faculty
        public async Task<ActionResult> getDeptsByFacultyAsync(string facultyId)
        {
            List<Department> listDept = await deptService.getDepartmentAsync(Convert.ToInt32(facultyId));
            SelectList deptList = new SelectList(listDept, "id", "name");
            var deptOptions = deptList.Select(f => new { Value = f.Value, Text = f.Text });

            return Json(deptOptions, JsonRequestBehavior.AllowGet);
        }

        //Get Degree on base of Dept and Academic Level
        public async Task<ActionResult> getDegrees(string deptId, string levelId)
        {
            int deptIdValue = Convert.ToInt32(deptId);
            int levelIdValue = Convert.ToInt32(levelId);

            List<SelectListItem> shiftOptions = new List<SelectListItem>();

            List<DegreeView> listDegree = null;
            HttpResponseMessage responseMessage = await apiServices.GetAsync($"Degree/GetDegreebyDeptLevel/{deptIdValue}/{levelIdValue}");

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listDegree = JsonConvert.DeserializeObject<List<DegreeView>>(responseData);
            }
            if (listDegree != null)
            {
                foreach (var shifts in listDegree)
                {
                    shiftOptions.Add(new SelectListItem { Value = shifts.SId.ToString(), Text = shifts.name.ToString() });
                }
            }

            return Json(shiftOptions, JsonRequestBehavior.AllowGet);
        }

        //Print Challan
        public async Task<ActionResult> DownloadFeePdf(int campusId)
        {
            try
            {
                List<DownloadFee> feeList = await FetchChallanData(campusId);

                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                string currentSession = "";

                if (currentMonth >= 9 || currentMonth <= 1) // September to January
                {
                    currentSession = $"Fall {currentYear}";
                }
                else if (currentMonth >= 2 && currentMonth <= 6) // February to June
                {
                    currentSession = $"Spring {currentYear}";
                }

                var parameters = new ReportParameterCollection
                {
                    new ReportParameter("session", currentSession)
                };

                var reportViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/FeeStructure.rdlc");
                reportViewer.LocalReport.SetParameters(parameters);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("FeeStructure", feeList));

                byte[] pdfBytes = reportViewer.LocalReport.Render("PDF");

                // Return the PDF file for download
                return File(pdfBytes, "application/pdf", "NUML-" +currentSession + "-FeeStructure.pdf");
            }
            catch (Exception ex)
            {
                ViewBag.AlertMessage = "An error occurred while generating the report: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //Fetch Main Fee Data
        private async Task<List<DownloadFee>> FetchChallanData(int campusId)
        {
            List<DownloadFee> listFee = null;

            HttpResponseMessage responseMessage = await apiServices.GetAsync($"FeeStructure/DownloadFee/{campusId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listFee = JsonConvert.DeserializeObject<List<DownloadFee>>(responseData);
            }

            return listFee;
        }
    }
}