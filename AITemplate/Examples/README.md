# AITemplate Examples

This folder contains example `.mdai` files that demonstrate different formats and features of the AITemplate tool.

## Running `.mdai` Files

When you run the AITemplate application for the first time, it:
1. Registers itself as the handler for `.mdai` files
2. Adds itself to your PATH environment variable
3. Creates an `AITemplate.cmd` command you can use from anywhere

This makes it easy to run `.mdai` files in multiple ways:

### Using VS Code Tasks

1. Open any `.mdai` file in VS Code
2. Press `Ctrl+Shift+B` to open the task runner
3. Select one of the following tasks:
   - **Run MDAI file** - Runs the file and updates it with the AI response
   - **Run MDAI file (no update)** - Runs the file without updating it

### Using Command Line

You can run any `.mdai` file from the command line:

```
AITemplate path\to\your-file.mdai
```

To run without updating the file:

```
AITemplate path\to\your-file.mdai --no-update
```

### Double-clicking `.mdai` Files

You can also double-click any `.mdai` file in File Explorer to run it.

## Example Files

- `sample.mdai` - Basic example of an AI prompt file
- `sample-input.md` - Example using standard markdown format
- `sample-input-shorthand.md` - Example using shorthand notation (s:, u:, a:)
- `sample-input-new-format.md` - Example using the new format
- `test-shorthand.mdai` - Another example using shorthand notation
- `test-indented.mdai` - Example with indented content
- `test-file-updating.mdai` - Example demonstrating file updating features

## File Format

Each `.mdai` file should have:

1. A `config:` section with provider details
2. One or more message sections using either full notation (`system:`, `user:`, `assistant:`) or shorthand (`s:`, `u:`, `a:`)

### Environment Variables in Config

You can use environment variables in your config section by prefixing them with `$`. 
The application will automatically replace these with the corresponding environment variable values.

Example:
```
config:
provider: OpenAI
apikey: $OPENAI_API_KEY
model: gpt-4

system:
You are a helpful assistant.

user:
Hello, how can you help me today?
```

In this example, the application will use the value of the `OPENAI_API_KEY` environment variable as the API key.
