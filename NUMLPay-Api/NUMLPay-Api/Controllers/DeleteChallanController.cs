using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NUMLPay_Api.Controllers
{
    public class DeleteChallanController : ApiController
    {
        private readonly context db = new context();

        [Route("api/DeleteChallan/Delete/{ChallanNumber}")]
        [HttpGet]
        public async Task<HttpResponseMessage> Delete(int ChallanNumber)
        {
            try
            {
                var unpaidFee = await db.unpaidFees.FirstOrDefaultAsync(f => f.challan_no == ChallanNumber);

                if(unpaidFee == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "No Challan Exist with this Challan Number.");
                }
                else
                {
                    db.unpaidFees.Remove(unpaidFee);

                    int rowAffected = await db.SaveChangesAsync();

                    if (rowAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Challan Deleted Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "OOPs! UnExp-ected Error Occurs. Please Try again after some time.");
                    }
                }

               
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
