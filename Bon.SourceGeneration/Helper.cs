using System;
using System.Collections.Generic;
using System.Text;

namespace Bon.SourceGeneration
{
    internal static class Helper
    {
        public static string Indent(IEnumerable<string> lines)
        {
            var builder = new StringBuilder();
            var indentation = 0;

            foreach (var line in lines)
            {
                if (line == "}" || line == "};")
                {
                    indentation -= 4;
                }

                builder.Append(' ', indentation);
                builder.AppendLine(line);

                if (line == "{")
                {
                    indentation += 4;
                }
            }

            return builder.ToString();
        }

        public static string SwapNullability(string type, bool isValueType)
        {
            if (isValueType)
            {
                return type.EndsWith("?") ? type.Substring(0, type.Length - 1) : type + "?";
            }

            return type;
        }

        public static bool IsNullableType(string type, bool isValueType) => !isValueType || type.EndsWith("?");
    }
}
