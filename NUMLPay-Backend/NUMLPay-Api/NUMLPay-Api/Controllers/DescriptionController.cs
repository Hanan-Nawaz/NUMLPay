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
    public class DescriptionController : ApiController
    {
        private readonly context db = new context();

        [Route("api/Description")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateDescription(Description newDescription)
        {
            try
            {
                bool descExists = await db.descriptions.AnyAsync(e => e.name.Equals(newDescription.name, StringComparison.OrdinalIgnoreCase));
                if (descExists)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Description Already Exists with a similar Name.");
                }
                else
                {
                    db.descriptions.Add(newDescription);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Description Created Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Create Description.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [Route("api/Description/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateDescription(int id, Description description)
        {
            try
            {
                var descriptionObj = await db.descriptions.FirstOrDefaultAsync(e => e.id == id);

                if (descriptionObj != null)
                {
                    descriptionObj.name = description.name;
                    descriptionObj.is_active = description.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Description Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Description.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Description")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                var descs = await db.descriptions.ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, descs);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/Description/getActive")]
        [HttpGet]
        public async Task<HttpResponseMessage> getActive()
        {
            try
            {
                var descs = await db.descriptions.Where(desc => desc.is_active == 1).ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, descs);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Description/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetDescriptionById(int id)
        {
            try
            {
                var descriptionObj = await db.descriptions.FirstOrDefaultAsync(e => e.id == id);

                if (descriptionObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, descriptionObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Description Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/Description/inActive/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> inActive(int id, Description desc)
        {
            try
            {
                var descObj = await db.descriptions.FirstOrDefaultAsync(e => e.id == id);

                if (descObj != null)
                {
                    descObj.is_active = desc.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Description Deleted Successfully!");
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
    }
}
