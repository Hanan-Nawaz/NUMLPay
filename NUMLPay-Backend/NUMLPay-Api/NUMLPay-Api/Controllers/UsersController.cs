using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
using NUMLPay_Api.CreateModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace NUMLPay_Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly context db = new context();

        [Route("api/Users/ForgotPassword")]
        [HttpPut]
        public async Task<HttpResponseMessage> ForgotPassword(Users user)
        {
            try
            {
                using (context db = new context())
                {
                    var userObj = await db.users.FirstOrDefaultAsync(e => e.numl_id == user.numl_id);

                    if (userObj != null)
                    {
                        userObj.fp_token = user.fp_token;
                        userObj.fp_token_expiry = user.fp_token_expiry;

                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, userObj.email);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to update User.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Admin with this Email doesn't found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [Route("api/Users/ResetPassword")]
        [HttpPut]
        public async Task<HttpResponseMessage> ResetPassword(Users user)
        {
            try
            {
                using (context db = new context())
                {
                    var userObj = await db.users.FirstOrDefaultAsync(e => e.numl_id == user.numl_id);

                    if (userObj != null && userObj.fp_token != null)
                    {
                        if (userObj.fp_token == user.fp_token && Convert.ToDateTime(userObj.fp_token_expiry) >= DateTime.Now)
                        {
                            userObj.fp_token = "";
                            userObj.password = user.password;

                            int rowsAffected = await db.SaveChangesAsync();

                            if (rowsAffected > 0)
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, "Password Reset Successfully!");
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Reset Password.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Token Expired! Kindly Try Again and Remember you get only 30 Minutes window.");
                        }

                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Admin with this Email doesn't found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [Route("api/Users/Get")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                var joinedData = await db.Database.SqlQuery<UsersView>("spGetUsers")
            .ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, joinedData);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }

        [Route("api/Users/Login")]
        [HttpPost]
        public async Task<HttpResponseMessage> Login(Users user)
        {
            try
            {
                using (context db = new context())
                {
                    var userObj = await db.Database.SqlQuery<Users>("spUpdateSemester @numl_id", new SqlParameter("@numl_id", user.numl_id))
                        .FirstOrDefaultAsync();

                    if (userObj != null && BCrypt.Net.BCrypt.Verify(user.password, userObj.password))
                    {
                        if (userObj.is_active == 1)
                        {
                            if (userObj.status_of_degree != 3)
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, userObj);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound, "Status of Degree is " + userObj.status_of_degree + ". Please Contact Admin.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Your Status is not Active. Please Contact Admin.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Wrong Credentails!!");
                    }
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [System.Web.Http.Route("api/Users/PutUsers")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> PutUsers(Users updatedUser)
        {
            using (context db = new context())
            {
                try
                {
                    var userObj = await db.users.FirstOrDefaultAsync(u => u.numl_id == updatedUser.numl_id);

                    if (userObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "User with this NUML Id does not exist.");
                    }
                    else
                    {
                        userObj.name = updatedUser.name;
                        userObj.father_name = updatedUser.father_name;
                        userObj.date_of_birth = updatedUser.date_of_birth;
                        userObj.email = updatedUser.email;
                        userObj.contact = updatedUser.contact;
                        userObj.nic = updatedUser.nic;
                        userObj.degree_id = updatedUser.degree_id;
                        userObj.semester = updatedUser.semester;
                        userObj.admission_session = updatedUser.admission_session;
                        userObj.dept_id = updatedUser.dept_id;
                        userObj.fee_plan = updatedUser.fee_plan;
                        userObj.image = updatedUser.image;
                        userObj.verified_by = updatedUser.verified_by;
                        userObj.status_of_degree = updatedUser.status_of_degree;
                        userObj.is_relegated = updatedUser.is_relegated;
                        userObj.passed_ceased_sems = updatedUser.passed_ceased_sems;
                        userObj.fp_token = updatedUser.fp_token;
                        userObj.fp_token_expiry = updatedUser.fp_token_expiry;
                        userObj.is_active = updatedUser.is_active;

                        int rowsAffected = await db.SaveChangesAsync().ConfigureAwait(false);

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "User Updated Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to update User.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request. " + ex.Message);
                }
            }
        }


        [System.Web.Http.Route("api/Users")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(Users users)
        {
            using (context db = new context())
            {
                try
                {
                    var userObj = await db.users.FirstOrDefaultAsync(u => u.numl_id == users.numl_id);

                    if (userObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "User Already Exists with this NUML ID.");
                    }
                    else
                    {
                        db.users.Add(users);
                        int rowsAffected = await db.SaveChangesAsync().ConfigureAwait(false);

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "User Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create User.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request." + ex.Message);
                }
            }
        }

        [System.Web.Http.Route("api/Users/GetUsers")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetUsers(Users user)
        {
            try
            {
                using (context db = new context())
                {
                    var adminObj = await db.users.FirstOrDefaultAsync(e => e.numl_id == user.numl_id);

                    if (adminObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, adminObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Users Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
          
        }

        [System.Web.Http.Route("api/Users/EligibleFees")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> PostEligibleFees(EligibleFees eligibleFees)
        {
            using (context db = new context())
            {
                try
                {
                    var userObj = await db.eligibleFees.FirstOrDefaultAsync(u => u.std_numl_id == eligibleFees.std_numl_id);

                    if (userObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "User Already Exists with this NUML Id.");
                    }
                    else
                    {
                        db.eligibleFees.Add(eligibleFees);
                        int rowsAffected = await db.SaveChangesAsync().ConfigureAwait(false);

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Eligible Fees Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create User.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request." + ex.Message);
                }
            }
        }

        [System.Web.Http.Route("api/Users/PutEligibleFees")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutEligibleFees(EligibleFees eligibleFees)
        {
            using (context db = new context())
            {
                try
                {
                    var userObj = await db.eligibleFees.FirstOrDefaultAsync(u => u.id == eligibleFees.id);

                    if (userObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "User with this NUML Id does not exist.");
                    }
                    else
                    {
                        userObj.bus_fee = eligibleFees.bus_fee;
                        userObj.hostel_fee = eligibleFees.hostel_fee;
                        userObj.semester_fee = eligibleFees.semester_fee;
                        userObj.std_numl_id = eligibleFees.std_numl_id;

                        int rowsAffected = await db.SaveChangesAsync().ConfigureAwait(false);


                            return Request.CreateResponse(HttpStatusCode.OK, "Eligible Fees Updated Successfully!");
                     
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request. " + ex.Message);
                }
            }
        }


        [System.Web.Http.Route("api/Users/GetByCampus/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetByCampus(int id)
        {
            using (context db = new context())
            {
                try
                {
                    var joinedData = await db.Database.SqlQuery<UsersView>("spGetUsersbyCampus @cId", new SqlParameter("cId", id))
                .ToListAsync();

                    if (joinedData != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, joinedData);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "User Not Found.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }
            }
        }

        [System.Web.Http.Route("api/Users/UpdateStatus/{statDegree}/{isActive}/{sessionId}/{shiftId}/{deptId}")]
        [HttpGet]
        public async Task<HttpResponseMessage> UpdateStatus(int statDegree, int isActive, int sessionId, int shiftId, int deptId)
        {
            using (context db = new context())
            {
                try
                {
                    var result = await db.Database.ExecuteSqlCommandAsync("spUpdateStatus @stat_degree, @is_active, @session, @shift_id, @dept_id",
                        new SqlParameter("@stat_degree", statDegree),
                        new SqlParameter("@is_active", isActive),
                        new SqlParameter("@session", sessionId),
                        new SqlParameter("@shift_id", shiftId),
                        new SqlParameter("@dept_id", deptId));

                    if (result > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Status updated successfully.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No users found matching the provided criteria.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }


        [System.Web.Http.Route("api/Users/GetByDept/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetByDept(int id)
        {
            using (context db = new context())
            {
                try
                {
                    var joinedData = await db.Database.SqlQuery<UsersView>("spGetUsersbyDept @dId", new SqlParameter("dId", id))
                .ToListAsync();

                    if (joinedData != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, joinedData);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "User Not Found.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }
            }
        }


        [Route("api/Users/GetEligibleFees")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetEligibleFees(EligibleFees user)
        {
            try
            {
                using (context db = new context())
                {
                    var userObj = await db.eligibleFees.FirstOrDefaultAsync(u => u.std_numl_id == user.std_numl_id);

                    if (userObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, userObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, " Don't find any Fee.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        private readonly emailService _emailService;

        public UsersController()
        {
            _emailService = new emailService();
        }

        [Route("api/Users/SendEamil")]
        [HttpPost]
        public async Task<HttpResponseMessage> SendEmail(EmailModel emailModel)
        {
            try
            {
                // Call your email service method to send email
                bool emailSent = _emailService.Send(emailModel.ReceiverEmail, emailModel.Subject, emailModel.Body, emailModel.AttachmentPath);

                if (emailSent)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Email sent successfully.");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to send email.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

    public class EmailModel
    {
        public string ReceiverEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentPath { get; set; }
    }

   
