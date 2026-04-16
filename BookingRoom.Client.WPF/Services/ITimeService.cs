using System.Windows.Threading;

namespace BookingRoom.Client.WPF.Services;

public interface ITimeService
{
    DateTime Now { get; }
    event Action Tick;
}

public class TimeService : ITimeService
{
    private readonly DispatcherTimer _timer;

    public DateTime Now => DateTime.Now;

    public event Action? Tick;

    public TimeService()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3) // o 1 min se basta
        };

        _timer.Tick += (_, _) => Tick?.Invoke();
        _timer.Start();
    }
}
