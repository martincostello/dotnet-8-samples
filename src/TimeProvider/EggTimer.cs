namespace DotNet8Samples;

public sealed class EggTimer : IAsyncDisposable
{
    private readonly TimeSpan _duration;
    private readonly TimeProvider _timeProvider;
    private readonly long _timestamp;
    private readonly ITimer _timer;
    private bool _cooked;

    public EggTimer(TimeSpan duration, TimeProvider timeProvider)
    {
        _duration = duration;
        _timeProvider = timeProvider;
        _timestamp = timeProvider.GetTimestamp();
        _timer = timeProvider.CreateTimer(Render, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    public event EventHandler<CookedEventArgs>? Cooked;

    public ValueTask DisposeAsync() => _timer.DisposeAsync();

    private void Render(object? _)
    {
        var elapsed = _timeProvider.GetElapsedTime(_timestamp);

        if (!_cooked && elapsed == _duration)
        {
            _cooked = true;
            Cooked?.Invoke(this, new(elapsed));
        }
    }

    public sealed class CookedEventArgs(TimeSpan duration) : EventArgs
    {
        public TimeSpan Duration { get; } = duration;
    }
}
