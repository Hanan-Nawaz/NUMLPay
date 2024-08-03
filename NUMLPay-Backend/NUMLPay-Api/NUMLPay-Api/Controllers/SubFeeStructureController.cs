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
    public class SubFeeStructureController : ApiController
    {
        [System.Web.Http.Route("api/SubFeeStructure")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> PostSubFeeStructure(SubFeeStructure subFeeStructure)
        {
            try
            {
                using (context db = new context())
                {
                    var subFeeObj = await db.subFeeStructures.FirstOrDefaultAsync(e => e.fee_structure_id == subFeeStructure.fee_structure_id);

                    if (subFeeObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee Structure Already Exists for this Degree.");
                    }
                    else
                    {
                        db.subFeeStructures.Add(subFeeStructure);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Fee Structure Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create SubFeeStructure.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/SubFeeStructure")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> PutSubFeeStructure(SubFeeStructure subFeeStructure)
        {
            try
            {
                using (context db = new context())
                {
                    var subFeeStructureObj = await db.subFeeStructures.FirstOrDefaultAsync(e => e.fee_structure_id == subFeeStructure.fee_structure_id);

                    if (subFeeStructureObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "SubFeeStructure Doesn't Exists for this Degree.");
                    }
                    else
                    {
                        subFeeStructureObj.exam_fee = subFeeStructure.exam_fee;
                        subFeeStructureObj.maintainence = subFeeStructure.maintainence;
                        subFeeStructureObj.computer_lab = subFeeStructure.computer_lab;
                        subFeeStructureObj.tutition_fee = subFeeStructure.tutition_fee;
                        subFeeStructureObj.library = subFeeStructure.library;
                        subFeeStructureObj.audio_it = subFeeStructure.audio_it;
                        subFeeStructureObj.sports = subFeeStructure.sports;
                        subFeeStructureObj.medical = subFeeStructure.medical;
                        subFeeStructureObj.magazine = subFeeStructure.magazine;
                        subFeeStructureObj.admission_fee = subFeeStructure.admission_fee;
                        subFeeStructureObj.library_security = subFeeStructure.library_security;
                        subFeeStructureObj.registration_fee = subFeeStructure.registration_fee;

                        await db.SaveChangesAsync();

                        return Request.CreateResponse(HttpStatusCode.Created, "FeeStructure Updated Successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }


        [System.Web.Http.Route("api/SubFeeStructure/GetByFeeId/{Id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetByFeeId(int Id)
        {
            try
            {
                using (context db = new context())
                {
                    var feeStructure = await db.subFeeStructures.FirstOrDefaultAsync(e => e.fee_structure_id == Id);

                    if (feeStructure != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, feeStructure);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No FeeStructure found for the given Shift ID.");
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
