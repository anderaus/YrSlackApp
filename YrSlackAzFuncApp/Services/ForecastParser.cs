using System.Collections.Generic;
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

            slackMessage.AppendLine(BuildTemperatureText(intervals));
            slackMessage.AppendLine(BuildPrecipitationText(intervals));
            slackMessage.AppendLine(BuildWindText(intervals));

            //slackMessage.AppendLine("```");
            //slackMessage.AppendLine("debug info:");
            //foreach (var interval in intervals)
            //{
            //    slackMessage.AppendLine($"Start: {interval.Start}\tEnd: {interval.End}\tTemperature: {interval.Temperature.Value}\tWind: {interval.Wind.Speed}\tRain: {interval.Precipitation.Value}");
            //}
            //slackMessage.AppendLine("```");

            return slackMessage.ToString().Trim();
        }

        private string BuildTemperatureText(IReadOnlyCollection<WeatherInterval> intervals)
        {
            var highestTemp = intervals.OrderByDescending(i => i.Temperature.Value).First();
            var lowestTemp = intervals.OrderBy(i => i.Temperature.Value).First();
            return
                lowestTemp.Start < highestTemp.Start
                    ? $":thermometer: Temperaturen svinger mellom {lowestTemp.Temperature.Value.ToString(_nbNo)}° kl. {lowestTemp.Start.Hour:00} og {highestTemp.Temperature.Value.ToString(_nbNo)}° kl. {highestTemp.Start.Hour:00}."
                    : $":thermometer: Temperaturen svinger mellom {highestTemp.Temperature.Value.ToString(_nbNo)}° kl. {highestTemp.Start.Hour:00} og {lowestTemp.Temperature.Value.ToString(_nbNo)}° kl. {lowestTemp.Start.Hour:00}.";

        }

        private string BuildWindText(IReadOnlyCollection<WeatherInterval> intervals)
        {
            var mostWind = intervals.OrderByDescending(i => i.Wind.Speed).First();
            return mostWind.Wind.Speed < 0.01f
                ? ":wind_blowing_face: Det er ikke meldt noe vind!"
                : $":wind_blowing_face: Mest vind mellom kl {mostWind.Start.Hour:00} og {mostWind.End.Hour:00}, med {mostWind.Wind.Speed.ToString(_nbNo)} m/s.";
        }

        private string BuildPrecipitationText(IReadOnlyCollection<WeatherInterval> intervals)
        {
            var mostRain = intervals.OrderByDescending(i => i.Precipitation.Value).First();
            return !mostRain.Precipitation.Value.HasValue || mostRain.Precipitation.Value < 0.01f
                ? ":rain_cloud: Det er ikke meldt noe nedbør! :smiley:"
                : $":rain_cloud: Mest nedbør mellom kl {mostRain.Start.Hour:00} og {mostRain.End.Hour:00}, " +
                  $"med {mostRain.Precipitation.Value} mm. Totalt {intervals.Sum(i => i.Precipitation.Value)?.ToString(_nbNo)} mm.";
        }
    }
}