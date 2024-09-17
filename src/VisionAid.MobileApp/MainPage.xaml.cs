using CommunityToolkit.Maui.Storage;
using System.Threading;
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
            if (string.IsNullOrEmpty(EntryQuery.Text))
            {
                await DisplayAlert("Error", "Please ensure the query text is not empty", "OK");
                return;
            }

            ChatBtn.IsEnabled = false;

            await _authenticationService.SignInAsync();

            using (ChatService chatService = new ChatService(_authenticationService))
            {
                var response = await chatService.GetChatResponseAsync(EntryQuery.Text);
                await DisplayAlert("Response", response, "OK");
            }

            ChatBtn.IsEnabled = true;

        }

        private async void PostImageBtn_Clicked(object sender, EventArgs e)
        {
            PostImageBtn.IsEnabled = false;
            await MainCameraView.CaptureImage(default);
            PostImageBtn.IsEnabled = true;
            //var result = await MainCameraView.CaptureAsync();
            //using (ChatService chatService = new ChatService(_authenticationService))
            //{
            //    using (var stream = await result!.OpenReadAsync(quality: 40))
            //    {
            //        await ProcessStream(chatService, stream);
            //    }
            //}
        }

        private async void MainCameraView_MediaCaptured(object sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
        {
            PostImageBtn.IsEnabled = false;

            using (ChatService chatService = new ChatService(_authenticationService))
            {
                await ProcessStream(chatService, e.Media);
            }

            MainThread.BeginInvokeOnMainThread(() => PostImageBtn.IsEnabled = true);
        }

        private async Task ProcessStream(ChatService chatService, Stream stream)
        {
            using (stream)
            {
                //var fileName = $"VisionAid_{DateTime.Now:yy_MM_dd_HH_mm_ss}.png";
                //var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream);

                await _authenticationService.SignInAsync();

                stream.Position = 0;
                var response = await chatService.GetImageResponseAsync(stream);

                MainThread.BeginInvokeOnMainThread(() => LblResponse.Text = response);
            }
        }
    }
}
