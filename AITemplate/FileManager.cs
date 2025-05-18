using System.Text;

namespace AITemplate;

/// <summary>
/// Represents the result of file validation
/// </summary>
public record struct FileValidationResult(bool IsValid, bool IsMdaiFile, bool IsMdFile);

/// <summary>
/// Handles file operations for markdown AI files, including reading, parsing, and updating files
/// </summary>
public struct FileManager
{
    /// <summary>
    /// Validates if the given file path exists and has an acceptable extension
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>A struct containing validation results</returns>
    public FileValidationResult ValidateFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new FileValidationResult(false, false, false);
        }

        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        bool isMdaiFile = extension == ".mdai";
        bool isMdFile = extension == ".md";
        
        return new FileValidationResult(true, isMdaiFile, isMdFile);
    }

}
