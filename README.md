# AITemplate

A C# application that reads and parses markdown-like files containing AI conversation prompts. The application interacts with AI providers, registers as the default handler for `.mdai` files in Windows, and updates the original files with AI responses.

## Features

- **File Parsing**: Parses markdown-like files containing AI conversation prompts
- **AI Provider Integration**: Interacts with various AI providers (OpenAI, Azure OpenAI)
- **File Type Registration**: Registers as the default handler for `.mdai` files in Windows
- **File Updating**: Appends AI responses back to the original file as an assistant block
- **Environment Variables**: Support for environment variables in configuration sections
- **Markdown Support**: Support for standard markdown syntax, including image references

## Installation

When you run the AITemplate application for the first time, it automatically:
1. Registers itself as the handler for `.mdai` files
2. Adds itself to your PATH environment variable
3. Creates an `AITemplate.cmd` command you can use from anywhere

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
    └── SpanExtensions.cs       # Helper methods for span operations
```

## File Format

Each `.mdai` file should have:

1. A `config:` section with provider details
2. One or more message sections using full notation (`system:`, `user:`, `assistant:`)

### Example:

```
config:
provider: OpenAI
apikey: $OPENAI_API_KEY
model: gpt-4

system:
You are a helpful assistant.

user:
Hello, how can you help me today?

user:
You can use standard markdown syntax in your messages, including:
- **Bold text**
- *Italic text*
- [Links](https://example.com)
- ![Image references](path/to/image.png)
```

### Environment Variables in Config

You can use environment variables in your config section by prefixing them with `$`. 
The application will automatically replace these with the corresponding environment variable values.

## Running `.mdai` Files

You can run `.mdai` files in several ways:

### Using VS Code Tasks

1. Open any `.mdai` file in VS Code
2. Press `Ctrl+Shift+B` to open the task runner
3. Select one of the following tasks:
   - **Run MDAI file** - Runs the file and updates it with the AI response
   - **Run MDAI file (no update)** - Runs the file without updating it

### Using Command Line

```
AITemplate path\to\your-file.mdai
```

To run without updating the file:

```
AITemplate path\to\your-file.mdai --no-update
```

### Double-clicking `.mdai` Files

You can also double-click any `.mdai` file in File Explorer to run it.

## Examples

Check out the [Examples folder](./AITemplate/Examples/) for sample `.mdai` files demonstrating different formats and features.

## Development

To build the project:

```
dotnet build
```

## Dependencies

- .NET 9.0
- Microsoft.Extensions.AI
- Microsoft.Extensions.Configuration
- Microsoft.Win32.Registry (for Windows file type registration)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
