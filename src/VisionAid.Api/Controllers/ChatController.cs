using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VisionAid.Api.Models;
using VisionAid.Api.Services;

namespace VisionAid.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(ChatService _chatService) : ControllerBase
    {
        [HttpPost("Chat")]
        public async Task<ActionResult<ChatResponse>> Chat(
            string message,
            string? prompt = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("The message cannot be null or empty.");
            }

            var stopwatch = Stopwatch.StartNew();
            var response = await _chatService.GetResponse(message, prompt, cancellationToken);
            stopwatch.Stop();

            return Ok(new ChatResponse { Message = response, Duration = stopwatch.Elapsed });
        }

        [HttpPost("Upload")]
        public async Task<ActionResult<ChatResponse>> UploadImage(
            IFormFile file,
            string? prompt = null,
            CancellationToken cancellationToken = default)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ReadOnlyMemory<byte> readOnlyMemory = default;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                readOnlyMemory = new ReadOnlyMemory<byte>(fileBytes);
            }
            
            var response = await _chatService.GetResponse(readOnlyMemory, file.ContentType, prompt, cancellationToken);

            stopwatch.Stop();

            return Ok(new ChatResponse { Message = response, Duration = stopwatch.Elapsed });
        }

        [HttpPost("Navigate")]
        public async Task<ActionResult<ChatResponse>> Navigate(
            IFormFile file,
            string navigationInstructions,
            string? prompt = null,
            CancellationToken cancellationToken = default)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ReadOnlyMemory<byte> readOnlyMemory = default;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                readOnlyMemory = new ReadOnlyMemory<byte>(fileBytes);
            }
            
            var response = await _chatService.GetResponse(readOnlyMemory, file.ContentType, navigationInstructions, prompt, cancellationToken);

            stopwatch.Stop();

            return Ok(new ChatResponse { Message = response, Duration = stopwatch.Elapsed });
        }
    }
}