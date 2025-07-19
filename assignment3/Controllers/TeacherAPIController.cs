using assignment3.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace assignment3.Controllers
{
    // Defines the route prefix for all actions in this controller and marks it as an API controller
    [Route("api/teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        // Instantiate the database context for connecting to MySQL
        SchoolDbContext School = new SchoolDbContext();

        // READ: Get a list of all teachers
        [HttpGet(template: "getTeacherList")]
        public List<Teacher> GetTeacherInfo()
        {
            // Create a list to store teacher objects
            List<Teacher> Example = new List<Teacher>();

            // Open a connection to the database
            using (MySqlConnection Connection = School.AccessDatabase())
            {
                Connection.Open();

                // Create the SQL command to select all teachers
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select * from teachers";

                // Execute the command and read the results
                using (MySqlDataReader ReadResult = Command.ExecuteReader())
                {
                    // Loop through each record
                    while (ReadResult.Read())
                    {
                        // Create a new Teacher object and populate its properties
                        Teacher Teacher = new Teacher();
                        Teacher.TeacherId = Convert.ToInt32(ReadResult["teacherid"]);
                        Teacher.TeacherFName = ReadResult["teacherfname"].ToString();
                        Teacher.TeacherLName = ReadResult["teacherlname"].ToString();
                        // Note: Only basic info returned here (can be expanded to include all columns)
                        Example.Add(Teacher); // Add the teacher to the list
                    }
                }
            }
            return Example; // Return the list of teachers as JSON
        }

        // CREATE: Add a new teacher to the database
        [HttpPost("addTeacher")]
        public ActionResult AddTeacher([FromBody] Teacher newTeacher)
        {
            try
            {
                // Open a database connection
                using (MySqlConnection conn = School.AccessDatabase())
                {
                    conn.Open();
                    // Prepare SQL INSERT statement with parameters to avoid SQL injection
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary)
                                VALUES (@fname, @lname, @empnum, @hiredate, @salary)";
                    // Assign values from the request body to SQL parameters
                    cmd.Parameters.AddWithValue("@fname", newTeacher.TeacherFName);
                    cmd.Parameters.AddWithValue("@lname", newTeacher.TeacherLName);
                    cmd.Parameters.AddWithValue("@empnum", newTeacher.EmployeeNumber);
                    cmd.Parameters.AddWithValue("@hiredate", newTeacher.HireDate);
                    cmd.Parameters.AddWithValue("@salary", newTeacher.Salary);

                    // Execute the INSERT command
                    cmd.ExecuteNonQuery();
                }
                return Ok("Teacher added successfully!"); // Return success response
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(500, "Error adding teacher.");
            }
        }

        // UPDATE: Edit an existing teacher's information
        [HttpPut("updateTeacher/{id}")]
        public ActionResult UpdateTeacher(int id, [FromBody] Teacher updatedTeacher)
        {
            try
            {
                // Open a database connection
                using (MySqlConnection conn = School.AccessDatabase())
                {
                    conn.Open();
                    // Prepare SQL UPDATE statement with parameters
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"UPDATE teachers SET 
                                    teacherfname=@fname, 
                                    teacherlname=@lname,
                                    employeenumber=@empnum,
                                    hiredate=@hiredate,
                                    salary=@salary
                                WHERE teacherid=@id";
                    // Assign updated values from the request body to SQL parameters
                    cmd.Parameters.AddWithValue("@fname", updatedTeacher.TeacherFName);
                    cmd.Parameters.AddWithValue("@lname", updatedTeacher.TeacherLName);
                    cmd.Parameters.AddWithValue("@empnum", updatedTeacher.EmployeeNumber);
                    cmd.Parameters.AddWithValue("@hiredate", updatedTeacher.HireDate);
                    cmd.Parameters.AddWithValue("@salary", updatedTeacher.Salary);
                    cmd.Parameters.AddWithValue("@id", id);

                    // Execute the UPDATE command
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        return Ok("Teacher updated successfully!"); // Success if at least one row affected
                    else
                        return NotFound("Teacher not found."); // No row updated means teacher does not exist
                }
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(500, "Error updating teacher.");
            }
        }

        // DELETE: Remove a teacher from the database
        [HttpDelete("deleteTeacher/{id}")]
        public ActionResult DeleteTeacher(int id)
        {
            try
            {
                // Open a database connection
                using (MySqlConnection conn = School.AccessDatabase())
                {
                    conn.Open();
                    // Prepare SQL DELETE statement with parameter
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM teachers WHERE teacherid=@id";
                    cmd.Parameters.AddWithValue("@id", id);

                    // Execute the DELETE command
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        return Ok("Teacher deleted successfully!"); // Success if at least one row affected
                    else
                        return NotFound("Teacher not found."); // No row deleted means teacher does not exist
                }
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(500, "Error deleting teacher.");
            }
        }
    }
}
