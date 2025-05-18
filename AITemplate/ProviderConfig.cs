namespace AITemplate;

/// <summary>
/// Configuration for an AI provider
/// </summary>
public record struct ProviderConfig
{
    /// <summary>
    /// The type of the provider (e.g., "OpenAI", "AzureOpenAI")
    /// </summary>
    public string ProviderType
    {
        get
        {
            return Options?.GetValueOrDefault("ProviderType")
            ?? Options?.GetValueOrDefault("Provider")
            ?? "OpenAI";
        }
    }

    /// <summary>
    /// The API key for the provider
    /// </summary>
    public string ApiKey
    {
        get
        {
            return Options?.GetValueOrDefault("ApiKey")
            ?? Options?.GetValueOrDefault("Api_Key")
            ?? "";
        }
    }

    /// <summary>
    /// The name of the model to use
    /// </summary>
    public string ModelName
    {
        get
        {
            return Options?.GetValueOrDefault("Model")
            ?? Options?.GetValueOrDefault("ModelName")
            ?? Options?.GetValueOrDefault("Model_Name")
            ?? Options?.GetValueOrDefault("Deployment")
            ?? string.Empty;
        }
    }

    public string? Url => Options?.GetValueOrDefault("Url");

    public ulong? MaxTokens
    {
        get
        {
            ulong? maxTokens = null;
            if (Options.TryGetValue("MaxTokens", out var maxTokensStr))
            {
                if (ulong.TryParse(maxTokensStr, out var parsedMaxTokens))
                {
                    maxTokens = parsedMaxTokens;
                }
            }
            else if (Options.TryGetValue("Max_Tokens", out var maxTokensStr2))
            {
                if (ulong.TryParse(maxTokensStr2, out var parsedMaxTokens))
                {
                    maxTokens = parsedMaxTokens;
                }
            }
            else if (Options.TryGetValue("MaxOutputTokens", out var maxTokensStr3))
            {
                if (ulong.TryParse(maxTokensStr3, out var parsedMaxTokens))
                {
                    maxTokens = parsedMaxTokens;
                }
            }
            else if (Options.TryGetValue("Max_Output_Tokens", out var maxTokensStr4))
            {
                if (ulong.TryParse(maxTokensStr4, out var parsedMaxTokens))
                {
                    maxTokens = parsedMaxTokens;
                }
            }
            return maxTokens;
        }
    }

    public int ShortTermMemory
    {
        get
        {
            int? shortTermMemory = null;
            if (Options.TryGetValue("ShortTermMemory", out var shortTermMemoryStr))
            {
                if (int.TryParse(shortTermMemoryStr, out var parsedShortTermMemory))
                {
                    shortTermMemory = parsedShortTermMemory;
                }
            }
            else if (Options.TryGetValue("Short_Term_Memory", out var shortTermMemoryStr2))
            {
                if (int.TryParse(shortTermMemoryStr2, out var parsedShortTermMemory))
                {
                    shortTermMemory = parsedShortTermMemory;
                }
            }
            return shortTermMemory ?? 20;
        }
    }

    /// <summary>
    /// Additional options for the provider
    /// </summary>
    public Dictionary<string, string?> Options;
}
