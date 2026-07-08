using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.Data.Entities;

namespace Library.ControllerApi.Mapping;

public class MappingProfile : Profile
{
    // We just use the constructor and set our mapping here
    public MappingProfile()
    {
        CreateMap<InventoryItem, InventoryDTO>()
            .ForCtorParam("Sku", o => o.MapFrom(s => s.Product.Sku))
            .ForCtorParam("Name", o => o.MapFrom(s => s.Product.Name))
            .ForCtorParam("CurrentStock", o => o.MapFrom(s => s.CurrentStock));
    }
}