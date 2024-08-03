using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NUMLPay_Api.Controllers
{
    public class SubjectController : ApiController
    {
        private readonly context db = new context();

        [Route("api/Subject/GetbyDeptId/{id}")]
        public async Task<HttpResponseMessage> GetbyDeptId(int id)
        {
            try
            {
                var subjects = await db.subjects
                    .Where(s => s.dept_id == id)
                    .ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, subjects);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/Subject/{id}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            try
            {
                var subjectObj = await db.subjects.FirstOrDefaultAsync(e => e.id == id);

                if (subjectObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, subjectObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Subject Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        public async Task<HttpResponseMessage> Post(Subjects newSubject)
        {
            try
            {
                bool subjectExists = await db.subjects.AnyAsync(e => e.name.Trim().ToLower() == newSubject.name.Trim().ToLower() & e.dept_id == newSubject.dept_id);

                if (subjectExists)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Subject Already Exists with the same Name.");
                }
                else
                {
                    db.subjects.Add(newSubject);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Subject Added Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Subject.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [Route("api/Subject/updateSubject/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> updateSubject(int id, Subjects subject)
        {
            try
            {
                var subjectObj = await db.subjects.FirstOrDefaultAsync(e => e.id == id);

                if (subjectObj != null)
                {
                    subjectObj.name = subject.name;
                    subjectObj.dept_id = subject.dept_id;
                    subjectObj.is_active = subject.is_active;
                    subjectObj.added_by = subject.added_by;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Subject Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Subject.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [Route("api/Subject/inActive/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> inActiveSubject(int id, Subjects subject)
        {
            try
            {
                var subjectObj = await db.subjects.FirstOrDefaultAsync(e => e.id == id);

                if (subjectObj != null)
                {
                    subjectObj.is_active = subject.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Subject Deleted Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to delete Subject.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
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

