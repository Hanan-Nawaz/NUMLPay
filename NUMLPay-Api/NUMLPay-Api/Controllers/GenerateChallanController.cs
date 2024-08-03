using NUMLPay_Api.Models;
using NUMLPay_Api.DatabaseContext;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using NUMLPay_Api.ViewModel;
using System.Web.Http.Results;

namespace NUMLPay_Api.Controllers
{
    public class GenerateChallanController : ApiController
    {
        private readonly context db = new context();


        [Route("api/GenerateChallan/{sessionId}/{feeFor}/{mode}/{shiftId}/{admissionSession}/{feePlan}/{dept_route}")]
        [HttpPost]
        public async Task<HttpResponseMessage> GenerateChallans(int sessionId, int feeFor, int mode, int shiftId, int admissionSession, int feePlan, int dept_route, UnpaidFees newChallan)
        {
            try
            {
                    GetFeesForGeneration result = null;
                    int installmentType = 0;
                    double totalFee = 0, tuitionFee = 0;
                    string dueDate = null, validDate = null;

                    if (feeFor == 1)
                    {
                        newChallan.fee_type = 1;
                        result = await db.Database.SqlQuery<GetFeesForGeneration>(
                            " spGetFees @currentSem, @modeId, @feePlanID, @sessionId, @feeForId, @admissionSession, @numlId",
                            parameters: new object[]
                            {
                            new SqlParameter("currentSem", newChallan.semester),
                            new SqlParameter("modeId", mode),
                            new SqlParameter("feePlanID", feePlan),
                            new SqlParameter("sessionId", sessionId),
                            new SqlParameter("feeForId", feeFor),
                            new SqlParameter("admissionSession", admissionSession),
                            new SqlParameter("numlId", newChallan.std_numl_id)
                            }
                        ).FirstOrDefaultAsync();
                    }
                    else
                    {
                        newChallan.fee_type = 3;
                        result = await db.Database.SqlQuery<GetFeesForGeneration>(
                            " spGetFeeStructures @sessionId, @feeForId,  @admissionSession, @modeId, @numlId, @dept_route",
                            parameters: new object[]
                            {
                            new SqlParameter("@sessionId", sessionId),
                            new SqlParameter("@feeForId", feeFor),
                            new SqlParameter("@modeId", mode),
                            new SqlParameter("@admissionSession", admissionSession),
                            new SqlParameter("@numlId", newChallan.std_numl_id),
                            new SqlParameter("@dept_route", dept_route)
                            }
                        ).FirstOrDefaultAsync();
                    }


                    if (result != null)
                    {
                        newChallan.challan_id = result.challan_id;
                        newChallan.fee_id = result.Id;
                        newChallan.fee_instalments_id = mode;
                        newChallan.security = Convert.ToInt32(result.security);

                        installmentType = result.mode;

                        if (feeFor == 1)
                        {
                            double totalFees = Convert.ToDouble(result.total_fee);
                            float tuition_fee = result.tuition_fee;
                            float discountPercentage = result.discount;

                            float discountedFee = tuition_fee * (1 - (discountPercentage / 100));

                            totalFee = totalFees + discountedFee;
                        }
                        else
                        {
                            totalFee = result.total_fee;
                        }

                        dueDate = result.due_date;
                        validDate = result.valid_date;

                        bool challanExists = await db.unpaidFees.AnyAsync(e => e.challan_id == newChallan.challan_id & e.std_numl_id == newChallan.std_numl_id);

                        if (challanExists)
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Challan Already Exists for Similar Challan Type and Session.");
                        }
                        else
                        {
                            db.unpaidFees.Add(newChallan);

                            int rowsAffected = await db.SaveChangesAsync();
                            int newId = newChallan.challan_no;

                            if (rowsAffected > 0)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlCommand(
                                        "spGenerateFeeInstallments @newId, @totalFee, @dueDate, @validDate, @installmentType",
                                        new SqlParameter("newId", newId),
                                        new SqlParameter("totalFee", totalFee),
                                        new SqlParameter("dueDate", (dueDate)),
                                        new SqlParameter("validDate", (validDate)),
                                        new SqlParameter("installmentType", installmentType)
                                    );
                                    return Request.CreateResponse(HttpStatusCode.Created, "Challan Generated Successfully!");

                                }
                                catch (Exception ex)
                                {
                                    var entityToRemove = db.unpaidFees.FirstOrDefault(e => e.challan_no == newId);

                                    if (entityToRemove != null)
                                    {
                                        db.unpaidFees.Remove(entityToRemove);
                                        db.SaveChanges();
                                    }

                                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Generate Challan.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Generate Challan.");
                            }
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Find Fee for this Session and Challan Type.");
                    }
                
                
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/GenerateChallan/GenerateRepeatChallans/{dept_id}")]
        [HttpPost]
        public async Task<HttpResponseMessage> GenerateRepeatChallans(int dept_id, UnpaidFees newChallan)
        {
            try
            {
                var entity = db.users.FirstOrDefault(e => e.dept_id == dept_id && e.numl_id == newChallan.std_numl_id);


                if (entity != null)
                {


                    int totalFee = newChallan.security;

                    newChallan.security = Convert.ToInt32(newChallan.challan_id);
                    newChallan.challan_id = null;
                    newChallan.fee_type = 6;
                    newChallan.issue_date = DateTime.Now.ToString();
                    newChallan.semester = 0;

                    var check = db.unpaidFees.FirstOrDefault(e => e.std_numl_id == newChallan.std_numl_id && e.fee_id == newChallan.fee_id && e.security == newChallan.security);

                    if (check == null)
                    {
                        db.unpaidFees.Add(newChallan);

                        int rowsAffected = await db.SaveChangesAsync();
                        int newId = newChallan.challan_no;

                        if (rowsAffected > 0)
                        {
                            DateTime currentDate = DateTime.Now;

                            DateTime nextMonthDate = currentDate.AddMonths(1);
                            DateTime nextTwoMonthDate = currentDate.AddMonths(2);
                            try
                            {
                                db.Database.ExecuteSqlCommand(
                                    "spGenerateFeeInstallments @newId, @totalFee, @dueDate, @validDate, @installmentType",
                                    new SqlParameter("newId", newId),
                                    new SqlParameter("totalFee", totalFee),
                                    new SqlParameter("dueDate", nextMonthDate),
                                    new SqlParameter("validDate", (nextTwoMonthDate)),
                                    new SqlParameter("installmentType", 1)
                                );
                                return Request.CreateResponse(HttpStatusCode.Created, "Challan Generated Successfully!");

                            }
                            catch (Exception ex)
                            {
                                var entityToRemove = db.unpaidFees.FirstOrDefault(e => e.challan_no == newId);

                                if (entityToRemove != null)
                                {
                                    db.unpaidFees.Remove(entityToRemove);
                                    db.SaveChanges();
                                }

                                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Generate Challan.");
                            }

                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Generate Challan.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Fee already exists.");
                    }

                }
       
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "No Student found with Id " + newChallan.std_numl_id + " in your department.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/GenerateChallan")]
        [HttpPost]
        public async Task<HttpResponseMessage> GenerateChallans(UnpaidFees newChallan)
        {
            try
            {
                var query = from miscellaneousFee in db.miscellaneousFees
                            where miscellaneousFee.Id == newChallan.fee_id
                            select miscellaneousFee.amount;

                var result = query.FirstOrDefault();

                db.unpaidFees.Add(newChallan);

               int rowsAffected = await db.SaveChangesAsync();
               int newId = newChallan.challan_no;

                if (rowsAffected > 0)
                {
                    DateTime currentDate = DateTime.Now;
                    DateTime nowDueDate = currentDate.AddDays(30);

                    FeeInstallments feeInstallment = new FeeInstallments();
                    feeInstallment.challan_id = newId;
                    feeInstallment.installment_no = 0;
                    feeInstallment.total_fee = result;
                    feeInstallment.due_date = nowDueDate.ToString("yyyy-MM-dd");
                    feeInstallment.valid_date = nowDueDate.AddDays(30).ToString("yyyy-MM-dd");
                    feeInstallment.status = 3;

                    db.feeInstallments.Add(feeInstallment);
                    int rowsAffectedFeeInstallments = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Miscellaneous Challan Generated Successfully!");
                    }
                    else
                    {
                        var entityToRemove = db.unpaidFees.FirstOrDefault(e => e.challan_no == newId);

                        db.unpaidFees.Remove(entityToRemove);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Generate Challan.");

                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Generate Challan.");
                }
                    
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [System.Web.Http.Route("api/GenerateChallan/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetUnPaidfee(string id)
        {
            using (context db = new context())
            {
                try
                {
                    var idParam = new SqlParameter("@std_numl_id", id);
                    var results = await db.Database.SqlQuery<UnPaidFeeView>("spGetUnPaidFee @std_numl_id", idParam).ToListAsync();

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
                catch(Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }

            }
        }

        [System.Web.Http.Route("api/GenerateChallan/{numlId}/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetUnPaidfeeByID(string numlId, int id)
        {
            using (context db = new context())
            {
                try
                {
                    var numlIdParam = new SqlParameter("@@stdNumlId", numlId);
                    var idParam = new SqlParameter("@@fiId", id);
                    var results = await db.Database.SqlQuery<UnPaidFeeView>("spGetUnPaidFeebyId @stdNumlId, @fiId", numlIdParam, idParam).ToListAsync();

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }

            }
        }
    }
}
