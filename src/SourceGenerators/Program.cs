var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(Random.Shared);
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseHttpsRedirection();

string[] summaries =
[
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
];

app.MapGet("/weatherforecast", (Random random, TimeProvider timeProvider) =>
{
    var now = timeProvider.GetLocalNow();
    return Enumerable.Range(1, 5)
        .Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(now.DateTime.AddDays(index)),
                random.Next(-20, 55),
                summaries[random.Next(summaries.Length)]))
        .ToArray();
});

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
