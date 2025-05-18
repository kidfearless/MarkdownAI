# AI Template

A C# application that reads and parses markdown-like files containing AI conversation prompts. The application interacts with AI providers, registers as the default handler for `.mdai` files in Windows, and updates the original files with AI responses.

## Project Structure

The project has been organized into a more modular structure:

```
AITemplate/
├── Program.cs                  # Entry point for the application
├── ProviderConfig.cs           # Configuration for AI providers
├── FileManager.cs              # Handles file I/O and updating
├── FileTypeRegistration.cs     # Registers as handler for .mdai files
├── Services/
│   ├── ApplicationService.cs   # Orchestrates application flow
│   ├── ParserService.cs        # Parses markdown AI files
│   ├── AiProviderService.cs    # Interacts with AI providers
└── Parser/
    ├── MarkdownBlock.cs        # Base class for markdown blocks
    ├── MarkdownParser.cs       # Parses markdown content
    ├── ConfigParser.cs         # Parses configuration blocks
    ├── SpanExtensions.cs       # Helper methods for span operations
    └── StringBuilderPool.cs    # Pool for reusing StringBuilder instances
```

## Features

- **File Parsing**: Parses markdown-like files containing AI conversation prompts
- **AI Provider Integration**: Interacts with various AI providers (OpenAI, Azure OpenAI)
- **File Type Registration**: Registers as the default handler for `.mdai` files in Windows
- **File Updating**: Appends AI responses back to the original file as an assistant block

## File Format

The application supports a custom markdown-like format where sections start with keywords:

```
config:
provider = OpenAI
api_key = your-api-key-here
model = gpt-4

system:
You are a helpful AI assistant.

user:
How does this application work?

assistant:
This application parses markdown-like files and sends them to AI providers.
```

## Usage

```
AITemplate.exe <file-path> [--no-update]
```

Options:
- `--no-update`: Do not update the original file with the AI response

## Development

To build the project:

```
dotnet build
```

To run the tests:

```
.\Tests\TestFileUpdating.ps1
```

## Dependencies

- .NET 9.0
- Microsoft.Extensions.AI
- Microsoft.Extensions.Configuration
- Microsoft.Win32.Registry (for Windows file type registration)

## License

MIT
