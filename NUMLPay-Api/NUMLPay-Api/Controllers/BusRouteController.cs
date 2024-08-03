using NUMLPay_Api.DatabaseContext;
using NUMLPay_Api.Models;
using NUMLPay_Api.ViewModel;
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
    public class BusRouteController : ApiController
    {
        private readonly context db = new context();

        [Route("api/BusRoute/GetActivebyCampus/{id}")]
        public async Task<HttpResponseMessage> GetActivebyCampus(int id)
        {
            try
            {
                var busRoutes = await db.busRoutes
                    .Where(e => e.campus_id == id && e.is_active == 1)
                    .ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, busRoutes);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/BusRoute/Get/{id}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            try
            {
                var busRoute = await db.busRoutes.FirstOrDefaultAsync(e => e.id == id);

                if (busRoute != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, busRoute);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Bus Route Not Found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [Route("api/BusRoute/GetActiveForDepartment/{deptId}")]
        public async Task<HttpResponseMessage> GetActiveForDepartment(int deptId)
        {
            try
            {
                var campusId = await db.departments
                .Where(d => d.id == deptId)
                .Join(db.faculties,
                      d => d.faculty_id,
                      f => f.id,
                      (d, f) => f.campus_id)
                .FirstOrDefaultAsync();

                if (campusId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No campus found for the provided department ID.");
                }

                var activeBusRoutes = await db.busRoutes
                    .Where(br => br.campus_id == campusId && br.is_active == 1)
                    .ToListAsync();

                if (activeBusRoutes != null && activeBusRoutes.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, activeBusRoutes);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No active bus routes found for the provided department.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }


        [Route("api/BusRoute/GetByCampusId/{campusId}")]
        public async Task<HttpResponseMessage> GetByCampusId(int campusId)
        {
            try
            {
                var busRoutes = await db.busRoutes
                    .Where(e => e.campus_id == campusId)
                    .Join(db.campus,
                        busRoute => busRoute.campus_id,
                        campus => campus.Id,
                        (busRoute, campus) => new BusRouteView
                        {
                            id = busRoute.id,
                            campus_id = busRoute.campus_id,
                            name = busRoute.name,
                            added_by = busRoute.added_by,
                            status = busRoute.is_active,
                            campus_name = campus.name
                        })
                    .ToListAsync();

                if (busRoutes != null && busRoutes.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, busRoutes);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No Bus Routes found for the specified Campus ID.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        public async Task<HttpResponseMessage> Post(BusRoute newBusRoute)
        {
            try
            {
                bool busRouteExists = await db.busRoutes.AnyAsync(e => e.name.Trim().ToLower() == newBusRoute.name.Trim().ToLower() && e.campus_id == newBusRoute.campus_id);

                if (busRouteExists)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Bus Route Already Exists with the same Name.");
                }
                else
                {
                    db.busRoutes.Add(newBusRoute);
                    int rowsAffected = await db.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Bus Route Added Successfully!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create Bus Route.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [Route("api/BusRoute/Update/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> Update(int id, BusRoute busRoute)
        {
            try
            {
                var busRouteObj = await db.busRoutes.FirstOrDefaultAsync(e => e.id == id);

                if (busRouteObj != null)
                {
                    busRouteObj.name = busRoute.name;
                    busRouteObj.campus_id = busRoute.campus_id;
                    busRouteObj.is_active = busRoute.is_active;
                    busRouteObj.added_by = busRoute.added_by;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Bus Route Updated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to Update Bus Route.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [Route("api/BusRoute/Deactivate/{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> Deactivate(int id, BusRoute busRoute)
        {
            try
            {
                var busRouteObj = await db.busRoutes.FirstOrDefaultAsync(e => e.id == id);

                if (busRouteObj != null)
                {
                    busRouteObj.is_active = busRoute.is_active;
                    await db.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, "Bus Route Deactivated Successfully!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Failed to deactivate Bus Route.");
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