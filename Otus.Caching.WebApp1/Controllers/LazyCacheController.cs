﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Otus.Caching.WebApp1.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class LazyCacheController : ControllerBase
  {
    private readonly IAppCache _cache;
    private readonly ILogger<MemoryCacheController> _logger;
    private readonly WeatherApiClient _weatherApiClient;

    public LazyCacheController(
      IAppCache cache,
      WeatherApiClient weatherApiClient,
      ILogger<MemoryCacheController> logger)
    {
      _cache = cache;
      _weatherApiClient = weatherApiClient;
      _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
      return await _cache.GetOrAddAsync("getWeatherForecast", PopulateCache, DateTimeOffset.Now.AddSeconds(10));
    }

    private async Task<List<WeatherForecast>> PopulateCache()
    {
      var weatherForecast = await _weatherApiClient.GetWeatherForecast();
      _logger.LogInformation("Loaded {Count} items", weatherForecast.Count);

      return weatherForecast;
    }
  }
}