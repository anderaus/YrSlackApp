using System.Collections.Generic;
using System.Linq;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp.Rules
{
    internal class PrecipitationHigherThanAndTemperatureLowerThanRule : IForecastRule
    {
        private readonly float _precipitationHigherThan;
        private readonly float _temperatureLowerThan;
        private readonly string _result;

        public PrecipitationHigherThanAndTemperatureLowerThanRule(float precipitationHigherThan, float temperatureLowerThan, string result)
        {
            _precipitationHigherThan = precipitationHigherThan;
            _temperatureLowerThan = temperatureLowerThan;
            _result = result;
        }


        public string FindRuleResult(IReadOnlyCollection<WeatherInterval> intervals)
        {
            if (intervals == null || intervals.Count == 0) return string.Empty;

            var highestPrecipitationInterval = intervals.OrderByDescending(i => i.Precipitation.Value)
                .First();

            if (highestPrecipitationInterval.Precipitation.Value > _precipitationHigherThan
                && highestPrecipitationInterval.Temperature.Value < _temperatureLowerThan)
            {
                return _result;
            }

            return string.Empty;
        }
    }
}