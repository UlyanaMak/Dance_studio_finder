using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace DanceStudioFinder.Services
{
    public class YandexMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public YandexMapsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["YandexMaps:ApiKey"];
        }

        public async Task<(bool IsValid, string? SettlementArea)> ValidateAddressAsync(
            string entity, string locality, string street, int buildingNumber, string? letter)
        {
            try
            {
                var fullAddress = $"{entity}, {locality}, {street} {buildingNumber}{letter}";
                var encodedAddress = WebUtility.UrlEncode(fullAddress);

                var url = $"https://geocode-maps.yandex.ru/1.x/?format=json&geocode={encodedAddress}&apikey={_apiKey}&lang=ru-RU";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return (false, null);

                var jsonString = await response.Content.ReadAsStringAsync();
                var yandexResponse = JsonSerializer.Deserialize<YandexGeocoderResponse>(jsonString);

                if (yandexResponse?.Response?.GeoObjectCollection?.MetaDataProperty?.GeocoderResponseMetaData?.Found <= 0)
                    return (false, null);

                var components = yandexResponse.Response.GeoObjectCollection.FeatureMember?
                    .FirstOrDefault()?.GeoObject?.MetaDataProperty?.GeocoderMetaData?.Address?.Components;

                if (components == null)
                    return (true, null); // адрес валиден, но район не найден

                var settlementArea = components.FirstOrDefault(c =>
                    c.Kind == "district" || c.Kind == "sublocality")?.Name;

                return (true, settlementArea);
            }
            catch
            {
                return (false, null);
            }
        }
    }

    // JSON модели
    public class YandexGeocoderResponse
    {
        public YandexResponse Response { get; set; }
    }

    public class YandexResponse
    {
        public GeoObjectCollection GeoObjectCollection { get; set; }
    }

    public class GeoObjectCollection
    {
        public MetaDataProperty MetaDataProperty { get; set; }
        public FeatureMember[] FeatureMember { get; set; }
    }

    public class MetaDataProperty
    {
        public GeocoderResponseMetaData GeocoderResponseMetaData { get; set; }
    }

    public class GeocoderResponseMetaData
    {
        public int Found { get; set; }
    }

    public class FeatureMember
    {
        public GeoObject GeoObject { get; set; }
    }

    public class GeoObject
    {
        public GeoObjectMetaDataProperty MetaDataProperty { get; set; }
    }

    public class GeoObjectMetaDataProperty
    {
        public GeocoderMetaData GeocoderMetaData { get; set; }
    }

    public class GeocoderMetaData
    {
        public Address Address { get; set; }
    }

    public class Address
    {
        public AddressComponent[] Components { get; set; }
    }

    public class AddressComponent
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}

