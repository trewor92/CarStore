using AutoMapper;
using CarStoreRest.Models.ApiModels;
using CarStoreRest.Models;

namespace CarStoreRest.Infrastructure
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CarAddApiModel, Car>();
          

            CreateMap<CarEditApiModel, Car>();

            CreateMap<Car, Car>().ForMember(c => c.CarID, opt => opt.Ignore());

        }
    }
}
