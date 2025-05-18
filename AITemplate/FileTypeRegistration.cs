using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace AITemplate;

/// <summary>
/// Helper class to register the application as a handler for .mdai files
/// </summary>
public static class FileTypeRegistration
{
    private const string FileExtension = ".mdai";
    private const string ProgId = "AITemplate.MDFile";
    private const string FileTypeDescription = "AI Template Markdown File";
    private const string ApplicationName = "AI Template Runner";

    /// <summary>
    /// Register the application as the handler for .mdai files
    /// </summary>
    public static void RegisterFileType()
    {
        // Only run on Windows
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("File type registration is only supported on Windows.");
            return;
        }

        try
        {
            // Check if already registered with current executable path
            if (IsRegistered()) return;

            string executablePath = Process.GetCurrentProcess().MainModule?.FileName ?? Assembly.GetExecutingAssembly().Location;

            // Use a platform-specific method for the Windows Registry operations
            RegisterFileTypeOnWindows(executablePath);

            Console.WriteLine($"Successfully registered as handler for {FileExtension} files.");
        }
        catch (SecurityException)
        {
            Console.WriteLine($"Warning: Insufficient permissions to register as handler for {FileExtension} files.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to register as handler for {FileExtension} files: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds the application directory to the PATH environment variable to make it callable from anywhere
    /// </summary>
    /// <returns>True if the PATH was updated or the application was already in the PATH</returns>
    public static bool AddApplicationToPath()
    {
        try
        {
            // Get the executable directory
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string? applicationDir = Path.GetDirectoryName(executablePath);

            if (string.IsNullOrEmpty(applicationDir))
            {
                Console.WriteLine("Warning: Could not determine application directory.");
                return false;
            }

            // Check if it's already in the PATH
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
            string[] pathDirs = currentPath.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (pathDirs.Any(dir => dir.Equals(applicationDir, StringComparison.OrdinalIgnoreCase)))
            {
                // Already in PATH
                return true;
            }

            // Add to PATH
            string newPath = currentPath + Path.PathSeparator + applicationDir;
            Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);

            Console.WriteLine($"Added AITemplate to your PATH. You can now run it from anywhere using 'AITemplate'.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not add application to PATH: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Creates a symbolic link "AITemplate" in the same directory as the application
    /// </summary>
    /// <returns>True if the symbolic link was created successfully</returns>
    public static bool CreateApplicationLink()
    {
        try
        {
            // Get the executable path
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string? applicationDir = Path.GetDirectoryName(executablePath);

            if (string.IsNullOrEmpty(applicationDir))
            {
                Console.WriteLine("Warning: Could not determine application directory.");
                return false;
            }

            // Create batch file that calls the application
            string batchFilePath = Path.Combine(applicationDir, "AITemplate.cmd");
            if (!File.Exists(batchFilePath))
            {
                string batchContent = $"@echo off\r\ndotnet \"%~dp0AITemplate.dll\" %*";
                File.WriteAllText(batchFilePath, batchContent);
                Console.WriteLine($"Created AITemplate command in {applicationDir}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not create application link: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Windows-specific implementation of file type registration
    /// </summary>
    private static void RegisterFileTypeOnWindows(string executablePath)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        // Create .mdai file extension entry
        using var fileExtKey = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{FileExtension}");
        fileExtKey?.SetValue("", ProgId);
        fileExtKey?.SetValue("Content Type", "text/plain");
        fileExtKey?.SetValue("PerceivedType", "text");

        // Create ProgID entry
        using var progIdKey = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{ProgId}");
        progIdKey?.SetValue("", FileTypeDescription);
        progIdKey?.SetValue("FriendlyTypeName", FileTypeDescription);

        // Create DefaultIcon entry
        using var defaultIconKey = progIdKey?.CreateSubKey("DefaultIcon");
        defaultIconKey?.SetValue("", $"{executablePath},0");

        // Create command entry
        using var commandKey = progIdKey?.CreateSubKey("shell\\open\\command");
        commandKey?.SetValue("", $"\"{executablePath}\" \"%1\"");

        // Register for "Open With" menu
        RegisterForOpenWithMenuOnWindows(executablePath);

        // Notify the shell that file associations have changed
        SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
    }

    /// <summary>
    /// Windows-specific implementation for "Open With" menu registration
    /// </summary>
    private static void RegisterForOpenWithMenuOnWindows(string executablePath)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        // Create Applications entry
        using var appKey = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\Applications\\{Path.GetFileName(executablePath)}");
        appKey?.SetValue("FriendlyAppName", ApplicationName);

        // Create SupportedTypes entry
        using var supportedTypesKey = appKey?.CreateSubKey("SupportedTypes");
        supportedTypesKey?.SetValue(FileExtension, "");
        supportedTypesKey?.SetValue(".md", ""); // Also support regular markdown

        // Create DefaultIcon entry
        using var defaultIconKey = appKey?.CreateSubKey("DefaultIcon");
        defaultIconKey?.SetValue("", $"{executablePath},0");

        // Create shell\open\command entry
        using var commandKey = appKey?.CreateSubKey("shell\\open\\command");
        commandKey?.SetValue("", $"\"{executablePath}\" \"%1\"");
    }

    /// <summary>
    /// Check if the application is already registered as the handler for .mdai files
    /// </summary>
    private static bool IsRegistered()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;

        try
        {
            string executablePath = Process.GetCurrentProcess().MainModule?.FileName
                ?? Assembly.GetExecutingAssembly().Location;

            using var commandKey = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{ProgId}\\shell\\open\\command");
            if (commandKey == null)
                return false;

            string command = commandKey.GetValue("")?.ToString() ?? string.Empty;

            // Check if the command points to the current executable
            return command.Contains(executablePath, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Notify the shell that file associations have changed
    /// </summary>
    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);
}
