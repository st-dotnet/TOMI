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
            CreateMap<MF2Model, Terminal_Department>();
            CreateMap<StoreModel, Store>();
            CreateMap<InfoDataResponse, InfoLoad>();
            CreateMap<RangesModel, Ranges>();
            CreateMap<GroupModel, Group>();
            CreateMap<StockAdjustmentModel, StockAdjustment>();
        }
    }
}
