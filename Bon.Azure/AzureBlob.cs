using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Bon.Serializer.Schemas;

namespace Bon.Azure;

public sealed class AzureBlob(string connectionString, string container, string folder) : IBlob
{
    public async Task<EntityTag?> TryAppendAsync(Stream stream, EntityTag entityTag)
    {
        var client = GetAppendBlobClient();

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
            await CreateHistoryBlobAsync();

            return GetEntityTag(response.GetRawResponse());
        }
        catch (RequestFailedException exception) when (exception.Status == 412)
        {
            return null;
        }
    }

    public async Task<EntityTag> LoadToAsync(Stream stream)
    {
        var client = GetAppendBlobClient();

        if (!await client.ExistsAsync().ConfigureAwait(false))
        {
            await client.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        var response = await client.DownloadToAsync(stream).ConfigureAwait(false);

        return GetEntityTag(response);
    }

    public async Task<EntityTag> GetEntityTagAsync()
    {
        var client = GetAppendBlobClient();

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

    private async Task CreateHistoryBlobAsync()
    {
        var sourceBlob = GetAppendBlobClient();
        var targetBlob = GetHistoryBlobClient();

        await targetBlob.StartCopyFromUriAsync(sourceBlob.Uri).ConfigureAwait(false);
    }

    private AppendBlobClient GetAppendBlobClient() => new(connectionString, container, folder + "/main");

    private BlobClient GetHistoryBlobClient()
    {
        var now = DateTime.UtcNow;
        return new(connectionString, container, $"{folder}/history/{now.Year}/{now:yyyyMMdd_HHmmss_ffffff}");
    }

    private static EntityTag GetEntityTag(Response response) =>
        new(response.Headers.ETag?.ToString() ?? throw new InvalidOperationException("No entity tag in response."));
}
