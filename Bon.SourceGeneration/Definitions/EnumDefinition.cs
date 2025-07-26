namespace Bon.SourceGeneration.Definitions
{
    internal sealed class EnumDefinition : Definition, ICriticalDefinition
    {
        /// <summary>
        /// Has the same nullability as the enum definition.
        /// </summary>
        public NativeDefinition UnderlyingDefinition { get; }

        public EnumDefinition(string type, NativeDefinition underlyingDefinition) :
            base(type, underlyingDefinition.SchemaType, true)
        {
            UnderlyingDefinition = underlyingDefinition;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        public ICriticalDefinition SwapNullability() =>
            new EnumDefinition(Helper.SwapNullability(Type, IsValueType), UnderlyingDefinition.SwapNullability());
    }
}
