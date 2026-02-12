

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace avalonia_dz_templates.Models
{
    
    // public class WeatherResponse
    // {
    //     [JsonPropertyName("weather")] public List<WeatherInfo> Weather { get; set; } = new();
    //     
    //     [JsonPropertyName("main")] public MainInfo Main { get; set; } = new();
    //     
    //     [JsonPropertyName("name")] public string Name { get; set; }
    //     
    //     [JsonPropertyName("timezone")] public int Timezone { get; set; }
    //     
    //     [JsonPropertyName("wind")] public WindInfo Wind { get; set; }
    //
    //
    // }
    public class WeatherResponse
    {
        [JsonPropertyName("list")] public List<ListWeather> List { get; set; } = new();
    
    
    }
    
    public class ListWeather
    {
        [JsonPropertyName("weather")] public List<WeatherInfo> Weather { get; set; } = new();
    
        [JsonPropertyName("main")] public MainInfo Main { get; set; } = new();
        
        [JsonPropertyName("name")] public string Name { get; set; }
        
        [JsonPropertyName("timezone")] public int Timezone { get; set; }
        
        [JsonPropertyName("wind")] public WindInfo Wind { get; set; }
        
        [JsonPropertyName("dt_txt")] public long Dt { get; set; }
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
        
        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    public class WindInfo
    {
        [JsonPropertyName("speed")] public int Speed { get; set; }
    }
}

