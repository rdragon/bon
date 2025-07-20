using System.Collections.Generic;

namespace Bon.SourceGeneration.Definitions
{
    internal sealed class Tuple2Definition : Definition, ITupleDefinition
    {
        public IDefinition Item1Definition { get; }

        public IDefinition Item2Definition { get; }

        public Tuple2Definition(
            string type,
            IDefinition item1Definition,
            IDefinition item2Definition) : base(type, SchemaType.Tuple2, true)
        {
            Item1Definition = item1Definition;
            Item2Definition = item2Definition;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        public override IEnumerable<IDefinition> GetInnerDefinitions()
        {
            yield return Item1Definition;
            yield return Item2Definition;
        }

        public override string SchemaBaseClass => "GenericSchema2";
    }
}
