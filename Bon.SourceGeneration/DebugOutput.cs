#if DEBUG
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
using System;
using System.IO;
using System.Threading;

namespace Bon.SourceGeneration
{
    internal static class DebugOutput
    {
        private const int Version = 5;
        private const string OutputDirectory = "C:/kw/out";

        private static readonly Mutex _mutex = new Mutex(false, "Bon.SourceGeneration." + Version);

        public static void AppendLine(string text, string name = "log")
        {
            _mutex.WaitOne();

            try
            {
                File.AppendAllText($"{OutputDirectory}/bon-{name}-{Version}.txt", $"{DateTimeOffset.Now:HH:mm:ss} {text}\n");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public static void WriteAllText(string text, string name = "output")
        {
            _mutex.WaitOne();

            try
            {
                File.WriteAllText($"{OutputDirectory}/bon-{name}-{Version}.txt", $"{DateTimeOffset.Now:HH:mm:ss}\n{text}\n");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public static void WriteException(Exception ex)
        {
            AppendLine(ex.ToString(), "errors");
        }
    }
}
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
#else
using System;

namespace Bon.SourceGeneration
{
    internal static class DebugOutput
    {
        public static void AppendLine(string text, string name = "log") { }

        public static void WriteAllText(string text, string name = "output") { }

        public static void WriteException(Exception ex) { }
    }
}
#endif
