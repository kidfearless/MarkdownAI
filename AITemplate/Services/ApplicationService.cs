using AIProvider;

namespace AITemplate.Services;

/// <summary>
/// Service that orchestrates the application flow
/// </summary>
public class ApplicationService
{
    private readonly FileManager _fileManager;
    private readonly ParserService _parserService;
    private readonly AiProviderService _aiProviderService;

    /// <summary>
    /// Initializes a new instance of the ApplicationService
    /// </summary>
    public ApplicationService()
    {
        _fileManager = new FileManager();
        _parserService = new ParserService();
        _aiProviderService = new AiProviderService();
    }

    /// <summary>
    /// Processes a markdown AI file, sends it to the AI provider, and optionally updates the file with the response
    /// </summary>
    /// <param name="filePath">Path to the file to process</param>
    /// <param name="updateFile">Whether to update the file with the AI response</param>
    /// <returns>The AI response</returns>
    public async Task ProcessFileAsync(string filePath)
    {
        // Validate file
        var validationResult = _fileManager.ValidateFile(filePath);

        if (!validationResult.IsValid)
        {
            throw new FileNotFoundException($"File '{filePath}' not found.");
        }

        if (!validationResult.IsMdaiFile && !validationResult.IsMdFile)
        {
            Console.WriteLine($"Warning: '{filePath}' does not have a recognized extension (.mdai or .md).");
            Console.WriteLine("The file may not be parsed correctly if it doesn't follow the expected format.");
            Console.WriteLine("Press Enter to continue anyway, or Ctrl+C to exit.");
            Console.ReadLine();
        }

        // Parse file
        var parserResult = _parserService.Parse(filePath);

        if (parserResult.Messages.Count == 0)
        {
            Console.WriteLine("Warning: No messages found in the file.");
        }

        // Create chat session and get response
        var chatSession = _aiProviderService.CreateChatSession(parserResult.ProviderConfig, parserResult.Messages);

        await using var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
        await using var writer = new StreamWriter(fileStream);
        writer.AutoFlush = true;
        try
        {
            writer.WriteLine("\nAssistant:");
            await foreach (var response in chatSession.StreamResponseAsync())
            {
                writer.Write(response.Content);
            }

            writer.Write("\n\nUser:");

            Console.WriteLine($"File '{filePath}' has been updated with the AI response.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not update the file with the response: {ex.Message}");
        }
    }
}
