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
    public class DegreeController : ApiController
    {
        [Route("api/Degree/Get")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                using (context db = new context())
                {
                    var degreesWithShifts = db.Database.SqlQuery<DegreeView>("spViewDegrees")
                               .ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, degreesWithShifts);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [Route("api/Degree/GetbyId/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetbyId(int Id)
        {
            try
            {
                using (context db = new context())
                {
                    var degree = await db.degree.FirstOrDefaultAsync(e => e.id == Id);

                    if (degree != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, degree);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Degree Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }   

        [System.Web.Http.Route("api/Degree/GetByDept/{deptId}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetByDept(int deptId)
        {
            try
            {
                using (context db = new context())
                {
                    var degreeList = await db.degree.Where(e => e.dept_id == deptId).ToListAsync();

                    if (degreeList.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, degreeList);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Degree's Found for the given Dept Id.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Degree/GetByDeptandLevel")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetByDeptandLevel(Degree getDegree)
        {
            try
            {
                using (context db = new context())
                {
                    var degreeList = await db.degree.Where(e => e.dept_id == getDegree.dept_id).ToListAsync();

                    if (degreeList.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, degreeList);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Degree's Found for the given Dept Id and Academic Level.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Degree/GetDegreesByCampusId/{campusId}")]
        public async Task<HttpResponseMessage> GetDegreesByCampusId(int campusId)
        {
            using (context db = new context())
            {
                try
                {
                    SqlParameter param = new SqlParameter("cId", campusId);
                    var degreesWithShifts = db.Database.SqlQuery<DegreeView>("spViewdegreesById @cId", param)
                           .ToList();

                    if (degreesWithShifts.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, degreesWithShifts);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Degrees Found for the given Campus.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
                }
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Degree/GetDegreesByDeptId/{deptId}")]
        public async Task<HttpResponseMessage> GetDegreesByDeptId(int deptId)
        {
            using (context db = new context())
            {
                try
                {
                    SqlParameter param = new SqlParameter("dId", deptId);
                    var degreesWithShifts =await db.Database.SqlQuery<DegreeView>("spViewdegreesById @dId", param)
                           .ToListAsync();

                    if (degreesWithShifts.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, degreesWithShifts);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Degrees Found for the given Campus.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
                }
            }
        }

        [Route("api/Degree")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(Degree newDegree)
        {
            try
            {
                using (context db = new context())
                {
                    bool degreeExistsInSameDepartment = await db.degree.AnyAsync(e =>
                        e.name.Trim().ToLower() == newDegree.name.Trim().ToLower() &&
                        e.dept_id == newDegree.dept_id);

                    if (degreeExistsInSameDepartment)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Degree Already Exists with a similar Name in the same Department.");
                    }
                    else
                    {
                        db.degree.Add(newDegree);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Degree Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Degree.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
          
        }

        [Route("api/Degree/GetDegreebyDeptLevel/{deptId}/{academicId}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetDegreebyDeptLevel(int deptId, int academicId)
        {
            using (context db = new context())
            {
                SqlParameter paramDeptId = new SqlParameter("@dId", deptId);
                SqlParameter paramAcademicId = new SqlParameter("@aId", academicId);
                try
                {
                        var degreesWithShifts = db.Database.SqlQuery<DegreeView>(
                       "spGetDegreebyDeptandLevel @dId, @aId",
                       paramDeptId,
                       paramAcademicId
                   ).ToList();

                    if (degreesWithShifts.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, degreesWithShifts);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Degrees Found for the given Dept and Academic Level.");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
                }


            }
        }

        [Route("api/Degree/inActive/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> inActive(int id, Shift newShift)
        {
            try
            {
                using (context db = new context())
                {
                    var degreeObj = await db.shift.FirstOrDefaultAsync(e => e.Id == id);

                    if (degreeObj != null)
                    {
                        degreeObj.is_active = newShift.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Degree Deleted Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Deactivate Degree.");
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
