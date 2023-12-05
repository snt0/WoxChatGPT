using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace WoxChatGPT
{
    public class Settings
    {
        public string filePath;

        public string apiKey;
        public string systemMessage = "You are a helpful assistant, tasked with answering queries in a concise and direct way.";

        public void Save()
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
        }
    }
}
