namespace Bon.Serializer.Test;

public class AttributeTest
{
    // Mostly for higher coverage.
    [Fact]
    public void Run()
    {
        Assert.NotNull(new BonMemberAttribute(1));
        Assert.NotNull(new BonIncludeAttribute(1, typeof(bool)));
        Assert.NotNull(new BonReservedMembersAttribute(1, 2));
    }
}
