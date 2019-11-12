using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Launcher.Models;

namespace Launcher.Converters
{
    public static class UserStatusToColourConverter
    {
        public static ISolidColorBrush ConvertColour(UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Online:
                    return Brushes.LimeGreen;
                case UserStatus.Away:
                    return Brushes.Yellow;
                case UserStatus.Busy:
                    return Brushes.Red;
                case UserStatus.Offline:
                    return Brushes.Gray;
                case UserStatus.Invisible:
                    goto case UserStatus.Offline;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}