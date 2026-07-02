using System.Text.Json.Serialization;
using TechnicalTest.Application.Abstractions;
using TechnicalTest.Application.Reservations;
using TechnicalTest.Infrastructure.Persistence;
using TechnicalTest.Infrastructure.Time;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IReservationRepository, InMemoryReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program { }
