using System.Reflection;
using static OloTechTest.Core.Models.RecordModels;

namespace OloTechTest.Models;

public class RestaurantViewModel
{
    public Restaurant? SelectedRestaurant { get; set; } = null;

    public IEnumerable<Restaurant> Restaurants { get; set; } = Enumerable.Empty<Restaurant>();

    public string FavId { get; set; } = string.Empty;

    public IEnumerable<Favourite> Favourites { get; set; } = Enumerable.Empty<Favourite>();

    public FavouriteAddResponse? Add { get; set; } = null;

    public FavouriteDeleteResponse? Delete { get; set; } = null;

}


