using Serilog;
using Serilog.Events;
using System;
using Logger = Serilog.Core.Logger;

namespace YourNamespace
{
    public static class Log
    {
        private static readonly Logger Logger;

        static Log()
        {
            Logger = new LoggerConfiguration()
    .MinimumLevel.Is(LogEventLevel.Verbose).CreateLogger();

        }


        public static void Trace(string message)
        {
            Logger.Debug(message);
        }

        public static void Tracef(string format, params object[] args)
        {
            Logger.Debug(format, args);
        }

        public static void Info(string message)
        {
            Logger.Information(message);
        }

        public static void Infof(string format, params object[] args)
        {
            Logger.Information(format, args);
        }

        public static void Notice(string message)
        {
            Logger.Information(message);
        }

        public static void Noticef(string format, params object[] args)
        {
            Logger.Information(format, args);
        }

        public static void Warn(string message)
        {
            Logger.Warning(message);
        }

        public static void Warnf(string format, params object[] args)
        {
            Logger.Warning(format, args);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void ErrorT(Exception exception)
        {
            if (exception != null)
            {
                Logger.Error(exception, "An error occurred");
            }
        }

        public static void Errorf(string format, params object[] args)
        {
            Logger.Error(format, args);
        }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Debugf(string format, params object[] args)
        {
            Logger.Debug(format, args);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public static void Fatalf(string format, params object[] args)
        {
            Logger.Fatal(format, args);
        }

        public static void Panic(string message)
        {
            Logger.Fatal(message);
        }

        public static void Panicf(string format, params object[] args)
        {
            Logger.Fatal(format, args);
        }
    }
}
