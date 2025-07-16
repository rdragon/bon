namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents the way in which a value is serialized.
/// A schema is critical in correctly deserializing a value.
/// Without knowing the schema, the bytes do not have any meaning.
/// A schema can only be used to obtain the members of a record or union.
/// </summary>
public sealed partial class Schema
{
    private SchemaType _schemaType;

    private SchemaFlags _schemaFlags;

    /// <summary>
    /// The schema arguments.
    /// Contains at least one value if this is a generic schema.
    /// </summary>
    public IReadOnlyList<Schema> SchemaArguments { get; set; } = null!;

    /// <summary>
    /// The ID of the layout corresponding to this schema.
    /// Only set for custom schemas (records and unions).
    /// This field contains a "fake" layout ID if the schema was created by the
    /// source generation context and not yet assigned a "real" layout ID.
    /// </summary>
    public int LayoutId { get; set; }

    /// <summary>
    /// The members of the schema.
    /// For records, these are the fields of the record.
    /// For unions, these are the cases of the union.
    /// For other schemas, this field is empty.
    /// </summary>
    public IReadOnlyList<SchemaMember> Members { get; set; } = null!;

    public SchemaType SchemaType
    {
        get => _schemaType;

        private set
        {
            _schemaType = value;
            _schemaFlags = value.GetSchemaFlags();
        }
    }

    /// <summary>
    /// Whether this is a custom schema (record or union).
    /// </summary>
    public bool IsCustom => (_schemaFlags & SchemaFlags.IsCustom) != 0;

    /// <summary>
    /// Whether this is a (nullable) record schema (struct or class).
    /// </summary>
    public bool IsRecord => SchemaType == SchemaType.Record || SchemaType == SchemaType.NullableRecord;

    /// <summary>
    /// Whether this is a union schema (interface or abstract class).
    /// </summary>
    public bool IsUnion => SchemaType == SchemaType.Union;

    /// <summary>
    /// Whether this is a native schema.
    /// </summary>
    public bool IsNative => (_schemaFlags & SchemaFlags.IsNative) != 0;

    /// <summary>
    /// Whether this is a (nullable) tuple schema (value tuple).
    /// </summary>
    public bool IsTuple => (_schemaFlags & SchemaFlags.IsTuple) != 0;

    /// <summary>
    /// Whether this is a (nullable) tuple2 schema (value tuple with two elements).
    /// </summary>
    public bool IsTuple2 => SchemaType == SchemaType.Tuple2 || SchemaType == SchemaType.NullableTuple2;

    /// <summary>
    /// Whether this is a (nullable) tuple3 schema (value tuple with three elements).
    /// </summary>
    public bool IsTuple3 => SchemaType == SchemaType.Tuple3 || SchemaType == SchemaType.NullableTuple3;

    /// <summary>
    /// Whether this is a nullable schema.
    /// </summary>
    public bool IsNullable => (_schemaFlags & SchemaFlags.IsNullable) != 0;

    /// <summary>
    /// Whether this is an array schema.
    /// </summary>
    public bool IsArray => SchemaType == SchemaType.Array;

    /// <summary>
    /// Whether this is a dictionary schema.
    /// </summary>
    public bool IsDictionary => SchemaType == SchemaType.Dictionary;

    public override bool Equals(object? obj) => obj is Schema schema && SchemaComparer.Equals(this, schema);

    public override int GetHashCode() => SchemaComparer.GetHashCode(this);

    public Schema GetClone(SchemaType schemaType) => Create(schemaType, SchemaArguments, LayoutId, Members);
}

/// <summary>
/// A member of a schema.
/// Represents a field of a record or a case of a union.
/// </summary>
/// <param name="Id">
/// The ID of the member.
/// This is the value from the BonMember or BonInclude attribute.
/// This value is non-negative.
/// </param>
/// <param name="Schema">The schema of the member.</param>
public readonly record struct SchemaMember(int Id, Schema Schema);

/// <summary>
/// A layout is a schema without a schema type and without schema arguments.
/// A layout is the object that is saved to the storage.
/// Layouts only exist for custom schemas (records and unions).
/// 
/// There are two reasons why the schema type is not part of a layout:
/// 1. The schema type is already included in the header of a message.
/// 2. By excluding the schema type, there are less layouts to save to the storage.
/// 
/// The schema arguments are not part of a layout because only custom schemas have
/// a layout, and custom schemas currently have no schema arguments (we don't support
/// generic custom schemas yet).
/// 
/// Layouts do not exist for tuples and arrays because it is not possible to create
/// a layout for every possible tuple and array. Also, we should only add new layouts
/// when a BonSerializer instance is created, and at that moment in time we don't know
/// which tuples and arrays the user is going to serialize.
/// </summary>
/// <param name="Id">The ID of the layout. The first layout has ID 1 and the next layout has ID 2, etc.</param>
/// <param name="Members">The members of the layout.</param>
public readonly record struct Layout(int Id, IReadOnlyList<SchemaMember> Members);
