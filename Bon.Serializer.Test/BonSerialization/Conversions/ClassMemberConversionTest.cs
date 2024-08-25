namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class ClassMemberConversionTest : BonSerializerTestBase
{
    [Fact] public void WithDogToWithNullableDog() => DeserializeSlow(WithDog, WithNullableDog);
    [Fact] public void WithDogToWithIDog() => DeserializeSlow(WithDog, DefaultWithIDog);
    [Fact] public void WithDogToWithNullableIDog() => DeserializeSlow(WithDog, DefaultWithNullableIDog);
    [Fact] public void WithDogToWithCat() => DeserializeSlow(WithDog, WithCat);
    [Fact] public void WithDogToWithNullableCat() => DeserializeSlow(WithDog, WithNullableCat);
    [Fact] public void WithDogToInt() => DeserializeSlow(WithDog, 0);

    [Fact] public void WithNullableDogToWithDog() => DeserializeSlow(WithNullableDog, WithDog);
    [Fact] public void WithNullableDogToWithIDog() => DeserializeSlow(WithNullableDog, DefaultWithIDog);
    [Fact] public void WithNullableDogToWithNullableIDog() => DeserializeSlow(WithNullableDog, DefaultWithNullableIDog);
    [Fact] public void WithNullableDogToWithCat() => DeserializeSlow(WithNullableDog, WithCat);
    [Fact] public void WithNullableDogToWithNullableCat() => DeserializeSlow(WithNullableDog, WithNullableCat);
    [Fact] public void WithNullableDogToInt() => DeserializeSlow(WithNullableDog, 0);

    [Fact] public void DefaultWithNullableDogToWithDog() => DeserializeSlow(DefaultWithNullableDog, DefaultWithDog);
    [Fact] public void DefaultWithNullableDogToWithIDog() => DeserializeSlow(DefaultWithNullableDog, DefaultWithIDog);
    [Fact] public void DefaultWithNullableDogToWithNullableIDog() => DeserializeSlow(DefaultWithNullableDog, DefaultWithNullableIDog);
    [Fact] public void DefaultWithNullableDogToWithCat() => DeserializeSlow(DefaultWithNullableDog, DefaultWithCat);
    [Fact] public void DefaultWithNullableDogToWithNullableCat() => DeserializeSlow(DefaultWithNullableDog, DefaultWithNullableCat);
    [Fact] public void DefaultWithNullableDogToInt() => DeserializeSlow(DefaultWithNullableDog, 0);

    [Fact] public void LongToDateTime() => DeserializeSlow(TestHelper.DateTimeOffset.UtcTicks, TestHelper.DateTime);
    [Fact] public void LongToDateTimeOffset() => DeserializeSlow(TestHelper.DateTimeOffset.UtcTicks, TestHelper.DateTimeOffset);
    [Fact] public void DateTimeToDateTimeOffset() => DeserializeSlow(TestHelper.DateTime, TestHelper.DateTimeOffset);

    [Fact] public void DateTimeOffsetToDateTime() => DeserializeSlow(TestHelper.DateTimeOffset, TestHelper.DateTime);
    [Fact] public void TimeOnlyToTimeSpan() => DeserializeSlow(TestHelper.TimeOnly, TestHelper.TimeSpan);
    [Fact] public void TimeSpanToTimeOnly() => DeserializeSlow(TestHelper.TimeSpan, TestHelper.TimeOnly);

    [Fact] public void WithDictionaryToWithDictionaryOfCats() => DeserializeSlow(WithDictionary, WithDictionaryOfCats);
}
