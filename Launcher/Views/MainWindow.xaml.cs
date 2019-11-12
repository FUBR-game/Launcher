using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Launcher.ViewModels;
using ReactiveUI;

namespace Launcher.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.WhenActivated(disposables =>
            {
                /* Handle view activation etc. */
            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}