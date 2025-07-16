using System.Collections.Generic;

namespace Bon.SourceGeneration.Definitions
{
    internal sealed class Tuple3Definition : Definition, ITupleDefinition
    {
        public IDefinition Item1Definition { get; }

        public IDefinition Item2Definition { get; }

        public IDefinition Item3Definition { get; }

        public Tuple3Definition(
            string type,
            SchemaType schemaType,
            IDefinition item1Definition,
            IDefinition item2Definition,
            IDefinition item3Definition) : base(type, schemaType)
        {
            Item1Definition = item1Definition;
            Item2Definition = item2Definition;
            Item3Definition = item3Definition;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        public override bool IsValueType => true;

        public override IEnumerable<IDefinition> GetInnerDefinitions()
        {
            yield return Item1Definition;
            yield return Item2Definition;
            yield return Item3Definition;
        }

        public override string SchemaBaseClass => "GenericSchema3";
    }
}
