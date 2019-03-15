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
}
