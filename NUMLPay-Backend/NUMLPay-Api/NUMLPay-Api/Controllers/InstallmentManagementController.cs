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
    public class InstallmentManagementController : ApiController
    {
        private readonly context db = new context();

        [Route("api/InstallmentManagement")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateInstallment(InstallmentManagement newInstallment)
        {
            try
            {
                bool installmentExists = await db.installmentManagements.AnyAsync(e => e.mode == newInstallment.mode && e.fee_for == newInstallment.fee_for);

                if (installmentExists)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Installment Mode Already Exists.");
                }
                else
                {
                    db.installmentManagements.Add(newInstallment);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Installment Created Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to Create Installment.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/InstallmentManagement/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateInstallment(int id, InstallmentManagement installment)
        {
            try
            {
                var installmentObj = await db.installmentManagements.FirstOrDefaultAsync(e => e.installment_id == id);

                if (installmentObj != null)
                {
                    installmentObj.mode = installment.mode;
                    installmentObj.fee_for = installment.fee_for;
                    installmentObj.is_active = installment.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Installment Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Installment.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/InstallmentManagement")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetInstallments()
        {
            try
            {
                var installments = await db.installmentManagements.ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, installments);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/InstallmentManagement/getActive/{fee_for}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetActiveInstallments(int fee_for)
        {
            try
            {
                var installments = await db.installmentManagements.Where(inst => inst.is_active == 1 && inst.fee_for == fee_for).ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, installments);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/InstallmentManagement/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetInstallmentById(int id)
        {
            try
            {
                var installmentObj = await db.installmentManagements.FirstOrDefaultAsync(e => e.installment_id == id);

                if (installmentObj != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, installmentObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Installment Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [Route("api/InstallmentManagement/inActive/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> DeactivateInstallment(int id, InstallmentManagement installment)
        {
            try
            {
                var installmentObj = await db.installmentManagements.FirstOrDefaultAsync(e => e.installment_id == id);

                if (installmentObj != null)
                {
                    installmentObj.is_active = installment.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Installment Deactivated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Deactivate Installment.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
          
        }

    }
}
