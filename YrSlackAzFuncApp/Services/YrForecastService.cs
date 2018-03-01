using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp.Services
{
    public class YrForecastService : IForecastService
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string YrTextualForecastUrl = "http://www.yr.no/api/v0/locations/{0}/forecast/autotext";
        private const string YrForecastUrl = "http://www.yr.no/api/v0/locations/{0}/forecast";

        public async Task<string> GetTextualForecast(string locationId)
        {
            var forecastJson = await HttpClient.GetStringAsync(string.Format(YrTextualForecastUrl, locationId));
            var textualForecast = JsonConvert.DeserializeObject<YrForecastText>(forecastJson);
            return textualForecast?.Text ?? string.Empty;
        }

        public async Task<YrForecast> GetFullForecast(string locationId)
        {
            var forecastJson = await HttpClient.GetStringAsync(string.Format(YrForecastUrl, locationId));
            return JsonConvert.DeserializeObject<YrForecast>(forecastJson);
        }
    }

    public interface IForecastService
    {
        Task<string> GetTextualForecast(string locationId);
        Task<YrForecast> GetFullForecast(string locationId);
    }
}