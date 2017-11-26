using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using YrSlackAzFuncApp.Models;
using YrSlackAzFuncApp.Services;

namespace YrSlackAzFuncApp
{
    public static class FetchAndPostForecast
    {
        private static readonly string SlackWebhookUrl = Environment.GetEnvironmentVariable("SlackWebhookUrl");
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string Every10Seconds = "*/10 * * * * *";             // For testing
        private const string EveryMorningAndAfternoon = "0 0 7,15 * * *";   // For prod use (set key WEBSITE_TIME_ZONE in app settings to "Central Europe Standard Time" to use Norwegian time zone)

        [FunctionName("FetchAndPostForecast")]
        public static async Task Run([TimerTrigger(EveryMorningAndAfternoon)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"FetchAndPostForecast function executed at: {DateTime.UtcNow}");

            var forecastService = new YrForecastService();
            var textualForecast = await forecastService.GetTextualForecast();
            log.Info($"FetchAndPostForecast got forecast: {textualForecast}");

            var fullForecast = await forecastService.GetFullForecast();

            var slackMessage = new ForecastParser().CreateSlackMessage(textualForecast, fullForecast);
            log.Info($"Sending Slack message:\n{slackMessage}");

            var response = await HttpClient.PostAsJsonAsync(SlackWebhookUrl,
                 new SlackMessage
                 {
                     Text = slackMessage
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