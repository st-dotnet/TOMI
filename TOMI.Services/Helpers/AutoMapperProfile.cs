using AutoMapper;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;
namespace TOMI.Services.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<CustomerModel, Customer>();
            CreateMap<UserModel, User>();
            CreateMap<StoreModel, Store>();
            CreateMap<SalesDetailResponse, Sales>();
            CreateMap<MasterDataResponse, Master>();
            CreateMap<StocksDataResponse, Stocks>();
            CreateMap<RangesModel, Ranges>();
            CreateMap<GroupModel, Group>();
        }
    }
}
