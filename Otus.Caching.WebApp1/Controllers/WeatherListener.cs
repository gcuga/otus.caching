using System;
using Microsoft.Extensions.Primitives;

namespace Otus.Caching.WebApp1.Controllers
{
  public interface IWeatherService
  {
    event Action<string> WeatherChanged;
  }

  public class WeatherListener : IChangeToken
  {
    private readonly IWeatherService _weatherService;

    public WeatherListener(IWeatherService weatherService)
    {
      _weatherService = weatherService;
    }

    public IDisposable RegisterChangeCallback(Action<object> callback, object state)
    {
      var callbackInvoker = new CallbackInvoker(callback, state, _weatherService, this);
      return callbackInvoker;
    }

    public bool HasChanged { get; private set; }
    public bool ActiveChangeCallbacks => true;

    public class CallbackInvoker : IDisposable
    {
      private readonly Action<object> _callback;
      private readonly object _state;
      private readonly IWeatherService _weatherService;
      private readonly WeatherListener _weatherListener;

      public CallbackInvoker(Action<object> callback, object state, IWeatherService weatherService,
        WeatherListener weatherListener)
      {
        _callback = callback;
        _state = state;
        _weatherService = weatherService;
        _weatherListener = weatherListener;

        _weatherService.WeatherChanged += OnWeatherServiceOnWeatherChanged;
      }

      public void Dispose()
      {
        _weatherService.WeatherChanged -= OnWeatherServiceOnWeatherChanged;
      }

      private void OnWeatherServiceOnWeatherChanged(string _)
      {
        _callback(_state);
        _weatherListener.HasChanged = true;
      }
    }
  }
}