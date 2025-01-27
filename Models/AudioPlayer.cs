using System;
using System.IO;
using LibVLCSharp.Shared;

namespace PomoFlow.Models
{
    public static class AudioPlayer
    {
        private static LibVLC _libVLC;
        private static MediaPlayer? _mediaPlayer;

        static AudioPlayer()
        {
            Core.Initialize(@"C:\Program Files\VideoLAN\VLC");
            _libVLC = new LibVLC();
        }

        public static void PlaySound(string relativePath)
        {
            try
            {
                _mediaPlayer?.Dispose();

                var absolutePath = Path.Combine(AppContext.BaseDirectory, "Assets/Audio", relativePath);
                //var absolutePath = @"C:\Users\levib\Desktop\C# coding projects\PomoFlow\Assets\Audio\Effects\Swipe.wav";


                if (!File.Exists(absolutePath))
                {
                    Console.WriteLine($"Audio file not found at: {absolutePath}");
                    return;
                }

                var media = new Media(_libVLC, absolutePath, FromType.FromPath);

                _mediaPlayer = new MediaPlayer(media);
                _mediaPlayer.Volume = 70;
                _mediaPlayer.Mute = false;
                _mediaPlayer.Play();

                Console.WriteLine($"Sound played from: {absolutePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        public static void StopSound()
        {
            _mediaPlayer?.Stop();
        }
    }
}
