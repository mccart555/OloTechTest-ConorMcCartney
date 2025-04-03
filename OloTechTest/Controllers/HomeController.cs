using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OloTechTest.Core.Interfaces;
using OloTechTest.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using static OloTechTest.Core.Models.RecordModels;

namespace OloTechTest.Controllers;
public class HomeController(ILogger<HomeController> logger, IApiClientService apiClient) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IApiClientService _apiClient = apiClient;

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _apiClient.GetRestaurants();
            HttpContext.Session.SetString("rests", response.Item2.ToString());

            var nameLookup = response.Item1?
                .Select(x => new { id = x.Id, name = x.Name })
                .ToDictionary(item => item.id, item => item.name);

            string json = JsonSerializer.Serialize(nameLookup, new JsonSerializerOptions { WriteIndented = true });

            HttpContext.Session.SetString("nameLookup", json);

            return View(GetAndDeserialiseViewModel());
        }
        catch(Exception)
        {
            return View(null);
        }
    }

    public async Task<IActionResult> GetRestaurant(string id)
    {
        try
        {
            var response = await _apiClient.GetRestaurant(id);
            HttpContext.Session.SetString("selectedRest", response.Item2.ToString());
            return Json(GetAndDeserialiseViewModel());
        }
        catch(Exception)
        {
            return Json(null);
        }
    }

    public async Task<IActionResult> CreateFavsList()
    {
        try
        {
            var response = await _apiClient.CreateFavsList();
            HttpContext.Session.SetString("favsId", response.Item1.ToString());
            return Json(GetAndDeserialiseViewModel());
        }
        catch (Exception)
        {
            return Json(null);
        }
    }

    public async Task<IActionResult> GetFavsList()
    {
        try
        {
            var response = await _apiClient.GetFavsList(HttpContext.Session.GetString("favsId") ?? string.Empty);

            var lookup = HttpContext.Session.GetString("nameLookup") ?? string.Empty;
            var nameLookup = JsonSerializer.Deserialize<Dictionary<string, string>>(lookup);

            //Do the objects with the name lookup
            if (response.Item1 != null)
            {
                foreach (var item in response.Item1)
                {

                    if (nameLookup != null && nameLookup.ContainsKey(item.RestaurantId))
                    {
                        item.RestaurantId = nameLookup[item.RestaurantId]; 
                    }
                }
            }

            string updatedResponseItem2 = JsonSerializer.Serialize(response.Item1, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine(updatedResponseItem2);
            HttpContext.Session.SetString("favs", updatedResponseItem2);
            return Json(GetAndDeserialiseViewModel());
        }
        catch (Exception)
        {
            return Json(null);
        }
    }
    public async Task<IActionResult> AddFavs([FromBody] object fav)
    {
        try
        {
            var response = await _apiClient.FavsListAdd(HttpContext.Session.GetString("favsId") ?? string.Empty, fav);
            HttpContext.Session.SetString("add", response.Item2.ToString());
            return Json(GetAndDeserialiseViewModel());
        }
        catch (Exception)
        {
            return Json(null);
        }
    }

    public class DeleteFavRequest
    {
        public string favId { get; set; } = "";
        public string restId { get; set; } = "";
    }

    public async Task<IActionResult> DeleteFav([FromBody] DeleteFavRequest del)
    {

        try
        {
            var response = await _apiClient.FavsListDelete(del.favId ?? string.Empty, del.restId ?? string.Empty);

            HttpContext.Session.SetString("delete", response.Item2.ToString());
            return Json(GetAndDeserialiseViewModel());
        }
        catch (Exception)
        {
            return Json(null);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private RestaurantViewModel? GetAndDeserialiseViewModel()
    {
        try
        {
            var restaurants = HttpContext.Session.GetString("rests") ?? string.Empty;
            var selected = HttpContext.Session.GetString("selectedRest") ?? string.Empty;
            var favsId = HttpContext.Session.GetString("favsId") ?? string.Empty;
            var favs = HttpContext.Session.GetString("favs") ?? string.Empty;
            var add = HttpContext.Session.GetString("add") ?? string.Empty;
            var delete = HttpContext.Session.GetString("delete") ?? string.Empty;

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Restaurant>? restaurantsObjects = JsonSerializer.Deserialize<List<Restaurant>>(restaurants, options);
            Restaurant? restaurant = !string.IsNullOrEmpty(selected) ? JsonSerializer.Deserialize<Restaurant>(selected, options) : null;
            //favsId is just a string not a json response
            List<Favourite>? favsList = !string.IsNullOrEmpty(favs) ? JsonSerializer.Deserialize<List<Favourite>>(favs, options) : null;
            FavouriteAddResponse? addResponse = !string.IsNullOrEmpty(add) ? JsonSerializer.Deserialize<FavouriteAddResponse>(add, options) : null;
            FavouriteDeleteResponse? deleteResponse = !string.IsNullOrEmpty(delete) ? JsonSerializer.Deserialize<FavouriteDeleteResponse>(delete, options) : null;

            var viewModel = new RestaurantViewModel
            {
                Restaurants = string.IsNullOrWhiteSpace(restaurants) ? Enumerable.Empty<Restaurant>() : restaurantsObjects ?? Enumerable.Empty<Restaurant>(),
                SelectedRestaurant = restaurant,
                FavId = favsId,
                Favourites = string.IsNullOrWhiteSpace(favs) ? Enumerable.Empty<Favourite>() : favsList ?? Enumerable.Empty<Favourite>(),
                Add = addResponse,
                Delete = deleteResponse

            };
            return viewModel;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
