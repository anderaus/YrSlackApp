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
        private static readonly string YrLocationId = Environment.GetEnvironmentVariable("YR_LOCATION_ID");
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("FetchAndPostForecast")]
        public static async Task Run([TimerTrigger("%CRON_EXPRESSION%")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"FetchAndPostForecast function executed at: {DateTime.UtcNow}");

            var forecastService = new YrForecastService();
            var textualForecast = await forecastService.GetTextualForecast(YrLocationId);
            log.Info($"FetchAndPostForecast got forecast: {textualForecast}");

            var fullForecast = await forecastService.GetFullForecast(YrLocationId);

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