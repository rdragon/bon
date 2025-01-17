﻿using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration
{
    /// <summary>
    /// Represents a class or struct.
    /// </summary>
    internal sealed class RecordDefinition : Definition, ICustomDefinition, IMajorDefinition
    {
        public override bool IsValueType { get; }

        /// <summary>
        /// All serializable members of the type ordered by ID.
        /// </summary>
        public IReadOnlyList<Member> Members { get; set; }

        /// <summary>
        /// Whether this type has a constructor with for each member a parameter.
        /// If false then a factory method will be generated.
        /// </summary>
        public bool HasValidConstructor { get; set; }

        /// <summary>
        /// Whether every type argument is replaced by a concrete type.
        /// Non-generic types are always considered concrete.
        /// </summary>
        public bool IsConcreteType { get; }

        public RecordDefinition(
            string type,
            SchemaType schemaType,
            bool isNullable,
            IReadOnlyList<Member> members,
            bool isValueType,
            bool hasValidConstructor,
            bool isConcreteType) : base(type, schemaType, isNullable)
        {
            Members = members;
            IsValueType = isValueType;
            HasValidConstructor = hasValidConstructor;
            IsConcreteType = isConcreteType;
        }

        public override bool Equals(object obj, AncestorCollection ancestors)
        {
            if (!(obj is RecordDefinition other))
            {
                return false;
            }

            // IsConcreteType is determined by Type.
            if (HasValidConstructor != other.HasValidConstructor)
            {
                return false;
            }

            return base.Equals(obj, ancestors);
        }

        public override void AppendHashCode(AncestorCollection ancestors, ref int hashCode)
        {
            hashCode = hashCode.AddHashOf(HasValidConstructor);
            base.AppendHashCode(ancestors, ref hashCode);
        }

        IReadOnlyList<IMember> ICustomDefinition.Members => Members;

        public IMajorDefinition ToNonNullable() => IsNullable ?
            new RecordDefinition(TypeNonNullable, SchemaType, false, Members, IsValueType, HasValidConstructor, IsConcreteType) : this;

        public override IDefinition ToNullable() => IsNullable ? this :
            new RecordDefinition(Type + "?", SchemaType, true, Members, IsValueType, HasValidConstructor, IsConcreteType);

        public string GetShortConstructorName(CodeGenerator generator) => generator.TryGetFactoryMethod(Type) ?? "new";

        public string GetLongConstructorName(CodeGenerator generator) => generator.TryGetFactoryMethod(Type) ?? $"new {TypeNonNullable}";

        public override IEnumerable<IDefinition> GetInnerDefinitions() => Members.Select(member => member.Definition);

        protected override IEnumerable<IRecursiveEquatable> GetInnerObjects() => Members;

        public override string SchemaBaseClass => "CustomSchema";
    }
}
