using System;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.Definitions
{
    /// <summary>
    /// Represents a type that can be serialized.
    /// </summary>
    internal abstract class Definition : IDefinition
    {
        public string Type { get; }

        public SchemaType SchemaType { get; }

        public bool IsNullable { get; }

        public string TypeNonNullable { get; }

        public bool IsValueType { get; }

        protected Definition(string type, SchemaType schemaType, bool isValueType)
        {
            var isNullable = Helper.IsNullableType(type, isValueType);
            Type = type;
            SchemaType = schemaType;
            IsNullable = isNullable;
            IsValueType = isValueType;
            TypeNonNullable = isNullable && isValueType ? Helper.SwapNullability(type, isValueType) : type;
        }

        public bool IsNullableValueType => IsNullable && IsValueType;

        // Equality is important for incremental source generators.
        public override bool Equals(object obj)
        {
            return Equals(obj, new AncestorCollection(0));
        }

        public override int GetHashCode()
        {
            var hashCode = 0;

            AppendHashCode(new AncestorCollection(0), ref hashCode);

            return hashCode;
        }

        public virtual bool Equals(object obj, AncestorCollection ancestors)
        {
            if (!(obj is Definition other))
            {
                return false;
            }

            if (ancestors.FirstCheck(this, other) is bool result)
            {
                return result;
            }

            // We also compare the IsValueType because a struct can become a class and vice versa.
            if (SchemaType != other.SchemaType ||
                Type != other.Type ||
                IsValueType != other.IsValueType)
            {
                return false;
            }

            var innerObjects = GetInnerObjects().ToArray();
            var otherInnerObjects = other.GetInnerObjects().ToArray();

            if (innerObjects.Length != otherInnerObjects.Length)
            {
                return false;
            }

            if (innerObjects.Length == 0)
            {
                return true;
            }

            ancestors.Add(this, other);

            for (int i = 0; i < innerObjects.Length; i++)
            {
                if (!innerObjects[i].Equals(otherInnerObjects[i], ancestors))
                {
                    return false;
                }
            }

            ancestors.Remove(this, other);

            return true;
        }

        public virtual void AppendHashCode(AncestorCollection ancestors, ref int hashCode)
        {
            if (ancestors.TryAppendHashCode(this, ref hashCode))
            {
                return;
            }

            hashCode = hashCode.AddHashOf(SchemaType)
                .AddHashOf(Type)
                .AddHashOf(IsValueType);

            ancestors.Add(this);

            foreach (var obj in GetInnerObjects())
            {
                obj.AppendHashCode(ancestors, ref hashCode);
            }

            ancestors.Remove(this);
        }

        public override string ToString() => Type;

        public string TypeOf => $"typeof({Type})";

        public virtual IEnumerable<IDefinition> GetInnerDefinitions() => Array.Empty<IDefinition>();

        protected virtual IEnumerable<IRecursiveEquatable> GetInnerObjects() => GetInnerDefinitions();

        public bool IsReferenceType => !IsValueType;

        public virtual string SchemaBaseClass => "Schema";

        public string SafeType => IsReferenceType ? TypeNonNullable + "?" : Type;//0at
    }
}
