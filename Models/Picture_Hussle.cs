using System;
using System.Collections.Generic;

namespace PomoFlow.Models
{
    internal class Picture_Hussle
    {
        private readonly Random _random = new Random();
        private readonly List<string> _imagePaths = new List<string>
        {
            "CloudGasm.jpg",
            "CurvyWater.jpg",
            "GroovyClouds.jpg",
            "MountainFlower.jpg",
            "SeaAndSand.jpg",
            "SnowyMountains.jpg",
            "MountainPorn.jpg"
        };

        // Event to pass on new images
        public event Action<string>? ImageChanged;

        public void SelectNewImage()
        {
            //if (_imagePaths.Count == 0) return;

            // Random select image
            var randomIndex = _random.Next(0, _imagePaths.Count);
            var selectedImage = _imagePaths[randomIndex];

            // Trigger ImageChanged-event
            ImageChanged?.Invoke(selectedImage);
        }
    }
}
