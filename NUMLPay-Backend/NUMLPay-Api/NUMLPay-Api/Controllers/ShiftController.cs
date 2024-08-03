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
    public class ShiftController : ApiController
    {
        [Route("api/Shift")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(Shift newShift)
        {
            try
            {
                using (var db = new context())
                {
                    bool shiftExistsInSameDepartment = await db.shift.AnyAsync(e => e.academic_id == newShift.academic_id && e.degree_id == newShift.degree_id && e.shift == newShift.shift);

                    if (shiftExistsInSameDepartment)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Degree Already Exists with a similar Shift and Acacdemic Level.");
                    }
                    else
                    {
                        db.shift.Add(newShift);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Shift Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Shift.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/Shift/GetbyId/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetShiftById(int id)
        {
            try
            {
                using (context db = new context())
                {
                    var shift = await db.shift.FirstOrDefaultAsync(e => e.Id == id);

                    if (shift != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, shift);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Shift Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }


        [Route("api/Shift/updateShift/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> updateShift(int id, Shift shift)
        {
            try
            {
                using (context db = new context())
                {
                    var shiftObj = await db.shift.FirstOrDefaultAsync(e => e.Id == id);

                    if (shiftObj != null)
                    {
                        shiftObj.is_active = shift.is_active;

                        await db.SaveChangesAsync();

                        return Request.CreateResponse(HttpStatusCode.OK, "Degree Updated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Shift.");
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
