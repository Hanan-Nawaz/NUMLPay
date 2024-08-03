using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
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
    public class SummerFeeController : ApiController
    {
        private readonly context db = new context();

        [Route("api/SummerFee/GetByDeptId/{id}")]
        public async Task<HttpResponseMessage> GetByDeptId(int id)
        {
            try
            {
                var summerFeesWithSubjects = await db.summerFees
                    .Include(sf => sf.subjects)
                    .Where(sf => sf.subjects.dept_id == id)
                    .Select(sf => new SummerFeeView
                    {
                        id = sf.id,
                        fee = sf.fee,
                        session_year = sf.session_year,
                        subject_name = sf.subjects.name,
                        added_by = sf.added_by,
                        is_active = sf.is_active
                    })
                    .ToListAsync();



                return Request.CreateResponse(HttpStatusCode.OK, summerFeesWithSubjects);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/SummerFee/GetById/{id}")]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            try
            {
                var summerFeeObj = await db.summerFees.FirstOrDefaultAsync(e => e.id == id);

                if (summerFeeObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, summerFeeObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "SummerFee Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        public async Task<HttpResponseMessage> Post(SummerFee newSummerFee)
        {
            try
            {
                var oldSummerFeeObj = await db.summerFees.FirstOrDefaultAsync(e => e.subject_id == newSummerFee.subject_id && e.session_year == newSummerFee.session_year);

                if (oldSummerFeeObj == null)
                {
                    db.summerFees.Add(newSummerFee);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "SummerFee Added Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create SummerFee.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee for that Subject and Session already exists.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/SummerFee/updateSummerFee/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> updateSummerFee(int id, SummerFee summerFee)
        {
            try
            {
                var summerFeeObj = await db.summerFees.FirstOrDefaultAsync(e => e.id == id);

                if (summerFeeObj != null)
                {
                    summerFeeObj.fee = summerFee.fee;
                    summerFeeObj.subject_id = summerFee.subject_id;
                    summerFeeObj.session_year = summerFee.session_year;
                    summerFeeObj.is_active = summerFee.is_active;

                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "SummerFee Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update SummerFee.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/SummerFee/inActive/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> inActiveSummerFee(int id, SummerFee summerFee)
        {
            try
            {
                var summerFeeObj = await db.summerFees.FirstOrDefaultAsync(e => e.id == id);

                if (summerFeeObj != null)
                {
                    summerFeeObj.is_active = summerFee.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "SummerFee Deleted Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to delete SummerFee.");
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

