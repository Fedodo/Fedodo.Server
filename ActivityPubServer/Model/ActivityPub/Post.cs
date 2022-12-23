using Newtonsoft.Json;

namespace ActivityPubServer.Model.ActivityPub;

public class Post
{
    [JsonProperty("to")] public string To { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("summary")] public string Summary { get; set; }
    [JsonProperty("sensitive")] public bool? Sensitive { get; set; }
    [JsonProperty("inReplyTo")] public Uri? InReplyTo { get; set; }
    [JsonProperty("content")] public string? Content { get; set; }
    [JsonProperty("id")] public Uri? Id { get; set; }
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("published")] public DateTime Published { get; set; }
    [JsonProperty("attributedTo")] public Uri? AttributedTo { get; set; }
}