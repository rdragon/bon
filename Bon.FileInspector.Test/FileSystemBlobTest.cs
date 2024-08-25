namespace Bon.FileInspector.Test;

public class FileSystemBlobTest
{
    [Fact]
    public async Task EntityTagMismatch()
    {
        var fileSystem = new MockFileSystem();
        var blob = new MockableFileSystemBlob(fileSystem, "data");
        Assert.Null(await blob.TryAppend(new MemoryStream([1]), new EntityTag("mismatch")));
    }

    [Fact]
    public async Task ReadOnlyBlob()
    {
        var fileSystem = new MockFileSystem();
        var blob = new MockableFileSystemBlob(fileSystem, "data", true);
        var etag = await blob.GetEntityTag();
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => blob.TryAppend(new MemoryStream([1]), etag));
        Assert.Contains("read only", exception.Message);
    }
}
