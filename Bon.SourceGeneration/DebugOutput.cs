#if DEBUG
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
using Bon.SourceGeneration.Definitions;
using System;
using System.Collections;
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

        public static void WriteAllText(string text, string name)
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
            WriteAllText(string.Join("\n", definitions.Select(x => x.ToPrettyString())), name);
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
        public static void AppendLine(string text, string name) { }

        public static void WriteAllText(string text, string name) { }

        public static void WriteException(Exception ex) { }

        public static void PrintDefinitions(IEnumerable<IDefinition> definitions) { }
    }
}
#endif
