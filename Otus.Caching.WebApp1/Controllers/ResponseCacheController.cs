using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Otus.Caching.WebApp1.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ResponseCacheController : ControllerBase
  {
    private readonly WeatherApiClient _weatherApiClient;
    private readonly ILogger<ResponseCacheController> _logger;

    public ResponseCacheController(WeatherApiClient weatherApiClient, ILogger<ResponseCacheController> logger)
    {
      _weatherApiClient = weatherApiClient;
      _logger = logger;
    }

    [HttpGet]
    // [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10)]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
      var weatherForecast = await _weatherApiClient.GetWeatherForecast();
      _logger.LogInformation("Loaded {Count} items", weatherForecast.Count);
      return weatherForecast;
    }
  }
}