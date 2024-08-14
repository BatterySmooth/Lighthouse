using Lighthouse.API.Configuration;
using Lighthouse.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
  app.UseSwagger();
  app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Config.Initialise();
Database.Initialise();

app.Run();