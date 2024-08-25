using System;

namespace Bon.SourceGeneration
{
    /// <summary>
    /// Represents a type that does not have its own schema.
    /// These types will use an existing native schema.
    /// </summary>
    internal class WeakDefinition : Definition
    {
        /// <summary>
        /// Has the same nullability as the weak definition.
        /// </summary>
        public NativeDefinition UnderlyingDefinition { get; }

        public string SimpleType { get; }

        private WeakDefinition(string type, string simpleType, bool isNullable, NativeDefinition underlyingDefinition) :
            base(type, underlyingDefinition.SchemaType, isNullable)
        {
            UnderlyingDefinition = underlyingDefinition;
            SimpleType = simpleType;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        public override bool IsValueType => true;

        public override IDefinition ToNullable() => IsNullable ? this : GetWeakDefinition(Type + "?");

        public override string SchemaBaseClass => "NativeSchema";

        // Bookmark 659516266 (char serialization)
        public static WeakDefinition Char { get; } = new WeakDefinition("char", "Char", false, NativeDefinition.WholeNumber);
        public static WeakDefinition NullableChar { get; } = new WeakDefinition("char?", "NullableChar", true, NativeDefinition.NullableULong);
        public static WeakDefinition DateTime { get; } = new WeakDefinition("System.DateTime", "DateTime", false, NativeDefinition.Long);
        public static WeakDefinition NullableDateTime { get; } = new WeakDefinition("System.DateTime?", "NullableDateTime", true, NativeDefinition.NullableLong);
        public static WeakDefinition DateTimeOffset { get; } = new WeakDefinition("System.DateTimeOffset", "DateTimeOffset", false, NativeDefinition.Long);
        public static WeakDefinition NullableDateTimeOffset { get; } = new WeakDefinition("System.DateTimeOffset?", "NullableDateTimeOffset", true, NativeDefinition.NullableLong);
        public static WeakDefinition TimeSpan { get; } = new WeakDefinition("System.TimeSpan", "TimeSpan", false, NativeDefinition.Long);
        public static WeakDefinition NullableTimeSpan { get; } = new WeakDefinition("System.TimeSpan?", "NullableTimeSpan", true, NativeDefinition.NullableLong);
        public static WeakDefinition DateOnly { get; } = new WeakDefinition("System.DateOnly", "DateOnly", false, NativeDefinition.Int);
        public static WeakDefinition NullableDateOnly { get; } = new WeakDefinition("System.DateOnly?", "NullableDateOnly", true, NativeDefinition.NullableInt);
        public static WeakDefinition TimeOnly { get; } = new WeakDefinition("System.TimeOnly", "TimeOnly", false, NativeDefinition.Long);
        public static WeakDefinition NullableTimeOnly { get; } = new WeakDefinition("System.TimeOnly?", "NullableTimeOnly", true, NativeDefinition.NullableLong);

        public static WeakDefinition GetWeakDefinition(string type) =>
            TryGetWeakDefinition(type) ?? throw new ArgumentOutOfRangeException($"Cannot handle '{type}'.", nameof(type), null);

        public static WeakDefinition TryGetWeakDefinition(string type)
        {
            switch (type)
            {
                case "char": return Char;
                case "System.DateTime": return DateTime;
                case "System.DateTimeOffset": return DateTimeOffset;
                case "System.TimeSpan": return TimeSpan;
                case "System.DateOnly": return DateOnly;
                case "System.TimeOnly": return TimeOnly;

                case "char?": return NullableChar;
                case "System.DateTime?": return NullableDateTime;
                case "System.DateTimeOffset?": return NullableDateTimeOffset;
                case "System.TimeSpan?": return NullableTimeSpan;
                case "System.DateOnly?": return NullableDateOnly;
                case "System.TimeOnly?": return NullableTimeOnly;

                default: return null;
            }
        }
    }
}
