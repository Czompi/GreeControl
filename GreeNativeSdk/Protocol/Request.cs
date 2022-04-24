using System.Text.Json.Serialization;

namespace GreeNativeSdk.Protocol
{
    public class Request : PackInfo
    {
        [JsonPropertyName("i")]
        public int I { get; set; }

        [JsonPropertyName("tcid")]
        public string TargetClientId { get; set; }

        [JsonPropertyName("uid")]
        public int UID { get; set; }

        [JsonPropertyName("pack")]
        public string Pack { get; set; }

        public static Request Create(string targetClientId, string pack, int i = 0)
        {
            return new Request()
            {
                ClientId = "app",
                Type = "pack",
                I = i,
                TargetClientId = targetClientId,
                Pack = pack,
                UID = 0
            };
        }
    }
}
