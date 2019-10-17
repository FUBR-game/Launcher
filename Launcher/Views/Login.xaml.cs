using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Launcher.Views
{
    public class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}