namespace Bon.Serializer.Test.BonSerialization.Storage;

public class TwoSchemasFromStorageTest : BonSerializerTestBase
{
    /// <summary>
    /// By using such a large member ID there is practically no chance that the schemas returned by <see cref="GetLayoutStorageContents"/>
    /// already exists somewhere in the source code.
    /// </summary>
    private const int MemberId = 295764629;

    // The contents IDs of the schemas.
    // Large numbers to not conflict with IDs created by the source generation context.
    private const int LayoutIdA = int.MaxValue;
    private const int LayoutIdB = int.MaxValue - 1;

    private const int Value = 355375878;
    private const uint BlockId = 0xb1cbdcb6;

    public TwoSchemasFromStorageTest() : base(new FakeBlob { Bytes = GetLayoutStorageContents() }) { }

    /// <summary>
    /// Test whether a type that was serialized using a schema that cannot be found via the source generation context but only
    /// via the storage can be deserialized.
    /// The type contains another type that cannot be found via the source generation context.
    /// Both types are found in the same schema block.
    /// </summary>
    [Fact] public void Run() => Assert.Equal(Instance, GetSerializationResult(GetInstanceBytes()).DeserializeSlow<ClassB>());

    private static byte[] GetLayoutStorageContents()
    {
        var memberA = new SchemaMemberData(MemberId, new SchemaData(SchemaType.Int, []));
        var schemaA = new SchemaContentsData(LayoutIdA, [memberA]);

        var memberB = new SchemaMemberData(MemberId, new CustomSchemaData(SchemaType.NullableRecord, LayoutIdA));
        var schemaB = new SchemaContentsData(LayoutIdB, [memberB]);

        var block = new Block(BlockId, [schemaA, schemaB]);
        var stream = new MemoryStream();
        BlockSerializer.Serialize(stream, block);

        return stream.ToArray();
    }

    private byte[] GetInstanceBytes() => [.. GetManualSerializer()
        .WriteSchemaType(SchemaType.NullableRecord)
        .WriteCompactInt(LayoutIdB)
        .WriteNotNull()
        .WriteNotNull()
        .WriteFullInt(Value)];

    private static ClassB Instance => new(0, new(0, Value));

    [BonObject] public sealed record class ClassA([property: BonMember(1)] int X, [property: BonMember(MemberId)] int Y);
    [BonObject] public sealed record class ClassB([property: BonMember(1)] int X, [property: BonMember(MemberId)] ClassA Y);
}
