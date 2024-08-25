namespace Bon.Serializer.Test.BonSerialization;

public class SerializationErrorTest : BonSerializerTestBase
{
    [Fact] public void ClassWithoutAttribute() => SerializationFailure(new object());
    [Fact] public void EnumWithoutAttribute() => SerializationFailure(SchemaType.Int);
    [Fact] public void InterfaceWithoutAttribute() => SerializationFailure<IDisposable>(new MemoryStream());
    [Fact] public void CustomGenericTypeWithNewTypeParameter() => SerializationFailure(new GenericClass<ushort>(1));

    [Fact]
    public async Task InvalidContext()
    {
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            BonSerializer.CreateAsync(new InvalidBonSerializerContext(), new InMemoryBlob()));

        Assert.Contains("did not find the class", exception.Message);
    }

    public class InvalidBonSerializerContext : IBonSerializerContext;
}
