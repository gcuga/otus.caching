using Microsoft.EntityFrameworkCore;

namespace Otus.Caching.WebApp1
{
  public class ApplicationContext : DbContext
  {
    public DbSet<User> Users { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
      : base(options)
    {
      Database.EnsureCreated();
    }
  }

  public class User
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
  }
}