using System.Text;
using Microsoft.VisualBasic;

namespace AITemplate.Parser;

public struct MarkdownParser
{
    // Parsing states
    private enum ParseState
    {
        ReadingNone,
        ReadingConfig,
        ReadingSystem,
        ReadingUser,
        ReadingAssistant
    }

    public IReadOnlyList<MarkdownBlock> ParseFile(string filePath)
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory());
        var fileContent = File.Exists(filePath) 
            ? File.ReadAllLines(filePath) 
            : filePath.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        
        var blocks = new List<MarkdownBlock>(8);
        var blockContent = new StringBuilder(1024);
        var fileRefs = new List<string>(4);
        
        ParseState currentState = ParseState.ReadingNone;
        ParseState previousState = ParseState.ReadingNone;
        
        foreach (var line in fileContent)
        {
            var lineSpan = line.AsSpan();
            var newState = GetBlockState(lineSpan);
            
            if (newState != ParseState.ReadingNone)
            {
                // Save previous block if we have content and are transitioning to a new block
                if (blockContent.Length > 0 && previousState != ParseState.ReadingNone)
                {
                    if (previousState == ParseState.ReadingUser)
                    {
                        blocks.Add(new UserBlock(blockContent.ToString(), new List<string>(fileRefs)));
                        fileRefs.Clear();
                    }
                    else
                    {
                        blocks.Add(CreateBlock(previousState, blockContent.ToString()));
                    }
                    blockContent.Clear();
                }
                
                currentState = newState;
                previousState = newState;
                
                // Skip the block header (e.g., "user:", "system:", etc.)
                int headerLength = lineSpan.IndexOf(':') + 1;
                if (headerLength < lineSpan.Length)
                {
                    var remainingLine = lineSpan.Slice(headerLength).ToString();
                    blockContent.AppendLine(remainingLine);
                    
                    // Extract file references for user blocks
                    if (currentState == ParseState.ReadingUser)
                    {
                        ExtractFileReferences(lineSpan.Slice(headerLength), fileRefs);
                    }
                }
            }
            else if (currentState != ParseState.ReadingNone)
            {
                // Continue adding content to current block
                blockContent.AppendLine(line);
                
                // Extract file references for user blocks
                if (currentState == ParseState.ReadingUser)
                {
                    ExtractFileReferences(lineSpan, fileRefs);
                }
            }
        }
        
        // Add the last block if we have content
        if (blockContent.Length > 0 && previousState != ParseState.ReadingNone)
        {
            if (previousState == ParseState.ReadingUser)
            {
                blocks.Add(new UserBlock(blockContent.ToString(), new List<string>(fileRefs)));
            }
            else
            {
                blocks.Add(CreateBlock(previousState, blockContent.ToString()));
            }
        }
        
        return blocks;
    }

    private ParseState GetBlockState(ReadOnlySpan<char> line)
    {
        if (StartsWith(line, "config:"))
            return ParseState.ReadingConfig;
        if (StartsWith(line, "system:"))
            return ParseState.ReadingSystem;
        if (StartsWith(line, "user:"))
            return ParseState.ReadingUser;
        if (StartsWith(line, "assistant:"))
            return ParseState.ReadingAssistant;

        return ParseState.ReadingNone;
    }

    private bool StartsWith(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
    {
        return source.StartsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    private MarkdownBlock CreateBlock(ParseState state, string content) => state switch
    {
        ParseState.ReadingConfig => new ConfigBlock(content),
        ParseState.ReadingSystem => new SystemBlock(content),
        ParseState.ReadingUser => new UserBlock(content),
        ParseState.ReadingAssistant => new AssistantBlock(content),
        _ => throw new ArgumentException($"Unexpected block state: {state}")
    };

    private void ExtractFileReferences(ReadOnlySpan<char> line, List<string> refs)
    {
        int idx = 0;
        while (idx < line.Length)
        {
            if (line[idx] == '!' && idx + 3 < line.Length && line[idx + 1] == '[')
            {
                int closeBracket = line.Slice(idx + 2).IndexOf(']');
                if (closeBracket >= 0 && idx + 2 + closeBracket + 1 < line.Length && line[idx + 2 + closeBracket + 1] == '(')
                {
                    int closeParen = line.Slice(idx + 2 + closeBracket + 2).IndexOf(')');
                    if (closeParen >= 0)
                    {
                        var fileRef = line.Slice(idx, 3 + closeBracket + closeParen + 2).ToString();
                        refs.Add(fileRef);
                        idx += 3 + closeBracket + closeParen + 2;
                        continue;
                    }
                }
            }
            else if (line[idx] == '[')
            {
                int closeBracket = line.Slice(idx + 1).IndexOf(']');
                if (closeBracket >= 0 && idx + 1 + closeBracket + 1 < line.Length && line[idx + 1 + closeBracket + 1] == '(')
                {
                    int closeParen = line.Slice(idx + 1 + closeBracket + 2).IndexOf(')');
                    if (closeParen >= 0)
                    {
                        var fileRef = line.Slice(idx, 2 + closeBracket + closeParen + 2).ToString();
                        refs.Add(fileRef);
                        idx += 2 + closeBracket + closeParen + 2;
                        continue;
                    }
                }
            }
            idx++;
        }
    }
}
