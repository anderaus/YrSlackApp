using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp
{
    public static class DailyMorningForecast
    {
        private static readonly string SlackWebhookUrl = ConfigurationManager.AppSettings["SlackWebhookUrl"];
        private const string YrTextualForecastUrl = "http://www.yr.no/api/v0/locations/1-72837/forecast/autotext";

        private const string Every10Seconds = "*/10 * * * * *";     // For testing
        private const string EveryMorningAt6 = "0 0 6,12 * * *";       // For prod use

        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("DailyMorningForecast")]
        public static async Task Run([TimerTrigger(EveryMorningAt6)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"DailyMorningForecast function executed at: {DateTime.UtcNow}");

            var textualForecast =
                JsonConvert.DeserializeObject<YrForecastText>(await HttpClient.GetStringAsync(YrTextualForecastUrl));
            log.Info($"DailyMorningForecast got forecast: {textualForecast.Text}");

            var response = await HttpClient.PostAsJsonAsync(SlackWebhookUrl,
                 new SlackMessage
                 {
                     Text = textualForecast.Text
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