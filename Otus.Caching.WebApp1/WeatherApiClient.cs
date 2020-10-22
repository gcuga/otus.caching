using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Otus.Caching.WebApp1
{
  public class WeatherApiClient
  {
    private readonly HttpClient _httpClient;

    public WeatherApiClient(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public Task<List<WeatherForecast>> GetWeatherForecast()
    {
      return _httpClient
        .GetFromJsonAsync<List<WeatherForecast>>("http://localhost:5002/weatherforecast");
    }
  }
}