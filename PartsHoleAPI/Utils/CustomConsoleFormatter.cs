using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace PartsHoleAPI.Utils;

public class CustomConsoleFormatter : ConsoleFormatter
{
   #region Local Props

   public CustomConsoleOptions Options { get; init; } = new();
   #endregion

   #region Constructors
   public CustomConsoleFormatter() : base("Custom") { }
   public CustomConsoleFormatter(string name) : base(name) { }
   public CustomConsoleFormatter(string name, CustomConsoleOptions options) : base(name) =>
     Options = options;
   #endregion

   #region Methods
   public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
   {
      WriteTime();
      WriteLogLevel(logEntry.LogLevel);
      WriteColoredText(" - ", ConsoleColor.Magenta);
      if (logEntry.Exception is not null)
      {
         WriteError(logEntry.Exception);
      }
      if (logEntry.Formatter is not null)
      {
         WriteText(logEntry.Formatter(logEntry.State, logEntry.Exception));
      }
      Console.WriteLine();
   }

   private void WriteLogLevel(LogLevel level)
   {
      Console.ForegroundColor = Options.LogLevelColorFormat[level];
      Console.Write(level);
      Console.ForegroundColor = Options.DefaultTextColor;
   }

   private void WriteColoredText(string text, ConsoleColor color, bool resetColor = true)
   {
      Console.ForegroundColor = color;
      Console.Write(text);
      if (resetColor)
      {
         Console.ForegroundColor = Options.DefaultTextColor;
      }
   }

   private void WriteText(string text)
   {
      if (text.StartsWith("api"))
      {
         var data = text.Split('|', StringSplitOptions.RemoveEmptyEntries);
         WriteApiMethodText(data[1]);
         WriteApiText(data[2..]);
      }
      else
      {
         Console.Write(text);
      }
   }

   private void WriteApiText(string[] data)
   {
      for (int i = 0; i < data.Length; i++)
      {
         Console.ForegroundColor = i >= Options.MessageTextColoring.Length
            ? Options.MessageTextColoring[^1]
            : Options.MessageTextColoring[i];
         Console.Write(data[i]);
         Console.ForegroundColor = Options.DefaultTextColor;
         if (i < data.Length - 1)
         {
            WriteColoredText(" - ", ConsoleColor.Magenta);
         }
      }
   }

   private void WriteApiMethodText(string method)
   {
      Console.ForegroundColor = Options.MethodTextColoring.ContainsKey(method)
         ? Options.MethodTextColoring[method]
         : Options.DefaultTextColor;
      Console.Write(method);
      WriteColoredText(" - ", ConsoleColor.Magenta);
   }

   private void WriteError(Exception e)
   {
      Console.ForegroundColor = ConsoleColor.White;
      Console.BackgroundColor = ConsoleColor.Red;
      Console.Write(e.Message);
      Console.ForegroundColor = Options.DefaultTextColor;
      Console.BackgroundColor = Options.DefaultBGColor;
   }

   private void WriteTime()
   {
      if (Options.TimestampFormat is null)
         return;
      Console.WriteLine(DateTime.Now.ToString(Options.TimestampFormat));
   }
   #endregion

   #region Full Props

   #endregion
}
