using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.Ebay.Response;

public sealed class EbaySeller
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("feedbackPercentage")]
    public string FeedbackPercentage { get; set; } = string.Empty;

    [JsonPropertyName("feedbackScore")]
    public int? FeedbackScore { get; set; }
}
