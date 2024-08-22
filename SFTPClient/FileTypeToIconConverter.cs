using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace SFTPClient
{
    public class FileTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fileType = value as string;
            if (fileType == "Folder")
            {
                return PackIconMaterialKind.Folder;
            }
            else if (fileType == "File")
            {
                return PackIconMaterialKind.File;
            }

            return PackIconMaterialKind.FileQuestion; // Belirsiz tipler için
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
