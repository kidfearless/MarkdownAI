using System.Text;

namespace AITemplate.Parser;

public struct ConfigParser
{
    // Parsing states
    private enum ParseState
    {
        ReadingKey,
        ReadingValue,
    }

    public ProviderConfig Parse(ReadOnlySpan<char> configContent)
    {
        var config = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        config.EnsureCapacity(12);

        ParseState state = ParseState.ReadingKey;
        var keyStart = 0;
        var keyEnd = 0;
        var valueStart = 0;
        var valueEnd = 0;

        for (int i = 0; i < configContent.Length; i++)
        {
            char c = configContent[i];

            // Handle line breaks - start fresh on a new line
            if (c is '\n' or '\r')
            {
                // Save current key-value pair if we have both
                if (state == ParseState.ReadingValue && keyEnd > keyStart)
                {
                    var keySpan = configContent.Slice(keyStart, keyEnd - keyStart).Trim();
                    var valueSpan = configContent.Slice(valueStart, valueEnd - valueStart).Trim();

                    if (!keySpan.IsEmpty)
                    {
                        // If value starts with $, try to read from environment variable
                        if (valueSpan.StartsWith("$") && valueSpan.Length > 1)
                        {
                            string envVarName = valueSpan[1..].ToString();
                            string? envValue = Environment.GetEnvironmentVariable(envVarName);
                            config[keySpan.ToString()] = envValue;
                        }
                        else
                        {
                            config[keySpan.ToString()] = valueSpan.ToString();
                        }

                    }
                }

                // Start reading a new key on next character
                state = ParseState.ReadingKey;
                keyStart = i + 1;
                keyEnd = i + 1;
                continue;
            }

            // Reading key
            if (state == ParseState.ReadingKey)
            {
                // Found separator, switch to reading value
                if (c == '=')
                {
                    // Set value start to character after the separator
                    valueStart = i + 1;
                    valueEnd = i + 1;
                    state = ParseState.ReadingValue;
                }
                // Accumulate key character
                else
                {
                    keyEnd = i + 1;
                }
            }
            // Reading value
            else if (state == ParseState.ReadingValue)
            {
                valueEnd = i + 1;
            }
        }

        // Save final key-value pair if we have both
        if (state == ParseState.ReadingValue && keyEnd > keyStart)
        {
            var keySpan = configContent.Slice(keyStart, keyEnd - keyStart).Trim();
            var valueSpan = configContent.Slice(valueStart, valueEnd - valueStart).Trim();

            if (!keySpan.IsEmpty)
            {
                // If value starts with $, try to read from environment variable
                if (valueSpan.StartsWith("$") && valueSpan.Length > 1)
                {
                    string envVarName = valueSpan[1..].ToString();
                    string? envValue = Environment.GetEnvironmentVariable(envVarName);
                    config[keySpan.ToString()] = envValue;
                }
                else
                {
                    config[keySpan.ToString()] = valueSpan.ToString();
                }

            }
        }

        return new ProviderConfig
        {
            Options = config
        };
    }
}
