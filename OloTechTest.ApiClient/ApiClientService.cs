using Microsoft.Extensions.Logging;
using OloTechTest.Core.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static OloTechTest.Core.Models.RecordModels;

namespace OloTechTest.ApiClient;
public class ApiClientService(ILogger<ApiClientService> logger) : IApiClientService
{
    private readonly ILogger<IApiClientService> _logger = logger;
    private HttpClient? _httpClient = null;

    private const string BASE_URL = "https://leibsju5t3.execute-api.us-east-1.amazonaws.com/";
    public async Task<(IEnumerable<Restaurant>?, string)> GetRestaurants()
    {
        try
        {
            using (_httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage response = await _httpClient.GetAsync("restaurants");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<Restaurant>? restaurants = JsonSerializer.Deserialize<List<Restaurant>>(responseBody, options);

                return (restaurants ?? Enumerable.Empty<Restaurant>(), responseBody);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error: {e.ToString}");
            return (Enumerable.Empty<Restaurant>(), string.Empty);
        }
    }

    public async Task<(Restaurant?, string)> GetRestaurant(string id)
    {
        try
        {
            using (_httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage response = await _httpClient.GetAsync($"restaurants/{id}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Restaurant? restaurant = JsonSerializer.Deserialize<Restaurant>(responseBody, options);

                return (restaurant, responseBody);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error: {e.ToString}");
            return (null, string.Empty);
        }
    }

    public async Task<(string, string)> CreateFavsList()
    {
        try
        {
            using (_httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage response = await _httpClient.PutAsync("faveslist/create/", null);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                string idString = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody)?.Values.FirstOrDefault() ?? string.Empty;

                return (idString, responseBody);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error: {e.ToString}");
            return (string.Empty, string.Empty);
        }
    }

    public async Task<(IEnumerable<Favourite>?, string)> GetFavsList(string id)
    {
        try
        {
            using (_httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage response = await _httpClient.GetAsync($"faveslist/{id}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<Favourite>? fav = JsonSerializer.Deserialize< List<Favourite>>(responseBody, options);

                return (fav, responseBody);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error: {e.ToString}");
            return (null, string.Empty);
        }
    }

    public async Task<(FavouriteAddResponse?, string)> FavsListAdd(string favesListId, object request)
    {
        try
        {
            using (_httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(BASE_URL);

                string jsonContent = JsonSerializer.Serialize(request);
                HttpContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync($"faveslist/{favesListId}/add", httpContent);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                FavouriteAddResponse? addResponse = JsonSerializer.Deserialize<FavouriteAddResponse>(responseBody, options);

                return (addResponse, responseBody);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error: {e.ToString}");
            return (null, string.Empty);
        }
    }

    public async Task<(FavouriteDeleteResponse?, string)> FavsListDelete(string favesListId, string restaurantId)
    {
        try
        {
            using (_httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage response = await _httpClient.DeleteAsync($"faveslist/{favesListId}/remove/{restaurantId}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                FavouriteDeleteResponse? addResponse = JsonSerializer.Deserialize<FavouriteDeleteResponse>(responseBody, options);

                return (addResponse, responseBody);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error: {e.ToString}");
            return (null, string.Empty);
        }
    }

}

