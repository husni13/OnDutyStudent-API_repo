using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_dezurni_ucenik.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [ForeignKey("SchoolClass")]
        public int ClassId { get; set; }
        public virtual SchoolClass SchoolClass { get; set; }
    }
}
