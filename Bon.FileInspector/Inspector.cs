namespace Bon.FileInspector;

internal class Inspector(IFileSystem fileSystem)
{
    public async Task Run(string[] args)
    {
        var input = InspectorInput.Create(args, new FileTypeReader(fileSystem));

        if (input.HasNonSchemaFile)
        {
            await ConvertNonSchemaFiles(input);
        }
        else
        {
            ConvertSchemaFiles(input);
        }
    }

    private async Task ConvertNonSchemaFiles(InspectorInput input)
    {
        var blob = input.SchemaFiles.Count > 0 ? (IBlob)new MockableFileSystemBlob(fileSystem, input.SchemaFiles[0], true) : new InMemoryBlob();
        var bonSerializer = await BonSerializer.CreateAsync(new BonSerializerContext(), blob);

        foreach (var path in input.BonFiles)
        {
            using var inputStream = fileSystem.File.OpenRead(path);
            using var outputStream = GetOutputStream(path, ".json");
            await BonFileInspector.BonToJson(inputStream, outputStream, bonSerializer);
        }

        foreach (var path in input.JsonFiles)
        {
            using var inputStream = fileSystem.File.OpenRead(path);
            using var outputStream = GetOutputStream(path, ".bon");
            await BonFileInspector.JsonToBon(inputStream, outputStream, bonSerializer);
        }
    }

    private void ConvertSchemaFiles(InspectorInput input)
    {
        foreach (var path in input.SchemaFiles)
        {
            using var inputStream = fileSystem.File.OpenRead(path);
            using var outputStream = GetOutputStream(path, ".txt");
            SchemaFileInspector.Run(inputStream, outputStream);
        }
    }

    private FileSystemStream GetOutputStream(string path, string extension)
    {
        var outputPath = path + extension;

        if (fileSystem.File.Exists(outputPath))
        {
            fileSystem.File.Delete(outputPath);
        }

        Console.WriteLine($"Output: '{outputPath}'");

        return fileSystem.File.OpenWrite(outputPath);
    }
}
