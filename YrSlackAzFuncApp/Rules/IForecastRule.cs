using System.Collections.Generic;
using YrSlackAzFuncApp.Models;

namespace YrSlackAzFuncApp.Rules
{
    public interface IForecastRule
    {
        string FindRuleResult(IReadOnlyCollection<WeatherInterval> intervals);
    }
}