using System.Collections.Generic;

namespace Bon.SourceGeneration.Definitions
{
    internal sealed class DictionaryDefinition : Definition
    {
        public IDefinition KeyDefinition { get; }

        public IDefinition ValueDefinition { get; }

        public DictionaryType DictionaryType { get; }

        public DictionaryDefinition(
            string type,
            IDefinition keyDefinition,
            IDefinition valueDefinition,
            DictionaryType dictionaryType) : base(type, SchemaType.Dictionary, false)
        {
            KeyDefinition = keyDefinition;
            ValueDefinition = valueDefinition;
            DictionaryType = dictionaryType;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.
        // DictionaryType is determined by Type.

        public string GetConstructor(string count)
        {
            return $"new Dictionary<{KeyDefinition.Type}, {ValueDefinition.Type}>({count})";
        }

        public override IEnumerable<IDefinition> GetInnerDefinitions()
        {
            yield return KeyDefinition;
            yield return ValueDefinition;
        }

        public override string SchemaBaseClass => "GenericSchema2";
    }

    public enum DictionaryType
    {
        Dictionary,
        IDictionary,
        IReadOnlyDictionary
    }
}
