


using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace avalonia_dz_templates.Models
{
    public class ForecastResponse
    {
        [JsonPropertyName("list")] public List<ForecastItem> List { get; set; } = new();
    }

    public class ForecastItem
    {
        [JsonPropertyName("dt")] public long Dt { get; set; }
        
        [JsonPropertyName("main")] public ForecastMain Main { get; set; }
        
        [JsonPropertyName("weather")] public List<ForecastWeather> Weather { get; set; }
    }

    public class ForecastMain
    {
        [JsonPropertyName("temp")] public double Temp { get; set; }
    }

    public class ForecastWeather
    {
        [JsonPropertyName("main")] public string Main { get; set; }
        
        [JsonPropertyName("description")] public string Description { get; set; }
    }
    
    
    
}