using AutoMapper;
using TastyGo.DTOs;
using TastyGo.DTOs.Vendor;
using TastyGo.Interfaces;
using TastyGo.Models;

namespace TastyGo.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<RegisterRequestDto, User>().ReverseMap();
            CreateMap<CreateVendorDto, Vendor>().ReverseMap();
            CreateMap<VendorDto, Vendor>().ReverseMap();
            CreateMap<CreateRestaurantDto, Restaurant>().ReverseMap();
            CreateMap<Restaurant, RestaurantDto>().ReverseMap();
            CreateMap<UpdateRestaurantRequestDto, Restaurant>().ReverseMap();
            CreateMap<Menu, MenuDto>().ReverseMap();


        }
    }
}