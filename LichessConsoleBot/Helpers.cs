namespace LichessConsoleBot;

public static class Helpers
{

    // Adapted from https://www.tpeczek.com/2020/10/consuming-json-objects-stream-ndjson.html
    public static async IAsyncEnumerable<string> ReadStringFromNdjsonAsync(this HttpContent content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        string? mediaType = content.Headers.ContentType?.MediaType;

        if (mediaType is null || !mediaType.Equals("application/x-ndjson", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException();
        }

        Stream contentStream = await content.ReadAsStreamAsync().ConfigureAwait(false);

        using (contentStream)
        {
            using (StreamReader contentStreamReader = new StreamReader(contentStream))
            {
                while (!contentStreamReader.EndOfStream)
                {
                    yield return await contentStreamReader.ReadLineAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
