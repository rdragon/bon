using Azure;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Bon.Serializer.Schemas;

namespace Bon.Azure;

public sealed class AzureBlob(string connectionString, string container, string blobName) : IBlob
{
    public async Task<EntityTag?> TryAppend(Stream stream, EntityTag entityTag)
    {
        var client = GetBlobClient();

        if (entityTag.Value is null)
        {
            throw new ArgumentException("Invalid entity tag.", nameof(entityTag));
        }

        var conditions = new AppendBlobRequestConditions
        {
            IfMatch = new ETag(entityTag.Value),
        };

        var options = new AppendBlobAppendBlockOptions
        {
            Conditions = conditions,
        };

        try
        {
            var response = await client.AppendBlockAsync(stream, options).ConfigureAwait(false);

            return GetEntityTag(response.GetRawResponse());
        }
        catch (RequestFailedException exception) when (exception.Status == 412)
        {
            return null;
        }
    }

    public async Task<EntityTag> LoadTo(Stream stream)
    {
        var client = GetBlobClient();

        if (!await client.ExistsAsync().ConfigureAwait(false))
        {
            await client.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        var response = await client.DownloadToAsync(stream).ConfigureAwait(false);

        return GetEntityTag(response);
    }

    public async Task<EntityTag> GetEntityTag()
    {
        var client = GetBlobClient();

        Response rawResponse;
        var response = await client.ExistsAsync().ConfigureAwait(false);

        if (response.Value)
        {
            rawResponse = response.GetRawResponse();
        }
        else
        {
            rawResponse = (await client.CreateIfNotExistsAsync().ConfigureAwait(false)).GetRawResponse();
        }

        return GetEntityTag(rawResponse);
    }

    private AppendBlobClient GetBlobClient() => new(connectionString, container, blobName);

    private static EntityTag GetEntityTag(Response response) =>
        new(response.Headers.ETag?.ToString() ?? throw new InvalidOperationException("No entity tag in response."));
}
