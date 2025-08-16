using assignment3.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace assignment3.Controllers
{
    // This is an MVC Controller for rendering web pages (not an API controller!)
    public class TeacherPageController : Controller
    {
        // Instantiate the SchoolDbContext for database connection
        private readonly SchoolDbContext _school = new SchoolDbContext();

        // ACTION: List all teachers and show them in a web page
        // URL: /TeacherPage/List
        public IActionResult List()
        {
            // Create a list to hold all teachers fetched from the database
            var teachers = new List<Teacher>();

            // Open the database connection
            using (MySqlConnection conn = _school.AccessDatabase())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                // SQL command to select all rows from the teachers table
                cmd.CommandText = "SELECT * FROM teachers";

                // Execute the query and get the results
                using (var reader = cmd.ExecuteReader())
                {
                    // Loop through each result and create a Teacher object
                    while (reader.Read())
                    {
                        teachers.Add(new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        });
                    }
                }
            }
            // Pass the list of teachers to the List view (Views/TeacherPage/List.cshtml)
            return View(teachers);
        }

        // ACTION: Show details for a single teacher
        // URL: /TeacherPage/Show/(id)
        public IActionResult Show(int id)
        {
            Teacher teacher = null;

            // Open the database connection
            using (MySqlConnection conn = _school.AccessDatabase())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                cmd.Parameters.AddWithValue("@id", id);

                // Execute the query and read the result (should be only one row)
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        teacher = new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        };
                    }
                }
            }
        }
    }
}
