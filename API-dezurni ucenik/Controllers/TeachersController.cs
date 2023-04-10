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
    public class TeachersController : ControllerBase
    {

        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAllTeachers")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<Teacher>> GetTeachers()
        {
            List<Teacher> teachers = new List<Teacher>();

            teachers = _context.teachers.ToList();

            return Ok(teachers);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult<Teacher> GetTeacher(int id)
        {
            var teacher = new
            {
                id = _context.teachers.Find(id).Id,
                name = _context.teachers.Find(id).FullName,
                Class = _context.teachers.Find(id).ClassId
            };

            return Ok(teacher);
        }

        [HttpPost]
        [Route("AddNewTeacher")]
        public async Task<ActionResult<string>> AddTeacher(Teacher teacherEntity)
        {
            if (teacherEntity == null)
            {
                return "Error";
            }
            else
            {
                var newTeacher = new Teacher
                {
                    FullName = teacherEntity.FullName,
                    ClassId = teacherEntity.ClassId,
                };

                _context.teachers.Add(newTeacher);
                await _context.SaveChangesAsync();

                return "Teacher added successfully";
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateTeacher(int id,Teacher teacherUpdated)
        {
            if (id != teacherUpdated.Id)
            {
                return "Error";
            }

            _context.Entry(teacherUpdated).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return "Teacher updated";
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteTeacher(int id)
        {
            if (id <= 0)
            {
                return "Error";
            }

            var deleteTeacher = await _context.teachers.FindAsync(id);

            if (deleteTeacher == null)
            {
                return "Error";
            }
            else
            {
                _context.teachers.Remove(deleteTeacher);
                await _context.SaveChangesAsync();

                return "Teacher deleted";
            }
        }

        [HttpGet]
        [Route("SearchTeachers/{teacherName}")]
        public async Task<IActionResult> SearchTeachers(string teacherName)
        {
            List<Teacher> searchResult = new List<Teacher>();

            searchResult = _context.teachers.Where(teacher => teacher.FullName.Contains(teacherName)).ToList();

            if (teacherName != null && teacherName.Length > 0 && searchResult.Count > 0)
            {
                return Ok(searchResult);
            }
            else
            {
                return BadRequest("Error");
            }
        }
    }
}
