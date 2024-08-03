using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NUMLPay_Api.ViewModel;

namespace NUMLPay_Api.Controllers
{
    public class DashboardController : ApiController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["dwcon"].ConnectionString;


        [HttpGet]
        [Route("api/dw/Dashboard/getcampus")]
        public async Task<HttpResponseMessage> getcampus()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT campus_id, campus_name from Campus";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    List<FacultyModel> results = new List<FacultyModel>();
                    while (reader.Read())
                    {
                        var data = new FacultyModel
                        {
                            CampusId = Convert.ToInt32(reader["campus_id"]),
                            FacultyName = reader["campus_name"].ToString(),
                            FacultyId = 0
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpGet]
        [Route("api/dw/Dashboard/feepaidbycampus")]
        public async Task<HttpResponseMessage> feepaidbycampus()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT campus_name, fee_paid FROM FeePaidByCampus";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    List<FeePaidByCampusModel> results = new List<FeePaidByCampusModel>();
                    while (reader.Read())
                    {
                        var data = new FeePaidByCampusModel
                        {
                            CampusName = reader["campus_name"].ToString(),
                            FeePaid = reader["fee_paid"] != DBNull.Value ? Convert.ToInt32(reader["fee_paid"]) : 0
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/dw/Dashboard/studentsincampus")]
        public async Task<HttpResponseMessage> studentsincampus()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT C.campus_name, S.total_students FROM Campus AS C LEFT JOIN StudentsInCampus AS S ON C.campus_id = S.campus_id";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    List<FeePaidByCampusModel> results = new List<FeePaidByCampusModel>();
                    while (reader.Read())
                    {
                        var data = new FeePaidByCampusModel
                        {
                            CampusName = reader["campus_name"].ToString(),
                            FeePaid = reader["total_students"] != DBNull.Value ? Convert.ToInt32(reader["total_students"]) : 0
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }


        [HttpGet]
        [Route("api/dw/Dashboard/feepaidbycategory/{category}")]
        public async Task<HttpResponseMessage> GetFeePaidByCategory(string category)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Modify the SQL query to filter by category if provided
                    string sqlQuery = "SELECT campus_name, fee_paid, category FROM FeePaidByCategory";
                    if (!string.IsNullOrEmpty(category))
                    {
                        sqlQuery += $" WHERE category = '{category}'";
                    }

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    List<FeePaidByCategory> results = new List<FeePaidByCategory>();
                    while (reader.Read())
                    {
                        var data = new FeePaidByCategory
                        {
                            CampusName = reader["campus_name"].ToString(),
                            FeePaid = Convert.ToDecimal(reader["fee_paid"]),
                            Category = reader["category"].ToString(),
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/dw/Dashboard/getFaculties/{campus_id}")]
        public async Task<HttpResponseMessage> getFaculties(int campus_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT campus_id, faculty_name, faculty_id FROM Faculties WHERE campus_id = @CampusId";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@CampusId", campus_id);

                    SqlDataReader reader = command.ExecuteReader();

                    List<FacultyModel> results = new List<FacultyModel>();
                    while (reader.Read())
                    {
                        var data = new FacultyModel
                        {
                            CampusId = Convert.ToInt32(reader["campus_id"]),
                            FacultyName = reader["faculty_name"].ToString(),
                            FacultyId = Convert.ToInt32(reader["faculty_id"])
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpGet]
        [Route("api/dw/Dashboard/getFacultiesdept/{faculty_id}")]
        public async Task<HttpResponseMessage> GetFacultiesDept(int faculty_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT dept_id, dept_name, faculty_id, total_students FROM FacultyDepartments WHERE faculty_id = @FacultyId";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@FacultyId", faculty_id);

                    SqlDataReader reader = command.ExecuteReader();

                    List<DepartmentModel> results = new List<DepartmentModel>();
                    while (reader.Read())
                    {
                        var data = new DepartmentModel
                        {
                            DeptId = Convert.ToInt32(reader["dept_id"]),
                            DeptName = reader["dept_name"].ToString(),
                            FacultyId = Convert.ToInt32(reader["faculty_id"]),
                            TotalStudents = Convert.ToInt32(reader["total_students"])
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/dw/Dashboard/GetHostelBusStudents/{faculty_id}")]
        public async Task<HttpResponseMessage> GetHostelBusStudents(int faculty_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"
                    SELECT H.category, H.total_students
                    FROM HostelBusStudents H
                    LEFT JOIN Faculties F ON H.dept_id = F.faculty_id
                    WHERE F.faculty_id = @FacultyId";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@FacultyId", faculty_id);

                    SqlDataReader reader = command.ExecuteReader();

                    List<StudentsBycategory> results = new List<StudentsBycategory>();
                    while (reader.Read())
                    {
                        var data = new StudentsBycategory
                        {
                            Category = reader["category"].ToString(),
                            TotalStudents = Convert.ToInt32(reader["total_students"])
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/dw/Dashboard/getDegrees/{dept_id}")]
        public async Task<HttpResponseMessage> getDegrees(int dept_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT degree_id, degree_name, dept_id FROM Degrees WHERE dept_id = @DeptId";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@DeptId", dept_id);

                    SqlDataReader reader = command.ExecuteReader();

                    List<FacultyModel> results = new List<FacultyModel>();
                    while (reader.Read())
                    {
                        var data = new FacultyModel
                        {
                            CampusId = Convert.ToInt32(reader["degree_id"]),
                            FacultyName = reader["degree_name"].ToString(),
                            FacultyId = Convert.ToInt32(reader["dept_id"])
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/dw/Dashboard/GetStudentsInDegree/{dept_id}")]
        public async Task<HttpResponseMessage> GetStudentsInDegree(int dept_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"
 SELECT S.total_students, D.degree_name
 FROM Degrees AS D 
 Left JOIN Students AS S ON S.degree_id = D.degree_id 
 WHERE D.dept_id = @DeptId
                    ";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@DeptId", dept_id);

                    SqlDataReader reader = command.ExecuteReader();

                    List<StudentsBycategory> results = new List<StudentsBycategory>();
                    while (reader.Read())
                    {
                        var data = new StudentsBycategory
                        {
                            Category = reader["degree_name"].ToString(),

                            TotalStudents = reader["total_students"] != DBNull.Value ? Convert.ToInt32(reader["total_students"]) : 0

                    };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/dw/Dashboard/GetCeasedStudentsInDegree/{dept_id}")]
        public async Task<HttpResponseMessage> GetCeasedStudentsInDegree(int dept_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"
 SELECT S.total_students, D.degree_name
 FROM Degrees AS D 
 Left JOIN CeasedStudents AS S ON S.degree_id = D.degree_id 
 WHERE D.dept_id = @DeptId
                    ";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@DeptId", dept_id);

                    SqlDataReader reader = command.ExecuteReader();

                    List<StudentsBycategory> results = new List<StudentsBycategory>();
                    while (reader.Read())
                    {
                        var data = new StudentsBycategory
                        {
                            Category = reader["degree_name"].ToString(),
                            TotalStudents = reader["total_students"] != DBNull.Value ? Convert.ToInt32(reader["total_students"]) : 0
                        };
                        results.Add(data);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}
