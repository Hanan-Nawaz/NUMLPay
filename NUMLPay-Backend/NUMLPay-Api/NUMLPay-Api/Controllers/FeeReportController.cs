using NUMLPay_Api.DatabaseContext;
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
    public class FeeReportController : ApiController
    {
        private readonly context db = new context();

        [Route("api/FeeReport/GetReportofTutionFee/{id}/{semester}/{session}")]
        public async Task<HttpResponseMessage> GetReportofTutionFee(int id, int semester, int session)
        {
            try
            {
                List<ReportFee> unpaidFees = null;
                    unpaidFees = await db.Database.SqlQuery<ReportFee>("spGetTuitionFeewithSem @shiftId, @semester, @session", new SqlParameter("@shiftId", id), new SqlParameter("@semester", semester), new SqlParameter("@session", session))
                      .ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, unpaidFees);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/FeeReport/GetReportofRepeatFee/{id}/{semester}/{session}")]
        public async Task<HttpResponseMessage> GetReportofRepeatFee(int id, int semester, int session)
        {
            try
            {
                List<ReportFee> unpaidFees = null;
                unpaidFees = await db.Database.SqlQuery<ReportFee>("spGetFeeRepeatCoursewithSem @shiftId, @semester, @SessionId", new SqlParameter("@shiftId", id), new SqlParameter("@semester", semester), new SqlParameter("@SessionId", session))
                  .ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, unpaidFees);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/FeeReport/GetReportofRepeatFee/{id}/{session}")]
        public async Task<HttpResponseMessage> GetReportofRepeatFee(int id, int session)
        {
            try
            {
                List<ReportFee> unpaidFees = null;
                unpaidFees = await db.Database.SqlQuery<ReportFee>("spGetFeeRepeatCourse @shiftId, @SessionId", new SqlParameter("@shiftId", id), new SqlParameter("@SessionId", session))
                  .ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, unpaidFees);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/FeeReport/GetReportofTutionFee/{id}/{session}")]
        public async Task<HttpResponseMessage> GetReportofTutionFee(int id, int session)
        {
            try
            {
                List<ReportFee> unpaidFees = null;
                    unpaidFees = await db.Database.SqlQuery<ReportFee>("spGetTuitionFee @shiftId, @session", new SqlParameter("@shiftId", id), new SqlParameter("@session", session))
                      .ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, unpaidFees);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/FeeReport/SummerFee/{id}/{year}")]
        [HttpGet]
        public async Task<HttpResponseMessage> SummerFee(int id, int year)
        {
            try
            {
                List<ReportFee> unpaidFees = await db.Database.SqlQuery<ReportFee>("spGetSummerFeesByDept @dept_id, @year", new SqlParameter("@dept_id", id), new SqlParameter("@year", year))
                      .ToListAsync();


                return Request.CreateResponse(HttpStatusCode.OK, unpaidFees);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/FeeReport/HMBFee/{campusId}/{feeFor}/{session}")]
        [HttpGet]
        public async Task<HttpResponseMessage> HMBFee(int campusId, int feeFor, int session)
        {
            try
            {
                List<ReportFee> unpaidFees = await db.Database.SqlQuery<ReportFee>("spGetHMBFeeReport @campusId, @feeFor, @session", new SqlParameter("@campusId", campusId), new SqlParameter("@feeFor", feeFor), new SqlParameter("@session", session))
                      .ToListAsync();


                return Request.CreateResponse(HttpStatusCode.OK, unpaidFees);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }
}
}
