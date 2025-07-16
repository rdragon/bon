using System.Security.Cryptography;

namespace Bon.Serializer;

/// <summary>
/// Uses the file system to store the blob.
/// </summary>
/// <param name="path">Path to a file that will store the contents of the blob.</param>
/// <param name="readOnly">If true then an exception will be thrown if data needs to be saved to the blob.</param>
public class FileSystemBlob(string path = "layouts", bool readOnly = false) : IBlob
{
    protected readonly string _path = path;

    public async Task<EntityTag?> TryAppendAsync(Stream stream, EntityTag entityTag)
    {
        if (readOnly)
        {
            throw new InvalidOperationException("This blob is read only.");
        }

        using var fileStream = GetFileStream();

        if (await GetEntityTag(fileStream).ConfigureAwait(false) != entityTag)
        {
            return null;
        }

        fileStream.Position = fileStream.Length;
        await stream.CopyToAsync(fileStream).ConfigureAwait(false);
        fileStream.Position = 0;

        return await GetEntityTag(fileStream).ConfigureAwait(false);
    }

    public async Task<EntityTag> LoadToAsync(Stream stream)
    {
        using var fileStream = GetFileStream();
        await fileStream.CopyToAsync(stream).ConfigureAwait(false);
        fileStream.Position = 0;

        return await GetEntityTag(fileStream).ConfigureAwait(false);
    }

    public async Task<EntityTag> GetEntityTagAsync()
    {
        using var fileStream = GetFileStream();

        return await GetEntityTag(fileStream).ConfigureAwait(false);
    }

    private static async Task<EntityTag> GetEntityTag(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(stream).ConfigureAwait(false);

        return new(Convert.ToBase64String(hash));
    }

    protected virtual Stream GetFileStream() => new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
}
