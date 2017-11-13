using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace YrSlackAzFuncApp.Models
{
    /// <summary>
    /// Subset of Swagger specification at: http://www.yr.no/api/swagger/ui/index#!/Forecast/Forecast_Forecast
    /// </summary>
    public class YrForecast
    {
        [JsonProperty(PropertyName = "shortIntervals")]
        public IEnumerable<WeatherInterval> ShortIntervals { get; set; }
        [JsonProperty(PropertyName = "longIntervals")]
        public IEnumerable<WeatherInterval> LongIntervals { get; set; }
    }

    public class WeatherInterval
    {
        [JsonProperty(PropertyName = "precipitation")]
        public PrecipitationData Precipitation { get; set; }
        [JsonProperty(PropertyName = "temperature")]
        public TemperatureData Temperature { get; set; }
        [JsonProperty(PropertyName = "wind")]
        public WindData Wind { get; set; }
        [JsonProperty(PropertyName = "feelsLike")]
        public Feelslike FeelsLike { get; set; }
        [JsonProperty(PropertyName = "humidity")]
        public HumidityData Humidity { get; set; }
        [JsonProperty(PropertyName = "start")]
        public DateTime Start { get; set; }
        [JsonProperty(PropertyName = "end")]
        public DateTime End { get; set; }
    }

    public class PrecipitationData
    {
        [JsonProperty(PropertyName = "min")]
        public float? Min { get; set; }
        [JsonProperty(PropertyName = "max")]
        public float? Max { get; set; }
        [JsonProperty(PropertyName = "value")]
        public float? Value { get; set; }
        [JsonProperty(PropertyName = "pop")]
        public float? Pop { get; set; }
    }

    public class TemperatureData
    {
        [JsonProperty(PropertyName = "value")]
        public float Value { get; set; }
        [JsonProperty(PropertyName = "min")]
        public float? Min { get; set; }
        [JsonProperty(PropertyName = "max")]
        public float? Max { get; set; }
    }

    public class WindData
    {
        [JsonProperty(PropertyName = "direction")]
        public int Direction { get; set; }
        [JsonProperty(PropertyName = "gust")]
        public float? Gust { get; set; }
        [JsonProperty(PropertyName = "speed")]
        public float Speed { get; set; }
    }

    public class Feelslike
    {
        [JsonProperty(PropertyName = "value")]
        public float? Value { get; set; }
    }

    public class HumidityData
    {
        [JsonProperty(PropertyName = "value")]
        public float? Value { get; set; }
    }
}