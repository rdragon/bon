namespace Bon.FileInspector.Test;

public class InspectorTest
{
    private readonly MockFileSystem _fileSystem;
    private readonly BonSerializer _serializer;

    public InspectorTest()
    {
        _fileSystem = new MockFileSystem();
        _serializer = CreateSerializer().Result;
    }

    [Fact] public Task IntToJson() => BonToJson(3);
    [Fact] public Task JsonToInt() => JsonToBon(3);
    [Fact] public Task RecursiveClassToJson() => BonToJson(new RecursiveClass(5, new RecursiveClass(6, null)));
    [Fact] public Task JsonToRecursiveClass() => JsonToBon(new RecursiveClass(5, new RecursiveClass(6, null)));

    [Fact]
    public async Task SchemaToText()
    {
        // The inspector will write the output to 'schemas.txt'.
        // If that file already exists it should be overwritten.
        _fileSystem.File.WriteAllText("schemas.txt", "These contents should be overwritten.");

        var blockId = await WriteSchemaFile();
        await RunInspector("schemas");
        var actual = _fileSystem.File.ReadAllText("schemas.txt").Trim();

        var expected = $"""
            schema file

            block {blockId}
                schema 1
                    (empty)

                schema 2
                    1: Int
                    2: NullableRecord_2

                schema 3
                    1: NullableRecord_1[]

                schema 4
                    2: Dictionary<String, SignedWholeNumber>

                schema 5
                    3: (WholeNumber, Long)
            """;

        Assert.Equal(expected, actual);
    }

    private async Task<uint> WriteSchemaFile(string path = "schemas")
    {
        var serializer = await CreateSerializer(path);

        return serializer.LastBlockId;
    }

    [Fact]
    public async Task FileNotFound()
    {
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => RunInspector("data"));
        Assert.Contains("File not found", exception.Message);
    }

    [Fact]
    public async Task NoFiles()
    {
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => RunInspector());
        Assert.Contains("at least one", exception.Message);
    }

    [Fact]
    public async Task MoreThanOneSchemaFile()
    {
        await WriteSchemaFile("schemas1");
        SerializeToFile(true);
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => RunInspector("data", "schemas", "schemas1"));
        Assert.Contains("more than one", exception.Message);
    }

    private async Task BonToJson<T>(T expected)
    {
        SerializeToFile(expected);
        await RunInspector("data", "schemas");
        var json = _fileSystem.File.ReadAllText("data.json");
        var bytes = _serializer.JsonToBon(json);
        var actual = _serializer.Deserialize<T>(bytes);

        Assert.Equal(expected, actual);
    }

    private void SerializeToFile<T>(T value)
    {
        var stream = _fileSystem.File.OpenWrite("data");
        _serializer.Serialize(stream, value);
        stream.Close();
    }

    private async Task JsonToBon<T>(T expected)
    {
        var json = GetJson(expected);
        _fileSystem.File.WriteAllText("data.json", json);
        await RunInspector("data.json", "schemas");
        var bytes = _fileSystem.File.ReadAllBytes("data.json.bon");
        var actual = _serializer.Deserialize<T>(bytes);

        Assert.Equal(expected, actual);
    }

    private string GetJson<T>(T value)
    {
        var bytes = _serializer.Serialize(value);
        return _serializer.BonToJson(bytes);
    }

    private async Task RunInspector(params string[] args)
    {
        var inspector = new Inspector(_fileSystem);
        await inspector.Run(args);
    }

    private async Task<BonSerializer> CreateSerializer(string path = "schemas")
    {
        return await BonSerializer.CreateAsync(new BonSerializerContext(), new MockableFileSystemBlob(_fileSystem, path));
    }
}
