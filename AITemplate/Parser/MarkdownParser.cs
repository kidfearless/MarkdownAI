using System.Text;

namespace AITemplate.Parser;

public struct MarkdownParser
{
    public IReadOnlyList<MarkdownBlock> ParseFile(string filePath)
    {
        var blocks = new List<MarkdownBlock>(1024);

        BlockType currentBlockType = BlockType.None;
        var blockContentBuilder = new StringBuilder(1024);

        foreach (var line in File.ReadLines(filePath))
        {
            // Process each line as we read it
            ProcessLine(line.AsSpan(), ref currentBlockType, blockContentBuilder, blocks);
        }

        // Add the last block if there is one
        if (currentBlockType != BlockType.None && blockContentBuilder.Length > 0)
        {
            blocks.Add(CreateBlock(currentBlockType, blockContentBuilder.ToString()));
        }

        return blocks;
    }

    private void ProcessLine(ReadOnlySpan<char> line, ref BlockType currentBlockType, StringBuilder blockContentBuilder, List<MarkdownBlock> blocks)
    {
        // Skip empty lines but preserve empty lines in content blocks
        if (line.IsEmpty)
        {
            if (currentBlockType != BlockType.None)
            {
                blockContentBuilder.AppendLine();
            }
            return;
        }

        // Skip whitespace-only lines in the beginning of blocks
        if (line.IsWhiteSpace())
        {
            if (currentBlockType != BlockType.None)
            {
                blockContentBuilder.AppendLine();
            }
            return;
        }

        // Trim the line for processing but keep original whitespace for content
        var trimmedLine = line.Trim();

        // Check if this line is a block start
        var newBlockType = GetBlockType(trimmedLine);

        if (newBlockType != BlockType.None)
        {
            // If we were building a block, finalize it and add it to the list
            if (currentBlockType != BlockType.None && blockContentBuilder.Length > 0)
            {
                blocks.Add(CreateBlock(currentBlockType, blockContentBuilder.ToString()));
                blockContentBuilder.Clear();
            }

            // Start new block and update current block type
            currentBlockType = newBlockType;

            // Find the position after the colon
            int contentStartIndex = line.IndexOf(':') + 1;
            if (contentStartIndex < trimmedLine.Length)
            {
                // Only convert to string when needed
                blockContentBuilder.AppendLine(trimmedLine.Slice(contentStartIndex).Trim().ToString());
            }
        }
        else if (currentBlockType != BlockType.None)
        {
            // Keep original formatting for non-marker lines
            blockContentBuilder.AppendLine(line.ToString());
        }
    }

    private BlockType GetBlockType(ReadOnlySpan<char> line)
    {
        // Check for block markers using span operations
        if (StartsWith(line, "config:"))
            return BlockType.Config;
        if (StartsWith(line, "system:"))
            return BlockType.System;
        if (StartsWith(line, "user:"))
            return BlockType.User;
        if (StartsWith(line, "assistant:"))
            return BlockType.Assistant;

        return BlockType.None;
    }


    private  bool StartsWith(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
    {
        return source.StartsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    private MarkdownBlock CreateBlock(BlockType blockType, string content) => blockType switch
    {
        BlockType.Config => new ConfigBlock(content),
        BlockType.System => new SystemBlock(content),
        BlockType.User => new UserBlock(content),
        BlockType.Assistant => new AssistantBlock(content),
        _ => throw new ArgumentException($"Unexpected block type: {blockType}")
    };

    private enum BlockType
    {
        None,
        Config,
        System,
        User,
        Assistant
    }
}
