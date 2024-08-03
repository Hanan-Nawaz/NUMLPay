using BCrypt.Net;
using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
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
    public class AdminController : ApiController
    {
        [Route("Admin/Login")]
        public async Task<HttpResponseMessage> Login(Admin admins)
        {
            try
            {
                using (context db = new context())
                {
                    var userObj = await db.admin.FirstOrDefaultAsync(e => e.email_id == admins.email_id);

                    if (userObj != null && BCrypt.Net.BCrypt.Verify(admins.password, userObj.password))
                    {
                        if (userObj.is_active == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, userObj);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Your Status is not Active. Please Contact Admin.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "User Not Found!!");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Admin")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(Admin admins)
        {
            try
            {
                using (context db = new context())
                {
                    var adminObj = await db.admin.FirstOrDefaultAsync(e => e.email_id == admins.email_id);

                    if (adminObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Admin Already Exists with this Email.");
                    }
                    else
                    {
                        if (admins.role == 1)
                        {
                            admins.faculty_id = null;
                            admins.dept_id = null;
                        }
                        else if (admins.role == 2)
                        {
                            admins.campus_id = null;
                            admins.faculty_id = null;
                            admins.dept_id = null;
                        }
                        else
                        {
                            admins.campus_id = admins.campus_id;
                            admins.faculty_id = admins.faculty_id;
                            admins.dept_id = admins.dept_id;
                        }

                        admins.fp_token_expiry = DateTime.UtcNow;

                        db.admin.Add(admins);
                        
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Admin Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Admin.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/Admin")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAdmin()
        {
            try
            {
                using (context db = new context())
                {
                    var admins = await db.Database.SqlQuery<AdminView>("spViewAdmins").ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, admins);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [System.Web.Http.Route("api/Admin/GetAdmin")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetAdmin(Admin admin)
        {
            try
            {
                using (context db = new context())
                {
                    var adminObj = await db.admin.FirstOrDefaultAsync(e => e.email_id == admin.email_id);

                    if (adminObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, adminObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Admin Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }


        [System.Web.Http.Route("api/Admin/GetByCampus/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetByCampus(int id)
        {
            try
            {
                using (context db = new context())
                {
                    SqlParameter param = new SqlParameter("cId", id);
                    var admins = await db.Database.SqlQuery<AdminView>("spViewAdmihnByCampus @cId", param).ToListAsync();

                    if (admins != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, admins);
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


        [Route("api/Admin/updateAdmin")]
        [HttpPut]
        public async Task<HttpResponseMessage> updateAdmin(Admin admin)
        {
            try
            {
                using (context db = new context())
                {
                    
                    var adminObj = await db.admin.FirstOrDefaultAsync(e => e.email_id == admin.email_id);

                    if (adminObj != null)
                    {
                        if (admin.role == 1)
                        {
                            adminObj.campus_id = admin.campus_id;
                            adminObj.faculty_id = null;
                            adminObj.dept_id = null;
                        }
                        else if (admin.role == 2)
                        {
                            adminObj.campus_id = null;
                            adminObj.faculty_id = null;
                            adminObj.dept_id = null;
                        }
                        else
                        {
                            adminObj.campus_id = admin.campus_id;
                            adminObj.faculty_id = admin.faculty_id;
                            adminObj.dept_id = admin.dept_id;
                        }

                        adminObj.name = admin.name;
                        adminObj.post = admin.post;
                        adminObj.added_by = admin.added_by;
                        adminObj.fp_token = admin.fp_token;
                        adminObj.fp_token_expiry = admin.fp_token_expiry;
                        adminObj.is_active = admin.is_active;

                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "Admin Updated Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "No Updation. Because nothing is Changed.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Admin.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/Admin/ForgotPassword")]
        [HttpPut]
        public async Task<HttpResponseMessage> ForgotPassword(Admin admin)
        {
            try
            {
                using (context db = new context())
                {
                    var adminObj = await db.admin.FirstOrDefaultAsync(e => e.email_id == admin.email_id);

                    if (adminObj != null)
                    {
                        adminObj.fp_token = admin.fp_token;
                        adminObj.fp_token_expiry = admin.fp_token_expiry;

                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "Admin Updated Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to update Admin due to FK.");
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

        [Route("api/Admin/ResetPassword")]
        [HttpPut]
        public async Task<HttpResponseMessage> ResetPassword(Admin admin)
        {
            try
            {
                using (context db = new context())
                {
                    var adminObj = await db.admin.FirstOrDefaultAsync(e => e.email_id == admin.email_id);

                    if (adminObj != null && adminObj.fp_token != null)
                    {
                        if(adminObj.fp_token == admin.fp_token && adminObj.fp_token_expiry >= DateTime.Now)
                        {
                            adminObj.fp_token = "";
                            adminObj.password = admin.password;

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

        [Route("api/Admin/inActive")]
        [HttpPost]
        public async Task<HttpResponseMessage> inActiveAdmin(Admin admin)
        {
            try
            {
                using (context db = new context())
                {
                    var adminObj = await db.admin.FirstOrDefaultAsync(e => e.email_id == admin.email_id);

                    if (adminObj != null)
                    {
                        adminObj.is_active = admin.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Admin Deleted Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to delete Admin.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }
    }
}
