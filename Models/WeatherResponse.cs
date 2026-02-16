

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace avalonia_dz_templates.Models
{
    
    public class WeatherResponse
    {
        [JsonPropertyName("weather")] public List<WeatherInfo> Weather { get; set; } = new();
    
        [JsonPropertyName("main")] public MainInfo Main { get; set; } = new();
        
        [JsonPropertyName("name")] public string Name { get; set; }
        
        [JsonPropertyName("timezone")] public int Timezone { get; set; }
    }

    public class WeatherInfo
    {
        [JsonPropertyName("description")] public string Description { get; set; } = "";

        [JsonPropertyName("main")] public string Main { get; set; } = "";

    }

    public class MainInfo
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
        
        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; }
        
        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; }
    }
}

