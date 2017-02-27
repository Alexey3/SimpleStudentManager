using Microsoft.EntityFrameworkCore;

namespace SimpleStudentManager.Models
{
    public class Database : DbContext
    {
		public Database(DbContextOptions<Database> options)
			: base(options)
		{}

		public DbSet<Student> Students { get; set; }
    }
}
