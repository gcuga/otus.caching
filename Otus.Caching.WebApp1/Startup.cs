using System;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Otus.Caching.WebApp1.Controllers;

namespace Otus.Caching.WebApp1
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddHttpClient<WeatherApiClient>();

      services.AddMemoryCache();

      services.AddLazyCache();

      
      services.AddResponseCaching();
      
      // services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost"; });

      services.AddDbContext<ApplicationContext>(options =>
        options.UseNpgsql("Host=localhost;Database=my_db;Username=postgres;Password=mysecretpassword"));

      services.AddEFSecondLevelCache(options =>
        options.UseMemoryCacheProvider(CacheExpirationMode.Sliding, TimeSpan.FromSeconds(10))
      );

      services.AddScoped<IRepository<User>, UserRepository>();
      services.Decorate<IRepository<User>, CachingRepository<User>>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationContext db)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseRouting();

      
      app.UseResponseCaching();

      app.Use(async (context, next) =>
      {
        context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
          Public = true,
        };
      
        await next();
      });

      app.UseAuthorization();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

      if (!db.Users.Any())
      {
        db.Users.AddRange(
          new User {Name = "Tom", Email = "tom@gmail.com", Age = 35},
          new User {Name = "Alice", Email = "alice@yahoo.com", Age = 29},
          new User {Name = "Sam", Email = "sam@online.com", Age = 37}
        );
        db.SaveChanges();
      }
    }
  }
}