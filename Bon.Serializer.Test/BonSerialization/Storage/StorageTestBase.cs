namespace Bon.Serializer.Test.BonSerialization.Storage;

public class StorageTestBase(FakeBlob? blob = null) : BonSerializerTestBase(blob)
{
    /// <summary>
    /// By using such a large member ID there is practically no chance that the schema returned by <see cref="GetLayoutStorageContents"/>
    /// already exists somewhere in the source code.
    /// </summary>
    private const int MemberId = 295764629;

    /// <summary>
    /// The contents ID of the schema.
    /// A large number to not conflict with IDs created by the source generation context.
    /// </summary>
    private const int SchemaLayoutId = int.MaxValue;

    private const int Value = 355375878;

    protected static byte[] GetLayoutStorageContents()
    {
        throw new NotImplementedException();//storage
        //var member = new SchemaMemberData(MemberId, new SchemaData(SchemaType.Int, []));
        //var schema = new SchemaContentsData(SchemaLayoutId, [member]);
        //var block = new Block(BlockId, [schema]);
        //var stream = new MemoryStream();
        //BlockSerializer.Serialize(stream, block);

        //return stream.ToArray();
    }

    protected byte[] GetInstanceBytes() => [.. GetManualSerializer()
        .WriteSchemaType(SchemaType.NullableRecord)
        .WriteCompactInt(SchemaLayoutId)
        .WriteNotNull()
        .WriteFullInt(Value)];

    protected static Class Instance => new(0, Value);

    [BonObject] public sealed record class Class([property: BonMember(1)] int X, [property: BonMember(MemberId)] int Y);
}
