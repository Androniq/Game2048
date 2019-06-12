using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Game2048.Mvvm;

namespace Game2048
{
    public class TileVm : ViewModelBase
    {
        public event Action<int, int> Animate;

        public event Action AnimateInitial;

        public int X { get; }

        public int Y { get; }

        /// <summary>
        /// Only for animation.
        /// </summary>
        public bool IsMoving { get; set; }

        private int _value;

        /// <summary>
        /// Gets or sets Value value.
        /// </summary>
        public int Value
        {
            get => _value;
            set => SetValue(ref _value, value);
        }

        private int _value2;

        /// <summary>
        /// Gets or sets Value2 value.
        /// </summary>
        public int Value2
        {
            get => _value2;
            set => SetValue(ref _value2, value);
        }

        private bool _hasValue;

        /// <summary>
        /// Gets or sets HasValue value.
        /// </summary>
        public bool HasValue
        {
            get => _hasValue;
            set => SetValue(ref _hasValue, value);
        }

        public TileVm()
        {
        }

        public TileVm(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Clear()
        {
            HasValue = false;
            Value = 0;
        }

        public void SetValue(int value)
        {
            if (Value == 0)
            {
                Value = value;
                Value2 = value;
                HasValue = true;
            }
            else
            {
                Value = value;
                HasValue = true;
                Task.Delay(200).ContinueWith(it => Value2 = value);
            }
        }

        public void SetAnimation(TileVm from)
        {
            RaiseAnimate(from.Y - Y, from.X - X);
        }

        public void SetInitialAnimation()
        {
            RaiseAnimateInitial();
        }

        protected virtual void RaiseAnimate(int x, int y)
        {
            Animate?.Invoke(x, y);
        }

        protected virtual void RaiseAnimateInitial()
        {
            AnimateInitial?.Invoke();
        }
    }
}
