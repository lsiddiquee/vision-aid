using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace VisionAid.MobileApp.Services
{
    public class ChatService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationService _authenticationService;

        private class ChatResponse
        {
            public required string Message { get; set; }
            public required TimeSpan Duration { get; set; }
        }

        public ChatService(AuthenticationService authenticationService)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(Configuration.ApiUrl) };
            _authenticationService = authenticationService;
        }

        public async Task<string> GetChatResponseAsync(string message)
        {
            await SetAuthenticationHeaderAsync();
            var response = await _httpClient.PostAsync($"api/Chat/Chat?message={Uri.EscapeDataString(message)}", null);
            response.EnsureSuccessStatusCode();

            var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>() ?? throw new ApplicationException();

            return chatResponse.Message;
        }

        public async Task<string> GetImageResponseAsync(Stream imageStream)
        {
            await SetAuthenticationHeaderAsync();

            using var content = new MultipartFormDataContent();
            using var imageContent = new StreamContent(imageStream);

            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            content.Add(imageContent, "file", $"VisionAid_{DateTime.Now:yy_MM_dd_HH_mm_ss}.png");

            var response = await _httpClient.PostAsync("api/Chat/Upload", content);
            response.EnsureSuccessStatusCode();

            var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>() ?? throw new ApplicationException();

            return chatResponse.Message;
        }

        public async Task<string> GetImageResponseForMultiStreamAsync(Stream[] imageStreams)
        {
            await SetAuthenticationHeaderAsync();

            using var content = new MultipartFormDataContent();

            int i = 0;
            foreach (var imageStream in imageStreams)
            {
                imageStream.Position = 0;
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"files\"",
                    FileName = $"\"VisionAid_{i++}.png\""
                }; // the extra quotes are key here

                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                content.Add(imageContent);
            }

            try
            {
                var response = await _httpClient.PostAsync("api/Chat/Navigate?navigationInstructions=test", content);
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    return responseStr;
                }
                response.EnsureSuccessStatusCode();

                var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>() ?? throw new ApplicationException();

                return chatResponse.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private async Task SetAuthenticationHeaderAsync()
        {
            var token = await _authenticationService.SignInAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }
    }
}
