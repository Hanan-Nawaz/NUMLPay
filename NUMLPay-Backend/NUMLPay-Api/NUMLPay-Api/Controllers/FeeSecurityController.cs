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
    public class FeeSecurityController : ApiController
    {
        [System.Web.Http.Route("api/FeeSecurity")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> PostFeeSecurity(FeeSecurity feeSecurity)
        {
            try
            {
                using (context db = new context())
                {
                    var feeSecurityObj = await db.feeSecurity.FirstOrDefaultAsync(e => e.fee_structure_id == feeSecurity.fee_structure_id);

                    if (feeSecurityObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee Security Already Exists for this Fee Structure.");
                    }
                    else
                    {
                        db.feeSecurity.Add(feeSecurity);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Fee Security Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Fee Security.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/FeeSecurity")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> PutFeeSecurity(FeeSecurity feeSecurity)
        {
            try
            {
                using (context db = new context())
                {
                    var feeSecurityObj = await db.feeSecurity.FirstOrDefaultAsync(e => e.fee_structure_id == feeSecurity.fee_structure_id);

                    if (feeSecurityObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee Security Doesn't Exists for this Fee Structure.");
                    }
                    else
                    {
                        feeSecurityObj.security = feeSecurity.security;
                        feeSecurityObj.fee_structure_id = feeSecurity.fee_structure_id;

                        await db.SaveChangesAsync();

                        return Request.CreateResponse(HttpStatusCode.Created, "Fee Security Updated Successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/FeeSecurity/GetByFeeId/{Id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetByFeeId(int Id)
        {
            try
            {
                using (context db = new context())
                {
                    var feeSecurity = await db.feeSecurity.FirstOrDefaultAsync(e => e.fee_structure_id == Id);

                    if (feeSecurity != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, feeSecurity);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Fee Security found for the given Fee Structure ID.");
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
