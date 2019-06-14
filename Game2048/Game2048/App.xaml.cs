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
            _mainVm.Init();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public async void Handle(object sender, UnhandledExceptionEventArgs args)
        {
            await MainPage.DisplayAlert(args.ExceptionObject?.GetType().Name ?? "NULL",
                (args.ExceptionObject as Exception)?.Message ?? "NULL", "OK");
        }
    }
}
