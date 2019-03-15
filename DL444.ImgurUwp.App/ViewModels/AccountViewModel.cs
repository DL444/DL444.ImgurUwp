using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class AccountViewModel : INotifyPropertyChanged
    {
        Account _account;

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                NotifyPropertyChanged();
            }
        }
        public int Id => _account.Id;
        public string Username => _account.Url;
        public string Biography
        {
            get => _account.Bio;
            set
            {
                _account.Bio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Biography)));
            }
        }
        public string Reputation => $"{_account.Reputation} pts";
        public DateTime CreatedTime => Convert.ToDateTime(_account.CreatedTime);
        public string ReputationName => _account.ReputationName.Capitalize();
        public bool IsBlocked => _account.IsBlocked;
        public string AvatarUrl => _account.Avatar;
        public string AvatarName => _account.AvatarName;
        public string CoverUrl => _account.Cover;
        public string CoverName => _account.CoverName;
        public bool IsFollowing => _account.UserFollow;

        public Windows.UI.Xaml.Media.Imaging.BitmapImage AvatarImage => 
            new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(_account.Avatar)) { CreateOptions = Windows.UI.Xaml.Media.Imaging.BitmapCreateOptions.IgnoreImageCache };

        public AccountViewModel() { }
        public AccountViewModel(Account account) : this()
        {
            Account = account;
        }

        protected void NotifyPropertyChanged(string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
