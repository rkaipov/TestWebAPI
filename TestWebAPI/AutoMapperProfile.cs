using AutoMapper;
using TestWebAPI.DataModels;
using TestWebAPI.DataModels.Models;


namespace TestWebAPI;

public class AutoMapperProfile : Profile
{
  public AutoMapperProfile()
  {
    CreateMap<ItemDto, Item>()
      .ReverseMap();
    CreateMap<OrderDto, Order>()
      .ReverseMap();
  }
}
