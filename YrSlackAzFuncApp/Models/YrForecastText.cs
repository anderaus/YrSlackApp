using Newtonsoft.Json;

namespace YrSlackAzFuncApp.Models
{
    public class YrForecastText
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}