using System.Diagnostics;
using AIProvider.Messages;
using Microsoft.Extensions.AI;

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
    public List<string> FileReferences { get; }

    public UserBlock(string rawContent, List<string> fileReferences) : base(rawContent)
    {
        FileReferences = fileReferences;
    }

    public UserBlock(string rawContent) : this(rawContent, new List<string>(2)) { }

    public override Message ToMessage()
    {
        var msg = new UserMessage(RawContent);
        if (FileReferences.Count > 0)
        {
            foreach (var file in FileReferences)
            {
                var path = file;
                if (path.StartsWith("!"))
                {
                    int start = path.IndexOf('(');
                    int end = path.LastIndexOf(')');
                    if (start >= 0 && end > start)
                    {
                        var filePath = path.Substring(start + 1, end - start - 1).Trim();
                        if (System.IO.Directory.Exists(filePath))
                        {
                            var files = System.IO.Directory.GetFiles(filePath);
                            foreach (var f in files)
                            {
                                ProcessFile(f, ref msg);
                            }
                        }
                        else if (System.IO.File.Exists(filePath))
                        {
                            ProcessFile(filePath, ref msg);
                        }
                    }
                }
            }
        }
        return msg;
    }

    private static void ProcessFile(string filePath, ref UserMessage msg)
    {
        var mediaType = MimeTypes.MimeTypeMap.GetMimeType(filePath);
        
        // Check if this is a text-based file
        if (mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) || 
            mediaType is "application/json" 
                    or "application/xml" 
                    or "application/javascript")
        {
            // For text files, read as text and include directly in content
            var fileContent = System.IO.File.ReadAllText(filePath);
            var fileName = System.IO.Path.GetFileName(filePath);
            
            // Build a markdown-style reference to the embedded file
            var contentBuilder = new System.Text.StringBuilder(msg.Content);
            contentBuilder.AppendLine("File References:");
            contentBuilder.AppendLine();
            contentBuilder.AppendLine($"```{fileName}");
            contentBuilder.AppendLine(fileContent);
            contentBuilder.AppendLine("```");
            
            // Create a new message with the updated content
            msg = new UserMessage(contentBuilder.ToString()) 
            { 
                Files = msg.Files 
            };
        }
        else
        {
            // For binary files (images, PDFs, etc.), use WithFile
            var data = System.IO.File.ReadAllBytes(filePath);
            msg = msg.WithFile(new Microsoft.Extensions.AI.DataContent(data, mediaType));
        }
    }
}

/// <summary>
/// Represents an assistant message block in the markdown document
/// </summary>
public record AssistantBlock : MarkdownBlock
{
    public AssistantBlock(string rawContent) : base(rawContent) { }

    public override Message ToMessage() => new AssistantMessage(RawContent);
}
