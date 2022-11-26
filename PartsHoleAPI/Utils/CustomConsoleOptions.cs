using Microsoft.Extensions.Logging.Console;

namespace PartsHoleAPI.Utils
{
   public class CustomConsoleOptions : ConsoleFormatterOptions
   {
      public ConsoleColor DefaultTextColor { get; set; } = ConsoleColor.White;
      public ConsoleColor DefaultBGColor { get; set; } = ConsoleColor.Black;
      public Dictionary<LogLevel, ConsoleColor> LogLevelColorFormat { get; set; } = new()
      {
         { LogLevel.Trace, ConsoleColor.Red },
         { LogLevel.Debug, ConsoleColor.Blue },
         { LogLevel.Information, ConsoleColor.Gray },
         { LogLevel.Warning, ConsoleColor.Yellow },
         { LogLevel.Error, ConsoleColor.Red },
         { LogLevel.Critical, ConsoleColor.Red },
         { LogLevel.None, ConsoleColor.White },
      };

      public ConsoleColor[] MessageTextColoring { get; set; } = new[]
      {
         ConsoleColor.DarkCyan,
         ConsoleColor.White
      };

      public Dictionary<string, ConsoleColor> MethodTextColoring { get; set; } = new()
      {
         { "GET", ConsoleColor.Green },
         { "POST", ConsoleColor.Blue },
         { "PUT", ConsoleColor.Cyan },
         { "DELETE", ConsoleColor.DarkRed },
      };
   }
}
