using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Game2048
{
    public partial class MainPage : ContentPage
    {
        private MainVm _bindingContext;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Sizes.SetScreen(width, height);
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            if (_bindingContext != null)
            {
                _bindingContext.Alert -= Alert;
            }
            _bindingContext = BindingContext as MainVm;
            if (_bindingContext != null)
            {
                _bindingContext.Alert += Alert;
            }
        }

        private async void Alert(string message)
        {
            await DisplayAlert("Message", message, "OK");
            _bindingContext.Reset.Execute(null);
        }

        public void SetProgress(int percentage)
        {
            Filler.WidthRequest = Back.Width * percentage / 100;
        }
    }
}
