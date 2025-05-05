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

public class ItemsControllerTests
{
  private readonly ILogger<ItemsController> _logger;
  private readonly IItemRepository _repo;
  private readonly ItemsController _controller;
  private readonly IMapper _mapper;

  public ItemsControllerTests()
  {
    _repo = GetInMemoryRepository();

    var serviceProvider = new ServiceCollection()
     .AddLogging()
     .BuildServiceProvider();
    var factory = serviceProvider.GetService<ILoggerFactory>();
    _logger = factory.CreateLogger<ItemsController>();

    var mappingConfig = new MapperConfiguration(config =>
    {
      config.AddProfile(new AutoMapperProfile());
    });
    _mapper = mappingConfig.CreateMapper();

    _controller = new ItemsController(_repo, _mapper, _logger);
  }

  [Fact]
  public async Task Test1()
  {
    // Act
    var result = await _controller.GetItemList(null, default);

    // Assert
    var actionResult = Assert.IsType<ActionResult<IEnumerable<ItemDto>>>(result);
    Assert.IsType<OkObjectResult>(actionResult.Result);
  }

  private IItemRepository GetInMemoryRepository()
  {
    var builder = new DbContextOptionsBuilder<TestWebAPIContext>();
    builder.UseInMemoryDatabase("InMemoryDB");
    var options = builder.Options;
    TestWebAPIContext itemsDataContext = new TestWebAPIContext(options);
    itemsDataContext.Database.EnsureDeleted();
    itemsDataContext.Database.EnsureCreated();
    return new ItemRepository(itemsDataContext);
  }
}