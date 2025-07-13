namespace Bon.FileInspector;

internal class FileTypeReader(IFileSystem fileSystem)
{
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
            _ => FileType.Bon,
        };
    }
}
