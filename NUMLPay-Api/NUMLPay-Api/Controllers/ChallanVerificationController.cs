using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NUMLPay_Api.Controllers
{
    public class ChallanVerificationController : ApiController
    {
        private readonly context db = new context();

        [Route("api/ChallanVerification")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateChallanVerification(ChallanVerification challanVerification)
        {
            try
            {
                SqlParameter idParam = new SqlParameter("@fee_installment_id", challanVerification.fee_installment_id);
                SqlParameter imageParam = new SqlParameter("@Image", challanVerification.image);

                int rowsAffected = db.Database.ExecuteSqlCommand(
                    "spUpdateFeeInstallmentsAndVerification @fee_installment_id, @image",
                    idParam,
                    imageParam
                );

                if (rowsAffected > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, "Your Request for Fee Verification, you pay through Bank is Created Successfully. An Admin will respond within 7 Working Days. An Email is also sent to your Email.!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "OOPs! UnExp-ected Error Occurs. Please Try again after some time.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/ChallanVerification/GetByDeptId/{DeptId}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetByDeptId(int DeptId)
        {
            try
            {
                using (context db = new context())
                {
                    var unVerifiedChallans = await db.Database.SqlQuery<ChllanVerificationView>("spGetChallanVerificationData @dId", new SqlParameter("dId", DeptId))
                               .ToListAsync();

                    return Request.CreateResponse(HttpStatusCode.OK, unVerifiedChallans);
                    
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);

            }
           
        }
    }
}
