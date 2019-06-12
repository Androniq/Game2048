using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Game2048.Mvvm
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T variable, T value, Action<T> callback = null, [CallerMemberName] string propertyName = null)
        {
            if (Equal(variable, value))
                return;
            variable = value;
            OnPropertyChanged(propertyName);
            callback?.Invoke(value);
        }

        protected void SetValue<T>(ref T variable, T value, Action<T, T> callback, [CallerMemberName] string propertyName = null)
        {
            if (Equal(variable, value))
                return;
            var oldValue = variable;
            variable = value;
            OnPropertyChanged(propertyName);
            callback?.Invoke(oldValue, value);
        }

        protected void SetValue<T>(ref T variable, T value, Action callback, [CallerMemberName] string propertyName = null)
        {
            if (Equal(variable, value))
                return;
            variable = value;
            OnPropertyChanged(propertyName);
            callback?.Invoke();
        }

        private static bool Equal<T>(T a, T b)
        {
            var isNullA = ReferenceEquals(a, null);
            var isNullB = ReferenceEquals(b, null);
            if (isNullA && isNullB) return true;
            if (isNullA || isNullB) return false;
            return a.Equals(b);
        }
    }
}
