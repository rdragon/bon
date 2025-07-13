namespace Bon.Serializer.Test.BonSerialization;

public class DeserializationErrorTest : BonSerializerTestBase
{
    [Fact] public void InvalidVersion() => Fail([0x01], "Cannot handle wire format version");
    [Fact] public void UnknownBlock() => Fail([.. GetSimpleSerializer().WriteFirstPartOfHeader(0x0d6948da)], "Cannot find the schemas");

    private void Fail(byte[] bytes, string expectedSubstring)
    {
        var exception = Assert.ThrowsAny<Exception>(() => BonSerializer.DeserializeAsync<int>(bytes).Result);
        Assert.Contains(expectedSubstring, exception.Message);
    }
}
