using Microsoft.AspNetCore.Mvc;
using VisionAid.Api.Models;
using VisionAid.Api.Services;

namespace VisionAid.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(ChatService _chatService) : ControllerBase
    {
        [HttpPost("Upload")]
        public async Task<ActionResult<ChatResponse>> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            ReadOnlyMemory<byte> readOnlyMemory = default;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                readOnlyMemory = new ReadOnlyMemory<byte>(fileBytes);
            }
            
            var response = await _chatService.GetResponse(readOnlyMemory, file.ContentType, cancellationToken);

            return Ok(new ChatResponse { Message = response });
        }

        [HttpPost("Chat")]
        public async Task<ActionResult<ChatResponse>> Chat(string message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("The message cannot be null or empty.");
            }

            var response = await _chatService.GetResponse(message, cancellationToken);

            return Ok(new ChatResponse { Message = response });
        }
    }
}