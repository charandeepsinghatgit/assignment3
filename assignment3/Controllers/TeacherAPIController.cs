using assignment3.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace assignment3.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        SchoolDbContext School = new SchoolDbContext();
        [HttpGet(template: "getTeacherList")]
        public List<Teacher> GetTeacherInfo()
        {
            //Store it to a list
            List<Teacher> Example = new List<Teacher>();

            using (MySqlConnection Connection = School.AccessDatabase())
            {

                Connection.Open();

                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "select * from teachers";

                using(MySqlDataReader ReadResult = Command.ExecuteReader())

                while (ReadResult.Read())
                {
                    Teacher Teacher = new Teacher();
                    Teacher.TeacherId = Convert.ToInt32(ReadResult["teacherid"]);
                    Teacher.TeacherFName = ReadResult["teacherfname"].ToString();
                    Teacher.TeacherLName = ReadResult["teacherlname"].ToString();
                    Example.Add(Teacher);

                }
                
            }
            return Example;
        }
    }
}
