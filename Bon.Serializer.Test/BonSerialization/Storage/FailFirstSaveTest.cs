namespace Bon.Serializer.Test.BonSerialization.Storage;

public sealed class FailFirstSaveTest : BonSerializerTestBase
{
    public FailFirstSaveTest() : base(new FakeBlob { FailFirstSave = true }) { }

    [Fact] public void Run() => RoundTripFast(Dog);
}
