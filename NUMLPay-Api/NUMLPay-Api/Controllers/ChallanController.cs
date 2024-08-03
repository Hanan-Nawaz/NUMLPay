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
    public class ChallanController : ApiController
    {
        private readonly context db = new context();

        [Route("api/Challan")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateChallan(Challans newChallan)
        {
            try
            {
                var challans = await db.challans.FirstOrDefaultAsync(e => e.session == newChallan.session && e.fee_for == newChallan.fee_for && e.admissison_session == newChallan.admissison_session); ;

                if (challans != null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Challan Already Exists for this Session.");
                }
                else
                {
                    db.challans.Add(newChallan);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Challan Created Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Create Challan.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/Challan/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateChallan(int id, Challans challan)
        {
            try
            {
                var challanObj = await db.challans.FirstOrDefaultAsync(e => e.challan_id == id);

                if (challanObj != null)
                {
                    challanObj.session = challan.session;
                    challanObj.admissison_session = challan.admissison_session;
                    challanObj.due_date = challan.due_date;
                    challanObj.fee_for = challan.fee_for;
                    challanObj.valid_date = challan.valid_date;
                    challanObj.added_by = challan.added_by;
                    challanObj.is_active = challan.is_active;

                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Challan Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Challan.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/Challan")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetChallans()
        {
            try
            {
                var challans = await (from c in db.challans
                                join s1 in db.sessions on c.session equals s1.id
                                select new
                                {
                                    SessionName = s1.name,
                                    SessionYear = s1.year,
                                    c.valid_date,
                                    c.due_date,
                                    c.fee_for,
                                    c.added_by,
                                    c.is_active,
                                    c.challan_id
                                }).ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, challans);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

            
        }

        [Route("api/Challan/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetChallanById(int id)
        {
            try
            {
                var challanObj = await db.challans.FirstOrDefaultAsync(e => e.challan_id == id);

                if (challanObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, challanObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Challan Not Found.");
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }
            

        [Route("api/Challan/getActive")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetActiveChallans()
        {
            try
            {
                var activeChallans = await db.challans.Where(challan => challan.is_active == 1).ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, activeChallans);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Challan/inActive/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> InactivateChallan(int id, Challans challan)
        {
            try
            {
                var challanObj = await db.challans.FirstOrDefaultAsync(e => e.challan_id == id);

                if (challanObj != null)
                {
                    challanObj.is_active = challan.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Challan Deactivated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Deactivate Challan.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

            
        }

    }
}
