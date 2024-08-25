namespace Bon.FileInspector;

internal class FileTypeReader(IFileSystem fileSystem)
{
    private const ushort BON_MARKER = 0x412b;
    private const ushort BLOCK_MARKER = 0xad5d;

    public FileType GetFileType(string path)
    {
        if (!fileSystem.File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: '{path}'.", path);
        }

        if (path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            return FileType.Json;
        }

        using var inputStream = fileSystem.File.OpenRead(path);
        var reader = new BinaryReader(inputStream);
        var marker = reader.ReadUInt16();

        return marker switch
        {
            BLOCK_MARKER => FileType.Schema,
            BON_MARKER => FileType.Bon,
            _ => throw new IOException($"Invalid file '{path}'. Expecting a schema, bon, or json file."),
        };
    }
}
