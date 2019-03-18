using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.Settings
{
    class ProfileImageViewModel : INotifyPropertyChanged
    {
        private ProfileImage _profileImage;
        public ProfileImage ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Name => ProfileImage.Name;
        public string Location => ProfileImage.Location;
        public bool IsCover => ProfileImage.Type == ProfileImageType.Cover;

        public ProfileImageViewModel() { }
        public ProfileImageViewModel(ProfileImage image)
        {
            ProfileImage = image;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class AccountSettingsViewModel : INotifyPropertyChanged
    {
        private AccountSettings _settings;
        public AccountSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public bool GalleryTermsNotAccepted
        {
            get => !Settings.GalleryTermsAccepted;
            set
            {
                Settings.GalleryTermsAccepted = !value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GalleryTermsNotAccepted)));
            }
        }
        public bool EmailNotVerified => Settings.ActiveEmails == null || Settings.ActiveEmails.Count == 0;
        public string Username => Settings.Username;
        public List<string> ActiveEmails => Settings.ActiveEmails;
        public AlbumPrivacyOptions AlbumPrivacy
        {
            get => Settings.AlbumPrivacy;
            set
            {
                Settings.AlbumPrivacy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumPrivacy)));
            }
        }
        public bool CommentReplyNotify => Settings.CommentReplyNotify;
        public string Email => Settings.Email;
        public bool FirstPartyLogin => Settings.FirstPartyLogin;
        public bool MessagingEnabled => Settings.MessagingEnabled;
        public bool NewsletterSubscribed => Settings.NewsletterSubscribed;
        public bool PublicImagesByDefault
        {
            get => Settings.PublicImagesByDefault;
            set
            {
                Settings.PublicImagesByDefault = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublicImagesByDefault)));
            }
        }
        public bool ShowMature
        {
            get => Settings.ShowMature;
            set
            {
                Settings.ShowMature = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowMature)));
            }
        }

        public AccountSettingsViewModel(AccountSettings settings) => Settings = settings;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
