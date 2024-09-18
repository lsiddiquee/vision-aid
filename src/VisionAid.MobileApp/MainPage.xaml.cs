using System.Reflection.Metadata;
using Microsoft.Maui.Graphics.Platform;
using VisionAid.MobileApp.Services;

namespace VisionAid.MobileApp
{
    public partial class MainPage : ContentPage
    {
        private Queue<Stream> _imageQueue = new Queue<Stream>();
        private int _currentImageIndex = 0;
        private System.Timers.Timer? _timer = null;
        private readonly AuthenticationService _authenticationService;
        private object _lock = new object();

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

            SetButtonsIsEnabled(false);

            await _authenticationService.SignInAsync();

            using (ChatService chatService = new ChatService(_authenticationService))
            {
                var response = await chatService.GetChatResponseAsync(EntryQuery.Text);
                await DisplayAlert("Response", response, "OK");
            }

            SetButtonsIsEnabled(true);
        }

        private async void PostImageBtn_Clicked(object sender, EventArgs e)
        {
            SetButtonsIsEnabled(false);
            await MainCameraView.CaptureImage(default);
        }

        private async void MainCameraView_MediaCaptured(object sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
        {
            using (ChatService chatService = new ChatService(_authenticationService))
            {
                await ProcessStream(chatService, e.Media);
            }

            // SetButtonsIsEnabled(true);
        }

        private void StartRealTimeMonitoringBtn_Clicked(object sender, EventArgs e)
        {
            SetButtonsIsEnabled(_timer != null);

            if (_timer == null)
            {
                StartRealTimeMonitoringBtn.Text = "Stop";
                _timer = StartRealtimeDetection();
            }
            else
            {
                StartRealTimeMonitoringBtn.Text = "Start";
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        private async Task ProcessStream(ChatService chatService, Stream imageStream)
        {
            using (imageStream)
            {
                //var fileName = $"VisionAid_{DateTime.Now:yy_MM_dd_HH_mm_ss}.png";
                //var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream);

                await _authenticationService.SignInAsync();

                imageStream.Position = 0;
                using (var newImageStream = ResizeImage(imageStream))
                {
                    var response = await chatService.GetImageResponseAsync(newImageStream);

                    MainThread.BeginInvokeOnMainThread(() => LblResponse.Text = response);
                }
            }
        }

        private void SetButtonsIsEnabled(bool isEnabled)
        {
            MainThread.BeginInvokeOnMainThread(() => PostImageBtn.IsEnabled = ChatBtn.IsEnabled = isEnabled);
        }

        private Stream ResizeImage(Stream imageStream, int maxWidth = 720)
        {
            var image = PlatformImage.FromStream(imageStream, ImageFormat.Png);

            if (image.Width > maxWidth)
            {
                float newHeight = (image.Height / image.Width) * maxWidth;
                var newImage = image.Resize(maxWidth, newHeight, ResizeMode.Fit);

                return newImage.AsStream();
            }
            
            imageStream.Position = 0;
            return imageStream;
        }

        private System.Timers.Timer StartRealtimeDetection()
        {
            var timer = new System.Timers.Timer(new TimeSpan(0, 0, 5));

            timer.Elapsed += async (sender, e) => await MainCameraView.CaptureImage(default);
            timer.AutoReset = true;
            timer.Start();

            return timer;
        }

        private void AddNewImage(Stream imageStream)
        {
            lock (_lock)
            {
                if (_imageQueue.Count == Configuration.Image_Buffer_Size)
                {
                    var imageToRemove = _imageQueue.Dequeue();
                    imageToRemove.Dispose();
                }
                
                _imageQueue.Enqueue(imageStream);
            }
        }

        private Stream[] GetImageStreams()
        {
            lock (_lock)
            {
                Stream[] images = new Stream[_imageQueue.Count];
                int index = 0;
                while (_imageQueue.Count > 0)
                {
                    images[index++] = _imageQueue.Dequeue();
                }

                return images;
            }
        }
    }
}
