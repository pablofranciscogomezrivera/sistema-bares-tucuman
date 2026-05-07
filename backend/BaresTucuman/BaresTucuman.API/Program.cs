using BaresTucuman.API.Domain.Interfaces;
using BaresTucuman.API.Infraestructure.Data;
using BaresTucuman.API.Infraestructure.Providers;
using BaresTucuman.API.Services;
using BaresTucuman.API.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IBarProvider, MockBarProvider>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<BarSyncService>(); 
builder.Services.AddHostedService<BarSyncWorker>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
