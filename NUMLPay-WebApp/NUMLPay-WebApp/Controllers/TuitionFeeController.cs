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
using System.Web.UI.WebControls;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 2, 3 })]
    public class TuitionFeeController : SessionController
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
        degreeService degreeServices;
        apiService apiServices;

        public TuitionFeeController()
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
            feeService = new feePlanService(baseAddress);
            degreeServices = new degreeService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        //add Fee
        public async Task<ActionResult> addTuitionFee()
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            if(admin.role == 3)
            {
                await getAccessAccordingtoRole(admin);
            }
            else
            {
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }


            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            ViewBag.feeFor = new SelectList(StatusService.getFeeForTuition(null), "value", "text");

            ViewBag.academicLevels = levelService.getLevel(null);

            return View();
        }

        //add Fee Post
        [HttpPost]
        public async Task<ActionResult> addTuitionFee(FeeStructure feeStructure)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            if (admin.role == 3)
            {
                await getAccessAccordingtoRole(admin);
            }
            else
            {
                ViewBag.campusList = await campusService.addCampustoListAsync();
            }

            SubFeeStructure subFeeStructure = new SubFeeStructure();

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            ViewBag.academicLevels = levelService.getLevel(null);

            ViewBag.feeFor = new SelectList(StatusService.getFeeForTuition(null), "value", "text");

            feeStructure.added_by = admin.email_id;

            //getting subFeeStructure
            int examFee, maintainceFee, magazineFee, computerLabFee, admissionFee, medicalFee, sportsFee,
                audioFee, librarySecFee, tuitionFee, registrationFee, libraryFee;

                if (!int.TryParse(Request.Form["examFee"], out examFee) ||
                     !int.TryParse(Request.Form["maintainceFee"], out maintainceFee) ||
                     !int.TryParse(Request.Form["magazineFee"], out magazineFee) ||
                     !int.TryParse(Request.Form["computerLabFee"], out computerLabFee) ||
                     !int.TryParse(Request.Form["admissionFee"], out admissionFee) ||
                     !int.TryParse(Request.Form["medicalFee"], out medicalFee) ||
                     !int.TryParse(Request.Form["sportsFee"], out sportsFee) ||
                     !int.TryParse(Request.Form["audioFee"], out audioFee) ||
                     !int.TryParse(Request.Form["librarySecFee"], out librarySecFee) ||
                     !int.TryParse(Request.Form["tuitionFee"], out tuitionFee) ||
                     !int.TryParse(Request.Form["registrationFee"], out registrationFee) ||
                     !int.TryParse(Request.Form["libraryFee"], out libraryFee))
                {
                    ViewBag.AlertMessage = "Please Enter Complete Fee Data";
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                    return View(feeStructure);
                }
                else
                {
                    subFeeStructure.exam_fee = examFee;
                    subFeeStructure.audio_it = audioFee;
                    subFeeStructure.maintainence = maintainceFee;
                    subFeeStructure.computer_lab = computerLabFee;
                    subFeeStructure.tutition_fee = tuitionFee;
                    subFeeStructure.library = libraryFee;
                    subFeeStructure.sports = sportsFee;
                    subFeeStructure.medical = medicalFee;
                    subFeeStructure.magazine = magazineFee;
                    subFeeStructure.admission_fee = admissionFee;
                    subFeeStructure.library_security = librarySecFee;
                    subFeeStructure.registration_fee = registrationFee;

                HttpResponseMessage responseMessageFees = await apiServices.PostAsync("FeeStructure", feeStructure);

                if (responseMessageFees.IsSuccessStatusCode)
                {
                    string data = await responseMessageFees.Content.ReadAsStringAsync();
                    
                        subFeeStructure.fee_structure_id = Convert.ToInt32(data);
                        await addSubFeeStructure(subFeeStructure);
                }
                else
                {
                    string responseData = responseMessageFees.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }

            }
            
            return View();
        }

        // View all Fees
        public async Task<ActionResult> viewTuitionFees()
        {
            Admin admin = userAccessAdmin();
            List<FeeView> listFee = null;

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            if(admin.role == 2)
            {
                listFee = await giveRealtiveData(null);
            }
            else
            {
                listFee = await giveRealtiveData(admin.dept_id);
            }

            return View(listFee);
        }

        // View Fee
        public async Task<ActionResult> viewTuitionFee(int Id)
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            FeeStructure fee = await getViewBags(Id);

            ViewBag.feeFor = new SelectList(StatusService.getFeeForTuition(fee.fee_for), "value", "text");

            return View(fee);
        }

        // Update Fee
        public async Task<ActionResult> updateTuitionFee(int Id)
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

           FeeStructure fee = await getViewBags(Id);

            ViewBag.feeFor = new SelectList(StatusService.getFeeForTuition(fee.fee_for), "value", "text");

            return View(fee);
        }

        // Update Fee Post
        [HttpPost]
        public async Task<ActionResult> updateTuitionFee(FeeStructure feeStructure)
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            ViewBag.feeFor = new SelectList(StatusService.getFeeForTuition(feeStructure.fee_for), "value", "text");

            await getViewBags(feeStructure.Id);

            feeStructure.added_by = admin.email_id;

            //getting subFeeStructure
            int examFee, maintainceFee, magazineFee, computerLabFee, admissionFee, medicalFee, sportsFee,
                audioFee, librarySecFee, tuitionFee, registrationFee, libraryFee;


                if (!int.TryParse(Request.Form["examFee"], out examFee) ||
                 !int.TryParse(Request.Form["maintainceFee"], out maintainceFee) ||
                 !int.TryParse(Request.Form["magazineFee"], out magazineFee) ||
                 !int.TryParse(Request.Form["computerLabFee"], out computerLabFee) ||
                 !int.TryParse(Request.Form["admissionFee"], out admissionFee) ||
                 !int.TryParse(Request.Form["medicalFee"], out medicalFee) ||
                 !int.TryParse(Request.Form["sportsFee"], out sportsFee) ||
                 !int.TryParse(Request.Form["audioFee"], out audioFee) ||
                 !int.TryParse(Request.Form["librarySecFee"], out librarySecFee) ||
                 !int.TryParse(Request.Form["tuitionFee"], out tuitionFee) ||
                 !int.TryParse(Request.Form["registrationFee"], out registrationFee) ||
                 !int.TryParse(Request.Form["libraryFee"], out libraryFee))
            {
                ViewBag.AlertMessage = "Please Enter valid numeric Data";
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
                return View(feeStructure);
            }
            else
            {
                SubFeeStructure subFeeStructure = new SubFeeStructure();
                subFeeStructure.exam_fee = examFee;
                subFeeStructure.audio_it = audioFee;
                subFeeStructure.maintainence = maintainceFee;
                subFeeStructure.computer_lab = computerLabFee;
                subFeeStructure.tutition_fee = tuitionFee;
                subFeeStructure.library = libraryFee;
                subFeeStructure.sports = sportsFee;
                subFeeStructure.medical = medicalFee;
                subFeeStructure.magazine = magazineFee;
                subFeeStructure.admission_fee = admissionFee;
                subFeeStructure.library_security = librarySecFee;
                subFeeStructure.registration_fee = registrationFee;

                HttpResponseMessage responseMessageFees = await apiServices.PutAsync("FeeStructure", feeStructure);

                if (responseMessageFees.IsSuccessStatusCode)
                {

                        subFeeStructure.fee_structure_id = feeStructure.Id;
                        HttpResponseMessage responseMessage = await apiServices.PutAsync("SubFeeStructure", subFeeStructure);

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
                }
                else
                {
                    string responseData = responseMessageFees.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }

            }

            List<FeeView> listFee = await giveRealtiveData(null);

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewTuitionFees", listFee);
        }

        public async Task<FeeStructure> getViewBags(int Id)
        {
            ViewBag.Display = "none;";

            FeeStructure fee = new FeeStructure();
            SubFeeStructure sub = new SubFeeStructure();

            using (var responses = await apiServices.GetAsync($"FeeStructure/GetbyId/{Id}"))
            {
                if (responses.IsSuccessStatusCode)
                {
                    string data = await responses.Content.ReadAsStringAsync();
                    fee = JsonConvert.DeserializeObject<FeeStructure>(data);
                }
            }

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(Convert.ToInt32(fee.session));

                using (var responses = await apiServices.GetAsync($"SubFeeStructure/GetByFeeId/{Id}"))
                {
                    if (responses.IsSuccessStatusCode)
                    {
                        string data = await responses.Content.ReadAsStringAsync();
                        sub = JsonConvert.DeserializeObject<SubFeeStructure>(data);
                    }
                }

                ViewBag.audioFee = sub.audio_it;
                ViewBag.maintainceFee = sub.maintainence;
                ViewBag.computerLabFee = sub.computer_lab;
                ViewBag.tuitionFee = sub.tutition_fee;
                ViewBag.libraryFee = sub.library;
                ViewBag.examFee = sub.exam_fee;
                ViewBag.sportsFee = sub.sports;
                ViewBag.medicalFee = sub.medical;
                ViewBag.magazineFee = sub.magazine;
                ViewBag.admissionFee = sub.admission_fee;
                ViewBag.librarySecFee = sub.library_security;
                ViewBag.registrationFee = sub.registration_fee;


                Shift shift = new Shift();

                using (var responseShift = await apiServices.GetAsync($"Shift/GetbyId/{fee.shift_id}"))
                {
                    if (responseShift.IsSuccessStatusCode)
                    {
                        string data = await responseShift.Content.ReadAsStringAsync();
                        shift = JsonConvert.DeserializeObject<Shift>(data);
                    }
                }

                ViewBag.academicLevels = levelService.getLevel(shift.academic_id);

                HttpResponseMessage response = await apiServices.GetAsync($"Degree/GetbyId/{shift.degree_id}");
                Degree degree = new Degree();
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    degree = JsonConvert.DeserializeObject<Degree>(data);
                }

                int? selectedValueDept = degree.dept_id;

                Tuple<int?, List<SelectListItem>> selectedDept = await departmentServices.getSelectedDepartments(selectedValueDept);
                ViewBag.deptList = selectedDept.Item2;

                Tuple<int?, List<SelectListItem>> selectedFaculty = await facultyServices.getSelectedFaculties(selectedDept.Item1);
                ViewBag.facultyList = selectedFaculty.Item2;

                ViewBag.campusList = await campusService.getSelectedCampus(selectedFaculty.Item1);

                int selectedDeptID = (int)degree.dept_id;

                List<SelectListItem> listDegree = await degreeServices.getDegrees(selectedDeptID, shift.academic_id, shift.Id);
                ViewBag.degreeList = listDegree;

            

            return fee;
        }       

        public async Task<List<FeeView>> giveRealtiveData(int? deptId)
        {
            List<FeeView> feeList = null;

            if (deptId.HasValue)
            {
                var response = await apiServices.GetAsync($"FeeStructure/GetbyDeptId/{deptId}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    feeList = JsonConvert.DeserializeObject<List<FeeView>>(data);
                }
            }
            else
            {
                var response = await apiServices.GetAsync($"FeeStructure/Get");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    feeList = JsonConvert.DeserializeObject<List<FeeView>>(data);
                }
            }

            return feeList;
        }

        public async Task<bool> getAccessAccordingtoRole(Admin admin)
        {
            SelectList allCampusesSelectList = await campusService.addCampustoListAsync();

            if (admin.campus_id != null)
            {
                int campusId = (int)admin.campus_id;
                var selectedCampus = allCampusesSelectList
                .FirstOrDefault(item => int.Parse(item.Value) == campusId);
                var filteredCampuses = new List<SelectListItem> { selectedCampus };
                ViewBag.campusList = new SelectList(filteredCampuses, "Value", "Text");
            }
            else
            {
                ViewBag.campusList = allCampusesSelectList;
            }

            Tuple<int?, List<SelectListItem>> listFaculties = await facultyServices.getSelectedFaculties(null);
            List<SelectListItem> allFacultiesSelectListItem = listFaculties.Item2;
            SelectList allFacultiesSelectList = new SelectList(allFacultiesSelectListItem, "Value", "Text");

            if (admin.faculty_id != null)
            {
                int facultyId = (int)admin.faculty_id;
                var selectedFaculty = allFacultiesSelectList
                    .FirstOrDefault(item => int.Parse(item.Value) == facultyId);
                var filteredFaculties = new List<SelectListItem> { selectedFaculty };
                ViewBag.facultyList = new SelectList(filteredFaculties, "Value", "Text");
            }


            List<SelectListItem> allDeptsListItem = await departmentServices.getSelectedDept(null);
            SelectList allDepartmentsSelectList = new SelectList(allDeptsListItem, "Value", "Text");
            if (admin.dept_id != null)
            {
                int deptId = (int)admin.dept_id;
                var selectedDept = allDepartmentsSelectList
                    .FirstOrDefault(item => int.Parse(item.Value) == deptId);
                var filteredDepartments = new List<SelectListItem> { selectedDept };
                ViewBag.deptList = new SelectList(filteredDepartments, "Value", "Text");
            }

            return true;
        }

        //adding SUBFeeStructure
        private async Task<bool> addSubFeeStructure(SubFeeStructure subFeeStructure)
        {
            HttpResponseMessage responseMessage = await apiServices.PostAsync("SubFeeStructure", subFeeStructure);
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

            return true;
        }

       
    }
}