using DL444.ImgurUwp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class AccountAlbumViewModel : INotifyPropertyChanged
    {
        private Album _album;
        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Id => Album.Id;
        public string Title => Album.Title;
        public string Description => Album.Description;
        public DateTime DateTime => Convert.ToDateTime(Album.DateTime);
        public string Cover => Album.Cover;
        public int CoverWidth => Album.CoverWidth ?? 0;
        public int CoverHeight => Album.CoverHeight ?? 0;
        public string AccountUrl => Album.AccountUrl;
        public int AccountId => Album.AccountId;
        public string Privacy => Album.Privacy;
        public int Views => Album.Views;
        public string Link => Album.Link;
        public bool Favorite => Album.Favorite;
        public bool Nsfw => Album.Nsfw == true;
        public int Order => Album.Order;
        public string DeleteHash => Album.DeleteHash;
        public int ImageCount => Album.ImageCount;
        public List<Image> Images => Album.Images;
        public bool InGallery => Album.InGallery;
        public bool IsAlbum => true;

        public bool HasImage => ImageCount > 0;
        public BitmapImage Thumbnail
        {
            get
            {
                if (string.IsNullOrEmpty(Cover)) { return null; }
                return new BitmapImage(new Uri($"https://i.imgur.com/{Cover}h.jpg"));
            }
        }
        public bool IsAnimated
        {
            get
            {
                if(Images == null || Images.Count == 0) { return false; }
                return Images[0].Animated;
            }
        }
        public bool HasTitle => string.IsNullOrEmpty(Title);

        public AccountAlbumViewModel() { }
        public AccountAlbumViewModel(Album album) : this()
        {
            Album = album;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
