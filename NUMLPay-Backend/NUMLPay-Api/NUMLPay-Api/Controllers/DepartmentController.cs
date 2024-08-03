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
using System.Web.Mvc;

namespace NUMLPay_Api.Controllers
{
    public class DepartmentController : ApiController
    {
        [System.Web.Http.Route("api/Department/Get")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                using (context db = new context())
                {
                    var departments = db.Database.SqlQuery<DeptView>("spViewDept").ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, departments);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Department/GetbyId/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetbyId(int Id)
        {
            try
            {
                using (context db = new context())
                {
                    var departmentObj = await db.departments.FirstOrDefaultAsync(e => e.id == Id);

                    if (departmentObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, departmentObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Department Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Department/GetbyFacultyId/{facultyId}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetbyFacultyId(int facultyId)
        {
            try
            {
                using (context db = new context())
                {
                    var departmentList = await db.departments.Where(e => e.faculty_id == facultyId && e.is_active == 1).ToListAsync();

                    if (departmentList.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, departmentList);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Departments Found for the given Faculty Id.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
          
        }

        [System.Web.Http.Route("api/Department/GetbyCampusId/{campusId}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetbyCampusId(int campusId)
        {
            try
            {
                using (context db = new context())
                {
                    SqlParameter param = new SqlParameter("cId", campusId);
                    var departmentList = db.Database.SqlQuery<DeptView>("spViewdeptsByCampus @cId", param).ToList();

                    if (departmentList.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, departmentList);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Departments Found for the given Faculty Id(s).");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Department")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(Department newDepartment)
        {
            try
            {
                using (context db = new context())
                {
                    bool departmentExistsInSameFaculty = await db.departments.AnyAsync(e =>
                        e.name.Trim().ToLower() == newDepartment.name.Trim().ToLower() &&
                        e.faculty_id == newDepartment.faculty_id);

                    if (departmentExistsInSameFaculty)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Department Already Exists with a similar Name in the same Faculty.");
                    }
                    else
                    {
                        db.departments.Add(newDepartment);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Department Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Department.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Department/updateDepartment/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> UpdateDepartment(int id, Department department)
        {
            try
            {
                using (context db = new context())
                {
                    var departmentObj = await db.departments.FirstOrDefaultAsync(e => e.id == id);

                    if (departmentObj != null)
                    {
                        departmentObj.name = department.name;
                        departmentObj.added_by = department.added_by;
                        departmentObj.date = department.date;
                        departmentObj.is_active = department.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Department Updated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Department.");
                    }
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/Department/inActive/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Inactivate(int id, Department department)
        {
            try
            {
                using (context db = new context())
                {
                    var departmentObj = await db.departments.FirstOrDefaultAsync(e => e.id == id);

                    if (departmentObj != null)
                    {
                        departmentObj.is_active = department.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Department Deactivated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Deactivate Department.");
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
