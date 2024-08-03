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
    public class FineController : ApiController
    {
        private readonly context db = new context();

        [Route("api/Fine")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateFine(Fines newFine)
        {
            try
            {
                var fines = await db.fine.FirstOrDefaultAsync(e => e.session == newFine.session && e.fine_for == newFine.fine_for);

                if (fines != null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fine Already Exists for this Session.");
                }
                else
                {
                    db.fine.Add(newFine);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Fine Created Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Create Fine.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/Fine/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateFine(int id, Fines fine)
        {
            try
            {
                var fineObj = await db.fine.FirstOrDefaultAsync(e => e.id == id);

                if (fineObj != null)
                {
                    fineObj.fine_after_30_days = fine.fine_after_30_days;
                    fineObj.fine_after_60_days = fine.fine_after_60_days;
                    fineObj.fine_after_10_days = fine.fine_after_10_days;

                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Fine Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Fine.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Fine")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetFines()
        {
            try
            {
                var fines = await (from f in db.fine
                                   join s in db.sessions on f.session equals s.id
                                   select new
                                   {
                                       f.id,
                                       f.fine_after_10_days,
                                       f.fine_after_30_days,
                                       f.fine_after_60_days,
                                       s.name,
                                       s.year,
                                       f.fine_for
                                   }).ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, fines);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Fine/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetFineById(int id)
        {
            try
            {
                var fineObj = await db.fine.FirstOrDefaultAsync(e => e.id == id);

                if (fineObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, fineObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Fine Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }
    }
}
