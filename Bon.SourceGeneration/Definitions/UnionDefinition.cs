using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.Definitions
{
    /// <summary>
    /// Represents either an interface or an abstract class.
    /// </summary>
    internal sealed class UnionDefinition : Definition, ICustomDefinition, ICriticalDefinition
    {
        /// <summary>
        /// All the concrete implementation types of this interface or abstract class ordered by ID.
        /// </summary>
        public IReadOnlyList<UnionMember> Members { get; }

        public UnionDefinition(string type, IReadOnlyList<UnionMember> members) :
            base(type, SchemaType.Union, false)
        {
            Members = members;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.

        IReadOnlyList<IMember> ICustomDefinition.Members => Members;

        public override IEnumerable<IDefinition> GetInnerDefinitions() => Members.Select(member => member.Definition);

        protected override IEnumerable<IRecursiveEquatable> GetInnerObjects() => Members;

        public override string SchemaBaseClass => "CustomSchema";

        public ICriticalDefinition SwapNullability() => this;
    }

    internal sealed class UnionMember : IMember
    {
        /// <summary>
        /// The ID as defined in the BonInclude attribute.
        /// </summary>
        public int Id { get; }

        public IDefinition Definition { get; }

        public UnionMember(int id, IDefinition definition)
        {
            Id = id;
            Definition = definition;
        }

        public override string ToString() => Definition.Type;

        public bool Equals(object obj, AncestorCollection ancestors)
        {
            return
                obj is UnionMember other &&
                Id == other.Id &&
                Definition.Equals(other.Definition, ancestors);
        }

        public void AppendHashCode(AncestorCollection ancestors, ref int hashCode)
        {
            hashCode = hashCode.AddHashOf(Id);
            Definition.AppendHashCode(ancestors, ref hashCode);
        }
    }
}
