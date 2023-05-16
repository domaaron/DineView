using System;
using System.ComponentModel.DataAnnotations;

namespace DineView.Webapp.Dto
{
    public record MenuDto
    (
        Guid guid,
        [Range(2, 500, ErrorMessage = "Invalid price")]
        decimal Price,
        Guid DishGuid,
        Guid RestaurantGuid
    );
}
