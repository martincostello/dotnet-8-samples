using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<WeatherClient>();
builder.Services.AddHttpClient<WeatherClient>(client => client.BaseAddress = new("https://api.open-meteo.com"));
builder.Services.Configure<WeatherOptions>(builder.Configuration.GetSection("Weather"));

var app = builder.Build();

app.MapGet("/forecast", async (
    WeatherClient client,
    IOptions<WeatherOptions> options,
    int? days = null,
    double? latitude = null,
    double? longitude = null,
    string? timeZone = null,
    WindSpeedUnit? windSpeedUnit = null) =>
{
    var defaults = options.Value;

    days ??= defaults.Days;
    latitude ??= defaults.Location.Latitude;
    longitude ??= defaults.Location.Longitude;
    timeZone ??= defaults.TimeZone;
    windSpeedUnit ??= defaults.WindSpeedUnit;

    var forecast = await client.GetForecastAsync(
        days.Value,
        latitude.Value,
        longitude.Value,
        windSpeedUnit.Value,
        timeZone);

    if (forecast is null)
    {
        return Results.NotFound();
    }

    return Results.Json(forecast);
});

app.Run();

public class WeatherClient(HttpClient client)
{
    public async Task<WeatherForecast?> GetForecastAsync(
        int days,
        double latitude,
        double longitude,
        WindSpeedUnit windSpeedUnit,
        string timeZone,
        CancellationToken cancellationToken = default)
    {
        var requestUri = QueryHelpers.AddQueryString("/v1/forecast", new Dictionary<string, string?>()
        {
            ["forecast_days"] = days.ToString(),
            ["hourly"] = "temperature_2m,wind_speed_10m",
            ["latitude"] = latitude.ToString(),
            ["longitude"] = longitude.ToString(),
            ["timezone"] = timeZone,
            ["wind_speed_unit"] = windSpeedUnit switch
            {
                WindSpeedUnit.Knots => "kn",
                WindSpeedUnit.MilesPerHour => "mph",
                WindSpeedUnit.MetersPerSecond => "ms",
                _ => "",
            },
        });

        return await client.GetFromJsonAsync<WeatherForecast>(requestUri, cancellationToken);
    }
}

public record HourlyForecast(
    IList<DateTimeOffset> Time,
    [property: JsonPropertyName("temperature_2m")] IList<float> Temperature,
    [property: JsonPropertyName("wind_speed_10m")] IList<float> WindSpeed);

public record WeatherForecast(
    double Latitude,
    double Longitude,
    string Timezone,
    double Elevation,
    HourlyForecast Hourly);

public enum WindSpeedUnit
{
    KilometersPerHour,
    Knots,
    MetersPerSecond,
    MilesPerHour,
}

public record struct DefaultLocation(double Latitude, double Longitude);

public class WeatherOptions
{
    public int Days { get; set; }

    public DefaultLocation Location { get; set; }

    public string TimeZone { get; set; } = "UTC";

    public WindSpeedUnit WindSpeedUnit { get; set; }
}
