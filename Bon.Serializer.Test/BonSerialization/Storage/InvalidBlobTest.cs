namespace Bon.Serializer.Test.BonSerialization.Storage;

public class InvalidBlobTest : BonSerializerTestBase
{
    private const byte Timestamp = 255;
    private const byte LayoutId = 1;
    private const byte MemberCount = 1;

    [Fact]
    public void LayoutIdOutOfRange() => Run([
        Timestamp,
        0], "Layout ID out of range");

    [Fact]
    public void MemberCountOutOfRange() => Run([
        Timestamp,
        LayoutId,
        .. GetCompactIntBytes(10_001)], "Member count out of range");

    [Fact]
    public void MemberIdOutOfRange() => Run([
        Timestamp,
        LayoutId,
        MemberCount,
        .. GetCompactIntBytes(-1)], "Member ID out of range");

    private static void Run(byte[] bytes, string expectedSubstring)
    {
        var blob = new FakeBlob { Bytes = bytes };
        var exception = Assert.ThrowsAny<Exception>(() => BonSerializer.CreateAsync(new TestBonSerializerContext(), blob).Result);
        Assert.Contains(expectedSubstring, exception.Message);
    }
}
