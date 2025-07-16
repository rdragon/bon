// Bookmark 413211217
using System;

namespace Bon.SourceGeneration
{
    internal sealed class NativeDefinition : Definition
    {
        public string SimpleType { get; }

        private NativeDefinition(string type, string simpleType, SchemaType schemaType, bool isNullable) :
            base(type, schemaType, isNullable)
        {
            SimpleType = simpleType;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        public override bool IsValueType => SchemaType != SchemaType.String; // Bookmark 413211217

        public override IDefinition ToNullable() => IsNullable ? this : GetNativeDefinition(Type + "?");

        public NativeDefinition ToNonNullable() => IsNullable ? GetNativeDefinition(TypeNonNullable) : this;

        public NativeDefinition ChangeNullability(bool isNullable) => isNullable ? (NativeDefinition)ToNullable() : ToNonNullable();

        public string GetWriteMethodName() => "NativeSerializer.Write" + (SchemaType == SchemaType.String ? "String" : $"{SimpleType}");

        public static NativeDefinition String { get; } = new NativeDefinition("string", "String", SchemaType.String, false);
        public static NativeDefinition Bool { get; } = new NativeDefinition("bool", "Bool", SchemaType.Bool, false);
        public static NativeDefinition Byte { get; } = new NativeDefinition("byte", "Byte", SchemaType.Byte, false);
        public static NativeDefinition SByte { get; } = new NativeDefinition("sbyte", "SByte", SchemaType.SByte, false);
        public static NativeDefinition Short { get; } = new NativeDefinition("short", "Short", SchemaType.Short, false);
        public static NativeDefinition UShort { get; } = new NativeDefinition("ushort", "UShort", SchemaType.UShort, false);
        public static NativeDefinition Int { get; } = new NativeDefinition("int", "Int", SchemaType.Int, false);
        public static NativeDefinition UInt { get; } = new NativeDefinition("uint", "UInt", SchemaType.UInt, false);
        public static NativeDefinition Long { get; } = new NativeDefinition("long", "Long", SchemaType.Long, false);
        public static NativeDefinition ULong { get; } = new NativeDefinition("ulong", "ULong", SchemaType.ULong, false);
        public static NativeDefinition Float { get; } = new NativeDefinition("float", "Float", SchemaType.Float, false);
        public static NativeDefinition Double { get; } = new NativeDefinition("double", "Double", SchemaType.Double, false);
        public static NativeDefinition Decimal { get; } = new NativeDefinition("decimal", "Decimal", SchemaType.Decimal, false);
        public static NativeDefinition Guid { get; } = new NativeDefinition("System.Guid", "Guid", SchemaType.Guid, false);

        public static NativeDefinition NullableString { get; } = new NativeDefinition("string?", "NullableString", SchemaType.String, true);
        public static NativeDefinition NullableBool { get; } = new NativeDefinition("bool?", "NullableBool", SchemaType.Bool, true);
        public static NativeDefinition NullableByte { get; } = new NativeDefinition("byte?", "NullableByte", SchemaType.WholeNumber, true);
        public static NativeDefinition NullableSByte { get; } = new NativeDefinition("sbyte?", "NullableSByte", SchemaType.SignedWholeNumber, true);
        public static NativeDefinition NullableShort { get; } = new NativeDefinition("short?", "NullableShort", SchemaType.SignedWholeNumber, true);
        public static NativeDefinition NullableUShort { get; } = new NativeDefinition("ushort?", "NullableUShort", SchemaType.WholeNumber, true);
        public static NativeDefinition NullableInt { get; } = new NativeDefinition("int?", "NullableInt", SchemaType.SignedWholeNumber, true);
        public static NativeDefinition NullableUInt { get; } = new NativeDefinition("uint?", "NullableUInt", SchemaType.WholeNumber, true);
        public static NativeDefinition NullableLong { get; } = new NativeDefinition("long?", "NullableLong", SchemaType.SignedWholeNumber, true);
        public static NativeDefinition NullableULong { get; } = new NativeDefinition("ulong?", "NullableULong", SchemaType.WholeNumber, true);
        public static NativeDefinition NullableFloat { get; } = new NativeDefinition("float?", "NullableFloat", SchemaType.Float, true);
        public static NativeDefinition NullableDouble { get; } = new NativeDefinition("double?", "NullableDouble", SchemaType.Double, true);
        public static NativeDefinition NullableDecimal { get; } = new NativeDefinition("decimal?", "NullableDecimal", SchemaType.Decimal, true);
        public static NativeDefinition NullableGuid { get; } = new NativeDefinition("System.Guid?", "NullableGuid", SchemaType.Guid, true);

        public static NativeDefinition WholeNumber { get; } = new NativeDefinition("ulong", "WholeNumber", SchemaType.WholeNumber, false);

        public static NativeDefinition GetNativeDefinition(string type) =>
            TryGetNativeDefinition(type) ?? throw new ArgumentOutOfRangeException($"Cannot handle '{type}'.", nameof(type), null);

        public static NativeDefinition TryGetNativeDefinition(string type)
        {
            switch (type)
            {
                case "string": return String;
                case "bool": return Bool;
                case "byte": return Byte;
                case "sbyte": return SByte;
                case "short": return Short;
                case "ushort": return UShort;
                case "int": return Int;
                case "uint": return UInt;
                case "long": return Long;
                case "ulong": return ULong;
                case "float": return Float;
                case "double": return Double;
                case "decimal": return Decimal;
                case "System.Guid": return Guid;

                case "string?": return NullableString;
                case "bool?": return NullableBool;
                case "byte?": return NullableByte;
                case "sbyte?": return NullableSByte;
                case "short?": return NullableShort;
                case "ushort?": return NullableUShort;
                case "int?": return NullableInt;
                case "uint?": return NullableUInt;
                case "long?": return NullableLong;
                case "ulong?": return NullableULong;
                case "float?": return NullableFloat;
                case "double?": return NullableDouble;
                case "decimal?": return NullableDecimal;
                case "System.Guid?": return NullableGuid;

                default: return null;
            }
        }
    }
}
