﻿namespace Bon.Serializer.Schemas;

/// <summary>
/// A <see cref="RecordSchema"/> or <see cref="UnionSchema"/>.
/// </summary>
public abstract class CustomSchema(SchemaType schemaType, bool isNullable) : Schema(schemaType, isNullable)
{
    /// <summary>
    /// The members of the record or union, ordered by ID.
    /// </summary>
    public IReadOnlyList<SchemaMember> Members { get; set; } = null!;

    /// <summary>
    /// An ID that uniquely identifies the members inside the schema.
    /// Only set for custom schemas generated by the source generation context as this ID is only needed when a custom schema
    /// is serialized and this only happens for schemas that correspond to a known type.
    /// </summary>
    public int ContentsId { get; set; }

    public SchemaContents Contents => new(Members);

    public override IEnumerable<Schema> GetInnerSchemas() => Members.Select(member => member.Schema);

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        if (StartEquals(obj, ancestors, out CustomSchema other) is bool result)
        {
            return result;
        }

        if (Members.Count != other.Members.Count)
        {
            return false;
        }

        for (int i = 0; i < Members.Count; i++)
        {
            var member = Members[i];
            var otherMember = other.Members[i];

            if (member.Id != otherMember.Id || !member.Schema.Equals(otherMember.Schema, ancestors))
            {
                return false;
            }
        }

        EndEquals(other, ancestors);

        return true;
    }

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
        hashCode.Add(IsNullable);

        if (ancestors.TryAdd(this, ancestors.Count))
        {
            foreach (var member in Members)
            {
                hashCode.Add(member.Id);
                member.Schema.AppendHashCode(ancestors, ref hashCode);
            }

            ancestors.Remove(this);
        }
        else
        {
            hashCode.Add(ancestors[this]);
        }
    }

    public void SetMembers(params SchemaMember[] members) => Members = members;

    public static CustomSchema Create(SchemaType schemaType, bool isNullable, IReadOnlyList<SchemaMember> members, int contentsId)
    {
        var schema = Create(schemaType, isNullable);

        schema.Members = members;
        schema.ContentsId = contentsId;

        return schema;
    }

    /// <summary>
    /// Creates a custom schema that does not yet have any members nor a contents ID.
    /// </summary>
    public static CustomSchema Create(SchemaType schemaType, bool isNullable)
    {
        CustomSchema schema = schemaType switch
        {
            SchemaType.Record => new RecordSchema(schemaType, isNullable),
            SchemaType.Union => new UnionSchema(schemaType, isNullable),
            _ => throw new ArgumentOutOfRangeException(nameof(schemaType), schemaType, null),
        };

        return schema;
    }
}

/// <summary>
/// Represents a member of a record or union.
/// </summary>
/// <param name="Id">The ID of the member retrieved from the <see cref="BonMemberAttribute"/> attribute.</param>
/// <param name="Schema">The schema of the member.</param>
public sealed record class SchemaMember(int Id, Schema Schema)
{
    public SchemaType SchemaType => Schema.SchemaType;
    public bool IsNullable => Schema.IsNullable;
}

/// <summary>
/// Represents a class or a struct.
/// The class or struct doesn't have to be a C# record (but it can be).
/// </summary>
public sealed class RecordSchema(SchemaType schemaType, bool isNullable) : CustomSchema(schemaType, isNullable);

/// <summary>
/// Represents an interface or an abstract class.
/// </summary>
/// <param name="ContentsId">An ID that uniquely identifies the members inside the schema.</param>
/// <param name="Members">The members of the union, ordered by ID.</param>
public sealed class UnionSchema(SchemaType schemaType, bool isNullable) : CustomSchema(schemaType, isNullable);
