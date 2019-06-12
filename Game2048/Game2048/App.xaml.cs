using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Game2048
{
    public partial class App : Application
    {
        private readonly MainVm _mainVm;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            _mainVm = (MainVm)MainPage.BindingContext;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            _mainVm.LoadState();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
