using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OloTechTest.Core.Models;

public class RecordModels
{
    public record Dietaryrestrictions
    {
        public bool glutenfree { get; set; }
        public bool dairyfree { get; set; }
    }

    public record Restaurant
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Cuisine { get; set; }
        public required string Streetaddress { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required int Zip { get; set; }
        public required object Phonenumber { get; set; }
        public required Dietaryrestrictions Dietaryrestrictions { get; set; }
        public required int Pricerange { get; set; }
        public bool Delivery { get; set; }

        public string GetFullAddress() => $"{Streetaddress}, {City}, {State} {Zip}";
        
    }

    public record Favourite
    {
        public required string Id { get; set; }
        public required string ListId { get; set; }
        public required int Rating { get; set; }
        public required string RestaurantId { get; set; }
        public required string Notes { get; set; }
    }

    public record FavouriteAddRequest
    {
        public Guid RestaurantId { get; set; }
        public int Rating { get; set; }
        public required string Notes { get; set; }
    }

    public record FavouriteAddResponse
    {
        public required string Id { get; set; }
        public required string RestaurantId { get; set; }
        public required int Rating { get; set; }
        public required string Notes { get; set; }
    }

    public record FavouriteDeleteResponse
    {
        public required string FaveListId { get; set; }
        public required string Message { get; set; }
    }
}
