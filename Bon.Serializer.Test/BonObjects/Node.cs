namespace Bon.Serializer.Test.BonObjects;

[BonObject]
[BonInclude(1, typeof(Leaf))]
[BonInclude(2, typeof(Node))]
public interface INode { }

[BonObject]
public sealed record class Node(
    [property: BonMember(1)] INode Left,
    [property: BonMember(2)] INode Right
) : INode;

[BonObject]
public sealed record class Leaf([property: BonMember(1)] int Int) : INode;
