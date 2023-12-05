using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WoxChatGPT
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {

        private readonly Settings settings;
        private readonly Plugin plugin;
        public SettingsControl(Settings settings, Plugin plugin)
        {
            InitializeComponent();
            this.settings = settings;
            this.plugin = plugin; 
        }
        public void OnLoad(object sender, RoutedEventArgs routedEvent) {
            // Assign settings text to textboxes
            APIKey.Text = settings.apiKey;
            SystemMessage.Text = settings.systemMessage;

            // Add text changed handlers
            APIKey.TextChanged += (snd, evnt) =>
            {
                settings.apiKey = APIKey.Text;
                settings.Save();
                plugin.UpdateSettings();
            };

            SystemMessage.TextChanged += (snd, evnt) =>
            {
                settings.systemMessage = SystemMessage.Text;
                settings.Save();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Settings should've already been saved, but just to make sure..
            settings.apiKey = APIKey.Text;
            settings.systemMessage = APIKey.Text;
            settings.Save();
            plugin.UpdateSettings();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
