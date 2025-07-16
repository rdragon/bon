using System;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration
{
    internal abstract class Definition : IDefinition
    {
        public string Type { get; }

        public SchemaType SchemaType { get; }

        public bool IsNullable { get; }

        protected Definition(string type, SchemaType schemaType, bool isNullable)
        {
            Type = type;
            SchemaType = schemaType;
            IsNullable = isNullable;
        }

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

            if (SchemaType != other.SchemaType ||
                Type != other.Type || // IsNullable is determined by Type.
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

        public string TypeOf => $"typeof({(IsValueType ? Type : TypeNonNullable)})";

        public string TypeNonNullable => Type.TrimEnd('?');

        public virtual bool IsValueType => false;

        public abstract IDefinition ToNullable();

        public virtual IEnumerable<IDefinition> GetInnerDefinitions() => Array.Empty<IDefinition>();

        protected virtual IEnumerable<IRecursiveEquatable> GetInnerObjects() => GetInnerDefinitions();

        public bool IsReferenceType => !IsValueType;

        public virtual string SchemaBaseClass => "Schema";

        public string SafeType => IsReferenceType ? TypeNonNullable + "?" : Type;
    }
}
