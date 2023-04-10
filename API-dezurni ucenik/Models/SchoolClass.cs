using System.ComponentModel.DataAnnotations;

namespace API_dezurni_ucenik.Models
{
    public class SchoolClass
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
