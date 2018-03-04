using System;
using System.Collections.Generic;
using Xunit;
using YrSlackAzFuncApp.Models;
using YrSlackAzFuncApp.Services;

namespace YrSlackAzFuncApp.Tests
{
    public class ForecastParserPrecipitationTests
    {
        [Fact]
        public void CreateSlackMessage_WhenNoPrecipitation_ShouldUseDryCloudEmoji()
        {
            var text = new ForecastParser().CreateSlackMessage(
                string.Empty,
                CreateForecastWithPrecipitation(0));

            Assert.Equal(@":thermometer: Temperaturen svinger mellom 10° kl. 14 og 10° kl. 14.
:cloud: Det er ikke meldt noe nedbør! :smiley:
:wind_blowing_face: Mest vind mellom kl 14 og 14, med 4 m/s.",
                text);
        }

        [Fact]
        public void CreateSlackMessage_WhenPrecipitation_ShouldUseRainCloudEmoji()
        {
            var text = new ForecastParser().CreateSlackMessage(
                string.Empty,
                CreateForecastWithPrecipitation(3));

            Assert.Equal(@":thermometer: Temperaturen svinger mellom 10° kl. 14 og 10° kl. 14.
:rain_cloud: Mest nedbør mellom kl 14 og 14, med 3 mm. Totalt 3 mm.
:wind_blowing_face: Mest vind mellom kl 14 og 14, med 4 m/s.",
                text);
        }

        private static YrForecast CreateForecastWithPrecipitation(float precipitationValue)
        {
            return new YrForecast
            {
                ShortIntervals = new List<WeatherInterval>
                {
                    new WeatherInterval
                    {
                        Start = new DateTime(2018, 3, 4, 14, 0, 0),
                        End = new DateTime(2018, 3, 4, 14, 30, 0),
                        Precipitation = new PrecipitationData
                        {
                            Value = precipitationValue
                        },
                        Temperature = new TemperatureData
                        {
                            Value = 10
                        },
                        Wind = new WindData
                        {
                            Speed = 4f
                        }
                    }
                }
            };
        }
    }
}