using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DanceStudioFinder.Services
{
    public class OpenStreetMapService
    {
        private readonly HttpClient _httpClient;  //http-клиент  для выполнения запросов
        private readonly string _userAgent;       //идентификатор приложения (требование OSM)

        public OpenStreetMapService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _userAgent = configuration["UserAgent"] ?? "DanceStudioFinderApp/1.0";  //получение User-Agent (по умолчанию)
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);  //установка заголовка
        }


        /// <summary>
        /// Метод проверки адреса
        /// </summary>
        /// <param name="entity">субъект</param>
        /// <param name="locality">населённый пункт</param>
        /// <param name="street">улица</param>
        /// <param name="buildingNumber">номер здания</param>
        /// <param name="letter">литера</param>
        /// <returns></returns>
        public async Task<(bool IsValid, string? District)> ValidateAddressAsync(
            string entity, string locality, string street, int buildingNumber, string? letter)
        {
            try
            {
                var fullAddress = $"{street} {buildingNumber}{letter}, {locality}, {entity}";  //адрес
                var encodedAddress = WebUtility.UrlEncode(fullAddress);  //включение адреса в url
                //url-запрос
                var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&addressdetails=1&limit=1";

                var response = await _httpClient.GetAsync(url);  //выполнение запроса

                if (!response.IsSuccessStatusCode)  //проверка
                    return (false, null);

                var jsonString = await response.Content.ReadAsStringAsync();  //считывание ответа (строка)
                var results = JsonSerializer.Deserialize<OsmResult[]>(jsonString);  //преобразование результатов в массив

                if (results == null || results.Length == 0)  //проверка наличия результатов
                    return (false, null);
                //детали адреса
                var address = results[0].Address;
                var district = address?.Neighbourhood
                            ?? address?.Suburb
                            ?? address?.CityDistrict
                            ?? address?.Municipality;

                return (true, district);  //возвращаем район, если он найден
            }
            catch
            {
                return (false, null);
            }
        }
    }

    //модели для десериализации ответа OSM
    public class OsmResult
    {
        [JsonPropertyName("address")]
        public OsmAddress Address { get; set; }
    }
    //десериализация деталей адреса
    public class OsmAddress
    {
        [JsonPropertyName("neighbourhood")]
        public string Neighbourhood { get; set; }

        [JsonPropertyName("suburb")]
        public string Suburb { get; set; }

        [JsonPropertyName("city_district")]
        public string CityDistrict { get; set; }

        [JsonPropertyName("municipality")]
        public string Municipality { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("town")]
        public string Town { get; set; }

        [JsonPropertyName("village")]
        public string Village { get; set; }
    }
}

