using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration
{
    internal sealed class EnumDefinition : Definition, IMajorDefinition
    {
        /// <summary>
        /// Has the same nullability as the enum definition.
        /// </summary>
        public NativeDefinition UnderlyingDefinition { get; }

        public EnumDefinition(string type, SchemaType schemaType, bool isNullable, NativeDefinition underlyingDefinition) :
            base(type, schemaType, isNullable)
        {
            UnderlyingDefinition = underlyingDefinition;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        public override bool IsValueType => true;

        public IMajorDefinition ToNonNullable() =>
            IsNullable ? new EnumDefinition(TypeNonNullable, SchemaType, false, UnderlyingDefinition.ToNonNullable()) : this;

        public override IDefinition ToNullable()
        {
            var underlyingDefinition = (NativeDefinition)UnderlyingDefinition.ToNullable();

            return IsNullable ? this : new EnumDefinition(Type + "?", underlyingDefinition.SchemaType, true, underlyingDefinition);
        }

        public override string SchemaBaseClass => "NativeSchema";
    }
}
