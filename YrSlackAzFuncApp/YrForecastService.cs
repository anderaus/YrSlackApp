using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp
{
    public class YrForecastService : IForecastService
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string YrTextualForecastUrl = "http://www.yr.no/api/v0/locations/1-72837/forecast/autotext";
        private const string YrForecastUrl = "http://www.yr.no/api/v0/locations/1-72837/forecast";

        public async Task<string> GetTextualForecast()
        {
            var forecastJson = await HttpClient.GetStringAsync(YrTextualForecastUrl);
            var textualForecast = JsonConvert.DeserializeObject<YrForecastText>(forecastJson);
            return textualForecast?.Text ?? string.Empty;
        }

        public async Task<YrForecast> GetFullForecast()
        {
            var forecastJson = await HttpClient.GetStringAsync(YrForecastUrl);
            return JsonConvert.DeserializeObject<YrForecast>(forecastJson);
        }
    }

    public interface IForecastService
    {
        Task<string> GetTextualForecast();
        Task<YrForecast> GetFullForecast();
    }
}