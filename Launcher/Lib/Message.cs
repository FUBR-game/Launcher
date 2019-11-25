using Newtonsoft.Json;

namespace Launcher.Lib
{
    public class Message
    {
        [JsonProperty("MessageData")] public string MessageData;

        [JsonProperty("MessageType")] public string MessageType;
    }
}