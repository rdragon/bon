namespace Bon.Serializer.Test.BonSerialization.Storage;

public class RecursiveSchemaFromStorageTest : BonSerializerTestBase
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
    private const int ContentsId = int.MaxValue;

    private const int Value1 = 355375878;
    private const int Value2 = 671172479;
    private const uint BlockId = 0xb1cbdcb6;

    public RecursiveSchemaFromStorageTest() : base(new FakeBlob { Bytes = GetSchemaStorageContents() }) { }

    /// <summary>
    /// Test whether a recursive type that was serialized using a schema that cannot be found via the source generation context but only
    /// via the storage can be deserialized.
    /// </summary>
    [Fact] public void Run() => Assert.Equal(Instance, GetSerializationResult(GetInstanceBytes()).DeserializeSlow<Class>());

    private static byte[] GetSchemaStorageContents()
    {
        var memberA = new SchemaMemberData(2, new CustomSchemaData(SchemaType.Record, true, ContentsId));
        var memberB = new SchemaMemberData(MemberId, new SchemaData(SchemaType.Int, false, []));
        var schema = new SchemaContentsData(ContentsId, [memberA, memberB]);

        var block = new Block(BlockId, [schema]);
        var stream = new MemoryStream();
        BlockSerializer.Serialize(stream, block);

        return stream.ToArray();
    }

    private byte[] GetInstanceBytes() => [.. GetSimpleSerializer()
        .WriteFirstPartOfHeader(BlockId)
        .WriteWholeNumber((int)SchemaType.Record)
        .WriteBool(false)
        .WriteWholeNumber(ContentsId)
        .WriteByte(NOT_NULL)
        .WriteByte(NULL)
        .WriteInt(Value1)
        .WriteInt(Value2)];

    private static Class Instance => new(0, new(0, null, Value1), Value2);

    [BonObject]
    public sealed record class Class(
        [property: BonMember(1)] int X,
        [property: BonMember(2)] Class? Y,
        [property: BonMember(MemberId)] int Z);
}
