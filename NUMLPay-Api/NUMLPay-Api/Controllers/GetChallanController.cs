using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.ModelFront;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace NUMLPay_Api.Controllers
{
    public class GetChallanController : ApiController
    {
        private readonly context db = new context();


        [Route("api/GetChallan/Get/{id}/{feeType}/{fee_for}")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(int id, int feeType, int fee_for)
        {
            try
            {
                if (feeType == 1)
                {
                    var idParam = new SqlParameter("@id", id);

                    await db.Database.ExecuteSqlCommandAsync(
                        "spUpdateFinesForFeeInstallmentFee @id",
                        idParam
                    );
                }
                else
                {
                    var idParam = new SqlParameter("@id", id);
                    var fineTypeParam = new SqlParameter("@fineType", feeType);
                    var fee_for_param = new SqlParameter("@fee_for", fee_for);

                    await db.Database.ExecuteSqlCommandAsync(
                        "spUpdateFinesForFeeInstallments @id, @fineType, @fee_for",
                        idParam, fineTypeParam, fee_for_param
                    );
                }

                SqlParameter param = new SqlParameter("id", id);
                var results = await db.Database.SqlQuery<JoinedDataChallan>("spGetFeeInstallmentDetails @id", param).SingleOrDefaultAsync();

                return Request.CreateResponse(HttpStatusCode.OK, results);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred." + ex.Message);
            }
        }

        [Route("api/GetChallan/GetChallan/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetChallan(int id)
        {
            try
            {
                SqlParameter param = new SqlParameter("id", id);
                var results = await db.Database.SqlQuery<JoinedDataChallan>("spGetFeeInstallmentDetails @id", param).SingleOrDefaultAsync();

                return Request.CreateResponse(HttpStatusCode.OK, results);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred." + ex.Message);
            }
        }

        [Route("api/GetChallan/{id}/{Cursem}/{feeType}/{numlId}/{feeFor}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetSubChallan(int id, int Cursem, int feeType, string numlId, int feeFor)
        {
            try
            {
                List<SubFeeView> listSubFee = null;

                if (feeType == 1)
                {
                    SqlParameter idParam = new SqlParameter("Id", id);
                    SqlParameter curSemParam = new SqlParameter("currentSem ", Cursem);
                    listSubFee = await db.Database.SqlQuery<SubFeeView>("spGetSubFee @currentSem, @Id", curSemParam, idParam).ToListAsync();
                }
                else if (feeType == 3)
                {
                    SqlParameter idParam = new SqlParameter("Id", id);
                    SqlParameter feeForParam = new SqlParameter("feeFor", feeFor);
                    SqlParameter numlParam = new SqlParameter("std_numl_id", numlId);
                    listSubFee = await db.Database.SqlQuery<SubFeeView>("spGetFeeDetailsWithSecurity  @std_numl_id, @feeFor, @Id", numlParam, feeForParam, idParam).ToListAsync();
                }

                return Request.CreateResponse(HttpStatusCode.OK, listSubFee);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred." + ex.Message);
            }
        }

        [Route("api/GetChallan/{id}/{Cursem}/{feeType}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetSubChallan(int id, int Cursem, int feeType)
        {
            try
            {
                List<SubFeeView> listSubFee = null;

                if (feeType == 1 )
                {
                    SqlParameter idParam = new SqlParameter("Id", id);
                    SqlParameter curSemParam = new SqlParameter("currentSem ", Cursem);
                    listSubFee = await db.Database.SqlQuery<SubFeeView>("spGetSubFee @currentSem, @Id", curSemParam, idParam).ToListAsync();
                }
                else if (feeType == 2)
                {
                    SqlParameter idParam = new SqlParameter("Id", id);
                    listSubFee = await db.Database.SqlQuery<SubFeeView>("spGetMisFee @Id", idParam).ToListAsync();
                }
                else if (feeType == 4)
                {
                    SqlParameter idParam = new SqlParameter("Id", id);
                    listSubFee = await db.Database.SqlQuery<SubFeeView>("spGetSumFee @Id", idParam).ToListAsync();
                }
                

                return Request.CreateResponse(HttpStatusCode.OK, listSubFee);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred." + ex.Message);
            }
        }
    }
}