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

namespace NUMLPay_Api.Controllers
{
    public class SummerEnrollmentController : ApiController
    {
        private readonly context db = new context();

        [Route("api/SummerEnrollment/GetByDeptId/{id}")]
        public async Task<HttpResponseMessage> GetByDeptId(int id)
        {
            try
            {
                var summerEnrollments = await db.summerEnrollments
                    .Include(se => se.summerFee)
                    .Include(se => se.summerFee.subjects)
                    .Where(se => se.summerFee.subjects.dept_id == id)
                    .Select(se => new SummerEnrollmentView
                    {
                        id = se.id,
                        subject_name = se.summerFee.subjects.name,
                        numl_id = se.std_numl_id,
                        session_year = se.summerFee.session_year,
                        added_by = se.added_by,
                    })
                    .ToListAsync();

                return Request.CreateResponse(HttpStatusCode.OK, summerEnrollments);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/SummerEnrollment/GetById/{id}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            try
            {
                var enrollmentObj = await db.summerEnrollments.FirstOrDefaultAsync(e => e.id == id);

                if (enrollmentObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, enrollmentObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Enrollment Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/SummerEnrollment/{dept_id}")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(int dept_id, SummerEnrollment newEnrollment)
        {
            int challan_id = 0;

            try
            {
                var entity = db.users.FirstOrDefault(e => e.dept_id == dept_id && e.numl_id == newEnrollment.std_numl_id);


                if (entity != null)
                {

                    bool enrollmentExists = await db.summerEnrollments.AnyAsync(e => e.summer_fee_id == newEnrollment.summer_fee_id && e.std_numl_id == newEnrollment.std_numl_id);
                    var summerFeeObj = await db.summerFees.FirstOrDefaultAsync(e => e.id == newEnrollment.summer_fee_id);

                    if (enrollmentExists)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Enrollment Already Exists for the specified Subject and User.");
                    }
                    else
                    {
                        db.summerEnrollments.Add(newEnrollment);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            challan_id = db.Database.SqlQuery<int>(
                                     "EXEC spGenerateSummerChallan @std_numl_id, @fee_id, @issue_date",
                                     new SqlParameter("std_numl_id", newEnrollment.std_numl_id),
                                     new SqlParameter("fee_id", newEnrollment.summer_fee_id),
                                     new SqlParameter("issue_date", DateTime.Now)
                                    ).FirstOrDefault();

                            if (challan_id > 0)
                            {
                                int installment_no = db.Database.SqlQuery<int>(
                                    "EXEC spAddSummerInstallment @total_fee, @challan_id",
                                    new SqlParameter("total_fee", summerFeeObj.fee),
                                    new SqlParameter("challan_id", challan_id)
                                ).FirstOrDefault();

                                if (installment_no > 0)
                                {
                                    return Request.CreateResponse(HttpStatusCode.Created, "Enrollment Added Successfully!");

                                }
                                else
                                {

                                    db.summerEnrollments.Remove(newEnrollment);
                                    await db.SaveChangesAsync();

                                    var unpaidfeeObj = await db.unpaidFees.FirstOrDefaultAsync(e => e.challan_no == challan_id);

                                    db.unpaidFees.Remove(unpaidfeeObj);
                                    await db.SaveChangesAsync();

                                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Enrollment.");
                                }

                            }
                            else
                            {
                                db.summerEnrollments.Remove(newEnrollment);
                                await db.SaveChangesAsync();

                                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Enrollment.");
                            }

                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Enrollment.");
                        }
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "No Student found with Id " + newEnrollment.std_numl_id + " in your department.");
                }
            }
            catch (Exception ex)
            {
                db.summerEnrollments.Remove(newEnrollment);
                await db.SaveChangesAsync();

                var unpaidfeeObj = await db.unpaidFees.FirstOrDefaultAsync(e => e.challan_no == challan_id);

                db.unpaidFees.Remove(unpaidfeeObj);
                await db.SaveChangesAsync();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/SummerEnrollment/DeleteEnrollment/{id}")]
        public async Task<HttpResponseMessage> DeleteEnrollment(int id)
        {
            try
            {
                var enrollmentObj = await db.summerEnrollments.FirstOrDefaultAsync(e => e.id == id);
                var unpaidfeeObj = await db.unpaidFees.FirstOrDefaultAsync(e => e.fee_id == enrollmentObj.summer_fee_id && e.std_numl_id == enrollmentObj.std_numl_id && e.fee_type == 4);
                var installmentfeeObj = await db.feeInstallments.FirstOrDefaultAsync(e => e.challan_id == unpaidfeeObj.challan_no);

                if (installmentfeeObj != null)
                {
                    db.summerEnrollments.Remove(enrollmentObj);
                    db.unpaidFees.Remove(unpaidfeeObj);
                    db.feeInstallments.Remove(installmentfeeObj); 

                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Enrollment Deleted Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Delete Enrollment.");
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

