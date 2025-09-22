namespace Bon.Serializer.Test.BonObjects;

[BonObject] public sealed record class EmptyClass;
[BonObject] public sealed record class Dog([property: BonMember(1)] int Age) : IDog;
[BonObject] public sealed record class Cat([property: BonMember(1)] string Name) : ICat;
[BonObject] public sealed record class WithIDog([property: BonMember(1)] IDog Dog);
[BonObject] public sealed record class WithICat([property: BonMember(1)] ICat Cat);
[BonObject] public sealed record class WithNullableIDog([property: BonMember(1)] IDog? Dog);
[BonObject] public sealed record class WithNullableICat([property: BonMember(1)] ICat? Cat);
[BonObject] public sealed record class WithIAnimal([property: BonMember(1)] IAnimal Animal);
[BonObject] public sealed record class WithNullableIAnimal([property: BonMember(1)] IAnimal? Animal);
[BonObject] public sealed record class WithDog([property: BonMember(1)] Dog Dog);
[BonObject] public sealed record class WithNullableDog([property: BonMember(1)] Dog? Dog);
[BonObject] public sealed record class WithCat([property: BonMember(1)] Cat Cat);
[BonObject] public sealed record class WithNullableCat([property: BonMember(1)] Cat? Cat);
[BonObject] public sealed record class WithWithDog([property: BonMember(1)] WithDog WithDog);

[BonObject] public sealed record class WithString([property: BonMember(1)] string Text) : IAnimalImitation, IAnimalFailedImitation;
[BonObject] public sealed record class WithNullableString([property: BonMember(1)] string? Text);
[BonObject] public sealed record class WithBool([property: BonMember(1)] bool Bool);
[BonObject] public sealed record class WithNullableBool([property: BonMember(1)] bool? Bool);
[BonObject] public sealed record class WithByte([property: BonMember(1)] byte Byte);
[BonObject] public sealed record class WithNullableByte([property: BonMember(1)] byte? Byte);
[BonObject] public sealed record class WithSByte([property: BonMember(1)] sbyte SByte);
[BonObject] public sealed record class WithNullableSByte([property: BonMember(1)] sbyte? SByte);
[BonObject] public sealed record class WithShort([property: BonMember(1)] short Short);
[BonObject] public sealed record class WithNullableShort([property: BonMember(1)] short? Short);
[BonObject] public sealed record class WithUShort([property: BonMember(1)] ushort UShort);
[BonObject] public sealed record class WithNullableUShort([property: BonMember(1)] ushort? UShort);
[BonObject] public sealed record class WithInt([property: BonMember(1)] int Int) : IAnimalImitation;
[BonObject] public sealed record class WithNullableInt([property: BonMember(1)] int? Int) : IAnimalFailedImitation;
[BonObject] public sealed record class WithUInt([property: BonMember(1)] uint UInt);
[BonObject] public sealed record class WithNullableUInt([property: BonMember(1)] uint? UInt);
[BonObject] public sealed record class WithLong([property: BonMember(1)] long Long);
[BonObject] public sealed record class WithNullableLong([property: BonMember(1)] long? Long);
[BonObject] public sealed record class WithULong([property: BonMember(1)] ulong ULong);
[BonObject] public sealed record class WithNullableULong([property: BonMember(1)] ulong? ULong);
[BonObject] public sealed record class WithFloat([property: BonMember(1)] float Float);
[BonObject] public sealed record class WithNullableFloat([property: BonMember(1)] float? Float);
[BonObject] public sealed record class WithDouble([property: BonMember(1)] double Double);
[BonObject] public sealed record class WithNullableDouble([property: BonMember(1)] double? Double);
[BonObject] public sealed record class WithDecimal([property: BonMember(1)] decimal Decimal);
[BonObject] public sealed record class WithNullableDecimal([property: BonMember(1)] decimal? Decimal);
[BonObject] public sealed record class WithGuid([property: BonMember(1)] Guid Guid);
[BonObject] public sealed record class WithNullableGuid([property: BonMember(1)] Guid? Guid);

[BonObject] public sealed record class WithDayOfWeek([property: BonMember(1)] DayOfWeek DayOfWeek);
[BonObject] public sealed record class WithNullableDayOfWeek([property: BonMember(1)] DayOfWeek? DayOfWeek);

[BonObject] public sealed record class WithDateTime([property: BonMember(1)] DateTime DateTime);
[BonObject] public sealed record class WithNullableDateTime([property: BonMember(1)] DateTime? DateTime);

[BonObject] public sealed record class WithDateTimeOffset([property: BonMember(1)] DateTimeOffset DateTimeOffset);
[BonObject] public sealed record class WithNullableDateTimeOffset([property: BonMember(1)] DateTimeOffset? DateTimeOffset);

[BonObject] public sealed record class WithDateOnly([property: BonMember(1)] DateOnly DateOnly);
[BonObject] public sealed record class WithNullableDateOnly([property: BonMember(1)] DateOnly? DateOnly);

[BonObject] public sealed record class WithTimeOnly([property: BonMember(1)] TimeOnly TimeOnly);
[BonObject] public sealed record class WithNullableTimeOnly([property: BonMember(1)] TimeOnly? TimeOnly);

[BonObject] public sealed record class WithTimeSpan([property: BonMember(1)] TimeSpan TimeSpan);
[BonObject] public sealed record class WithNullableTimeSpan([property: BonMember(1)] TimeSpan? TimeSpan);

[BonObject] public sealed record class WithChar([property: BonMember(1)] char Char);
[BonObject] public sealed record class WithNullableChar([property: BonMember(1)] char? Char);

[BonObject] public sealed record class WithTuple2([property: BonMember(1)] (Dog, int) Tuple);
[BonObject] public sealed record class WithNullableTuple2([property: BonMember(1)] (Dog, int)? Tuple);
[BonObject] public sealed record class WithTuple2OfNullables([property: BonMember(1)] (Dog?, int?) Tuple);
[BonObject] public sealed record class WithAlternativeTuple2([property: BonMember(1)] (Cat, string) Tuple);

[BonObject] public sealed record class WithTuple3([property: BonMember(1)] (Dog, int, IAnimal) Tuple);
[BonObject] public sealed record class WithNullableTuple3([property: BonMember(1)] (Dog, int, IAnimal)? Tuple);
[BonObject] public sealed record class WithTuple3OfNullables([property: BonMember(1)] (Dog?, int?, IAnimal?) Tuple);
[BonObject] public sealed record class WithAlternativeTuple3([property: BonMember(1)] (Cat, string, IDog) Tuple);

[BonObject] public sealed record class WithIntOnPositionTwo([property: BonMember(2)] int Int);

[BonObject] public sealed record class Turtle([property: BonMember(1)] int Age, [property: BonMember(2)] Turtle? Parent);
[BonObject] public sealed record class Tortoise([property: BonMember(1)] string Age, [property: BonMember(2)] Tortoise? Parent);

// A type for which the parameterless constructor needs to be selected.
[BonObject]
public sealed record class TwoInts
{
    [BonMember(1)] public int Int1 { get; init; }
    [BonMember(2)] public int Int2 { get; init; }

    public TwoInts() { }
    public TwoInts(int int1) => Int1 = int1;
    public TwoInts(int int1, uint int2) => (Int1, Int2) = (int1, (int)int2 + 1); // Argument two has the wrong type.
    public TwoInts(int int1, int int3) => (Int1, Int2) = (int1, int3 + 1); // Argument two has the wrong name.
}

[BonObject]
public sealed record class TwoIntsWithGap(
    [property: BonMember(1)] int Int1,
    [property: BonMember(3)] int Int2
);

[BonObject]
public sealed record class ThreeInts(
    [property: BonMember(1)] int Int1,
    [property: BonMember(2)] int Int2,
    [property: BonMember(3)] int Int3
);

[BonObject]
public sealed record class ThreeIntsScrambled(
    [property: BonMember(1)] int Int1,
    [property: BonMember(3)] int Int2,
    [property: BonMember(2)] int Int3
);

[BonObject]
public sealed record class FourInts(
    [property: BonMember(11)] int Int1,
    [property: BonMember(12)] int Int2,
    [property: BonMember(13)] int Int3,
    [property: BonMember(14)] int Int4
);

// Tests the situation of a type with a parameterless constructor as member of another type.
// It also tests a non-record struct and whether the required keyword doesn't cause issues.
// It also tests support for the internal keyword.
[BonObject]
internal readonly struct HoldsTwoInts
{
    [BonMember(1)] public required TwoInts TwoInts { get; init; }
}

[BonObject]
public sealed record class ChildClass([property: BonMember(1)] int Int1) : ParentClass;

[BonObject]
[BonInclude(1, typeof(ChildClass))]
public abstract record class ParentClass;

[BonObject]
public sealed record class GenericClass<T>([property: BonMember(1)] T Value);

[BonObject]
public sealed record class WithGenericClass([property: BonMember(1)] GenericClass<int> GenericClass);

[BonObject]
public class WithNonPublicMembers
{
    private int PrivateInt { get; }
    protected int ProtectedInt { get; }
    private readonly int _privateInt;
    protected readonly int _protectedInt;

    public WithNonPublicMembers() { }

    public WithNonPublicMembers(int value)
    {
        PrivateInt = value;
        ProtectedInt = value;
        _privateInt = value;
        _protectedInt = value;
    }

    public bool HasOnlyZeroes() => PrivateInt == 0 && ProtectedInt == 0 && _privateInt == 0 && _protectedInt == 0;
}

[BonObject]
public sealed record class WithField
{
    [BonMember(1)]
    public int Int;
}

[BonObject]
public sealed record class WithScrambledConstructor([property: BonMember(2)] int Int, [property: BonMember(1)] string String);
