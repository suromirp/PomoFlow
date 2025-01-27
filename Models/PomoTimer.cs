using System;
using System.Timers;

namespace PomoFlow.Models
{
    internal class PomoTimer
    {
        private Timer _timer = new Timer(); // Initialize _timer here
        public TimeSpan _timeRemaining;
        public bool TimerRunning { get; private set; }
        public bool TimerPaused { get; private set; }
        public event Action<string>? TimerUpdated;
        public event Action? TimerEnded;
        public TimerMode CurrentMode => _currentMode;

        public PomoTimer()
        {
            InitializeTimer();
        }

        public void Start()
        {
            _timer.Start();
            TimerPaused = false;
            TimerRunning = true;
        }

        public void Pause()
        {
            _timer.Stop();
            TimerPaused = true;
            TimerRunning = false;
        }

        public void Stop()
        {
            _timer.Stop();
            TimerPaused = false;
            TimerRunning = false;
        }

        public void Reset()
        {
            _timer.Stop();
            _timeRemaining = TimeSpan.FromMinutes((int)_currentMode);
            TimerUpdated?.Invoke(_timeRemaining.ToString(@"mm\:ss"));
            TimerPaused = false;
            TimerRunning = false;
        }

        public enum TimerMode
        {
            Pomodoro = 25,
            ShortBreak = 5,
            LongBreak = 30,
        }

        private void InitializeTimer()
        {
            // Timer set to pomodoro mode
            _timeRemaining = TimeSpan.FromMinutes((int)TimerMode.Pomodoro);

            //SetTimerDuration((int)TimerMode.Pomodoro, TimerMode.Pomodoro);

            // Tick every second
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed; // Update the event-handler name
            _timer.AutoReset = true;
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (_timeRemaining.TotalSeconds > 0)
            {
                _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1));
                TimerUpdated?.Invoke(_timeRemaining.ToString(@"mm\:ss")); // Trigger the event
            }
            else // If the timer is over, reset the timer to 25:00 minutes
            {
                AudioPlayer.PlaySound("Alarms/Alarm-Sound.wav");
                _timer.Stop();
                TimerUpdated?.Invoke("25:00");
                TimerEnded?.Invoke();
            }
        }

        public TimerMode _currentMode = TimerMode.Pomodoro;
        public void SetTimerDuration(int minutes, TimerMode mode)
        {
            _timer.Stop();
            _timeRemaining = TimeSpan.FromMinutes(minutes);
            _currentMode = mode;
            TimerUpdated?.Invoke(_timeRemaining.ToString(@"mm\:ss"));
            TimerRunning = false;
            TimerPaused = false;
        }

        public void AddTime(int minutes)
        {
            _timeRemaining = _timeRemaining.Add(TimeSpan.FromMinutes(minutes));
            TimerUpdated?.Invoke(_timeRemaining.ToString(@"mm\:ss"));
        }

    }
}
