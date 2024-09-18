using System.Diagnostics;
using VisionAid.MobileApp.Services;

namespace VisionAid.MobileApp
{
    public partial class MainPage : ContentPage
    {
        private Queue<Stream> _imageQueue = new Queue<Stream>();
        private System.Timers.Timer? _imageCaptureTimer = null;
        private System.Timers.Timer? _imagePostTimer = null;
        private readonly AuthenticationService _authenticationService;
        private object _lock = new object();

        public MainPage()
        {
            InitializeComponent();

            _authenticationService = new AuthenticationService(); // TODO: DI

            DeviceDisplay.Current.KeepScreenOn = true;
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
            using ChatService chatService = new ChatService(_authenticationService);
            using (e.Media)
            {
                AddNewImage(ResizeImage(e.Media));
            }

            await Task.CompletedTask;
        }

        private void StartRealTimeMonitoringBtn_Clicked(object sender, EventArgs e)
        {
            SetButtonsIsEnabled(_imageCaptureTimer != null);

            if (_imageCaptureTimer != null && _imagePostTimer != null)
            {
                StartRealTimeMonitoringBtn.Text = "Start";
                _imageCaptureTimer.Stop();
                _imageCaptureTimer.Dispose();
                _imageCaptureTimer = null;
                _imagePostTimer.Stop();
                _imagePostTimer.Dispose();
                _imagePostTimer = null;
            }
            else
            {
                StartRealTimeMonitoringBtn.Text = "Stop";
                (_imageCaptureTimer, _imagePostTimer) = StartRealtimeDetection();
            }
        }

        private async Task ProcessStream(ChatService chatService, Stream imageStream)
        {
            //var fileName = $"VisionAid_{DateTime.Now:yy_MM_dd_HH_mm_ss}.png";
            //var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream);

            await _authenticationService.SignInAsync();

            imageStream.Position = 0;
            var stopwatch = Stopwatch.StartNew();
            var response = await chatService.GetImageResponseAsync(imageStream);
            stopwatch.Stop();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LblResponse.Text = response;
                LblOpenAIResponseTime.Text = $"Response Time: {stopwatch.ElapsedMilliseconds}ms";
            });
        }

        private async Task ProcessStreams(ChatService chatService, Stream[] imageStreams)
        {
            //var fileName = $"VisionAid_{DateTime.Now:yy_MM_dd_HH_mm_ss}.png";
            //var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream);

            await _authenticationService.SignInAsync();

            var stopwatch = Stopwatch.StartNew();
            var response = await chatService.GetImageResponseForMultiStreamAsync(imageStreams);
            stopwatch.Stop();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LblResponse.Text = response;
                LblOpenAIResponseTime.Text = $"Response Time: {stopwatch.ElapsedMilliseconds}ms";
            });
        }

        private void SetButtonsIsEnabled(bool isEnabled)
        {
            MainThread.BeginInvokeOnMainThread(() => PostImageBtn.IsEnabled = ChatBtn.IsEnabled = isEnabled);
        }

        private Stream ResizeImage(Stream imageStream, int maxWidth = Configuration.MaxWidth)
        {
            using SkiaSharp.SKBitmap image = SkiaSharp.SKBitmap.Decode(imageStream);

            if (image.Width > maxWidth)
            {
                float newHeight = ((float)image.Height / image.Width) * maxWidth;
                var newImage = image.Resize(new SkiaSharp.SKImageInfo(maxWidth, (int)newHeight), SkiaSharp.SKFilterQuality.Medium);

                var newImageStream = new MemoryStream();
                newImage.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).SaveTo(newImageStream);

                return newImageStream;
            }

            var originalImageStream = new MemoryStream();
            image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).SaveTo(originalImageStream);
            return originalImageStream;
        }

        private (System.Timers.Timer, System.Timers.Timer) StartRealtimeDetection()
        {
            var captureTime = new System.Timers.Timer(Configuration.ImageCaptureIntervalInMilliseconds);

            captureTime.Elapsed += async (sender, e) => await MainCameraView.CaptureImage(default);
            captureTime.AutoReset = true;
            captureTime.Start();

            var postTime = new System.Timers.Timer(Configuration.ImagePostIntervalInMilliseconds);

            postTime.Elapsed += async (sender, e) => await PostImages();
            postTime.AutoReset = true;
            postTime.Start();

            return (captureTime, postTime);
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

            MainThread.BeginInvokeOnMainThread(() => LblBufferStatus.Text = $"Buffer: {_imageQueue.Count}");
        }

        private Stream[] GetImageStreams()
        {
            MainThread.BeginInvokeOnMainThread(() => LblBufferStatus.Text = "Buffer: 0");

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

        private async Task PostImages()
        {
            using ChatService chatService = new ChatService(_authenticationService);
            var images = GetImageStreams();

            if (images.Length > 0)
            {
                await ProcessStreams(chatService, images);
            }

            foreach (var image in images)
            {
                image.Dispose();
            }
        }
    }
}
