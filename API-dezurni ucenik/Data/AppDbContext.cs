using API_dezurni_ucenik.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace API_dezurni_ucenik.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<SchoolClass> schoolClasses { get; set; }

        public DbSet<Student> students { get; set; }

        public DbSet<Teacher> teachers { get; set; }
    }

}