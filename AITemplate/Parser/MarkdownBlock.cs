using AIProvider.Messages;

namespace AITemplate.Parser;

/// <summary>
/// Base class for all markdown blocks in the document
/// </summary>
public abstract record MarkdownBlock
{
    public string RawContent { get; }

    protected MarkdownBlock(string rawContent)
    {
        RawContent = rawContent.Trim();
    }

    /// <summary>
    /// Converts the block to a message if applicable
    /// </summary>
    public virtual Message? ToMessage() => null;
}

/// <summary>
/// Represents a config block in the markdown document
/// </summary>
public record ConfigBlock : MarkdownBlock
{
    public ConfigBlock(string rawContent) : base(rawContent) { }
}

/// <summary>
/// Represents a system message block in the markdown document
/// </summary>
public record SystemBlock : MarkdownBlock
{
    public SystemBlock(string rawContent) : base(rawContent) { }

    public override Message ToMessage() => new SystemPromptMessage(RawContent);
}

/// <summary>
/// Represents a user message block in the markdown document
/// </summary>
public record UserBlock : MarkdownBlock
{
    public UserBlock(string rawContent) : base(rawContent) { }

    public override Message ToMessage() => new UserMessage(RawContent);
}

/// <summary>
/// Represents an assistant message block in the markdown document
/// </summary>
public record AssistantBlock : MarkdownBlock
{
    public AssistantBlock(string rawContent) : base(rawContent) { }

    public override Message ToMessage() => new AssistantMessage(RawContent);
}
