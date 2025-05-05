using AutoMapper;
using TestWebAPI.DataModels;
using TestWebAPI.DataModels.Models;

namespace TestWebAPI.Tests;

internal class AutoMapperProfile : Profile
{
  public AutoMapperProfile()
  {
    CreateMap<ItemDto, Item>()
      .ReverseMap();
    CreateMap<OrderDto, Order>()
      .ReverseMap();
  }
}