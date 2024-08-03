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
    public class FeePlanController : ApiController
    {
        [System.Web.Http.Route("api/FeePlan/Get")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                using (context db = new context())
                {
                    var feePlan = await db.feePlans.ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, feePlan);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/FeePlan/GetById/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            try
            {
                using (context db = new context())
                {
                    var feePlanObj = await db.feePlans.FirstOrDefaultAsync(e => e.id == id);

                    if (feePlanObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, feePlanObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Fee Plan Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/FeePlan")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(FeePlan newFeePlan)
        {
            try
            {
                 using (context db = new context())
            {
                bool feePlanExists = await db.feePlans.AnyAsync(e =>
                    e.name == newFeePlan.name);

                if (feePlanExists)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee Plan Already Exists with a similar Name.");
                }
                else
                {
                    db.feePlans.Add(newFeePlan);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Fee Plan Added Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Fee Plan.");
                    }
                }
            }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/FeePlan/updateFeePlan/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> UpdateFeePlan(int id, FeePlan feePlan)
        {
            try
            {
                using (context db = new context())
                {
                    var feePlanObj = await db.feePlans.FirstOrDefaultAsync(e => e.id == id);

                    if (feePlanObj != null)
                    {
                        feePlanObj.name = feePlan.name;
                        feePlanObj.discount = feePlan.discount;
                        feePlanObj.added_by = feePlan.added_by;
                        feePlanObj.is_active = feePlan.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Fee Plan Updated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Fee Plan.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/FeePlan/inActive/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> InactivateFeePlan(int id, FeePlan feePlan)
        {
            try
            {
                using (context db = new context())
                {
                    var feePlanObj = await db.feePlans.FirstOrDefaultAsync(e => e.id == id);

                    if (feePlanObj != null)
                    {
                        feePlanObj.is_active = feePlan.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Fee Plan Inactivated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to inactivate Fee Plan.");
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
