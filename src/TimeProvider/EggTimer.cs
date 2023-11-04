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
        _timestamp = timeProvider.GetTimestamp();
        _timer = timeProvider.CreateTimer(CheckCooked, null, Resolution, Resolution);
    }

    public event EventHandler<CookedEventArgs>? Cooked;

    public bool IsCooked { get; private set; }

    public ValueTask DisposeAsync() => _timer.DisposeAsync();

    private void CheckCooked(object? _)
    {
        var elapsed = _timeProvider.GetElapsedTime(_timestamp);

        if (!IsCooked && elapsed == _duration)
        {
            IsCooked = true;
            Cooked?.Invoke(this, new(elapsed));
            _timer.Dispose();
        }
    }

    public sealed class CookedEventArgs(TimeSpan duration) : EventArgs
    {
        public TimeSpan Duration { get; } = duration;
    }
}
