namespace Bon.Serializer.Test.BonSerialization.Storage;

public class InvalidBlobTest
{
    [Fact] public void InvalidMarker() => Run([1, 2], "Expected marker");

    [Fact] public void InvalidVersion() => Run([0x5d, 0xad, 0xff], "Cannot handle block version");

    private static void Run(byte[] bytes, string expectedSubstring)
    {
        var blob = new FakeBlob { Bytes = bytes };
        var exception = Assert.ThrowsAny<Exception>(() => BonSerializer.CreateAsync(new BonSerializerContext(), blob).Result);
        Assert.Contains(expectedSubstring, exception.Message);
    }
}
