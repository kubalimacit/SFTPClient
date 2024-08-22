using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace SFTPClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Uygulama başlarken son seçilen tema ve metin rengini yükleyelim
            LoadThemeColor();
            LoadTextColor();
        }

        private void LoadThemeColor()
        {
            string savedColor = ConfigurationManager.AppSettings["ThemeColor"];
            if (!string.IsNullOrEmpty(savedColor))
            {
                ApplyThemeColor(savedColor);
            }
        }

        private void ApplyThemeColor(string colorCode)
        {
            Application.Current.Resources["AccentColor"] = (Color)ColorConverter.ConvertFromString(colorCode);
            Application.Current.Resources["ThemeColorBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
        }

        private void LoadTextColor()
        {
            string savedTextColor = ConfigurationManager.AppSettings["TextColorBrush"];
            if (!string.IsNullOrEmpty(savedTextColor))
            {
                ApplyTextColor(savedTextColor);
            }
        }

        private void ApplyTextColor(string colorCode)
        {
            Application.Current.Resources["TextColorBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
        }
    }
}
