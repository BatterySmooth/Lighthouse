using Lighthouse.API.Controllers.Relay;
using Lighthouse.Tower.Configuration;
using Lighthouse.Tower.Data;

namespace Lighthouse.API;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }
    app.UseWebSockets();
    // app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    var config = new Config();
    var db = new Database();
    
    var listener = new RelayListener();
    _ = listener.Start();
    
    app.Run();
  }
}