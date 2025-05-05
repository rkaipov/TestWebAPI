using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestWebAPI.Controllers;
using TestWebAPI.Data;
using TestWebAPI.DataModels.Models;
using TestWebAPI.Repositories;

namespace TestWebAPI.Tests;

public class OrdersControllerTest
{
  private readonly ILogger<OrdersController> _logger;
  private readonly IOrderRepository _repo;
  private readonly OrdersController _controller;
  private readonly IMapper _mapper;

  public OrdersControllerTest()
  {
    _repo = GetInMemoryRepository();

    var serviceProvider = new ServiceCollection()
     .AddLogging()
     .BuildServiceProvider();
    var factory = serviceProvider.GetService<ILoggerFactory>();
    _logger = factory.CreateLogger<OrdersController>();

    var mappingConfig = new MapperConfiguration(config =>
    {
      config.AddProfile(new AutoMapperProfile());
    });
    _mapper = mappingConfig.CreateMapper();

    _controller = new OrdersController(_repo, _mapper, _logger);
  }

  [Fact]
  public async Task GetOrderList_Test_OkResult()
  {
    // Act
    var result = await _controller.GetOrderList(null, default);

    // Assert
    var actionResult = Assert.IsType<ActionResult<IEnumerable<OrderDto>>>(result);
    Assert.IsType<OkObjectResult>(actionResult.Result);
  }

  private IOrderRepository GetInMemoryRepository()
  {
    var builder = new DbContextOptionsBuilder<TestWebAPIContext>();
    builder.UseInMemoryDatabase("InMemoryDB");
    var options = builder.Options;
    TestWebAPIContext ordersDataContext = new TestWebAPIContext(options);
    ordersDataContext.Database.EnsureDeleted();
    ordersDataContext.Database.EnsureCreated();
    return new OrderRepository(ordersDataContext);
  }
}