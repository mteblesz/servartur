using Servartur.Api;
using Servartur.Api.Core.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.RegisterServices();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.RegisterEndpoints();

app.UseRouting();

app.UseHttpsRedirection();

app.Run();
