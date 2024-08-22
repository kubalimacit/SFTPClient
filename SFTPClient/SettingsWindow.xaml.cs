using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SFTPClient
{
    public partial class SettingsWindow : MetroWindow
    {
        // SessionDeleted olayı ekleniyor
        public event Action SessionDeleted;
        // Event for breadcrumb visibility change
        public event Action<bool> BreadcrumbVisibilityChanged;
        // SearchBoxVisibilityChanged olayı ekleniyor
        public event Action<bool> SearchBoxVisibilityChanged;
        public event Action<bool> SessionComboBoxVisibilityChanged;

        public bool IsSessionComboBoxVisible { get; set; }

        public SettingsWindow(bool isSearchBoxVisible, bool isBreadcrumbVisible, bool isSessionComboBoxVisible)
        {
            InitializeComponent();
            LoadSessions();
            LoadThemeColor();
            LoadSavedColors();
            SearchBoxVisibilityCheckBox.IsChecked = isSearchBoxVisible;
            ShowBreadcrumbCheckBox.IsChecked = isBreadcrumbVisible;
            IsSessionComboBoxVisible = isSessionComboBoxVisible;
            ShowSessionComboBoxCheckBox.IsChecked = isSessionComboBoxVisible;

        }
        //Theme color
        private void ThemeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                var selectedColor = e.NewValue.Value;

                // Seçilen rengi kaydet
                var colorCode = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                SaveThemeColor(colorCode);

                // Temayı uygula
                ApplyThemeColor(colorCode);
            }
        }
        //LOAD Theme colors
        private void LoadSavedColors()
        {
            string savedThemeColor = ConfigurationManager.AppSettings["ThemeColor"];
            string savedTextColor = ConfigurationManager.AppSettings["TextColorBrush"];

            if (!string.IsNullOrEmpty(savedThemeColor))
            {
                ThemeColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(savedThemeColor);
            }

            if (!string.IsNullOrEmpty(savedTextColor))
            {
                TextColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(savedTextColor);
            }
        }
        private void SaveThemeColor(string colorCode)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["ThemeColor"] != null)
            {
                config.AppSettings.Settings["ThemeColor"].Value = colorCode;
            }
            else
            {
                config.AppSettings.Settings.Add("ThemeColor", colorCode);
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void ApplyThemeColor(string colorCode)
        {
            Application.Current.Resources["AccentColor"] = (Color)ColorConverter.ConvertFromString(colorCode);
            Application.Current.Resources["ThemeColorBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
        }

        private void LoadThemeColor()
        {
            string savedColor = ConfigurationManager.AppSettings["ThemeColor"];
            if (!string.IsNullOrEmpty(savedColor))
            {
                ApplyThemeColor(savedColor);
            }
        }
        //Theme color
        private void LoadSessions()
        {
            SessionListBox.Items.Clear();
            string sessionFolder = ".session";
            if (Directory.Exists(sessionFolder))
            {
                var sessionFiles = Directory.GetFiles(sessionFolder, "*.session");
                foreach (var sessionFile in sessionFiles)
                {
                    string sessionName = Path.GetFileNameWithoutExtension(sessionFile);
                    SessionListBox.Items.Add(sessionName);
                }
            }
        }

        private void DeleteSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionListBox.SelectedItem != null)
            {
                string sessionName = SessionListBox.SelectedItem.ToString();
                string sessionFilePath = Path.Combine(".session", $"{sessionName}.session");

                if (File.Exists(sessionFilePath))
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the session '{sessionName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        File.Delete(sessionFilePath);
                        LoadSessions();

                        // SessionDeleted olayını tetikle
                        SessionDeleted?.Invoke();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a session to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchBoxVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SearchBoxVisibilityChanged?.Invoke(true);
        }

        private void SearchBoxVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SearchBoxVisibilityChanged?.Invoke(false);
        }
        private void ShowBreadcrumbCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BreadcrumbVisibilityChanged?.Invoke(true);
        }
        private void ShowBreadcrumbCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            BreadcrumbVisibilityChanged?.Invoke(false);
        }
        private void ShowSessionComboBoxCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SessionComboBoxVisibilityChanged?.Invoke(true);
        }

        private void ShowSessionComboBoxCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SessionComboBoxVisibilityChanged?.Invoke(false);
        }

        private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
           
        }

        private void TextColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                Color selectedColor = e.NewValue.Value;
                Application.Current.Resources["TextColorBrush"] = new SolidColorBrush(selectedColor);

                // Rengi App.config dosyasına kaydedin
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["TextColorBrush"].Value = selectedColor.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
    }
}
