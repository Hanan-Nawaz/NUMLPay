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
    public class FacultyController : ApiController
    {
        [System.Web.Http.Route("api/Faculty/Get")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (context db = new context())
            {
                try
                {
                    var faculties = await db.Database.SqlQuery<FacultyView>("spViewfaculities").ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, faculties);
                }
                catch(Exception ex) 
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
                }

            }
        }

        [System.Web.Http.Route("api/Faculty/GetbyCampus/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetbyCampus(int id)
        {
            try
            {
                using (context db = new context())
                {
                    SqlParameter param = new SqlParameter("cId", id);
                    var faculties = db.Database.SqlQuery<FacultyView>("spViewfacultybyCId @cId", param).ToList();

                    if (faculties.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, faculties);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Faculty Found for the given Campus Id.");
                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Faculty/GetbyId/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetbyId(int Id)
        {
            try
            {
                using (context db = new context())
                {
                    var facultyObj = await db.faculties.FirstOrDefaultAsync(e => e.id == Id);

                    if (facultyObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, facultyObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Faculty Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
          
        }


        [System.Web.Http.Route("api/Faculty/GetbyCampusId/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetbyCampusId(int id)
        {
            try
            {
                using (context db = new context())
                {
                    var facultyList = await db.faculties.Where(e => e.campus_id == id && e.is_active == 1).ToListAsync();

                    if (facultyList.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, facultyList);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "No Faculty Found for the given Campus Id.");
                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }
        

        [System.Web.Http.Route("api/Faculty")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(Faculty newFaculty)
        {
            try
            {
                using (context db = new context())
                {

                    bool facultyExistsInSameCampus = await db.faculties.AnyAsync(e =>
                         e.name.Trim().ToLower() == newFaculty.name.Trim().ToLower() &&
                         e.campus_id == newFaculty.campus_id);

                    if (facultyExistsInSameCampus)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Faculty Already Exists with a similar Name.");
                    }
                    else
                    {
                        db.faculties.Add(newFaculty);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Faculty Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Faculty.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/Faculty/updateFaculty/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> updateFaculty(int id, Faculty Faculty)
        {
            try
            {
                using (context db = new context())
                {
                    var facultyObj = await db.faculties.FirstOrDefaultAsync(e => e.id == id);

                    if (facultyObj != null)
                    {
                        facultyObj.name = Faculty.name;
                        facultyObj.added_by = Faculty.added_by;
                        facultyObj.date = Faculty.date;
                        facultyObj.is_active = Faculty.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Faculty Updated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Faculty.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/Faculty/inActive/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> inActive(int id, Faculty Faculty)
        {
            try
            {
                using (context db = new context())
                {
                    var facultyObj = await db.faculties.FirstOrDefaultAsync(e => e.id == id);

                    if (facultyObj != null)
                    {
                        facultyObj.is_active = Faculty.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Faculty Deleted Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to delete Faculty.");
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
