using System.Net;
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

            using (var content = new MultipartFormDataContent())
            {
                var imageContent = new StreamContent(imageStream);
                content.Add(imageContent, "file", "test.png");

                var response = await _httpClient.PostAsync("api/Chat/Upload", content);
                response.EnsureSuccessStatusCode();

                var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>() ?? throw new ApplicationException();

                return chatResponse.Message;
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
