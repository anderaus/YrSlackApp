using Newtonsoft.Json;

namespace YrSlackAzFuncApp.Models
{
    public class SlackMessage
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}