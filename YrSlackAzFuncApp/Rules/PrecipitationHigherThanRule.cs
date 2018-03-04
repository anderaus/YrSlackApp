using System.Collections.Generic;
using System.Linq;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp.Rules
{
    public class PrecipitationHigherThanRule : IForecastRule
    {
        private readonly float _precipitationHigherThan;
        private readonly string _result;

        public PrecipitationHigherThanRule(float precipitationHigherThan, string result)
        {
            _precipitationHigherThan = precipitationHigherThan;
            _result = result;
        }

        public string FindRuleResult(IReadOnlyCollection<WeatherInterval> intervals)
        {
            if (intervals == null || intervals.Count == 0) return string.Empty;

            var highestPrecipitation = intervals.OrderByDescending(i => i.Precipitation.Value)
                .First().Precipitation.Value;

            return highestPrecipitation > _precipitationHigherThan ? _result : string.Empty;
        }
    }
}