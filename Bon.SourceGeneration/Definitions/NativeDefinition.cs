using System;

namespace Bon.SourceGeneration.Definitions
{
    /// <summary>
    /// Represents a native type that can be serialized.
    /// </summary>
    internal sealed partial class NativeDefinition : Definition
    {
        public string TypeAlphanumeric { get; }

        public override bool IsValueType { get; }//0at, waarvoor wordt deze gebruikt? guid is beide namelijk nu. of wss gwon value type.

        /// <summary>
        /// //2at
        /// </summary>
        public string SchemaIdentifier { get; }

        private NativeDefinition(string type, SchemaType schemaType, bool isValueType, string schemaIdentifier) : base(type, schemaType)
        {
            TypeAlphanumeric = GetTypeAlphanumeric(type);
            IsValueType = isValueType;
            SchemaIdentifier = schemaIdentifier;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        public string GetWriteMethodName() => "NativeSerializer.Write" + TypeAlphanumeric;

        public NativeDefinition SwapNullability() => IsReferenceType ? this : GetNativeDefinition(Helper.SwapNullability(Type));

        private static string GetTypeAlphanumeric(string type)
        {
            var prefix = GetPrefix(ref type);

            switch (type)
            {
                case "string": return "String";
                case "bool": return prefix + "Bool";
                case "byte": return prefix + "Byte";
                case "sbyte": return prefix + "SByte";
                case "short": return prefix + "Short";
                case "ushort": return prefix + "UShort";
                case "int": return prefix + "Int";
                case "uint": return prefix + "UInt";
                case "long": return prefix + "Long";
                case "ulong": return prefix + "ULong";
                case "float": return prefix + "Float";
                case "double": return prefix + "Double";
                case "decimal": return prefix + "Decimal";
                case "System.Guid": return prefix + "Guid";
                case "char": return prefix + "Char";
                case "System.DateTime": return prefix + "DateTime";
                case "System.DateTimeOffset": return prefix + "DateTimeOffset";
                case "System.TimeSpan": return prefix + "TimeSpan";
                case "System.DateOnly": return prefix + "DateOnly";
                case "System.TimeOnly": return prefix + "TimeOnly";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static string GetPrefix(ref string type)
        {
            if (type.EndsWith("?"))
            {
                type = type.Substring(0, type.Length - 1);
                return "Nullable";
            }
            else
            {
                return "";
            }
        }
    }
}
