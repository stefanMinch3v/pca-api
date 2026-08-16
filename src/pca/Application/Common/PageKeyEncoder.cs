using System.Buffers.Text;
using System.Text;
using System.Text.Json;

namespace pca.Application.Common;

/// <summary>
/// Encodes/decodes opaque, base64url pagination keys. Kept as an opaque
/// blob (JSON payload, not a raw id) so a page key can grow to carry extra
/// state (e.g. secondary sort fields) later without changing the public
/// query-string contract. Mirrors the API's existing TypeScript
/// encode/decodePageKey helpers.
/// </summary>
public static class PageKeyEncoder
{
    public static string Encode<T>(T payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes(json);

        return Base64Url.EncodeToString(bytes);
    }

    /// <summary>
    /// Returns <see langword="default"/> for a missing or malformed page
    /// key (invalid base64url, invalid JSON, shape mismatch, ...) rather
    /// than throwing, since a page key is client-supplied, opaque input.
    /// </summary>
    public static T? Decode<T>(string? encodedPageKey)
    {
        if (string.IsNullOrEmpty(encodedPageKey))
        {
            return default;
        }

        try
        {
            var bytes = Base64Url.DecodeFromChars(encodedPageKey);
            var json = Encoding.UTF8.GetString(bytes);

            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }
}
