using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    class GalleryProfileViewModel : INotifyPropertyChanged
    {
        GalleryProfile _profile;

        public GalleryProfile Profile
        {
            get => _profile;
            set
            {
                _profile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public int GalleryCommentCount => Profile.GalleryCommentCount;
        public int GalleryFavoriteCount => Profile.GalleryFavoriteCount;
        public int GallerySubmissionCount => Profile.GallerySubmissionCount;

        public List<Trophy> Trophies => Profile.Trophies;

        public event PropertyChangedEventHandler PropertyChanged;

        public GalleryProfileViewModel() { }
        public GalleryProfileViewModel(GalleryProfile profile) : this() => Profile = profile;
    }
}
