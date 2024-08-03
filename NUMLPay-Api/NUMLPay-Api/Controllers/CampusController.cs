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
    public class CampusController : ApiController
    {
        private readonly context db = new context();

        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                var campuses = await db.campus.ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, campuses);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/Campus/GetbyId/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(int id)
        {
            try
            {
                var campusObj = await db.campus.FirstOrDefaultAsync(e => e.Id == id);

                if (campusObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, campusObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Campus Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        public async Task<HttpResponseMessage> Post(Campus newCampus)
        {
            try
            {
                string[] parts = newCampus.name.Split(' ');
                string firstPart = parts[0];

                bool campusExists = await db.campus.AnyAsync(e => e.name.Trim().ToLower().StartsWith(firstPart.Trim().ToLower()));

                if (campusExists)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Campus Already Exists with a similar Name.");
                }
                else
                {
                    db.campus.Add(newCampus);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Campus Added Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Campus.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Campus/updateCampus/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> updateCampus(int id, Campus campus)
        {
            try
            {
                var campusObj = await db.campus.FirstOrDefaultAsync(e => e.Id == id);

                if (campusObj != null)
                {
                    campusObj.name = campus.name;
                    campusObj.added_by = campus.added_by;
                    campusObj.date = campus.date;
                    campusObj.is_active = campus.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Campus Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Campus.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Campus/inActive/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> inActive(int id, Campus campus)
        {
            try
            {
                var campusObj = await db.campus.FirstOrDefaultAsync(e => e.Id == id);

                if (campusObj != null)
                {
                    campusObj.is_active = campus.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Campus Deleted Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to delete Campus.");
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
