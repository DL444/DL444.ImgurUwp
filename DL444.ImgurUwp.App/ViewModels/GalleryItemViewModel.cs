using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class GalleryItemViewModel : INotifyPropertyChanged
    {
        IGalleryItem _item;
        //private AsyncProperty<Image> _displayImage;
        private Image _displayImage;

        public IGalleryItem Item
        {
            get => _item;
            set
            {
                _item = value;
                if(value.IsAlbum)
                {
                    GalleryAlbum album = value as GalleryAlbum;
                    foreach(var i in album.Images)
                    {
                        if(i.Id == album.Cover)
                        {
                            DisplayImage = i;
                            break;
                        }
                    }
                }
                else
                {
                    DisplayImage = (value as GalleryImage).ToImage();
                }
                NotifyPropertyChanged();
            }
        }
        public bool IsAlbum => _item.IsAlbum;

        public string Id => _item.Id;
        public string Title => _item.Title;
        public string Description => _item.Description;
        public DateTime DateTime => Convert.ToDateTime(_item.DateTime);
        public string Link => _item.Link;
        public string AccountUrl => _item.AccountUrl;
        public string AccountId => _item.AccountId;
        public string Topic => _item.Topic;
        public int TopicId => _item.TopicId;
        public bool Nsfw => _item.Nsfw == true;
        public int CommentCount => _item.CommentCount;
        public int Ups => _item.Ups;
        public int Downs => _item.Downs;
        public int Points => _item.Points;
        public int Score => _item.Score;
        public int Views => _item.Views;
        public bool InMostViral => _item.InMostViral;
        public bool Favorite => _item.Favorite;

        public Image DisplayImage
        {
            get => _displayImage;
            private set
            {
                _displayImage = value;
                NotifyPropertyChanged(nameof(DisplayImage));
            }
        }

        //public AsyncProperty<Image> DisplayImage
        //{
        //    get => _displayImage;
        //    private set
        //    {
        //        _displayImage = value;
        //        NotifyPropertyChanged(nameof(DisplayImage));
        //    }
        //}

        public GalleryItemViewModel() : this(Defaults.DefaultImage) { }
        public GalleryItemViewModel(IGalleryItem item) { Item = item; }

        protected void NotifyPropertyChanged(string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
