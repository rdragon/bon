#pragma warning disable RS1035 // Do not use APIs banned for analyzers
#if DEBUG
using Bon.SourceGeneration.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Bon.SourceGeneration
{
    internal static class DebugOutput
    {
        private const string OutputDirectory = "C:/kw/out/bon";

        private static readonly Mutex _mutex = new Mutex(false, OutputDirectory);

        public static void AppendLine(string text, string name)
        {
            _mutex.WaitOne();

            try
            {
                Directory.CreateDirectory(OutputDirectory);
                File.AppendAllText($"{OutputDirectory}/{name}", $"{DateTimeOffset.Now:HH:mm:ss} {text}\n");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public static void WriteAllText(string text, string name, string _)
        {
            _mutex.WaitOne();

            try
            {
                Directory.CreateDirectory(OutputDirectory);
                File.WriteAllText($"{OutputDirectory}/{name}", $"{text}\n");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public static void WriteException(Exception ex)
        {
            AppendLine(ex.ToString(), "errors.txt");
        }

        public static void PrintDefinitions(IEnumerable<IDefinition> definitions, string name)
        {
            WriteAllText(string.Join("\n", definitions.Select(x => x.ToPrettyString())), name, null);
        }
    }
}
#else
using Bon.SourceGeneration.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Bon.SourceGeneration
{
    internal static class DebugOutput
    {
        private static Mutex _mutex;

        public static void AppendLine(string text, string name) { }

        public static void WriteAllText(string text, string name, string debugOutputDirectory)
        {
            if (string.IsNullOrEmpty(debugOutputDirectory))
            {
                return;
            }

            _mutex = _mutex ?? new Mutex(false, "BonDebugOutput");

            _mutex.WaitOne();

            try
            {
                Directory.CreateDirectory(debugOutputDirectory);
                File.WriteAllText(Path.Combine(debugOutputDirectory, name), text);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public static void WriteException(Exception ex) { }

        public static void PrintDefinitions(IEnumerable<IDefinition> definitions, string name) { }
    }
}
#endif
