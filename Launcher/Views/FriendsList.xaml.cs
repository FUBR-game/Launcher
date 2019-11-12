using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Launcher.ViewModels;

namespace Launcher.Views
{
    /// <summary>
    /// Interaction logic for FriendsList.xaml
    /// </summary>
    public partial class FriendsList : ReactiveUserControl<FriendsListViewModel>
    {
        public FriendsList()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}