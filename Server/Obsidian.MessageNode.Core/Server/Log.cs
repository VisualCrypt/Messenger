using System;
using System.Diagnostics;

namespace Obsidian.MessageNode.Core.Server
{
    public static class Log
    {
        public static event EventHandler<LogEntry> EntryWritten;
        public static void Info(string text)
        {
            RaiseEvent(text,LogLevel.Info);
        }
        public static void Warn(string text)
        {
            RaiseEvent(text, LogLevel.Warn);
        }
        public static void Error(string text)
        {
            RaiseEvent(text, LogLevel.Error);
        }

        static void RaiseEvent(string text, LogLevel logLevel)
        {
            var handler = EntryWritten;
            handler?.Invoke(null, new LogEntry { DateTime = DateTime.Now, LogLevel = logLevel, Text = text });
            //Trace.TraceError(text);
            Debug.WriteLine(text);
        }
    }

    public class LogEntry
    {
        public string Text;
        public DateTime DateTime;
        public LogLevel LogLevel;
    }

    public enum LogLevel : byte
    {
        None = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
    }
}
