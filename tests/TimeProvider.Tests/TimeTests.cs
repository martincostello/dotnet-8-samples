using Microsoft.Extensions.Time.Testing;
using Xunit.Abstractions;

namespace DotNet8Samples;

public class TimeTests(ITestOutputHelper outputHelper)
{
    [Theory]
    [InlineData("2023-01-30")]
    [InlineData("2023-02-01")]
    [InlineData("2023-02-27")]
    [InlineData("2023-03-01")]
    [InlineData("2023-04-30")]
    [InlineData("2023-09-30")]
    [InlineData("2023-12-31")]
    public void It_Is_Not_Payday(string date)
    {
        // Arrange
        var startDateTime = new DateTimeOffset(DateOnly.Parse(date), TimeOnly.MinValue, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(startDateTime);
        var calculator = new PaydayCalculator(timeProvider);

        // Act
        var actual = calculator.IsItPayday();

        // Assert
        Assert.False(actual, "It is payday when it shouldn't be 🤑");
    }

    [Theory]
    [InlineData("2023-01-31")]
    [InlineData("2023-02-28")]
    [InlineData("2023-03-31")]
    [InlineData("2023-04-28")]
    [InlineData("2023-05-31")]
    [InlineData("2023-06-30")]
    [InlineData("2023-07-31")]
    [InlineData("2023-08-31")]
    [InlineData("2023-09-29")]
    [InlineData("2023-10-31")]
    [InlineData("2023-11-30")]
    [InlineData("2023-12-29")]
    public void It_Is_Payday(string date)
    {
        // Arrange
        var startDateTime = new DateTimeOffset(DateOnly.Parse(date), TimeOnly.MinValue, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(startDateTime);
        var calculator = new PaydayCalculator(timeProvider);

        // Act
        var actual = calculator.IsItPayday();

        // Assert
        Assert.True(actual, "It is not payday 😢");
    }

    [Fact]
    public void Time_Moves_Forwards()
    {
        // Arrange
        var startDateTime = new DateTimeOffset(2023, 11, 29, 0, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(startDateTime);
        var calculator = new PaydayCalculator(timeProvider);

        // Assert
        Assert.False(calculator.IsItPayday());
        Assert.Equal(new DateOnly(2023, 11, 29), CurrentDate());

        // Act
        timeProvider.Advance(TimeSpan.FromDays(1));

        // Assert
        Assert.True(calculator.IsItPayday());
        Assert.Equal(new DateOnly(2023, 11, 30), CurrentDate());

        // Act
        timeProvider.Advance(TimeSpan.FromDays(1));

        // Assert
        Assert.False(calculator.IsItPayday());
        Assert.Equal(new DateOnly(2023, 12, 01), CurrentDate());

        // Act
        timeProvider.Advance(TimeSpan.FromDays(2));

        // Assert
        Assert.False(calculator.IsItPayday());
        Assert.Equal(new DateOnly(2023, 12, 03), CurrentDate());

        DateOnly CurrentDate()
            => DateOnly.FromDateTime(timeProvider.GetUtcNow().Date);
    }

    [Fact]
    public void Time_Moves_Forwards_Automatically()
    {
        // Arrange
        var startDateTime = new DateTimeOffset(2023, 11, 29, 0, 0, 0, TimeSpan.Zero);

        var timeProvider = new FakeTimeProvider(startDateTime)
        {
            AutoAdvanceAmount = TimeSpan.FromDays(1),
        };

        var calculator = new PaydayCalculator(timeProvider);

        // Act and Assert
        Assert.False(calculator.IsItPayday());
        Assert.True(calculator.IsItPayday());
        Assert.False(calculator.IsItPayday());
    }

    [Fact]
    public void Back_To_The_Future_2()
    {
        // Arrange
        var hillValley = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var nineteenEightyFive = new DateTimeOffset(1985, 10, 21, 16, 29, 0, TimeSpan.FromHours(-8));

        var timeProvider = new FakeTimeProvider(nineteenEightyFive);

        // Act
        timeProvider.SetLocalTimeZone(hillValley);

        // Assert
        Assert.Equal(1985, timeProvider.GetLocalNow().Year);

        // Act
        TimeTravelYears(30);

        // Assert
        Assert.Equal(2015, timeProvider.GetLocalNow().Year);

        void TimeTravelYears(int value)
        {
            var present = timeProvider.GetUtcNow();
            var destination = present.AddYears(value);

            timeProvider.SetUtcNow(destination);
        }
    }

    [Fact]
    public async Task Timers_Can_Be_Sped_Up()
    {
        // Arrange
        var runny = TimeSpan.FromMinutes(3);
        var timeProvider = new FakeTimeProvider();
        await using var eggTimer = new EggTimer(runny, timeProvider);

        var start = DateTimeOffset.UtcNow;

        eggTimer.Cooked += (_, args) =>
        {
            var elapsed = DateTimeOffset.UtcNow - start;
            outputHelper.WriteLine($"The 🍳 reported being cooked after {args.Duration.TotalMinutes} minutes.");
            outputHelper.WriteLine($"The 🍳 actually cooked after {elapsed.TotalMilliseconds}ms.");
        };

        var tcs = new TaskCompletionSource<bool>();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        // Act
        _ = Task.Run(async () =>
        {
            while (!eggTimer.IsCooked && !cts.IsCancellationRequested)
            {
                timeProvider.Advance(TimeSpan.FromMinutes(1));
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }

            if (cts.IsCancellationRequested)
            {
                tcs.SetCanceled(cts.Token);
            }
            else
            {
                tcs.SetResult(eggTimer.IsCooked);
            }
        }, cts.Token);

        // Assert
        Assert.True(await tcs.Task, "The 🍳 did not cook.");
    }
}
