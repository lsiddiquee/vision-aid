using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace VisionAid.Api.Services;

public class ChatService(IChatCompletionService _chatCompletionService)
{
    public async Task<string> GetResponse(string message, CancellationToken cancellationToken)
    {
        var response = await _chatCompletionService.GetChatMessageContentAsync(message, cancellationToken: cancellationToken);

        return response.Content ?? "I'm sorry, I don't understand.";
    }

    public async Task<string> GetResponse(ReadOnlyMemory<byte> data, string? mimeType, CancellationToken cancellationToken)
    {
        var chatHistory = new ChatHistory(Prompts.GetImageProcessingPrompt());

        chatHistory.AddUserMessage(new ChatMessageContentItemCollection
        {
            new ImageContent(data, mimeType)
        });

        var response = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);

        return response.Content ?? "I'm sorry, I don't understand.";
    }
}