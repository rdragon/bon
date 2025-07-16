using Bon.SourceGeneration.Definitions;

namespace Bon.SourceGeneration
{
    /// <summary>
    /// Represents a property or field of a custom class or struct.
    /// </summary>
    internal sealed class Member : IMember
    {
        /// <summary>
        /// The name of the member, e.g. "ExampleProperty".
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The ID of this member as defined in the BonMember attribute.
        /// </summary>
        public int Id { get; }

        public IDefinition Definition { get; }

        public bool HasSetter { get; }

        /// <summary>
        /// The index of the argument corresponding to this member in the constructor.
        /// E.g. if this member corresponds to the first argument in the constructor of the class or struct then this value is 0.
        /// </summary>
        public int ConstructorIndex { get; set; }

        public Member(string name, int id, IDefinition definition, bool hasSetter)
        {
            Name = name;
            Id = id;
            Definition = definition;
            HasSetter = hasSetter;
        }

        public bool Equals(object obj, AncestorCollection ancestors)
        {
            return
                obj is Member member &&
                member.Name == Name &&
                member.Id == Id &&
                member.ConstructorIndex == ConstructorIndex &&
                member.HasSetter == HasSetter &&
                member.Definition.Equals(Definition, ancestors);
        }

        public void AppendHashCode(AncestorCollection ancestors, ref int hashCode)
        {
            hashCode = hashCode.AddHashOf(Name)
                .AddHashOf(Id)
                .AddHashOf(ConstructorIndex)
                .AddHashOf(HasSetter);

            Definition.AppendHashCode(ancestors, ref hashCode);
        }

        public override string ToString() => Name;
    }
}
