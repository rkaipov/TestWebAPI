using Microsoft.EntityFrameworkCore;
using Serilog;
using TestWebAPI.Data;
using TestWebAPI.Repositories;

namespace TestWebAPI
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      builder.Services.AddDbContext<TestWebAPIContext>(options =>
          options.UseSqlite(builder.Configuration.GetConnectionString("TestWebAPIContext") ?? throw new InvalidOperationException("Connection string 'TestWebAPIContext' not found.")));

      builder.Services.AddScoped<IItemRepository, ItemRepository>();
      builder.Services.AddScoped<IOrderRepository, OrderRepository>();

      builder.Services.AddControllers();
      builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Information()
      .WriteTo.Console()
      .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
      .Enrich.FromLogContext()
      .CreateLogger();

      builder.Host.UseSerilog();

      var app = builder.Build();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();
      app.UseAuthorization();
      app.MapControllers();

      app.Run();
    }
  }
}
