using System;
using System.Collections.Generic;
using System.Text;
using Game2048.Mvvm;
using Xamarin.Forms;

namespace Game2048
{
    public class Sizes : ViewModelBase
    {
        private double _width;

        /// <summary>
        /// Gets or sets Width value.
        /// </summary>
        public double Width
        {
            get => _width;
            set => SetValue(ref _width, value);
        }

        private double _width70Percent;

        /// <summary>
        /// Gets or sets Width70Percent value.
        /// </summary>
        public double Width70Percent
        {
            get => _width70Percent;
            set => SetValue(ref _width70Percent, value);
        }

        private double _unit;

        /// <summary>
        /// Gets or sets Unit value.
        /// </summary>
        public double Unit
        {
            get => _unit;
            set => SetValue(ref _unit, value);
        }

        private double _margin;

        /// <summary>
        /// Gets or sets Margin value.
        /// </summary>
        public double Margin
        {
            get => _margin;
            set => SetValue(ref _margin, value);
        }

        private CornerRadius _cornerRadius;

        /// <summary>
        /// Gets or sets CornerRadius value.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set => SetValue(ref _cornerRadius, value);
        }

        private static Sizes _instance;

        public static Sizes Instance => _instance ?? (_instance = new Sizes());

        private Sizes()
        {
        }

        public static void SetScreen(double width, double height)
        {
            var min = Math.Min(width, height);
            Instance.SetWidth(min);
        }

        private void SetWidth(double width)
        {
            Width = width;
            Width70Percent = 0.7 * width;
            Unit = 0.15 * width;
            Margin = 0.02 * width;
            CornerRadius = new CornerRadius(0.02 * width);
        }
    }
}
