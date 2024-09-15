using VisionAid.MobileApp.Services;

namespace VisionAid.MobileApp
{
    public partial class App : Application
    {
        public App(AuthenticationService authenticationService)
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
