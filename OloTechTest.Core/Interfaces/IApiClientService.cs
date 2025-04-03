using static OloTechTest.Core.Models.RecordModels;

namespace OloTechTest.Core.Interfaces;

public interface IApiClientService
{
    Task<(IEnumerable<Restaurant>?, string)> GetRestaurants();

    Task<(Restaurant?, string)> GetRestaurant(string id);

    Task<(string, string)> CreateFavsList();

    Task<(IEnumerable<Favourite>?, string)> GetFavsList(string id);

    Task<(FavouriteAddResponse?, string)> FavsListAdd(string favesListId, object request);

    Task<(FavouriteDeleteResponse?, string)> FavsListDelete(string favesListId, string restaurantId);
}

