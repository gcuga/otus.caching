using System.Collections.Generic;
using System.Threading.Tasks;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Otus.Caching.WebApp1.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly ApplicationContext _db;

    public UserController(ApplicationContext db)
    {
      _db = db;
    }

    public Task<List<User>> Get()
    {
      return _db.Users.Cacheable().ToListAsync();
    }
  }
}