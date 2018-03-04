using System.Collections.Generic;
using YrSlackAzFuncApp.Models;
using YrSlackAzFuncApp.Rules;

namespace YrSlackAzFuncApp.Finders
{
    public class PrecipitationEmojiFinder
    {
        private readonly List<IForecastRule> _rules = new List<IForecastRule>();

        public PrecipitationEmojiFinder()
        {
            _rules.Add(new PrecipitationHigherThanAndTemperatureLowerThanRule(0.01f, 0f, ":snow_cloud:"));
            _rules.Add(new PrecipitationHigherThanRule(0.01f, ":rain_cloud:"));
            _rules.Add(new DefaultRule(":cloud:"));
        }

        public string FindPrecipitationEmoji(IReadOnlyCollection<WeatherInterval> intervals)
        {
            var emoji = string.Empty;

            foreach (var rule in _rules)
            {
                emoji = rule.FindRuleResult(intervals);
                if (!string.IsNullOrWhiteSpace(emoji)) break;
            }

            return emoji;
        }
    }
}