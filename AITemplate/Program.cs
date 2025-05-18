using AIProvider;
using AIProvider.Messages;
using AITemplate.Parser;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;
using AITemplate.Services;
using AITemplate;


try
{
    FileTypeRegistration.RegisterFileType();
    FileTypeRegistration.AddApplicationToPath();
    FileTypeRegistration.CreateApplicationLink();

    if (args.Length < 1)
    {
        DisplayUsageInformation();
        return;
    }

    // Parse command-line arguments
    string filePath = args[0];
    
    // Process the file using the ApplicationService
    var applicationService = new ApplicationService();
    await applicationService.ProcessFileAsync(filePath);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

/// <summary>
/// Displays usage information for the application
/// </summary>
static void DisplayUsageInformation()
{
    Console.WriteLine("""
Usage: AITemplate <file-path>


This application is registered to handle .mdai files.
You can simply double-click a .mdai file to process it.

Format for .mdai files:
  config:        - Configuration section (provider, API key, model)
  system:/s:     - System prompt
  user:/u:       - User message
  assistant:/a:  - Assistant message
""");
}
