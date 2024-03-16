using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using _3D_Renderer;

internal static class Debug
{
    /// <summary>
    /// Writes message to console.
    /// </summary>
    /// <param name="str"></param>
    public static void Print(string str)
    {
        Console.WriteLine(str);
    }

    /// <summary>
    /// Writes message to console, including when and where the message came from.
    /// </summary>
    public static void Log(string str,
        [CallerMemberName] string callingMember = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(str, ConsoleColor.White, callingMember, 
            filePath, lineNumber);
    }

    /// <summary>
    /// Writes a warning to console, including when and where the message came from.
    /// </summary>
    public static void LogWarning(string str,
        [CallerMemberName] string callingMember = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(str, ConsoleColor.Yellow, callingMember, 
            filePath, lineNumber);
    }

    /// <summary>
    /// Writes an error to console, including when and where the message came from.
    /// </summary>
    public static void LogError(string str,
        [CallerMemberName] string callingMember = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(str, ConsoleColor.Red, callingMember, 
            filePath, lineNumber);
    }

    /// <summary>
    /// Writes a fatal error to console, including when and where the message came from.
    /// <br></br>After writing it throws an exception that ends the current process.
    /// </summary>
    public static void LogFatalError(string str,
        [CallerMemberName] string callingMember = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(str, ConsoleColor.Red, callingMember, 
            filePath, lineNumber);
        throw new Exception(str);
    }

    private static void Log(string str, ConsoleColor foregroundColor,
        [CallerMemberName] string callingMember = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        string[] filePathSplit = filePath.Split('/','\\');
        int filePathEnd = filePathSplit[filePathSplit.Length - 1].IndexOf('.');
        string filePathFinal = filePathSplit[filePathSplit.Length - 1].Substring(0, filePathEnd);

        ConsoleColor defaultForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        Console.Write($"[{DateTime.Now}, {Program.GetWindow().timeSinceStartup}" +
            $" seconds after startup] From {filePathFinal}.");
        Console.Write(callingMember);
        Console.Write($" (Line: {lineNumber}): ");
        Console.ForegroundColor = defaultForegroundColor;
        Console.WriteLine(str);
    }
}
