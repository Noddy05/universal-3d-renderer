using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Debug
{
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
        /// <param name="str"></param>
        /// <param name="callingMember"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void Log(string str,
            [CallerMemberName] string callingMember = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            Console.Write($"From {filePath}.");
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(callingMember);
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine($"(Line: {lineNumber}) at (" +
                $"{DateTime.Now}, {Program.GetWindow().timeSinceStartup}" +
                $" seconds after startup): {str}");
        }

        /// <summary>
        /// Writes a warning to console, including when and where the message came from.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="callingMember"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogWarning(string str,
            [CallerMemberName] string callingMember = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"From {filePath}.");
            Console.Write(callingMember);
            Console.WriteLine($"(Line: {lineNumber}) at (" +
                $"{DateTime.Now}, {Program.GetWindow().timeSinceStartup}" +
                $" seconds after startup): {str}");
            Console.ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// Writes an error to console, including when and where the message came from.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="callingMember"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogError(string str,
            [CallerMemberName] string callingMember = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"From {filePath}.");
            Console.Write(callingMember);
            Console.WriteLine($"(Line: {lineNumber}) at (" +
                $"{DateTime.Now}, {Program.GetWindow().timeSinceStartup}" +
                $" seconds after startup): {str}");
            Console.ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// Writes a fatal error to console, including when and where the message came from.
        /// <br></br>After writing it throws an exception that ends the current process.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="callingMember"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogFatalError(string str,
            [CallerMemberName] string callingMember = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Fatal error occured!");

            Console.Write($"From {filePath}.");
            Console.Write(callingMember);
            Console.WriteLine($"(Line: {lineNumber}) at (" +
                $"{DateTime.Now}, {Program.GetWindow().timeSinceStartup}" +
                $" seconds after startup): {str}");

            throw new Exception(str);
        }
    }
}
