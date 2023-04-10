using API_dezurni_ucenik.Data;
using API_dezurni_ucenik.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_dezurni_ucenik.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }


        [Route("GetAllStudents")]
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            List<Student> students = new List<Student>();

            students = _context.students.ToList();

            return Ok(students);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult<Student> GetStudent(int id)
        {
            var student = new
            {
                id = _context.students.Find(id).Id,
                name = _context.students.Find(id).FullName,
                Class = _context.students.Find(id).SchoolClass
            };

            return Ok(student);
        }

        [Route("AddStudent")]
        [HttpPost]
        public async Task<ActionResult<string>> AddStudent(Student studentEntity)
        {
            if (studentEntity == null)
            {
                return "Error";
            }
            else
            {
                var newStudent = new Student
                {
                    FullName = studentEntity.FullName,
                    ClassId = studentEntity.ClassId,
                };

                _context.students.Add(newStudent);
                await _context.SaveChangesAsync();

                return "Student added successfully";
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateStudent(int id, Student studentUpdated)
        {
            if (id != studentUpdated.Id)
            {
                return "Error";
            }

            _context.Entry(studentUpdated).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return "Student updated";
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteStudent(int id)
        {
            if(id <= 0)
            {
                return "Error";
            }

            var deleteStudent = await _context.students.FindAsync(id);

            if(deleteStudent == null)
            {
                return "Error";
            }
            else
            {
                _context.students.Remove(deleteStudent);
                await _context.SaveChangesAsync();

                return "Student deleted";
            }
        }

        [HttpGet]
        [Route("SearchStudents/{studentName}")]
        public async Task<IActionResult> SearchStudents(string studentName)
        {
            List<Student> searchResult = new List<Student>();

            searchResult = _context.students.Where(student => student.FullName.Contains(studentName)).ToList();

            if (studentName != null && studentName.Length > 0 && searchResult.Count > 0)
            {
                return Ok(searchResult);
            }
            else
            {
                return BadRequest("No results");
            }
        
        }

    }

}
