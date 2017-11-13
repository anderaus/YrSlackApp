using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace YrSlackAzFuncApp.Tests.Integration
{
    public class ForecastTests
    {
        [Fact]
        public async Task FetchingFullForecast_ShouldIncludeMinMaxTemperatures()
        {
            var forecastService = new YrForecastService();

            var forecast = await forecastService.GetFullForecast();

            Assert.Equal(3, forecast.ShortIntervals.Count());
        }
    }
}