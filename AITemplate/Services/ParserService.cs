using AIProvider.Messages;
using AITemplate.Parser;

namespace AITemplate.Services;

/// <summary>
/// Represents the result of parsing a markdown AI file
/// </summary>
public record struct ParserResult(ProviderConfig ProviderConfig, List<Message> Messages);

/// <summary>
/// Service responsible for parsing markdown AI files and extracting configuration and messages
/// </summary>
public struct ParserService
{
    private readonly MarkdownParser _markdownParser;
    private readonly ConfigParser _configParser;

    /// <summary>
    /// Initializes a new instance of the ParserService
    /// </summary>
    public ParserService()
    {
        _markdownParser = new MarkdownParser();
        _configParser = new ConfigParser();
    }

    public ParserResult Parse(string filePathOrContent)
    {
        // Parse the file
        var blocks = _markdownParser.ParseFile(filePathOrContent);

        var configBlock = blocks.FirstOrDefault(b => b is ConfigBlock) ?? throw new InvalidOperationException("No config block found.");
        var providerConfig = _configParser.Parse(configBlock.RawContent.AsSpan());

        var messages = blocks
            .TakeLast(providerConfig.ShortTermMemory)
            .Select(b => b.ToMessage())
            .Where(m => m != null)
            .ToList();

        return new ParserResult(providerConfig, messages!);
    }
}