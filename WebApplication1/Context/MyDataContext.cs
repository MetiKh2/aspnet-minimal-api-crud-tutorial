using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Context
{
	public class MyDataContext:DbContext
	{
		public MyDataContext(DbContextOptions<MyDataContext> options):base(options)
		{

		}

		public DbSet<Student> Students => Set<Student>();
    }
}
