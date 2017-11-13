using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp
{
    public static class FetchAndPostForecast
    {
        private static readonly string SlackWebhookUrl = ConfigurationManager.AppSettings["SlackWebhookUrl"];
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string Every10Seconds = "*/10 * * * * *";     // For testing
        private const string EveryMorningAndAfternoon = "0 0 7,15 * * *";    // For prod use (set key WEBSITE_TIME_ZONE in app settings to "Central Europe Standard Time" to use Norwegian time zone)

        [FunctionName("FetchAndPostForecast")]
        public static async Task Run([TimerTrigger(EveryMorningAndAfternoon)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"FetchAndPostForecast function executed at: {DateTime.UtcNow}");

            var forecastService = new YrForecastService();
            var slackForecast = new StringBuilder();

            var textualForecast = await forecastService.GetTextualForecast();
            log.Info($"FetchAndPostForecast got forecast: {textualForecast}");
            slackForecast.AppendLine(textualForecast);

            var forecast = await forecastService.GetFullForecast();

            var intervals = forecast.ShortIntervals.TakeWhile(i => i.End.Hour != 0).ToList();

            var highestTemp = intervals.OrderByDescending(i => i.Temperature.Value).First();
            var lowestTemp = intervals.OrderBy(i => i.Temperature.Value).First();
            slackForecast.AppendLine(
                $":thermometer: Temperaturen svinger mellom {lowestTemp.Temperature.Value}° kl. {lowestTemp.Start.Hour:00} og {highestTemp.Temperature.Value}° kl. {highestTemp.Start.Hour:00}.");

            var mostRain = intervals.OrderByDescending(i => i.Precipitation.Value).First();
            if (!mostRain.Precipitation.Value.HasValue || mostRain.Precipitation.Value < 0.01f)
            {
                slackForecast.AppendLine(":rain_cloud: Det er ikke meldt noe regn! :smiley:");
            }
            else
            {
                slackForecast.AppendLine($":rain_cloud: Mest regn mellom kl {mostRain.Start.Hour:00} og {mostRain.End.Hour:00}, med {mostRain.Precipitation.Value} mm. Totalt {intervals.Sum(i => i.Precipitation.Value)} mm.");
            }

            var mostWind = intervals.OrderByDescending(i => i.Wind.Speed).First();
            if (mostWind.Wind.Speed < 0.01f)
            {
                slackForecast.AppendLine(":wind_blowing_face: Det er ikke meldt noe vind!");
            }
            else
            {
                slackForecast.AppendLine($":wind_blowing_face: Mest vind mellom kl {mostWind.Start.Hour:00} og {mostWind.End.Hour:00}, med {mostWind.Wind.Speed} m/s.");
            }

            slackForecast.AppendLine("```");
            slackForecast.AppendLine("debug info:");
            foreach (var interval in intervals)
            {
                slackForecast.AppendLine($"Start: {interval.Start}\tEnd: {interval.End}\tTemperature: {interval.Temperature.Value}\tWind: {interval.Wind.Speed}\tRain: {interval.Precipitation.Value}");
            }
            slackForecast.AppendLine("```");

            log.Info(slackForecast.ToString());

            var response = await HttpClient.PostAsJsonAsync(SlackWebhookUrl,
                 new SlackMessage
                 {
                     Text = slackForecast.ToString()
                 });

            if (response.IsSuccessStatusCode)
            {
                log.Info("Message successfully sent to Slack webhook");
            }
            else
            {
                log.Error(response.StatusCode.ToString());
                log.Error(response.ReasonPhrase);
                if (response.Content != null) log.Error(await response.Content.ReadAsStringAsync());
            }
        }
    }
}