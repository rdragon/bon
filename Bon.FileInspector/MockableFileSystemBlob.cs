namespace Bon.FileInspector;

public sealed class MockableFileSystemBlob(IFileSystem fileSystem, string path, bool readOnly = false) : FileSystemBlob(path, readOnly)
{
    protected override Stream GetFileStream() =>
        fileSystem.FileStream.New(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
}
