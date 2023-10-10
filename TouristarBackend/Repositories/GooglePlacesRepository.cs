using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Models;

namespace TouristarBackend.Repositories;

public class GooglePlacesRepository : IGooglePlacesRepository
{
    private readonly GooglePlacesConfig _config;
    private readonly HttpClient _client;

    private readonly ILogger<IGooglePlacesRepository> _logger;

    public JsonSerializerSettings JsonSettings
    {
        get
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            return new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
        }
    }

    public GooglePlacesRepository(
        IOptionsMonitor<GooglePlacesConfig> config,
        ILogger<IGooglePlacesRepository> logger
    )
    {
        _config = config.CurrentValue;
        _client = new HttpClient();
        _logger = logger;
    }

    public async Task<byte[]> GetLocationImage(string query)
    {
        var encodedQuery = WebUtility.UrlEncode(query);
        var autocompleteUrl =
            $"{_config.BaseUrl}/place/autocomplete/json?input={encodedQuery}&sensor=false&types=(regions)&key={_config.ApiKey}";
        var content = await (await _client.GetAsync(autocompleteUrl)).Content.ReadAsStringAsync();
        GooglePlacesAutocompleteResponse? result =
            JsonConvert.DeserializeObject<GooglePlacesAutocompleteResponse>(content, JsonSettings);
        if (result == null)
        {
            _logger.LogError("Could not fetch autocomplete places response from Google API.");
            throw new InvalidOperationException();
        }

        var placeId = result.Predictions.First().PlaceId;
        var detailsUrl =
            $"{_config.BaseUrl}/place/details/json?placeid={placeId}&key={_config.ApiKey}";
        var detailsContent = await (await _client.GetAsync(detailsUrl)).Content.ReadAsStringAsync();
        GooglePlaceDetailsResponse? detailsResult =
            JsonConvert.DeserializeObject<GooglePlaceDetailsResponse>(detailsContent, JsonSettings);
        if (detailsResult == null)
        {
            _logger.LogError(
                $"Could not fetch place details response from Google API. Place id: {placeId}"
            );
            throw new InvalidOperationException();
        }

        var photoReference = detailsResult.Result.Photos.First().PhotoReference;
        var photoUrl =
            $"{_config.BaseUrl}/place/photo?photoreference={photoReference}&key={_config.ApiKey}&maxwidth=1200";
        var photoResult = await (await _client.GetAsync(photoUrl)).Content.ReadAsByteArrayAsync();
        return photoResult;
    }

    public async Task<IEnumerable<ActivityLocation>?> FindActivityLocations(
        string query,
        Location tripLocation
    )
    {
        var encodedQuery = WebUtility.UrlEncode(query);
        var url =
            $"{_config.BaseUrl}/place/autocomplete/json?input={encodedQuery}&radius=50000&location={tripLocation.Latitude},{tripLocation.Longitude}&key={_config.ApiKey}";
        var content = await (await _client.GetAsync(url)).Content.ReadAsStringAsync();
        GooglePlacesAutocompleteResponse? result =
            JsonConvert.DeserializeObject<GooglePlacesAutocompleteResponse>(content, JsonSettings);
        if (result == null || !result.Predictions.Any())
        {
            throw new InvalidOperationException($"Could not find predictions for query {query}.");
        }

        return result?.Predictions.Select(x =>
        {
            var terms = x.Terms.ToList();
            return new ActivityLocation
            {
                MainText = x.StructuredFormatting.MainText,
                SecondaryText = x.StructuredFormatting.SecondaryText,
                Description = x.Description,
                Address1 = terms.Count >= 1 ? terms[0].Value : "",
                Address2 = terms.Count >= 2 ? terms[1]?.Value ?? "" : "",
                Address3 = terms.Count >= 3 ? terms[2]?.Value ?? "" : "",
                Address4 = terms.Count >= 4 ? terms[3]?.Value ?? "" : "",
                PlacesId = x.PlaceId,
                Types = x.Types,
            };
        });
    }

    public async Task<GoogleGeometryResponseDto> FindGeometry(string placeId)
    {
        try
        {
            var url = $"{_config.BaseUrl}/geocode/json?place_id={placeId}&key={_config.ApiKey}";
            var content = await (await _client.GetAsync(url)).Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GoogleGeometryResponseDto>(
                content,
                JsonSettings
            );
            if (result == null)
                throw new JsonException();
            return result;
        }
        catch (Exception e)
        {
            var message = $"Could not find geometry for place id {placeId}, error: {e}";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }
    }

    public async Task<byte[]> GetMapsImage(float latitude, float longitude)
    {
        try
        {
            var url =
                $"{_config.BaseUrl}/staticmap?center={latitude},{longitude}&size=400x400&format=JPEG&maptype=terrain&zoom=15&markers={latitude},{longitude}&key={_config.ApiKey}";
            return await (await _client.GetAsync(url)).Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            var message =
                $"Could not create trip image, lat {latitude}, lng {longitude}, error {e}.";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }
    }
}
