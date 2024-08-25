namespace Bon.FileInspector;

internal sealed record class InspectorInput(
    IReadOnlyList<string> BonFiles,
    IReadOnlyList<string> SchemaFiles,
    IReadOnlyList<string> JsonFiles)
{
    public bool HasNonSchemaFile => BonFiles.Count + JsonFiles.Count > 0;

    public static InspectorInput Create(string[] args, FileTypeReader fileTypeReader)
    {
        if (args.Length == 0)
        {
            throw new InvalidOperationException("Expecting at least one file.");
        }

        var lookup = args.ToLookup(fileTypeReader.GetFileType);

        var input = new InspectorInput(
            lookup[FileType.Bon].ToArray(),
            lookup[FileType.Schema].ToArray(),
            lookup[FileType.Json].ToArray());

        if (input.HasNonSchemaFile && input.SchemaFiles.Count > 1)
        {
            throw new InvalidOperationException("Cannot inspect file(s) because more than one schema file is found.");
        }

        return input;
    }
}
