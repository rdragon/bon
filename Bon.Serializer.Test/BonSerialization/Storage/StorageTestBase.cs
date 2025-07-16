namespace Bon.Serializer.Test.BonSerialization.Storage;

public class StorageTestBase(FakeBlob? blob = null) : BonSerializerTestBase(blob)
{
    /// <summary>
    /// By using such a large member ID there is practically no chance that the schema returned by <see cref="GetSchemaStorageContents"/>
    /// already exists somewhere in the source code.
    /// </summary>
    private const int MemberId = 295764629;

    /// <summary>
    /// The contents ID of the schema.
    /// A large number to not conflict with IDs created by the source generation context.
    /// </summary>
    private const int SchemaContentsId = int.MaxValue;

    private const int Value = 355375878;
    private const uint BlockId = 0xb1cbdcb6;

    protected static byte[] GetSchemaStorageContents()
    {
        var member = new SchemaMemberData(MemberId, new SchemaData(SchemaType.Int, []));
        var schema = new SchemaContentsData(SchemaContentsId, [member]);
        var block = new Block(BlockId, [schema]);
        var stream = new MemoryStream();
        BlockSerializer.Serialize(stream, block);

        return stream.ToArray();
    }

    protected byte[] GetInstanceBytes() => [.. GetManualSerializer()
        .WriteFirstPartOfHeader(BlockId)
        .WriteSchemaType(SchemaType.NullableRecord)
        .WriteBool(false)
        .WriteCompactInt(SchemaContentsId)
        .WriteFullInt(Value)];

    protected static Class Instance => new(0, Value);

    [BonObject] public sealed record class Class([property: BonMember(1)] int X, [property: BonMember(MemberId)] int Y);
}
