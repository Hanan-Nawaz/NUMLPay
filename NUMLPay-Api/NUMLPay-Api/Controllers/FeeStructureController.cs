using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace NUMLPay_Api.Controllers
{
    public class FeeStructureController : ApiController
    {
        [System.Web.Http.Route("api/FeeStructure")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> PostFeeStructure(FeeStructure feeStructure)
        {
            try
            {
                using (context db = new context())
                {
                    FeeStructure feeStr = null;
                    if (feeStructure.fee_for == 1)
                    {
                        feeStr = await db.feeStructures.FirstOrDefaultAsync(e => (e.shift_id == feeStructure.shift_id) && e.session == feeStructure.session);
                    }
                    else
                    {
                        if(feeStructure.fee_for == 2)
                        {
                            feeStr = await db.feeStructures.FirstOrDefaultAsync(e => (e.fee_for == feeStructure.fee_for) && e.session == feeStructure.session && e.shift_id == feeStructure.shift_id);
                        }
                        else
                        {
                            feeStr = await db.feeStructures.FirstOrDefaultAsync(e => (e.fee_for == feeStructure.fee_for) && e.session == feeStructure.session);
                        }
                    }

                    if (feeStr != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee Already Exists.");
                    }
                    else
                    {
                        db.feeStructures.Add(feeStructure);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, feeStructure.Id);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create FeeStructure.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/FeeStructure")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> PuttFeeStructure(FeeStructure feeStructure)
        {
            try
            {
                using (context db = new context())
                {
                    var feeStructureObj = await db.feeStructures.FirstOrDefaultAsync(e => e.Id == feeStructure.Id);

                    if (feeStructureObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee Already Exists.");
                    }
                    else
                    {
                        feeStructureObj.session = feeStructure.session;
                        feeStructureObj.fee_for = feeStructure.fee_for;
                        feeStructureObj.total_fee = feeStructure.total_fee;
                        feeStructureObj.added_by = feeStructure.added_by;
                        feeStructureObj.shift_id = feeStructure.shift_id;

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

        [System.Web.Http.Route("api/FeeStructure/GetByShiftId/{shiftId}/{session}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetByShiftId(int shiftId, int session)
        {
            try
            {
                using (context db = new context())
                {
                    var feeStructure = await db.feeStructures.FirstOrDefaultAsync(e => e.shift_id == shiftId && e.session == session);

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


        [Route("api/FeeStructure/Get")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (context db = new context())
            {
                try
                {
                    var joinedData = await db.Database.SqlQuery<FeeView>("spViewFees").ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, joinedData);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }


            }
        }

        [Route("api/FeeStructure/DownloadFee/{cid}")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadFee(int cid)
        {
            using (context db = new context())
            {
                try
                {
                    var joinedData = await db.Database.SqlQuery<DownloadFee>("spGetAllFeeStructure @campus_id", new SqlParameter("@campus_id", cid)).ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, joinedData);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }


            }
        }


        [Route("api/FeeStructure/GetHostelBusMess")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetHostelBusMess()
        {
            using (context db = new context())
            {
                try
                {
                    var joinedData = db.Database.SqlQuery<FeeView>("spGetFeeDetailsHMB").ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, joinedData);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }


            }
        }

        [Route("api/FeeStructure/GetbyDeptId/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetbyDeptId(int id)
        {
            try
            {
                using (context db = new context())
                {
                    SqlParameter param = new SqlParameter("dId", id);
                    var joinedData = await db.Database.SqlQuery<FeeView>("spViewFeebyDeptId @dId", param).ToListAsync();

                    return Request.CreateResponse(HttpStatusCode.OK, joinedData);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }

        }

        [System.Web.Http.Route("api/FeeStructure/GetById/{Id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetById(int Id)
        {
            try
            {
                using (context db = new context())
                {
                    var feeStructure = await db.feeStructures.FirstOrDefaultAsync(e => e.Id == Id);

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
