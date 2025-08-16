using assignment3.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace assignment3.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly SchoolDbContext _school = new SchoolDbContext();

        // List
        public IActionResult List()
        {
            var teachers = new List<Teacher>();
            using (MySqlConnection conn = _school.AccessDatabase())
            {
                conn.Open();
                var cmd = conn.CreateCommand(); cmd.CommandText = "SELECT * FROM teachers";
                using (var reader = cmd.ExecuteReader())
                {
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
            return View(teachers);
        }

        //Show single teacher
        public IActionResult Show(int id)
        {
            Teacher teacher = null;
            using (MySqlConnection conn = _school.AccessDatabase())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                cmd.Parameters.AddWithValue("@id", id);
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
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        // GET: /TeacherPage/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher teacher = null;
            using (MySqlConnection conn = _school.AccessDatabase())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM teachers WHERE teacherid=@id";
                cmd.Parameters.AddWithValue("@id", id); using (var reader = cmd.ExecuteReader())
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

            if (teacher == null) return NotFound();
            return View(teacher); // sends model to Edit.cshtml
        }

        // POST: /TeacherPage/Update/5
        [HttpPost]
        public IActionResult Update(int id, Teacher updatedTeacher)
        {
            try
            {
                using (MySqlConnection conn = _school.AccessDatabase())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"UPDATE teachers 
                                        SET teacherfname=@fname, 
                                            teacherlname=@lname,
                                            employeenumber=@empnum,
                                            hiredate=@hiredate,
                                            salary=@salary
                                        WHERE teacherid=@id";
                    cmd.Parameters.AddWithValue("@fname", updatedTeacher.TeacherFName);
                    cmd.Parameters.AddWithValue("@lname", updatedTeacher.TeacherLName);
                    cmd.Parameters.AddWithValue("@empnum", updatedTeacher.EmployeeNumber);
                    cmd.Parameters.AddWithValue("@hiredate", updatedTeacher.HireDate);
                    cmd.Parameters.AddWithValue("@salary", updatedTeacher.Salary);
                    cmd.Parameters.AddWithValue("@id", id);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        ViewBag.ErrorMessage = "Teacher not found.";
                        return View("Edit", updatedTeacher);
                    }
                }
                return RedirectToAction("Show", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error updating teacher: " + ex.Message;
                return View("Edit", updatedTeacher);
            }
        }
    }
}
