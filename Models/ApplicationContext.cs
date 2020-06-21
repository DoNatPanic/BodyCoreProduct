using Microsoft.EntityFrameworkCore;

namespace BodyCore.Models
{
	public class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public ApplicationContext( DbContextOptions<ApplicationContext> options )
			: base(options)
		{
			Database.EnsureCreated();
		}
	}
}
