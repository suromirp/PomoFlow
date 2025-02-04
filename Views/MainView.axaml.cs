using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using PomoFlow.Models;
using static PomoFlow.Models.PomoTimer;

namespace PomoFlow.Views
{
    public partial class MainView : UserControl
    {
        private readonly PomoTimer _pomodoroTimer;
        private readonly Picture_Hussle _pictureHussle;

        public MainView()
        {
            InitializeComponent();

            // Pomodoro Timer
            _pomodoroTimer = new PomoTimer();
            _pomodoroTimer.TimerUpdated += OnTimerUpdated;
            _pomodoroTimer.TimerEnded += OnTimerEnded;

            // Picture Hussle
            _pictureHussle = new Picture_Hussle();
            _pictureHussle.ImageChanged += OnImageChanged;
        }

        private void PomodoroButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_pomodoroTimer.CurrentMode == TimerMode.Pomodoro)
            {
                return;
            }

            _pomodoroTimer.Reset();
            SetTimerMode(TimerMode.Pomodoro);
            DefaultBackground.IsVisible = true;
            ShortBreakBackground.IsVisible = false;
            LongBreakBackground.IsVisible = false;

            AudioPlayer.PlaySound("Effects/Swipe.wav");
            UpdateButtonStyles();
        }
        private void ShortBreakButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_pomodoroTimer.CurrentMode == TimerMode.ShortBreak)
            {
                return;
            }

            _pomodoroTimer.Reset();
            SetTimerMode(TimerMode.ShortBreak);
            DefaultBackground.IsVisible = false;
            ShortBreakBackground.IsVisible = true;
            LongBreakBackground.IsVisible = false;

            AudioPlayer.PlaySound("Effects/Swipe.wav");
            UpdateButtonStyles();
        }

        private void LongBreakButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_pomodoroTimer.CurrentMode == TimerMode.LongBreak)
            {
                return;
            }

            _pomodoroTimer.Reset();
            SetTimerMode(TimerMode.LongBreak);
            DefaultBackground.IsVisible = false;
            LongBreakBackground.IsVisible = true;
            ShortBreakBackground.IsVisible = false;

            AudioPlayer.PlaySound("Effects/Swipe.wav");
            UpdateButtonStyles();
        }

        private void SetTimerMode(TimerMode mode)
        {
            _pomodoroTimer.Stop();
            StartPauseButton.Content = "Start";
            _pomodoroTimer.SetTimerDuration((int)mode, mode);
            DefaultBackground.IsVisible = mode == TimerMode.Pomodoro;
            ShortBreakBackground.IsVisible = mode == TimerMode.ShortBreak;
            LongBreakBackground.IsVisible = mode == TimerMode.LongBreak;
        }

        // Start or pause the timer
        private void StartPauseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AudioPlayer.PlaySound("Effects/Pop.wav");

            if (_pomodoroTimer.TimerRunning)
            {
                _pomodoroTimer.Pause();
                StartPauseButton.Content = "Start";
                ToggleView(false);
                this.ResetButton.IsVisible = true;
            }
            else
            {
                _pomodoroTimer.Start();
                StartPauseButton.Content = "Pause";
                _pictureHussle.SelectNewImage(); // Select new random image
                ToggleView(true);
                this.ResetButton.IsVisible = false;

            }
        }

        private void ResetButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AudioPlayer.PlaySound("Effects/SmoothBeep.wav");
            _pomodoroTimer.Reset();
            this.ResetButton.IsVisible = false;
            ToggleView(false);
        }

        // Pull images
        private readonly Dictionary<string, Avalonia.Media.Imaging.Bitmap> _imageCache = new();
        private async void OnImageChanged(string newImagePath)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    var assetUri = $"avares://PomoFlow/Assets/Picture_Hussle/{newImagePath}";
                    Console.WriteLine($"Loading image from {assetUri}");

                    using (var stream = Avalonia.Platform.AssetLoader.Open(new Uri(assetUri)))
                    {
                        var bitmap = new Avalonia.Media.Imaging.Bitmap(stream);
                        BackgroundImage.Source = bitmap;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image: {ex.Message}");
                    BackgroundImage.Source = null; // Fallback to black background
                }
            });
        }

        // Update the timer display
        private void OnTimerUpdated(string time)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Timer.Text = time;
            });
        }

        //Reset the timer display when timer ends
        private void OnTimerEnded()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Timer.Text = "25:00";
            });
        }

        // Toggle the background view
        private void ToggleView(bool showBackgroundimage)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                DefaultBackground.IsVisible = !showBackgroundimage; // Display/Hide Default background
                BackgroundImage.IsVisible = showBackgroundimage; // Display/Hide Background image
            });
        }

        //TODO: make button only visible when close to ending time
        private void FlowStateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AudioPlayer.PlaySound("Effects/FlowState.wav");

            if (_pomodoroTimer.TimerPaused)
            {
                return;
            }
            //TODO: make time customizable in settings
            _pomodoroTimer.AddTime(20);
        }

        private void UpdateButtonStyles()
        {
            var activeBackground = new SolidColorBrush(Color.Parse("#33FFFFFF"));
            var inactiveBackground = Brushes.Transparent;

            PomodoroButton.Background = _pomodoroTimer.CurrentMode == PomoTimer.TimerMode.Pomodoro
                ? activeBackground
                : inactiveBackground;

            ShortBreakButton.Background = _pomodoroTimer.CurrentMode == PomoTimer.TimerMode.ShortBreak
                ? activeBackground
                : inactiveBackground;

            LongBreakButton.Background = _pomodoroTimer.CurrentMode == PomoTimer.TimerMode.LongBreak
                ? activeBackground
                : inactiveBackground;
        }
    }
}
