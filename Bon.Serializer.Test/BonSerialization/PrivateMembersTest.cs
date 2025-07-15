namespace Bon.Serializer.Test.BonSerialization;

public class PrivateMembersTest : BonSerializerTestBase
{
    [Fact]
    public void WithNonPublicMembersTest()
    {
        var result = Serialize(new WithNonPublicMembers(1)).DeserializeFast<WithNonPublicMembers>();
        Assert.NotNull(result);
        Assert.True(result.HasOnlyZeroes());
    }
}
