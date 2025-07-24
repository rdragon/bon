namespace Bon.Serializer.Test.BonSerialization;

public class BonSerializerTestBase
{
    public const byte NOT_NULL = NativeWriter.NOT_NULL;
    public const byte NULL = NativeWriter.NULL;

    public const int Age = 70;
    public const int OtherAge = 80;
    public const int YetAnotherAge = 90;
    public const int FourthAge = 100;

    public const int DogId = 86;
    public const int CatId = 44;

    public FakeBlob Blob { get; }
    public BonSerializer BonSerializer { get; }

    public BonSerializerTestBase(FakeBlob? blob = null)
    {
        Blob = blob ?? new FakeBlob();
        BonSerializer = BonSerializer.CreateAsync(new TestBonSerializerContext(), Blob).Result;
    }

    public static EmptyClass EmptyClass => new();

    public static EmptyStruct EmptyStruct => default;
    public static EmptyStruct? NullableEmptyStruct => EmptyStruct;

    public static WithInt WithInt70 => new(70);
    public static WithInt? NullWithInt => null;


    public static Dog Dog => new(Age);
    public static Dog OtherDog => new(OtherAge);
    public static Dog DefaultDog => new(0);
    public static Dog RealDefaultDog => null!;
    public static Dog? DefaultNullableDog => null;
    public static Cat Cat => new(Age.ToString());
    public static Cat OtherCat => new(OtherAge.ToString());
    public static Cat DefaultCat => new(DefaultString);
    public static IAnimal IAnimal => Dog;
    public static IAnimal OtherIAnimal => Cat;
    public static IAnimal DefaultIAnimal => DefaultCat;
    public static IAnimal RealDefaultIAnimal => null!;
    public static IDog IDog => Dog;
    public static IDog DefaultIDog => DefaultDog;
    public static IDog RealDefaultIDog => null!;
    public static ICat ICat => Cat;
    public static ICat DefaultICat => DefaultCat;
    public static ICat RealDefaultICat => null!;
    public static WithIDog WithIDog => new(Dog);
    public static WithIDog DefaultWithIDog => new(DefaultDog);
    public static WithIDog RealDefaultWithIDog => new(null!);
    public static WithNullableIDog WithNullableIDog => new(Dog);
    public static WithNullableIDog DefaultWithNullableIDog => new(null);
    public static WithICat WithICat => new(Cat);
    public static WithICat DefaultWithICat => new(DefaultCat);
    public static WithICat DefaultWithNullICat => new(null!);
    public static WithNullableICat WithNullableICat => new(Cat);
    public static WithNullableICat DefaultWithNullableICat => new(null);
    public static WithIAnimal WithIAnimal => new(Dog);
    public static WithIAnimal DefaultWithIAnimal => new(DefaultIAnimal);
    public static WithIAnimal DefaultWithNullIAnimal => new(null!);
    public static WithNullableIAnimal WithNullableIAnimal => new(Dog);
    public static WithNullableIAnimal DefaultWithNullableIAnimal => new(null);
    public static WithNullableIAnimal RealDefaultWithNullableIAnimal => null!;
    public static WithDog WithDog => new(Dog);
    public static WithDog DefaultWithDog => new(DefaultDog);
    public static WithDog DefaultWithDogNull => new(null!);
    public static WithNullableDog WithNullableDog => new(Dog);
    public static WithNullableDog DefaultWithNullableDog => new(null);
    public static WithCat WithCat => new(Cat);
    public static WithCat DefaultWithCat => new(DefaultCat);
    public static WithCat RealDefaultWithCat => new(null!);
    public static WithNullableCat WithNullableCat => new(Cat);
    public static WithNullableCat DefaultWithNullableCat => new(null);

    public static byte[] ByteArray => [1, 2, 3];
    public static byte[] DefaultByteArray => [];
    public static int[] IntArray => [4, 5];
    public static int[] RealDefaultIntArray => null!;
    public static Dog[] Array => [Dog, OtherDog];
    public static Dog[] DefaultArray => [];
    public static Dog[] OtherArray => [Dog];
    public static List<Dog> List => new(Array);
    public static List<Dog> DefaultList => new();
    public static List<Dog> OtherList => new(OtherArray);
    public static IList<Dog> IList => Array;
    public static IList<Dog> DefaultIList => DefaultArray;
    public static IList<Dog> OtherIList => OtherArray;
    public static IReadOnlyList<Dog> IReadOnlyList => Array;
    public static IReadOnlyList<Dog> DefaultIReadOnlyList => DefaultArray;
    public static IReadOnlyList<Dog> OtherIReadOnlyList => OtherArray;
    public static ICollection<Dog> ICollection => Array;
    public static ICollection<Dog> DefaultICollection => DefaultArray;
    public static ICollection<Dog> OtherICollection => OtherArray;
    public static IReadOnlyCollection<Dog> IReadOnlyCollection => Array;
    public static IReadOnlyCollection<Dog> DefaultIReadOnlyCollection => DefaultArray;
    public static IReadOnlyCollection<Dog> OtherIReadOnlyCollection => OtherArray;
    public static IEnumerable<Dog> IEnumerable => Array;
    public static IEnumerable<Dog> DefaultIEnumerable => DefaultArray;
    public static IEnumerable<Dog> OtherIEnumerable => OtherArray;
    public static Dictionary<int, Dog> Dictionary => new() { [Int] = Dog };
    public static Dictionary<int, Dog> DefaultDictionary => new();
    public static Dictionary<int, Dog> RealDefaultDictionary => null!;
    public static IDictionary<int, Dog> IDictionary => Dictionary;
    public static IDictionary<int, Dog> DefaultIDictionary => DefaultDictionary;
    public static IReadOnlyDictionary<int, Dog> IReadOnlyDictionary => Dictionary;
    public static IReadOnlyDictionary<int, Dog> DefaultIReadOnlyDictionary => DefaultDictionary;
    public static Dictionary<string, Cat> DictionaryOfCats => new() { [Int.ToString()] = Cat };

    public static WithArray WithArray => new(Array);
    public static WithArray DefaultWithArray => new(DefaultArray);
    public static WithArray OtherWithArray => new(OtherArray);
    public static WithList WithList => new(List);
    public static WithList DefaultWithList => new(DefaultList);
    public static WithList OtherWithList => new(OtherList);
    public static WithIList WithIList => new(IList);
    public static WithIList DefaultWithIList => new(DefaultIList);
    public static WithIList OtherWithIList => new(OtherIList);
    public static WithIReadOnlyList WithIReadOnlyList => new(IReadOnlyList);
    public static WithIReadOnlyList DefaultWithIReadOnlyList => new(DefaultIReadOnlyList);
    public static WithIReadOnlyList OtherWithIReadOnlyList => new(OtherIReadOnlyList);
    public static WithICollection WithICollection => new(ICollection);
    public static WithICollection DefaultWithICollection => new(DefaultICollection);
    public static WithICollection OtherWithICollection => new(OtherICollection);
    public static WithIReadOnlyCollection WithIReadOnlyCollection => new(IReadOnlyCollection);
    public static WithIReadOnlyCollection DefaultWithIReadOnlyCollection => new(DefaultIReadOnlyCollection);
    public static WithIReadOnlyCollection OtherWithIReadOnlyCollection => new(OtherIReadOnlyCollection);
    public static WithIEnumerable WithIEnumerable => new(IEnumerable);
    public static WithIEnumerable DefaultWithIEnumerable => new(DefaultIEnumerable);
    public static WithIEnumerable OtherWithIEnumerable => new(OtherIEnumerable);
    public static WithDictionary WithDictionary => new(Dictionary);
    public static WithDictionary DefaultWithDictionary => new(DefaultDictionary);
    public static WithDictionary DefaultWithDictionaryNull => new(null!);
    public static WithDictionaryOfCats WithDictionaryOfCats => new(DictionaryOfCats);
    public static WithIDictionary WithIDictionary => new(IDictionary);
    public static WithIDictionary DefaultWithIDictionary => new(DefaultIDictionary);
    public static WithIReadOnlyDictionary WithIReadOnlyDictionary => new(IReadOnlyDictionary);
    public static WithIReadOnlyDictionary DefaultWithIReadOnlyDictionary => new(DefaultIReadOnlyDictionary);
    public static WithNullableDictionary WithNullableDictionary => new(Dictionary);
    public static WithNullableDictionary DefaultWithNullableDictionary => new(null);

    public static WithNullableArray WithNullableArray => new(Array);
    public static WithNullableArray DefaultWithNullableArray => new(null);
    public static WithNullableArray OtherWithNullableArray => new(OtherArray);
    public static WithNullableByteArray WithNullableByteArray => new(ByteArray);
    public static WithNullableByteArray DefaultWithNullableByteArray => new(null);
    public static WithByteArray WithByteArray => new(ByteArray);
    public static WithByteArray DefaultWithByteArray => new([]);
    public static WithArrayOfNullable WithArrayOfNullable => new([Dog, null]);
    public static WithArrayOfNullable DefaultWithArrayOfNullable => new([]);
    public static WithArrayOfNullable OtherWithArrayOfNullable => new([Dog]);
    public static WithArrayOfNullable AnotherWithArrayOfNullable => new([Dog, OtherDog]);
    public static WithArrayOfEmptyClass WithArrayOfEmptyClass => new([EmptyClass, EmptyClass]);
    public static WithArrayOfEmptyClass DefaultWithArrayOfEmptyClass => new([]);
    public static WithListOfEmptyClass WithListOfEmptyClass => new([EmptyClass, EmptyClass]);
    public static WithListOfEmptyClass DefaultWithListOfEmptyClass => new([]);
    public static WithNullableArrayOfEmptyClass WithNullableArrayOfEmptyClass => new([EmptyClass, EmptyClass]);
    public static WithNullableArrayOfEmptyClass DefaultWithNullableArrayOfEmptyClass => new(null);
    public static WithNullableListOfEmptyClass WithNullableListOfEmptyClass => new([EmptyClass, EmptyClass]);
    public static WithNullableListOfEmptyClass DefaultWithNullableListOfEmptyClass => new(null);
    public static WithNullableDictionaryOfCats WithNullableDictionaryOfCats => new(DictionaryOfCats);
    public static WithNullableDictionaryOfCats DefaultWithNullableDictionaryOfCats => new(null);

    public static Cat[] ArrayOfCats => [Cat, OtherCat];
    public static Cat[] DefaultArrayOfCats => [];
    public static Cat[] OtherArrayOfCats => [Cat];

    public static DayOfWeek DayOfWeek => DayOfWeek.Monday;
    public static DayOfWeek DefaultDayOfWeek => DayOfWeek.Sunday;
    public static DayOfWeek? NullableDayOfWeek => DayOfWeek;
    public static DayOfWeek? DefaultNullableDayOfWeek => null;
    public static WithDayOfWeek WithDayOfWeek => new(DayOfWeek);
    public static WithDayOfWeek DefaultWithDayOfWeek => new(DefaultDayOfWeek);
    public static WithNullableDayOfWeek WithNullableDayOfWeek => new(DayOfWeek);
    public static WithNullableDayOfWeek DefaultWithNullableDayOfWeek => new(null);

    public static WithString WithString => new(String);
    public static WithString WithStringNull => new(null!);
    public static WithString DefaultWithString => new(DefaultString);
    public static WithNullableString WithNullableString => new(String);
    public static WithNullableString DefaultWithNullableString => new(null);
    public static WithBool WithBool => new(Bool);
    public static WithBool DefaultWithBool => new(DefaultBool);
    public static WithNullableBool WithNullableBool => new(NullableBool);
    public static WithNullableBool DefaultWithNullableBool => new(DefaultNullableBool);
    public static WithByte WithByte => new(Byte);
    public static WithByte DefaultWithByte => new(DefaultByte);
    public static WithNullableByte WithNullableByte => new(NullableByte);
    public static WithNullableByte DefaultWithNullableByte => new(DefaultNullableByte);
    public static WithSByte WithSByte => new(SByte);
    public static WithSByte DefaultWithSByte => new(DefaultSByte);
    public static WithNullableSByte WithNullableSByte => new(NullableSByte);
    public static WithNullableSByte DefaultWithNullableSByte => new(DefaultNullableSByte);
    public static WithShort WithShort => new(Short);
    public static WithShort DefaultWithShort => new(DefaultShort);
    public static WithNullableShort WithNullableShort => new(NullableShort);
    public static WithNullableShort DefaultWithNullableShort => new(DefaultNullableShort);
    public static WithUShort WithUShort => new(UShort);
    public static WithUShort DefaultWithUShort => new(DefaultUShort);
    public static WithNullableUShort WithNullableUShort => new(NullableUShort);
    public static WithNullableUShort DefaultWithNullableUShort => new(DefaultNullableUShort);
    public static WithInt WithInt => new(Int);
    public static WithInt DefaultWithInt => new(DefaultInt);
    public static WithNullableInt WithNullableInt => new(NullableInt);
    public static WithNullableInt DefaultWithNullableInt => new(DefaultNullableInt);
    public static WithUInt WithUInt => new(UInt);
    public static WithUInt DefaultWithUInt => new(DefaultUInt);
    public static WithNullableUInt WithNullableUInt => new(NullableUInt);
    public static WithNullableUInt DefaultWithNullableUInt => new(DefaultNullableUInt);
    public static WithLong WithLong => new(Long);
    public static WithLong DefaultWithLong => new(DefaultLong);
    public static WithNullableLong WithNullableLong => new(NullableLong);
    public static WithNullableLong DefaultWithNullableLong => new(DefaultNullableLong);
    public static WithULong WithULong => new(ULong);
    public static WithULong DefaultWithULong => new(DefaultULong);
    public static WithNullableULong WithNullableULong => new(NullableULong);
    public static WithNullableULong DefaultWithNullableULong => new(DefaultNullableULong);
    public static WithFloat WithFloat => new(Float);
    public static WithFloat DefaultWithFloat => new(DefaultFloat);
    public static WithNullableFloat WithNullableFloat => new(NullableFloat);
    public static WithNullableFloat DefaultWithNullableFloat => new(DefaultNullableFloat);
    public static WithDouble WithDouble => new(Double);
    public static WithDouble DefaultWithDouble => new(DefaultDouble);
    public static WithNullableDouble WithNullableDouble => new(NullableDouble);
    public static WithNullableDouble DefaultWithNullableDouble => new(DefaultNullableDouble);
    public static WithDecimal WithDecimal => new(Decimal);
    public static WithDecimal DefaultWithDecimal => new(DefaultDecimal);
    public static WithNullableDecimal WithNullableDecimal => new(NullableDecimal);
    public static WithNullableDecimal DefaultWithNullableDecimal => new(DefaultNullableDecimal);
    public static WithGuid WithGuid => new(Guid);
    public static WithGuid DefaultWithGuid => new(DefaultGuid);
    public static WithNullableGuid WithNullableGuid => new(NullableGuid);
    public static WithNullableGuid DefaultWithNullableGuid => new(DefaultNullableGuid);

    public static IntEnum IntEnum => IntEnum.A;
    public static IntEnum DefaultIntEnum => 0;
    public static IntEnum? NullableIntEnum => IntEnum.A;
    public static IntEnum? DefaultNullableIntEnum => null;
    public static ByteEnum ByteEnum => ByteEnum.A;
    public static ByteEnum DefaultByteEnum => 0;
    public static ByteEnum? NullableByteEnum => ByteEnum.A;
    public static ByteEnum? DefaultNullableByteEnum => null;
    public static WithWithDog WithWithDog => new(WithDog);
    public static int Int1 => 10;
    public static int Int2 => 20;
    public static int Int3 => 30;
    public static ThreeIntsScrambled ThreeIntsScrambled => new(Int1, Int2, Int3);
    public static ThreeIntsScrambled DefaultThreeIntsScrambled => new(0, 0, 0);
    public static TwoInts TwoInts => CreateTwoInts(Int1, Int2);
    public static TwoInts DefaultTwoInts => CreateTwoInts(0, 0);
    public static TwoIntsWithGap TwoIntsWithGap => new(Int1, Int2);
    public static ThreeInts ThreeInts => new(Int1, Int2, Int3);
    public static ThreeInts DefaultThreeInts => new(0, 0, 0);
    internal static HoldsTwoInts HoldsTwoInts => new() { TwoInts = TwoInts };
    internal static HoldsTwoInts DefaultHoldsTwoInts => new() { TwoInts = DefaultTwoInts };
    public static House House => new(Age);
    public static House DefaultHouse => default;
    public static House? NullableHouse => House;
    public static House? DefaultNullableHouse => null;
    public static HoldsHouse HoldsHouse => new(House);
    public static HoldsHouse DefaultHoldsHouse => new(DefaultHouse);
    public static HoldsNullableHouse HoldsNullableHouse => new(House);
    public static HoldsNullableHouse? NullableHoldsNullableHouse => HoldsNullableHouse;
    public static HoldsNullableHouse DefaultHoldsNullableHouse => new(null);
    public static HoldsNullableHouse? NullableDefaultHoldsNullableHouse => (HoldsNullableHouse?)DefaultHoldsNullableHouse;
    public static HoldsInt HoldsInt => new(Int);
    public static HoldsInt HoldsSmallInt => new(SmallInt);

    public static string String => "abc";
    public static string OtherString => "d";
    public static string DefaultString => "";
    public static string? NullableString => String;
    public static string? DefaultNullableString => null;
    public static bool Bool => true;
    public static bool DefaultBool => false;
    public static bool? NullableBool => true;
    public static bool? DefaultNullableBool => null;
    public static byte Byte => byte.MaxValue - 10;
    public static byte DefaultByte => 0;
    public static byte? NullableByte => Byte;
    public static byte? DefaultNullableByte => null;
    public static sbyte SByte => sbyte.MaxValue - 10;
    public static sbyte DefaultSByte => 0;
    public static sbyte NegativeSByte => sbyte.MinValue + 10;
    public static sbyte? NullableSByte => SByte;
    public static sbyte? DefaultNullableSByte => null;
    public static short Short => short.MaxValue - 10;
    public static short DefaultShort => 0;
    public static short NegativeShort => short.MinValue + 10;
    public static short? NullableShort => Short;
    public static short? DefaultNullableShort => null;
    public static ushort UShort => ushort.MaxValue - 10;
    public static ushort DefaultUShort => 0;
    public static ushort? NullableUShort => UShort;
    public static ushort? DefaultNullableUShort => null;
    public static int Int => int.MaxValue - 10;
    public static int OtherInt => int.MaxValue - 20;
    public static int SmallInt => 30;
    public static int DefaultInt => 0;
    public static int NegativeInt => int.MinValue + 10;
    public static int? NullableInt => Int;
    public static int? DefaultNullableInt => null;
    public static uint UInt => uint.MaxValue - 10;
    public static uint DefaultUInt => 0;
    public static uint? NullableUInt => UInt;
    public static uint? DefaultNullableUInt => null;
    public static long Long => long.MaxValue - 10;
    public static long DefaultLong => 0;
    public static long NegativeLong => long.MinValue + 10;
    public static long? NullableLong => Long;
    public static long? DefaultNullableLong => null;
    public static ulong ULong => ulong.MaxValue - 10;
    public static ulong DefaultULong => 0;
    public static ulong? NullableULong => ULong;
    public static ulong? DefaultNullableULong => null;
    public static float Float => float.Pi;
    public static float DefaultFloat => 0;
    public static float? NullableFloat => Float;
    public static float? DefaultNullableFloat => null;
    public static double Double => double.E;
    public static double DefaultDouble => 0;
    public static double? NullableDouble => Double;
    public static double? DefaultNullableDouble => null;
    public static decimal Decimal => decimal.MaxValue - 10;
    public static decimal DefaultDecimal => 0;
    public static decimal? NullableDecimal => Decimal;
    public static decimal? DefaultNullableDecimal => null;
    public static Guid Guid => TestHelper.Guid;
    public static Guid DefaultGuid => Guid.Empty;
    public static Guid? NullableGuid => Guid;
    public static Guid? DefaultNullableGuid => null;

    public static IAnimalFailedImitation IAnimalFailedImitation => WithNullableInt;
    public static WithIntOnPositionTwo WithIntOnPositionTwo => new(Int);

    public static DateTime DefaultDateTime => default;
    public static DateTime? NullableDateTime => TestHelper.DateTime;
    public static DateTime? DefaultNullableDateTime => null;
    public static WithDateTime WithDateTime => new(TestHelper.DateTime);
    public static WithDateTime DefaultWithDateTime => new(default);
    public static WithNullableDateTime WithNullableDateTime => new(TestHelper.DateTime);
    public static WithNullableDateTime DefaultWithNullableDateTime => new(null);

    public static DateTimeOffset DefaultDateTimeOffset => default;
    public static DateTimeOffset? NullableDateTimeOffset => TestHelper.DateTimeOffset;
    public static DateTimeOffset? DefaultNullableDateTimeOffset => null;
    public static WithDateTimeOffset WithDateTimeOffset => new(TestHelper.DateTimeOffset);
    public static WithDateTimeOffset DefaultWithDateTimeOffset => new(default);
    public static WithNullableDateTimeOffset WithNullableDateTimeOffset => new(TestHelper.DateTimeOffset);
    public static WithNullableDateTimeOffset DefaultWithNullableDateTimeOffset => new(null);

    public static DateOnly DefaultDateOnly => default;
    public static DateOnly? NullableDateOnly => TestHelper.DateOnly;
    public static DateOnly? DefaultNullableDateOnly => null;
    public static WithDateOnly WithDateOnly => new(TestHelper.DateOnly);
    public static WithDateOnly DefaultWithDateOnly => new(default);
    public static WithNullableDateOnly WithNullableDateOnly => new(TestHelper.DateOnly);
    public static WithNullableDateOnly DefaultWithNullableDateOnly => new(null);

    public static TimeOnly DefaultTimeOnly => default;
    public static TimeOnly? NullableTimeOnly => TestHelper.TimeOnly;
    public static TimeOnly? DefaultNullableTimeOnly => null;
    public static WithTimeOnly WithTimeOnly => new(TestHelper.TimeOnly);
    public static WithTimeOnly DefaultWithTimeOnly => new(default);
    public static WithNullableTimeOnly WithNullableTimeOnly => new(TestHelper.TimeOnly);
    public static WithNullableTimeOnly DefaultWithNullableTimeOnly => new(null);

    public static TimeSpan DefaultTimeSpan => default;
    public static TimeSpan? NullableTimeSpan => TestHelper.TimeSpan;
    public static TimeSpan? DefaultNullableTimeSpan => null;
    public static WithTimeSpan WithTimeSpan => new(TestHelper.TimeSpan);
    public static WithTimeSpan DefaultWithTimeSpan => new(default);
    public static WithNullableTimeSpan WithNullableTimeSpan => new(TestHelper.TimeSpan);
    public static WithNullableTimeSpan DefaultWithNullableTimeSpan => new(null);

    public static char Char => 'a';
    public static char DefaultChar => '\0';
    public static char? NullableChar => Char;
    public static char? DefaultNullableChar => null;
    public static WithChar WithChar => new(Char);
    public static WithChar DefaultWithChar => new(DefaultChar);
    public static WithNullableChar WithNullableChar => new(Char);
    public static WithNullableChar DefaultWithNullableChar => new(null);
    public static char[] CharArray => [Char, 'b'];

    public static (int, int) IntTuple2 => (Int, OtherInt);
    public static (int, int)? NullableIntTuple2 => (Int, OtherInt);
    public static (int, int)? DefaultNullableIntTuple2 => null;
    public static (Dog, int) Tuple2 => (Dog, Int);
    public static (Dog, int) DefaultTuple2 => (DefaultDog, DefaultInt);
    public static (Dog, int) RealDefaultTuple2 => (null!, DefaultInt);
    public static (Dog, int)? NullableTuple2 => Tuple2;
    public static (Dog, int)? DefaultNullableTuple2 => null;
    public static WithTuple2 WithTuple2 => new(Tuple2);
    public static WithTuple2 DefaultWithTuple2 => new(DefaultTuple2);
    public static WithTuple2 RealDefaultWithTuple2 => new(RealDefaultTuple2);
    public static WithNullableTuple2 WithNullableTuple2 => new(Tuple2);
    public static WithNullableTuple2 DefaultWithNullableTuple2 => new(null);
    public static WithTuple2OfNullables WithTuple2OfNullables => new((Dog, Int));
    public static WithTuple2OfNullables DefaultWithTuple2OfNullables => new((null, null));
    public static (Cat, string) AlternativeTuple2 => (Cat, Int.ToString());
    public static WithAlternativeTuple2 WithAlternativeTuple2 => new(AlternativeTuple2);

    public static (Dog, int, IAnimal) Tuple3 => (Dog, Int, IAnimal);
    public static (Dog, int, IAnimal) DefaultTuple3 => (DefaultDog, DefaultInt, DefaultIAnimal);
    public static (Dog, int, IAnimal)? NullableTuple3 => Tuple3;
    public static (Dog, int, IAnimal)? DefaultNullableTuple3 => null;
    public static WithTuple3 WithTuple3 => new(Tuple3);
    public static WithTuple3 DefaultWithTuple3 => new(DefaultTuple3);
    public static WithNullableTuple3 WithNullableTuple3 => new(Tuple3);
    public static WithNullableTuple3 DefaultWithNullableTuple3 => new(null);
    public static WithTuple3OfNullables WithTuple3OfNullables => new((Dog, Int, IAnimal));
    public static WithTuple3OfNullables DefaultWithTuple3OfNullables => new((null, null, null));
    public static (Cat, string, IDog) AlternativeTuple3 => (Cat, Int.ToString(), Dog);
    public static WithAlternativeTuple3 WithAlternativeTuple3 => new(AlternativeTuple3);

    public static Turtle Turtle1 => new(Int1, null);
    public static Turtle Turtle2 => new(Int2, Turtle1);
    public static Turtle Turtle3 => new(Int3, Turtle2);
    public static Turtle DefaultTurtle => new(DefaultInt, null);
    public static Tortoise Tortoise1 => new(Int1.ToString(), null);
    public static Tortoise Tortoise2 => new(Int2.ToString(), Tortoise1);
    public static Tortoise Tortoise3 => new(Int3.ToString(), Tortoise2);

    public static INode Leaf1 => new Leaf(Int1);
    public static INode Leaf2 => new Leaf(Int2);
    public static INode Leaf3 => new Leaf(Int3);
    public static INode Node1 => new Node(Leaf1, Leaf2);
    public static INode Node2 => new Node(Node1, Leaf3);
    public static INode Node3 => new Node(Node1, Node2);
    public static INode DefaultINode => new Leaf(DefaultInt);

    public static WithField WithField => new() { Int = Int };
    public static WithField DefaultWithField => new();

    public SerializationResult Serialize<T>(T value, BonSerializerOptions? options = null)
    {
        var stream = new MemoryStream();
        BonSerializer.Serialize(stream, value, options);
        return new SerializationResult(BonSerializer, stream);
    }

    public SerializationResult GetSerializationResult(byte[] bytes) => new(BonSerializer, new MemoryStream(bytes));

    /// <summary>
    /// First serializes the source value.
    /// Then deserializes the result into a new value of type <typeparamref name="T2"/>.
    /// Finally asserts that the new value is equal to the expected value.
    /// The distinction between fast and slow is that fast means the deserialize method already exists and for slow the
    /// deserialize method is created on the fly.
    /// </summary>
    public void DeserializeSlow<T1, T2>(T1 source, T2 expected, BonSerializerOptions? serializationOptions = null) =>
        Assert.Equal(expected, Serialize(source, serializationOptions).DeserializeSlow<T2>());

    /// <summary>
    /// First serializes the source value.
    /// Then deserializes the result into a new value of type <typeparamref name="T2"/>.
    /// Finally asserts that the new value is equal to the expected value.
    /// The distinction between fast and slow is that fast means the deserialize method already exists and for slow the
    /// deserialize method is created on the fly.
    /// </summary>
    public void DeserializeFast<T1, T2>(T1 source, T2 expected, BonSerializerOptions? serializationOptions = null) =>
        Assert.Equal(expected, Serialize(source, serializationOptions).DeserializeFast<T2>());

    public ManualSerializer GetManualSerializer() => new(this);

    /// <summary>
    /// First serializes the value.
    /// Then deserializes the result into a new value of type <typeparamref name="T"/>.
    /// Finally asserts that the new value is equal to the original value.
    /// The distinction between fast and slow is that fast means the deserialize method already exists and for slow the
    /// deserialize method is created on the fly.
    /// </summary>
    public void RoundTripFast<T>(T value) => Assert.Equal(value, Serialize(value).DeserializeFast<T>());

    /// <summary>
    /// First serializes the value.
    /// Then deserializes the result into a new value of type <typeparamref name="T"/>.
    /// Finally asserts that the new value is equal to the original value.
    /// The distinction between fast and slow is that fast means the deserialize method already exists and for slow the
    /// deserialize method is created on the fly.
    /// </summary>
    public void RoundTripSlow<T>(T value) => Assert.Equal(value, Serialize(value).DeserializeSlow<T>());

    public int GetLayoutId<T>() => BonSerializer.GetLayoutId(typeof(T));

    public string PrintSchema<T>() => BonSerializer.PrintSchema<T>();

    protected void RequireSameSerialization<T1, T2>(T1 expected, T2 actual, BonSerializerOptions? expectedValueOptions = null) =>
        Assert.Equal(Serialize(expected, expectedValueOptions), Serialize(actual));

    public static TwoInts CreateTwoInts(int int1, int int2) => new() { Int1 = int1, Int2 = int2 };

    protected void SerializationFailure<T>(T value)
    {
        var exception = Assert.Throws<SchemaException>(() => Serialize(value));
        Assert.StartsWith("No schema for", exception.Message);

        exception = Assert.Throws<SchemaException>(() => Serialize(0).DeserializeSlow<T>());
        Assert.StartsWith("No schema for", exception.Message);
    }

    /// <summary>
    /// This value is used to get the serializer output a specific schema type, namely the schema type of the type that
    /// is serialized.
    /// </summary>
    public static BonSerializerOptions ForbidSchemaTypeOptimization => new() { AllowSchemaTypeOptimization = false };
}
