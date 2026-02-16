



using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using avalonia_dz_templates.Models;

namespace avalonia_dz_templates.Services
{
    public class WeatherService
    {
        private const string ApiKey = "2ebad42a39eba021545ba0242709e9d3";
        
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={ApiKey}&units=metric&lang=ua";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<WeatherResponse>(json);
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<ForecastResponse?> GetForecastAsync(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={ApiKey}&units=metric&lang=ua";
    
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ForecastResponse>(json);
            }
            catch
            {
                return null;
            }
        }

    }
    
    
}