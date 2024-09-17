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

        private async void ChatBtn_Clicked(object sender, EventArgs e)
        {
            ChatBtn.IsEnabled = false;

            await _authenticationService.SignInAsync();

            await DisplayAlert("Success", "You are now signed in", "OK");

            using (ChatService chatService = new ChatService(_authenticationService))
            {
                var response = await chatService.GetChatResponseAsync(EntryQuery.Text);
                await DisplayAlert("Response", response, "OK");
            }

            ChatBtn.IsEnabled = true;

        }

        private async void PostImageBtn_Clicked(object sender, EventArgs e)
        {
            var result = await MainCameraView.CaptureAsync();
            using (ChatService chatService = new ChatService(_authenticationService))
            {
                var stream = await result!.OpenReadAsync(quality: 40);
                var response = await chatService.GetImageResponseAsync(stream);
                await DisplayAlert("Response", response, "OK");
            }
        }
    }
}
