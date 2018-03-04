using System.Collections.Generic;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp.Rules
{
    public class DefaultRule : IForecastRule
    {
        private readonly string _defaultResult;

        public DefaultRule(string defaultResult)
        {
            _defaultResult = defaultResult;
        }

        public string FindRuleResult(IReadOnlyCollection<WeatherInterval> intervals)
        {
            return _defaultResult;
        }
    }
}