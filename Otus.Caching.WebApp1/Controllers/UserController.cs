using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFCoreSecondLevelCacheInterceptor;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Otus.Caching.WebApp1.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly ApplicationContext _db;
    private readonly IRepository<User> _repository;

    public UserController(ApplicationContext db, IRepository<User> repository)
    {
      _db = db;
      _repository = repository;
    }

    public Task<List<User>> Get()
    {
      var listAsync = _db
        .Users
        .Cacheable()
        .ToListAsync();
      
      return listAsync;
    }

    public async Task<User> Post(User user)
    {
      _db.Users.Add(user);
      await _db.SaveChangesAsync();

      return user;
    }

    [HttpGet("{id}")]
    public Task<User> Get(int id)
    {
      return _repository.Get(id);
    }
  }

  public interface IRepository<T>
  {
    Task<T> Get(int id);
  }

  public class UserRepository : IRepository<User>
  {
    private readonly ApplicationContext _db;

    public UserRepository(ApplicationContext db)
    {
      _db = db;
    }

    public Task<User> Get(int id)
    {
      return _db.Users.FirstOrDefaultAsync(x => x.Id == id);
    }
  }

  public class CachingRepository<T> : IRepository<T>
  {
    private readonly IAppCache _cache;
    private readonly IRepository<T> _repository;

    public CachingRepository(IRepository<T> repository, IAppCache cache)
    {
      _repository = repository;
      _cache = cache;
    }

    public async Task<T> Get(int id)
    {
      return await _cache.GetOrAddAsync(typeof(T).Name + id, () => _repository.Get(id));
    }
  }
}