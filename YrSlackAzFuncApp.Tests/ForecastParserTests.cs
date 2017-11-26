using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using YrSlackAzFuncApp.Models;
using YrSlackAzFuncApp.Services;

namespace YrSlackAzFuncApp.Tests
{
    public class ForecastParserTests
    {
        [Fact]
        public void CreateSlackMessage_WhenIsCloudyAndRainy_ShouldReturnProperText()
        {
            const string textualForecast = "Det blir delvis skyet på morgenen og i kveld, regn i ettermiddag.";
            var forecast = ParseIntervals(
                @"Start: 11/26/2017 7:00:00 AM    End: 11/26/2017 8:00:00 AM    Temperature: 2.5    Wind: 5    Rain: 0.5
Start: 11/26/2017 8:00:00 AM    End: 11/26/2017 9:00:00 AM    Temperature: 2.2    Wind: 2.8    Rain: 0
Start: 11/26/2017 9:00:00 AM    End: 11/26/2017 10:00:00 AM    Temperature: 2.2    Wind: 2.1    Rain: 0
Start: 11/26/2017 10:00:00 AM    End: 11/26/2017 11:00:00 AM    Temperature: 2.7    Wind: 2    Rain: 0
Start: 11/26/2017 11:00:00 AM    End: 11/26/2017 12:00:00 PM    Temperature: 2.7    Wind: 2.5    Rain: 0
Start: 11/26/2017 12:00:00 PM    End: 11/26/2017 1:00:00 PM    Temperature: 2.5    Wind: 2.2    Rain: 0.5
Start: 11/26/2017 1:00:00 PM    End: 11/26/2017 2:00:00 PM    Temperature: 2.5    Wind: 1.5    Rain: 0.9
Start: 11/26/2017 2:00:00 PM    End: 11/26/2017 3:00:00 PM    Temperature: 2    Wind: 2.7    Rain: 0.6
Start: 11/26/2017 3:00:00 PM    End: 11/26/2017 4:00:00 PM    Temperature: 1.8    Wind: 3.2    Rain: 0.3
Start: 11/26/2017 4:00:00 PM    End: 11/26/2017 5:00:00 PM    Temperature: 1.8    Wind: 3.2    Rain: 0
Start: 11/26/2017 5:00:00 PM    End: 11/26/2017 6:00:00 PM    Temperature: 1.8    Wind: 2.9    Rain: 0
Start: 11/26/2017 6:00:00 PM    End: 11/26/2017 7:00:00 PM    Temperature: 1.7    Wind: 2.6    Rain: 0
Start: 11/26/2017 7:00:00 PM    End: 11/26/2017 8:00:00 PM    Temperature: 1.5    Wind: 2.8    Rain: 0
Start: 11/26/2017 8:00:00 PM    End: 11/26/2017 9:00:00 PM    Temperature: 1.1    Wind: 2.1    Rain: 0
Start: 11/26/2017 9:00:00 PM    End: 11/26/2017 10:00:00 PM    Temperature: 0.7    Wind: 1.4    Rain: 0
Start: 11/26/2017 10:00:00 PM    End: 11/26/2017 11:00:00 PM    Temperature: 1.2    Wind: 0.5    Rain: 0");

            var result = new ForecastParser().CreateSlackMessage(textualForecast, forecast);

            Assert.Equal(@"Det blir delvis skyet på morgenen og i kveld, regn i ettermiddag.
:thermometer: Temperaturen svinger mellom 0,7° kl. 21 og 2,7° kl. 10.
:rain_cloud: Mest regn mellom kl 13 og 14, med 0,9 mm. Totalt 2,8 mm.
:wind_blowing_face: Mest vind mellom kl 07 og 08, med 5 m/s.",
                    result);
        }

        [Fact]
        public void CreateSlackMessage_WhenTemperatureTurnsNegative_ShouldReturnProperText()
        {
            const string textualForecast = "Det blir delvis skyet resten av dagen.";
            var forecast = ParseIntervals(
                @"Start: 11/25/2017 3:00:00 PM    End: 11/25/2017 4:00:00 PM    Temperature: 2.2    Wind: 2    Rain: 0
Start: 11/25/2017 4:00:00 PM    End: 11/25/2017 5:00:00 PM    Temperature: 1.3    Wind: 2    Rain: 0
Start: 11/25/2017 5:00:00 PM    End: 11/25/2017 6:00:00 PM    Temperature: 1.1    Wind: 3.2    Rain: 0
Start: 11/25/2017 6:00:00 PM    End: 11/25/2017 7:00:00 PM    Temperature: 0.9    Wind: 3.2    Rain: 0
Start: 11/25/2017 7:00:00 PM    End: 11/25/2017 8:00:00 PM    Temperature: 0.4    Wind: 2.8    Rain: 0
Start: 11/25/2017 8:00:00 PM    End: 11/25/2017 9:00:00 PM    Temperature: 0.1    Wind: 2.5    Rain: 0
Start: 11/25/2017 9:00:00 PM    End: 11/25/2017 10:00:00 PM    Temperature: 0    Wind: 2.8    Rain: 0
Start: 11/25/2017 10:00:00 PM    End: 11/25/2017 11:00:00 PM    Temperature: -0.2    Wind: 3.2    Rain: 0");

            var result = new ForecastParser().CreateSlackMessage(textualForecast, forecast);

            Assert.Equal(@"Det blir delvis skyet resten av dagen.
:thermometer: Temperaturen svinger mellom -0,2° kl. 22 og 2,2° kl. 15.
:rain_cloud: Det er ikke meldt noe regn! :smiley:
:wind_blowing_face: Mest vind mellom kl 17 og 18, med 3,2 m/s.",
                    result);
        }

        private static YrForecast ParseIntervals(string input)
        {
            var shortIntervals = new List<WeatherInterval>();

            foreach (var weatherInterval in input.Split("Start: "))
            {
                if (weatherInterval.Trim().Length == 0) continue;

                var intervalParts = weatherInterval.Split("    ");
                shortIntervals.Add(new WeatherInterval
                {
                    Start = DateTime.Parse(intervalParts[0].Substring(0), new CultureInfo("en-US")),
                    End = DateTime.Parse(intervalParts[1].Substring(5), new CultureInfo("en-US")),
                    Temperature = new TemperatureData
                    {
                        Value = float.Parse(intervalParts[2].Substring(13).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                    },
                    Wind = new WindData
                    {
                        Speed = float.Parse(intervalParts[3].Substring(6).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                    },
                    Precipitation = new PrecipitationData
                    {
                        Value = float.Parse(intervalParts[4].Substring(6).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                    }
                });
            }

            return new YrForecast
            {
                ShortIntervals = shortIntervals
            };
        }
    }
}