namespace Bon.FileInspector.Test;

public partial class InspectorTest
{
    private readonly IFileSystem _fileSystem = new MockFileSystem();

    [Fact] public Task IntToJson() => BonToJson(3, "CwA=", "3");
    [Fact] public Task RecursiveClassToJson() => BonToJson(new RecursiveClass(5, new RecursiveClass(6, null)), "AQAC", "[5,[6,null]]");
    [Fact] public Task JsonToInt() => JsonToBon(3);
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
                    2: Record_2?

                schema 3
                    1: Record_1[]

                schema 4
                    2: Dictionary<String, SignedWholeNumber?>

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
    public async Task InvalidFile()
    {
        _fileSystem.File.WriteAllBytes("data", [1, 2]);
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => RunInspector("data"));
        Assert.Contains("Invalid file", exception.Message);
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
        await SerializeToFile(true);
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => RunInspector("data", "schemas", "schemas1"));
        Assert.Contains("more than one", exception.Message);
    }

    private async Task BonToJson<T>(T value, string expectedSchema, string expectedData)
    {
        var blockId = await SerializeToFile(value);
        await RunInspector("data", "schemas");

        var expected = $$"""
            {"blockId":{{blockId}},"schema":"{{expectedSchema}}","data":{{expectedData}}}
            """;

        var actual = _fileSystem.File.ReadAllText("data.json");

        Assert.Equal(expected, actual);
    }

    private async Task<uint> SerializeToFile<T>(T value)
    {
        var serializer = await CreateSerializer();
        var stream = _fileSystem.File.OpenWrite("data");
        serializer.Serialize(stream, value);
        stream.Close();

        return serializer.LastBlockId;
    }

    private async Task JsonToBon<T>(T value)
    {
        var serializer = await CreateSerializer();
        var expected = serializer.Serialize(value);
        var jsonObject = await serializer.BonToJsonAsync(new MemoryStream(expected));
        _fileSystem.File.WriteAllText("data.json", jsonObject.ToJsonString());
        await RunInspector("data.json", "schemas");
        var actual = _fileSystem.File.ReadAllBytes("data.json.bon");

        Assert.Equal(expected, actual);
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
