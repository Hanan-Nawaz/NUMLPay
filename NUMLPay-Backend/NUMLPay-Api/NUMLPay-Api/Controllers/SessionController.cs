using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.ViewModel;
using NUMLPay_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.SqlClient;

namespace NUMLPay_Api.Controllers
{
    public class SessionController : ApiController
    {
        [System.Web.Http.Route("api/Session/Get")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                using (context db = new context())
                {
                    var session = await db.sessions.ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, session);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    string innerErrorMessage = ex.InnerException.Message;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + innerErrorMessage);
                }
                else
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                }
            }
            
        }

        [System.Web.Http.Route("api/Session/GetforDdl")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetforDdl()
        {
            using (context db = new context())
            {
                try
                {
                    var session = await db.Database.SqlQuery<SeesionView>("spgetSessions").ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, session);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);

                }
            }
        }

        [System.Web.Http.Route("api/Session/GetforEligibleDdl/{Id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetforEligibleDdl(int Id)
        {
            using (context db = new context())
            {
                try
                {
                    var sessions = await db.Database.SqlQuery<SeesionView>("GetSessionsBasedOnId @sessionId", new SqlParameter("sessionId", Id)).ToListAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, sessions);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ex.Message);

                }
            }
        }

        [System.Web.Http.Route("api/Session/GetById/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            try
            {
                using (context db = new context())
                {
                    var sessionObj = await db.sessions.FirstOrDefaultAsync(e => e.id == id);

                    if (sessionObj != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, sessionObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Session Not Found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
            
        }

        [System.Web.Http.Route("api/Session")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(Session newSession)
        {
            try
            {
                using (context db = new context())
                {
                    bool sessionExistsInSameCampus = await db.sessions.AnyAsync(e =>
                         e.name == newSession.name &&
                         e.year == newSession.year);

                    if (sessionExistsInSameCampus)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Session Already Exists with a similar Name and Year.");
                    }
                    else
                    {
                        db.sessions.Add(newSession);
                        int rowsAffected = await db.SaveChangesAsync();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Session Added Successfully!");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Session.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Session/updateSession/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> UpdateSession(int id, Session session)
        {
            try
            {
                using (context db = new context())
                {
                    var sessionObj = await db.sessions.FirstOrDefaultAsync(e => e.id == id);

                    if (sessionObj != null)
                    {
                        sessionObj.name = session.name;
                        sessionObj.added_by = session.added_by;
                        sessionObj.is_active = session.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Session Updated Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Session.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Session/inActive/{id}")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Inactivate(int id, Session session)
        {
            try
            {
                using (context db = new context())
                {
                    var sessionObj = await db.sessions.FirstOrDefaultAsync(e => e.id == id);

                    if (sessionObj != null)
                    {
                        sessionObj.is_active = session.is_active;
                        await db.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK, "Session Deleted Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to delete Session.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
           
        }

        [System.Web.Http.Route("api/Session/GetSemester/{selectedSessionId}")]
        [System.Web.Http.HttpGet]
        public async Task<int> GetSemester(int selectedSessionId)
        {
            try
            {
                using (context db = new context())
                {
                    var selectedSession = await db.sessions.FirstOrDefaultAsync(s => s.id == selectedSessionId);

                    if (selectedSession == null)
                    {
                        return 0;
                    }

                    int selectedYear = selectedSession.year;
                    int selectedName = selectedSession.name;

                    var sessionsAfterSelected = db.sessions.Where(s => s.year > selectedYear || (s.year == selectedYear && s.name > selectedName));

                    int numberOfSessionsAfterSelected = sessionsAfterSelected.Count();

                    return numberOfSessionsAfterSelected;
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine("An error occurred: " + ex.Message);
                return 0;
            }
           
        }

    }
}
