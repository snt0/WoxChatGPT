using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using OpenAI_API;
using Wox.Plugin;

namespace WoxChatGPT
{
    public class Plugin : IPlugin, ISettingProvider
    {
        OpenAIAPI api;
        private Settings settings;
        private bool validApiKey;
        const string DEFAULT_SYSTEM_MESSAGE = "You are a helpful assistant, tasked with answering queries in a concise and direct way.";
        public void Init(PluginInitContext context)
        {
            string pwd = context.CurrentPluginMetadata.PluginDirectory;
            string configFilePath = pwd + "/config/config.json";
            if (File.Exists(configFilePath))
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(configFilePath));
            else
            { 
                settings = new Settings(); 
                settings.filePath = configFilePath;
                settings.apiKey = "API Key";
                settings.systemMessage = DEFAULT_SYSTEM_MESSAGE;
                settings.Save();
            }
            if (Regex.IsMatch(settings.apiKey, @"^sk-[a-zA-Z0-9]{32,}$")) {
                api = new OpenAIAPI(settings.apiKey);
                validApiKey = true;
            }
            else
                validApiKey = false;

        }

        public void UpdateSettings()
        {
            // Reloads API
            if (!Regex.IsMatch(settings.apiKey, @"^sk-[a-zA-Z0-9]{32,}$"))
                return;
            api = new OpenAIAPI(settings.apiKey);
            validApiKey = true;
        }

        public List<Result> Query(Query query)
        {
            if (validApiKey)
            {
                List<Result> results = new List<Result>();
                results.Add(new Result()
                {
                    Title = "ChatGPT",
                    SubTitle = $"Ask ChatGPT.. \"{query.Search}\"",
                    IcoPath = "Images\\chatgpt.png",
                    Action = e =>
                    {
                        SubmitQuery(query.Search);
                        return false;
                    }
                });
                return results;
            }
            else
            {
                List<Result> results = new List<Result>();
                results.Add(new Result()
                {
                    Title = "ChatGPT Error",
                    SubTitle = $"You must enter a valid OpenAI API key into the settings!",
                    IcoPath = "Images\\chatgpt.png",
                    Action = e =>
                    {
                        return false;
                    }
                });
                return results;
            }
        }

        public async void SubmitQuery(string query) {
            var chat = api.Chat.CreateConversation();
            chat.AppendSystemMessage(settings.systemMessage);
            chat.AppendUserInput(query);

            string response = await chat.GetResponseFromChatbotAsync();

            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon= MessageBoxIcon.Question;
            MessageBox.Show(response, "Ask ChatGPT..", buttons, icon);
        }

        System.Windows.Controls.Control ISettingProvider.CreateSettingPanel()
        {
            return new SettingsControl(settings, this);
        }
    }
}
