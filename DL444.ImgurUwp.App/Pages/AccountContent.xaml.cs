using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DL444.ImgurUwp.App.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountContent : Page, INotifyPropertyChanged
    {
        public AccountContent()
        {
            this.InitializeComponent();
        }

        private AccountViewModel _account;

        public AccountViewModel Account
        {
            get => _account;
            set
            {
                _account = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Account)));
            }
        }
        ObservableCollection<CommentViewModel> Comments { get; } = new ObservableCollection<CommentViewModel>();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is AccountViewModel vm)
            {
                await PrepareViewModels(vm);
            }
            else if(e.Parameter is ValueTuple<AccountViewModel, int> vmIndex)
            {
                RootPivot.SelectedIndex = vmIndex.Item2;
                await PrepareViewModels(vmIndex.Item1);
            }
        }

        async System.Threading.Tasks.Task PrepareViewModels(AccountViewModel vm)
        {
            Account = vm;
            var comments = await ApiClient.Client.GetAccountCommentsAsync(vm.Username);
            foreach (var c in comments)
            {
                if (c.Deleted) { continue; }
                Comments.Add(new CommentViewModel(c));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
