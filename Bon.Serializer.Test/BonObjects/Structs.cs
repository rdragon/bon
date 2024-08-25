namespace Bon.Serializer.Test.BonObjects;

[BonObject] public readonly record struct EmptyStruct;
[BonObject] public readonly record struct House([property: BonMember(1)] int Floors);
[BonObject] public readonly record struct HoldsHouse([property: BonMember(1)] House House);
[BonObject] public readonly record struct HoldsNullableHouse([property: BonMember(1)] House? House);
[BonObject] public readonly record struct HoldsInt([property: BonMember(1)] int Int);

