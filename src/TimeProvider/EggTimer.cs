namespace DotNet8Samples;

public sealed class EggTimer : IAsyncDisposable
{
    private static readonly TimeSpan Resolution = TimeSpan.FromSeconds(1);

    private readonly TimeSpan _duration;
    private readonly TimeProvider _timeProvider;
    private readonly long _timestamp;
    private readonly ITimer _timer;

    public EggTimer(TimeSpan duration, TimeProvider timeProvider)
    {
        _duration = duration;
        _timeProvider = timeProvider;

        // Get the current timestamp so we can measure how long the egg was cooking for
        _timestamp = timeProvider.GetTimestamp();

        // Check for whether the egg has been cooked once a second
        _timer = timeProvider.CreateTimer(CheckEggCooked, null, Resolution, Resolution);
    }

    public event EventHandler<CookedEventArgs>? Cooked;

    public bool IsCooked { get; private set; }

    public ValueTask DisposeAsync() => _timer.DisposeAsync();

    private void CheckEggCooked(object? _)
    {
        // See how much time has elapsed since the timer started
        var elapsed = _timeProvider.GetElapsedTime(_timestamp);

        if (!IsCooked && elapsed == _duration)
        {
            // Fire event that the egg is cooked and how long it took to cook
            IsCooked = true;
            Cooked?.Invoke(this, new(elapsed));

            // Stop the timer
            _timer.Dispose();
        }
    }

    public sealed class CookedEventArgs(TimeSpan duration) : EventArgs
    {
        public TimeSpan Duration { get; } = duration;
    }
}
