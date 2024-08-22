using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Renci.SshNet;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SFTPClient
{
    public partial class MainWindow : MetroWindow
    {
        private SftpClient sftpClient;
        private string currentDirectory = ".";
        private const string SessionFolder = ".session";
        private MetroDialogSettings mySettings_connect_btn;
        private bool isSearchBoxVisible;
        private bool isBreadcrumbVisible;
        private bool isSessionComboBoxVisible;

        public ObservableCollection<FileSystemItem> Items { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Items = new ObservableCollection<FileSystemItem>();
            FilesListView.ItemsSource = Items;
            LoadSessions();

            // Ayarı okuyoruz
            isSearchBoxVisible = bool.Parse(ConfigurationManager.AppSettings["IsSearchBoxVisible"]);
            SearchTextBox.Visibility = isSearchBoxVisible ? Visibility.Visible : Visibility.Collapsed;
            isBreadcrumbVisible = bool.Parse(ConfigurationManager.AppSettings["IsBreadcrumbVisible"]);
            Breadcrumb.Visibility = isBreadcrumbVisible ? Visibility.Visible : Visibility.Collapsed;
            isSessionComboBoxVisible = bool.Parse(ConfigurationManager.AppSettings["IsSessionComboBoxVisible"]);
            SessionComboBox.Visibility = isSessionComboBoxVisible ? Visibility.Visible : Visibility.Collapsed;
            SaveSessionButton.Visibility = isSessionComboBoxVisible ? Visibility.Visible : Visibility.Collapsed;

            // Özel MahApps mesaj kutusu ayarları

            mySettings_connect_btn = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                ColorScheme = MetroDialogColorScheme.Theme,
                DialogButtonFontSize = 18D,
            };

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed; // Bu satır ekleniyor
        }
        public void ChangeLanguage(string languageCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (languageCode)
            {
                case "tr-TR":
                    dict.Source = new Uri("Resources/Strings.tr.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Resources/Strings.en.xaml", UriKind.Relative);
                    break;
            }

            // Mevcut resource dictionary'yi değiştir
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ProtocolComboBox_SelectionChanged(null, null);
            UpdatePasteMenuItemStatus(); // Uygulama yüklendiğinde yapıştırma durumu güncellenir
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Ayarı kaydediyoruz
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["IsSearchBoxVisible"].Value = isSearchBoxVisible.ToString();
            config.AppSettings.Settings["IsBreadcrumbVisible"].Value = isBreadcrumbVisible.ToString();
            config.AppSettings.Settings["IsSessionComboBoxVisible"].Value = isSessionComboBoxVisible.ToString();

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private async void ConnectDisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sftpClient == null || !sftpClient.IsConnected)
            {
                // Connect işlemi
                string ipAddress = IpAddressTextBox.Text;
                string username = UsernameTextBox.Text;
                string password = PasswordBox.Password;
                int port = int.Parse(PortTextBox.Text);

                try
                {
                    sftpClient = new SftpClient(ipAddress, port, username, password);
                    sftpClient.Connect();

                    // Bağlantı başarılı olduğunda ikon ve buton rengini değiştir
                    ConnectDisconnectIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.LanDisconnect;
                    ConnectDisconnectIcon.Foreground = new SolidColorBrush(Colors.Salmon);
                    RefreshButton.IsEnabled = true; // RefreshButton aktif hale getiriliyor

                    await this.ShowMessageAsync("Connection Status", "Connected successfully!", MessageDialogStyle.Affirmative, mySettings_connect_btn);

                    ListDirectory(currentDirectory);

                    // Bağlantı durumu gösterimi
                    ConnectionStatusTextBlock.Text = $"Connected to {ipAddress}";
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Connection Status", "Connection failed: " + ex.Message, MessageDialogStyle.Affirmative, mySettings_connect_btn);

                    ConnectionStatusTextBlock.Text = $"Failed to connect to {ipAddress}";
                }
            }
            else
            {
                // Disconnect işlemi
                sftpClient.Disconnect();
                sftpClient.Dispose();
                sftpClient = null;

                // ListView içeriğini temizle
                Items.Clear();

                // Bağlantı kesildiğinde ikon ve buton rengini değiştir
                ConnectDisconnectIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Connection;
                ConnectDisconnectIcon.Foreground = (SolidColorBrush)Application.Current.Resources["ThemeColorBrush"];
                RefreshButton.IsEnabled = false; // RefreshButton inaktif hale getiriliyor

                await this.ShowMessageAsync("Connection Status", "Connection disconnected by the user.", MessageDialogStyle.Affirmative, mySettings_connect_btn);

                ConnectionStatusTextBlock.Text = "Not Connected";

                // Seçim durumunu da temizle
                SelectionStatusTextBlock.Text = "";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            string parentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf('/'));
            if (string.IsNullOrEmpty(parentDirectory))
            {
                parentDirectory = ".";
            }
            ListDirectory(parentDirectory);
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem != null)
            {
                var selectedItem = (FileSystemItem)FilesListView.SelectedItem;
                string sourcePath = $"{currentDirectory}/{selectedItem.Name}";

                // Panoya dosya veya klasör yolunu kopyalayalım
                StringCollection paths = new StringCollection();
                paths.Add(sourcePath);
                Clipboard.SetFileDropList(paths);

                UpdatePasteMenuItemStatus(); // Kopyalama yapıldığında yapıştırma durumu güncellenir
            }
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string sourcePath = Clipboard.GetText();
                string destinationPath = Path.Combine(currentDirectory, Path.GetFileName(sourcePath));

                try
                {
                    if (Directory.Exists(sourcePath))
                    {
                        DirectoryCopy(sourcePath, destinationPath, true);
                    }
                    else if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, destinationPath);
                    }

                    ListDirectory(currentDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Paste failed: {ex.Message}");
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem != null)
            {
                var selectedItem = (FileSystemItem)FilesListView.SelectedItem;
                string pathToDelete = $"{currentDirectory}/{selectedItem.Name}";

                if (MessageBox.Show($"Are you sure you want to delete {selectedItem.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (selectedItem.Type == "Folder")
                        {
                            sftpClient.DeleteDirectory(pathToDelete);
                        }
                        else
                        {
                            sftpClient.DeleteFile(pathToDelete);
                        }
                        ListDirectory(currentDirectory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Delete failed: {ex.Message}");
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Eğer sunucuya bağlantı sağlanmışsa, mevcut dizini yeniden listele
            if (sftpClient != null && sftpClient.IsConnected)
            {
                ListDirectory(currentDirectory);
            }
        }

        private void RefreshMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ListDirectory(currentDirectory); // Mevcut dizini yeniler
        }

        private void UpdatePasteMenuItemStatus()
        {
            // Eğer panoda dosya veya klasör varsa, Paste butonu aktif olsun
            PasteMenuItem.IsEnabled = Clipboard.ContainsFileDropList();
        }

        private void ListDirectory(string path)
        {
            try
            {
                var files = sftpClient.ListDirectory(path);
                var tempItems = new ObservableCollection<FileSystemItem>();

                foreach (var file in files)
                {
                    if (file.Name != "." && file.Name != "..")
                    {
                        string size;
                        if (file.IsDirectory)
                        {
                            size = "Folder";
                        }
                        else
                        {
                            size = FormatSize(file.Length);
                        }

                        tempItems.Add(new FileSystemItem
                        {
                            Name = file.Name,
                            Type = file.IsDirectory ? "Folder" : "File",
                            Size = size
                        });
                    }
                }

                // Items koleksiyonunu güncellemek yerine doğrudan tempItems'ı atıyoruz
                Items = tempItems;
                FilesListView.ItemsSource = Items;

                currentDirectory = path;
                Breadcrumb.Text = currentDirectory.Replace('/', '\\');
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to list directory: {ex.Message}");
            }
        }

        private string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void FilesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FilesListView.SelectedItem != null)
            {
                var selectedItem = (FileSystemItem)FilesListView.SelectedItem;
                string selectedPath = $"{currentDirectory}/{selectedItem.Name}";

                try
                {
                    var selectedItemInfo = sftpClient.GetAttributes(selectedPath);

                    if (selectedItemInfo.IsDirectory)
                    {
                        ListDirectory(selectedPath);
                    }
                    else
                    {
                        MessageBox.Show("This is a file, please select a folder.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to access folder: {ex.Message}");
                }
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (sftpClient != null && sftpClient.IsConnected && FilesListView.SelectedItem != null)
            {
                var selectedItem = (FileSystemItem)FilesListView.SelectedItem;
                string remoteFilePath = $"{currentDirectory}/{selectedItem.Name}";
                string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), selectedItem.Name);

                try
                {
                    using (Stream fileStream = File.OpenWrite(localPath))
                    {
                        sftpClient.DownloadFile(remoteFilePath, fileStream);
                    }
                    MessageBox.Show("Download completed!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Download failed: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a file to download.");
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (sftpClient != null && sftpClient.IsConnected)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string localFilePath = dlg.FileName;
                    string fileName = Path.GetFileName(localFilePath);
                    string remoteFilePath = $"{currentDirectory}/{fileName}";

                    try
                    {
                        using (Stream fileStream = File.OpenRead(localFilePath))
                        {
                            sftpClient.UploadFile(fileStream, remoteFilePath);
                        }
                        MessageBox.Show("Upload completed!");
                        ListDirectory(currentDirectory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Upload failed: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please connect to the SFTP server first.");
            }
        }

        private void FilesListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void FilesListView_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void FilesListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string remoteFilePath = $"{currentDirectory}/{fileName}";

                    try
                    {
                        using (Stream fileStream = File.OpenRead(file))
                        {
                            sftpClient.UploadFile(fileStream, remoteFilePath);
                        }
                        MessageBox.Show($"File uploaded: {fileName}");
                        ListDirectory(currentDirectory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"File upload failed: {ex.Message}");
                    }
                }
            }
        }

        private void FilesListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var selectedItem = FilesListView.SelectedItem as FileSystemItem;
                if (selectedItem != null && selectedItem.Type == "File")
                {
                    string remoteFilePath = $"{currentDirectory}/{selectedItem.Name}";
                    string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), selectedItem.Name);

                    try
                    {
                        DataObject data = new DataObject(DataFormats.FileDrop, new string[] { localFilePath });
                        DragDrop.DoDragDrop(FilesListView, data, DragDropEffects.Copy);

                        using (Stream fileStream = File.OpenWrite(localFilePath))
                        {
                            sftpClient.DownloadFile(remoteFilePath, fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Drag-and-drop operation failed: {ex.Message}");
                    }
                }
            }
        }

        private void ProtocolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PortTextBox != null && ProtocolComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "FTP":
                        PortTextBox.Text = "21";
                        break;
                    case "SFTP":
                        PortTextBox.Text = "22";
                        break;
                }
            }
        }

        private void SaveSessionButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSessionWindow saveSessionWindow = new SaveSessionWindow();
            saveSessionWindow.SessionSaved += SaveSession;
            saveSessionWindow.ShowDialog();
        }

        private void SaveSession(string sessionName)
        {
            Directory.CreateDirectory(SessionFolder);

            string sessionFilePath = Path.Combine(SessionFolder, $"{sessionName}.session");

            string encryptedPassword = PasswordEncryption.EncryptPassword(PasswordBox.Password);

            string[] sessionData = new string[]
            {
                $"Protocol={ProtocolComboBox.Text}",
                $"IpAddress={IpAddressTextBox.Text}",
                $"Username={UsernameTextBox.Text}",
                $"Password={encryptedPassword}",
                $"Port={PortTextBox.Text}"
            };

            File.WriteAllLines(sessionFilePath, sessionData);
            LoadSessions();
        }

        private void LoadSessions()
        {
            SessionComboBox.Items.Clear();

            if (Directory.Exists(SessionFolder))
            {
                var sessionFiles = Directory.GetFiles(SessionFolder, "*.session");

                foreach (var sessionFile in sessionFiles)
                {
                    string sessionName = Path.GetFileNameWithoutExtension(sessionFile);
                    SessionComboBox.Items.Add(sessionName);
                }
            }
        }

        private void SessionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SessionComboBox.SelectedItem != null)
            {
                string sessionName = SessionComboBox.SelectedItem.ToString();
                string sessionFilePath = Path.Combine(SessionFolder, $"{sessionName}.session");

                if (File.Exists(sessionFilePath))
                {
                    var sessionData = File.ReadAllLines(sessionFilePath);

                    foreach (var line in sessionData)
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            switch (parts[0])
                            {
                                case "Protocol":
                                    ProtocolComboBox.Text = parts[1];
                                    break;
                                case "IpAddress":
                                    IpAddressTextBox.Text = parts[1];
                                    break;
                                case "Username":
                                    UsernameTextBox.Text = parts[1];
                                    break;
                                case "Password":
                                    PasswordBox.Password = PasswordEncryption.DecryptPassword(parts[1]);
                                    break;
                                case "Port":
                                    PortTextBox.Text = parts[1];
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(isSearchBoxVisible, isBreadcrumbVisible, isSessionComboBoxVisible);
            settingsWindow.SearchBoxVisibilityChanged += (isVisible) =>
            {
                isSearchBoxVisible = isVisible;
                SearchTextBox.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            };
            settingsWindow.BreadcrumbVisibilityChanged += (isVisible) =>
            {
                isBreadcrumbVisible = isVisible;
                Breadcrumb.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            };
            settingsWindow.SessionComboBoxVisibilityChanged += (isVisible) =>
            {
                isSessionComboBoxVisible = isVisible;
                SessionComboBox.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                SaveSessionButton.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;

            };
            settingsWindow.ShowDialog();
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is an SFTP client application.\n\nFor more information, visit the help section or contact support.", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem != null)
            {
                var selectedItem = (FileSystemItem)FilesListView.SelectedItem;
                string sourcePath = $"{currentDirectory}/{selectedItem.Name}";
                string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), selectedItem.Name);

                try
                {
                    if (selectedItem.Type == "Folder")
                    {
                        DirectoryCopy(sourcePath, destinationPath, true);
                    }
                    else
                    {
                        sftpClient.DownloadFile(sourcePath, File.OpenWrite(destinationPath));
                    }
                    MessageBox.Show($"Copied {selectedItem.Name} to {destinationPath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Copy failed: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a file or folder to copy.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem != null)
            {
                var selectedItem = (FileSystemItem)FilesListView.SelectedItem;
                string pathToDelete = $"{currentDirectory}/{selectedItem.Name}";

                if (MessageBox.Show($"Are you sure you want to delete {selectedItem.Name}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (selectedItem.Type == "Folder")
                        {
                            sftpClient.DeleteDirectory(pathToDelete);
                        }
                        else
                        {
                            sftpClient.DeleteFile(pathToDelete);
                        }
                        ListDirectory(currentDirectory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Delete failed: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a file or folder to delete.");
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // İkonları güncelle
            foreach (FileSystemItem item in Items)
            {
                ListViewItem listViewItem = (ListViewItem)FilesListView.ItemContainerGenerator.ContainerFromItem(item);
                if (listViewItem != null)
                {
                    PackIconMaterial icon = FindVisualChild<PackIconMaterial>(listViewItem);
                    if (icon != null)
                    {
                        if (listViewItem.IsSelected)
                        {
                            icon.Foreground = Brushes.White;
                        }
                        else
                        {
                            icon.Foreground = (SolidColorBrush)Application.Current.Resources["ThemeColorBrush"];
                        }
                    }
                }
            }

            // Seçili öğelerin bilgisini göster
            if (FilesListView.SelectedItems != null && FilesListView.SelectedItems.Count > 0)
            {
                long totalSize = 0;
                foreach (FileSystemItem selectedItem in FilesListView.SelectedItems)
                {
                    if (selectedItem.Type != "Folder")
                    {
                        totalSize += ConvertSizeToBytes(selectedItem.Size);
                    }
                }
                SelectionStatusTextBlock.Text = " | "+$"{FilesListView.SelectedItems.Count} item(s) selected, Total size: {FormatSize(totalSize)}";
            }
            else
            {
                SelectionStatusTextBlock.Text = "";
            }
        }

        private long ConvertSizeToBytes(string size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = double.Parse(size.Split(' ')[0]);
            string order = size.Split(' ')[1];
            int index = Array.IndexOf(sizes, order);
            return (long)(len * Math.Pow(1024, index));
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T t)
                {
                    return t;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Arama kutusu boşsa, tüm dosya ve klasörleri yeniden listele
                ListDirectory(currentDirectory); // Mevcut dizini yeniden yükle
            }
            else
            {
                // Filtreleme işlemi
                var filteredItems = Items.Where(item => item.Name.ToLower().Contains(searchText)).ToList();

                // ListView'i güncelle
                FilesListView.ItemsSource = null;  // Mevcut kaynakları sıfırla
                FilesListView.ItemsSource = filteredItems;  // Yeni filtrelenmiş listeyi ekle
            }
        }

        private void PropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem != null)
            {
                var selectedItem = (FileSystemItem)FilesListView.SelectedItem;
                string selectedPath = $"{currentDirectory}/{selectedItem.Name}";

                try
                {
                    var selectedItemInfo = sftpClient.GetAttributes(selectedPath);

                    // Dosya/Klasör özelliklerini gösterecek bir pencere oluşturuyoruz
                    PropertiesWindow propertiesWindow = new PropertiesWindow(selectedItem.Name, selectedItemInfo);
                    propertiesWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to retrieve properties: {ex.Message}");
                }
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }

    }

    public class FileSystemItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
    }
}
