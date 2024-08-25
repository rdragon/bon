using System.Text;

namespace Bon.Serializer.Test.Serialization;

public class StringSerializerTest
{
    [Theory]
    [MemberData(nameof(RoundTripData))]
    public void RoundTrip(string expected)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        var reader = new BinaryReader(stream);
        StringSerializer.WriteString(writer, expected);
        stream.Position = 0;
        var actual = StringDeserializer.ReadString(reader);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string> RoundTripData
    {
        get
        {
            var random = new Random(1);
            var data = new TheoryData<string>();

            for (int i = 0; i < 10_000; i += i < 200 ? 1 : random.Next(i / 5))
            {
                data.Add(GetRandomString(random, i));
            }

            for (int i = 0; i < 50; i++)
            {
                data.Add(new string('€', i));
            }

            data.Add(new string('€', 64 * 1024 / 2));

            return data;
        }
    }

    private static string GetRandomString(Random random, int length)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.Append((char)('A' + random.Next(26)));
        }

        return sb.ToString();
    }
}
