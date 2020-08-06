using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Otus.Caching.WebApp1.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class DistributedCacheController : ControllerBase
  {
    private readonly WeatherApiClient _weatherApiClient;
    private readonly ILogger<ResponseCacheController> _logger;
    private readonly IDistributedCache _cache;

    public DistributedCacheController(WeatherApiClient weatherApiClient, ILogger<ResponseCacheController> logger,
      IDistributedCache cache)
    {
      _weatherApiClient = weatherApiClient;
      _logger = logger;
      _cache = cache;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
      const string cacheKey = "getWeatherForecast";

      var weatherForecastBytes = await _cache.GetAsync(cacheKey);

      if (weatherForecastBytes != null)
      {
        _logger.LogInformation("Return result from cache");
        return JsonSerializer.Deserialize<List<WeatherForecast>>(weatherForecastBytes);
      }

      var weatherForecast = await _weatherApiClient.GetWeatherForecast();

      var cacheExpirationOptions = new DistributedCacheEntryOptions
      {
        AbsoluteExpiration = DateTime.Now.AddSeconds(10),
      };
      
      weatherForecastBytes = JsonSerializer.SerializeToUtf8Bytes(weatherForecast);
      await _cache.SetAsync(cacheKey, weatherForecastBytes, cacheExpirationOptions);

      _logger.LogInformation("Loaded {Count} items", weatherForecast.Count);

      return weatherForecast;
    }
  }
}