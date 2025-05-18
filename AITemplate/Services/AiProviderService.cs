using AIProvider;
using AIProvider.Messages;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static AIProvider.Provider; // Import the Provider static class to access ChatSession

namespace AITemplate.Services;

/// <summary>
/// Service responsible for interacting with AI providers
/// </summary>
public struct AiProviderService
{
    /// <summary>
    /// Creates a chat session with the specified provider configuration and messages
    /// </summary>
    /// <param name="providerConfig">The provider configuration</param>
    /// <param name="messages">The chat messages</param>
    /// <returns>A configured chat session</returns>
    public ChatSession CreateChatSession(ProviderConfig providerConfig, List<Message> messages)
    {
        if(providerConfig.Url != null)
        {
            providerConfig.Options!.TryAdd($"Provider:{providerConfig.ProviderType}:Url", providerConfig.Url);
        }

        // Create configuration
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(providerConfig.Options);
        var config = configBuilder.Build();

        // Create provider
        var provider = Provider.GetProvider(providerConfig.ProviderType, providerConfig.ApiKey, config);
        
        // Create chat model
        var chatModel = new ChatModel(providerConfig.ModelName);

        // Create and initialize chat session
        var chatSession = provider.CreateChatSession(chatModel);
        chatSession.MaxOutputTokens = providerConfig.MaxTokens;
        chatSession.ShortTermMemoryLength = providerConfig.ShortTermMemory;
        chatSession.Messages = messages;
        
        return chatSession;
    }

}
