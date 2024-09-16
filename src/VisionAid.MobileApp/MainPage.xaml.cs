using VisionAid.MobileApp.Services;

namespace VisionAid.MobileApp
{
    public partial class MainPage : ContentPage
    {
        private readonly AuthenticationService _authenticationService;

        public MainPage()
        {
            InitializeComponent();

            _authenticationService = new AuthenticationService(); // TODO: DI
        }

        private async void LoginBtn_Clicked(object sender, EventArgs e)
        {
            LoginBtn.IsEnabled = false;

            await _authenticationService.SignInAsync();

            await DisplayAlert("Success", "You are now signed in", "OK");

            using (ChatService chatService = new ChatService(_authenticationService))
            {
                var response = await chatService.GetChatResponseAsync("Hello");
                await DisplayAlert("Response", response, "OK");
            }

            LoginBtn.IsEnabled = true;

        }
    }
}
