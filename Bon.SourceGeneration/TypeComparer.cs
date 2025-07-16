using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;

namespace Bon.SourceGeneration
{
    internal class TypeComparer : IEqualityComparer<IDefinition>
    {
        public bool Equals(IDefinition x, IDefinition y) => x?.Type == y?.Type;

        public int GetHashCode(IDefinition x) => x.Type.GetHashCode();

        public static TypeComparer Instance { get; } = new TypeComparer();
    }
}
