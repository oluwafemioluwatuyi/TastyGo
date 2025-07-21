using AutoMapper;
using TastyGo.DTOs;
using TastyGo.Interfaces;
using TastyGo.Models;

namespace TastyGo.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterRequestDto, User>();
            CreateMap<CreateRestaurantDto, Restaurant>();

        }
    }
}