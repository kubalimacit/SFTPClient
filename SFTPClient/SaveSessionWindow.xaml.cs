using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace SFTPClient
{
    public partial class SaveSessionWindow : MetroWindow
    {
        public event Action<string> SessionSaved;

        public SaveSessionWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string sessionName = SessionNameTextBox.Text;
            if (!string.IsNullOrEmpty(sessionName))
            {
                SessionSaved?.Invoke(sessionName);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a session name.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
