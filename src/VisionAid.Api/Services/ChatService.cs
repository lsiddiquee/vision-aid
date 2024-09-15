using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace VisionAid.Api.Services;

public class ChatService(IChatCompletionService _chatCompletionService)
{
    public async Task<string> GetResponse(
        string message,
        string? prompt = null,
        CancellationToken cancellationToken = default)
    {
        var chatHistory = prompt == null ? new ChatHistory() : new ChatHistory(prompt);
        chatHistory.AddUserMessage(message);
        var response = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);

        return response.Content ?? "I'm sorry, I don't understand.";
    }

    public async Task<string> GetResponse(
        ReadOnlyMemory<byte> data,
        string? mimeType,
        string? prompt = null,
        CancellationToken cancellationToken = default)
    {
        var chatHistory = prompt == null ? new ChatHistory(Prompts.GetImageProcessingPrompt(0)) : new ChatHistory(prompt);

        chatHistory.AddUserMessage(new ChatMessageContentItemCollection
        {
            new ImageContent(data, mimeType)
        });

        var response = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);

        return response.Content ?? "I'm sorry, I don't understand.";
    }

    public async Task<string> GetResponse(
        ReadOnlyMemory<byte> data,
        string? mimeType,
        string navigationInstructions,
        string? prompt = null,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt = prompt ?? Prompts.GetImageProcessingPrompt(3);
        var chatHistory = new ChatHistory($"{systemPrompt}\nNavigation Instructions:{navigationInstructions}");

        chatHistory.AddUserMessage(new ChatMessageContentItemCollection
        {
            new ImageContent(data, mimeType)
        });

        var response = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);

        return response.Content ?? "I'm sorry, I don't understand.";
    }
}