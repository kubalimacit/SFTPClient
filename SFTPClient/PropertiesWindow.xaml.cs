using System;
using System.Text;
using System.Windows;
using Renci.SshNet.Sftp;

namespace SFTPClient
{
    public partial class PropertiesWindow
    {
        public string FileName { get; set; }
        public string CreationTime { get; set; }
        public string LastWriteTime { get; set; }
        public string Permissions { get; set; }
        public string FileSize { get; set; }  // Boyut için yeni bir özellik ekledik

        public PropertiesWindow(string fileName, SftpFileAttributes fileAttributes)
        {
            InitializeComponent();
            FileName = fileName;
            CreationTime = fileAttributes.LastAccessTime.ToString();
            LastWriteTime = fileAttributes.LastWriteTime.ToString();
            Permissions = GetFormattedPermissions(fileAttributes); // İzinleri özel bir formatta alıyoruz
            FileSize = FormatSize(fileAttributes.Size);  // Boyut bilgisi hesaplanıyor

            DataContext = this;
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

        private string GetFormattedPermissions(SftpFileAttributes fileAttributes)
        {
            // Bu örnek, Unix tarzı izinleri manuel olarak oluşturur
            string permissions = "";
            permissions += (fileAttributes.OwnerCanRead ? "r" : "-");
            permissions += (fileAttributes.OwnerCanWrite ? "w" : "-");
            permissions += (fileAttributes.OwnerCanExecute ? "x" : "-");
            permissions += (fileAttributes.GroupCanRead ? "r" : "-");
            permissions += (fileAttributes.GroupCanWrite ? "w" : "-");
            permissions += (fileAttributes.GroupCanExecute ? "x" : "-");
            permissions += (fileAttributes.OthersCanRead ? "r" : "-");
            permissions += (fileAttributes.OthersCanWrite ? "w" : "-");
            permissions += (fileAttributes.OthersCanExecute ? "x" : "-");

            return permissions;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var fileName = (string)this.DataContext.GetType().GetProperty("FileName").GetValue(this.DataContext, null);
            var creationTime = (string)this.DataContext.GetType().GetProperty("CreationTime").GetValue(this.DataContext, null);
            var lastWriteTime = (string)this.DataContext.GetType().GetProperty("LastWriteTime").GetValue(this.DataContext, null);
            var permissions = (string)this.DataContext.GetType().GetProperty("Permissions").GetValue(this.DataContext, null);
            var fileSize = (string)this.DataContext.GetType().GetProperty("FileSize").GetValue(this.DataContext, null);  // Boyut bilgisi

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"File Name: {fileName}");
            sb.AppendLine($"Created: {creationTime}");
            sb.AppendLine($"Last Modified: {lastWriteTime}");
            sb.AppendLine($"Permissions: {permissions}");
            sb.AppendLine($"Size: {fileSize}");  // Boyut bilgisini ekledik

            Clipboard.SetText(sb.ToString());
            MessageBox.Show("Information copied to clipboard.", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
