using System.Globalization;
using System.Linq;
using System.Text;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp.Services
{
    public class ForecastParser
    {
        private readonly CultureInfo _nbNo = new CultureInfo("nb-NO");

        public string CreateSlackMessage(string textualForecast, YrForecast forecast)
        {
            var slackMessage = new StringBuilder();

            slackMessage.AppendLine(textualForecast);

            var intervals = forecast.ShortIntervals.TakeWhile(i => i.End.Hour != 0).ToList();

            var highestTemp = intervals.OrderByDescending(i => i.Temperature.Value).First();
            var lowestTemp = intervals.OrderBy(i => i.Temperature.Value).First();
            slackMessage.AppendLine(
                lowestTemp.Start < highestTemp.Start
                    ? $":thermometer: Temperaturen svinger mellom {lowestTemp.Temperature.Value.ToString(_nbNo)}° kl. {lowestTemp.Start.Hour:00} og {highestTemp.Temperature.Value.ToString(_nbNo)}° kl. {highestTemp.Start.Hour:00}."
                    : $":thermometer: Temperaturen svinger mellom {highestTemp.Temperature.Value.ToString(_nbNo)}° kl. {highestTemp.Start.Hour:00} og {lowestTemp.Temperature.Value.ToString(_nbNo)}° kl. {lowestTemp.Start.Hour:00}.");

            var mostRain = intervals.OrderByDescending(i => i.Precipitation.Value).First();
            if (!mostRain.Precipitation.Value.HasValue || mostRain.Precipitation.Value < 0.01f)
            {
                slackMessage.AppendLine(":rain_cloud: Det er ikke meldt noe regn! :smiley:");
            }
            else
            {
                slackMessage.AppendLine($":rain_cloud: Mest regn mellom kl {mostRain.Start.Hour:00} og {mostRain.End.Hour:00}, med {mostRain.Precipitation.Value} mm. Totalt {intervals.Sum(i => i.Precipitation.Value)?.ToString(_nbNo)} mm.");
            }

            var mostWind = intervals.OrderByDescending(i => i.Wind.Speed).First();
            if (mostWind.Wind.Speed < 0.01f)
            {
                slackMessage.AppendLine(":wind_blowing_face: Det er ikke meldt noe vind!");
            }
            else
            {
                slackMessage.Append($":wind_blowing_face: Mest vind mellom kl {mostWind.Start.Hour:00} og {mostWind.End.Hour:00}, med {mostWind.Wind.Speed.ToString(_nbNo)} m/s.");
            }

            //slackMessage.AppendLine("```");
            //slackMessage.AppendLine("debug info:");
            //foreach (var interval in intervals)
            //{
            //    slackMessage.AppendLine($"Start: {interval.Start}\tEnd: {interval.End}\tTemperature: {interval.Temperature.Value}\tWind: {interval.Wind.Speed}\tRain: {interval.Precipitation.Value}");
            //}
            //slackMessage.AppendLine("```");

            return slackMessage.ToString();
        }
    }
}