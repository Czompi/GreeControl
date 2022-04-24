namespace GreeNativeSdk.Protocol
{
    using System.Text.Json.Serialization;

    public class PackInfo
    {
        [JsonPropertyName("t")]
        public string Type { get; set; }

        [JsonPropertyName("cid")]
        public string ClientId { get; set; }
    }
}
