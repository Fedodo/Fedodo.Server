using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fedodo.Server.Model.DTOs;

public class CreateActivityDto
{
    [JsonPropertyName("@context")]
    public IEnumerable<object>? Context { get; set; } = new List<object>
    {
        "https://www.w3.org/ns/activitystreams"
    };

    [Required] [JsonPropertyName("type")] public string Type { get; set; }

    [Required]
    [JsonPropertyName("object")]
    public object Object { get; set; }

    [JsonPropertyName("to")] public IEnumerable<string>? To { get; set; }
    [JsonPropertyName("bto")] public IEnumerable<string>? Bto { get; set; }
    [JsonPropertyName("cc")] public IEnumerable<string>? Cc { get; set; }
    [JsonPropertyName("bcc")] public IEnumerable<string>? Bcc { get; set; }
    [JsonPropertyName("audience")] public IEnumerable<string>? Audience { get; set; }
}