



using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using avalonia_dz_templates.Models;

namespace avalonia_dz_templates.Services
{
    public class WeatherService
    {
        private const string ApiKey = "c3c7dd0d8e63c30b03f32d8c5b575f19";

        private const string BaseUrl =
            "https://api.openweathermap.org/data/2.5/weather?q={city}&appid={Key}&units=metric";
        
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            string url = $"{BaseUrl}?weatherq={city}&appid={ApiKey}&units=metric&lang=ua";

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

    }
}