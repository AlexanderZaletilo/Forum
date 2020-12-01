using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Forum
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private LogLevel minLevel;
        private static object _lock = new object();
        public FileLogger(string path, LogLevel minLevel)
        {
            filePath = path;
            this.minLevel = minLevel;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= minLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(filePath, formatter(state, exception) + Environment.NewLine);
                }
            }
        }
    }
    public class FileLoggerProvider : ILoggerProvider
    {
        private string path;
        private LogLevel level;
        public FileLoggerProvider(string _path, LogLevel _level)
        {
            path = _path;
            level = _level;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path, level);
        }

        public void Dispose()
        {
        }
    }
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFileLogger(this ILoggerFactory factory,
                                        string filePath, LogLevel level)
        {
            factory.AddProvider(new FileLoggerProvider(filePath, level));
            return factory;
        }
    }
}
