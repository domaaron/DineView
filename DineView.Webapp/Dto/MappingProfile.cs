using AutoMapper;
using DineView.Application.models;
using System;

namespace DineView.Webapp.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RestaurantDto, Restaurant>();
            CreateMap<Restaurant, RestaurantDto>();
            CreateMap<MenuDto, Menu>()
                .ForMember(
                    m => m.Guid,
                    opt => opt.MapFrom(m => m.guid == default ? Guid.NewGuid() : m.guid));
            CreateMap<Menu, MenuDto>();
        }
    }
}
