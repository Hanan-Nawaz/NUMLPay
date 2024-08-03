using NUMLPay_Api.CreateModel;
using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Migrations;
using NUMLPay_Api.ModelFront;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NUMLPay_Api.Controllers
{
    public class AccountBookController : ApiController
    {
        private readonly context db = new context();

        [Route("api/AccountBook")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAccountBook(AccountBook accountBook)
        {
            try
            {
                int rowsAffected = db.Database.ExecuteSqlCommand(
                     "spUpdateFeeAndAccountBooks @Id, @numlId",
                     new SqlParameter("Id", accountBook.challan_no),
                     new SqlParameter("numlId", accountBook.std_numl_id)
                 );

                if (rowsAffected > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, "Your Fee is Paid. You will download Paid Challan from your Account Book. An Email is also sent to your Email.!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "OOPs! UnExp-ected Error Occurs. Please Try again after some time.");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/AccountBook/CreateAccountBookbyBank")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAccountBookbyBank(CreateFeeAndAccountBook newBook)
        {
            try
            {
                int rowsAffected = db.Database.ExecuteSqlCommand(
                     "spUpdateFeeByBankAndAccountBooks @Id, @numlId, @status, @verified_by",
                     new SqlParameter("Id", newBook.Id),
                     new SqlParameter("numlId", newBook.numlId),
                     new SqlParameter("status", newBook.status),
                     new SqlParameter("verified_by", newBook.verified_by)
                 );

                if (rowsAffected > 0)
                {
                    if (newBook.status == 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Fee is Verified!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Fee is Disapproved!");
                    }
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

        [System.Web.Http.Route("api/AccountBook/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPaidfee(string id)
        {
            using (context db = new context())
            {
                try
                {
                    var idParam = new SqlParameter("@studentId", id);
                    var results = await db.Database.SqlQuery<UnPaidFeeView>("spGetPaidFees @studentId", idParam).ToListAsync();

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }

            }
        }

        [Route("api/AccountBook/GetPaidChallan/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPaidChallan(int id)
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
    }
}
