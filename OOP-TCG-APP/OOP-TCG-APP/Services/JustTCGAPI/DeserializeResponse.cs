using System.Text.Json;
namespace JustTCG
{
    /// <summary>
    /// Deserializes raw JustTCG JSON payloads into strongly typed API response objects.
    /// </summary>
    public static class DeserializeResponse
    {
        /// <summary>
        /// Deserializes the supplied JSON string into an <see cref="ApiResponse{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type stored inside the response data payload.</typeparam>
        /// <param name="json">The raw JSON returned by the API.</param>
        /// <returns>The deserialized response, or <see langword="null"/> when the payload is empty or invalid.</returns>
        public static Response<T>? Run<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<Response<T>>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}