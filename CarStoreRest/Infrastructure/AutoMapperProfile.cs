using AutoMapper;
using CarStoreRest.Models.ApiModels;
using CarStoreWeb.Models;

namespace CarStoreRest.Infrastructure
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CarAddApiModel, Car>();

            CreateMap<CarEditApiModel, Car>();
        }
    }
}
