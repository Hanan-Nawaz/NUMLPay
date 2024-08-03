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
    public class MiscellaneousFeeController : ApiController
    {
        public class MiscellaneousFeesController : ApiController
        {
            private readonly context db = new context();

            [Route("api/MiscellaneousFees")]
            [HttpGet]
            public async Task<HttpResponseMessage> Get()
            {
                try
                {
                    var miscellaneousFees = await db.miscellaneousFees.ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, miscellaneousFees);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                }
              
            }

            [Route("api/MiscellaneousFees/Active")]
            [HttpGet]
            public async Task<HttpResponseMessage> GetActive()
            {
                try
                {
                    var query = await (from miscellaneousFee in db.miscellaneousFees
                                join description in db.descriptions on miscellaneousFee.desc_id equals description.id
                                where miscellaneousFee.is_active == 1
                                select new
                                {
                                    name = description.name,
                                    Id = miscellaneousFee.Id
                                }).ToListAsync();

                    return Request.CreateResponse(HttpStatusCode.OK, query);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                }
               
            }

            [Route("api/MiscellaneousFees/{id}")]
            [HttpGet]
            public async Task<HttpResponseMessage> Get(int id)
            {
                try
                {
                    var feeObj = await db.miscellaneousFees.FirstOrDefaultAsync(e => e.Id == id);

                    if (feeObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, feeObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Fee Not Found.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                }
              
            }

            [Route("api/MiscellaneousFees")]
            [HttpPost]
            public async Task<HttpResponseMessage> Post(MiscellaneousFees newFee)
            {
                try
                {
                    bool feeExists = await db.miscellaneousFees
                   .AnyAsync(e => e.desc_id == newFee.desc_id);

                    if (feeExists)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee Already Exists with a similar Description.");
                    }
                    else
                    {
                        db.miscellaneousFees.Add(newFee);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Fee Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Fee.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                }
               
            }

            [Route("api/MiscellaneousFees/updateMiscellaneousFee/{id}")]
            [HttpPut]
            public async Task<HttpResponseMessage> UpdateFee(int id, MiscellaneousFees fee)
            {
                try
                {
                    var feeObj = await db.miscellaneousFees.FirstOrDefaultAsync(e => e.Id == id);

                    if (feeObj != null)
                    {
                        feeObj.desc_id = fee.desc_id;
                        feeObj.amount = fee.amount;
                        feeObj.added_by = fee.added_by;
                        feeObj.is_active = fee.is_active;

                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Fee Updated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Fee.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                }
               
            }

           [Route("api/MiscellaneousFees/inActive/{id}")]
            [HttpPut]
            public async Task<HttpResponseMessage> Inactive(int id, MiscellaneousFees fee)
            {
                try
                {
                    var feeObj = await db.miscellaneousFees.FirstOrDefaultAsync(e => e.Id == id);

                    if (feeObj != null)
                    {
                        feeObj.is_active = fee.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Fee Marked as Inactive Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to mark Fee as Inactive.");
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
}
