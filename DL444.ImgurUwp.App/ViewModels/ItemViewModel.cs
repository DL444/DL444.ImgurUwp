using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        IItem _item;
        Image _displayImage;
        string _thumbnail;

        public IItem Item
        {
            get => _item;
            set
            {
                _item = value;
                if(value.IsAlbum == true)
                {
                    Album album = value as Album;
                    _displayImage = album.Images.FirstOrDefault(x => x.Id == album.Cover);
                    if(_displayImage == null)
                    {
                        _displayImage = album.Images.FirstOrDefault();
                    }
                }
                else
                {
                    _displayImage = value as Image;
                }

                if(_displayImage != null)
                {
                    string link = DisplayImage.Link;
                    if (string.IsNullOrWhiteSpace(link)) { _thumbnail = null; }
                    else
                    {
                        if (DisplayImage.Animated)
                        {
                            _thumbnail = $"{DisplayImage.Link.Remove(DisplayImage.Link.LastIndexOf('/'))}/{DisplayImage.Id}_d.jpg?maxwidth=520&shape=thumb&fidelity=mid";
                        }
                        else
                        {
                            _thumbnail = $"{DisplayImage.Link.Replace(DisplayImage.Id, $"{DisplayImage.Id}_d")}?maxwidth=520&shape=thumb&fidelity=mid";
                        }
                    }
                }

                PropertyChanged(this, new PropertyChangedEventArgs(null));
            }
        }
        public bool IsAlbum => Item.IsAlbum == true;
        public bool InGallery => Item.InGallery;

        public string Id => Item.Id;
        public string Title => Item.Title;
        public string Description => Item.Description;
        public DateTime DateTime => Convert.ToDateTime(Item.DateTime);
        public string Link => Item.Link;

        public int Views => Item.Views;
        public string DeleteHash => Item.DeleteHash;
        public string Section => Item.Section;
        public bool Nsfw => Item.Nsfw == true;
        public bool Favorite
        {
            get => Item.Favorite;
            set
            {
                Item.Favorite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Favorite)));
            }
        }

        public int ImageCount
        {
            get
            {
                switch (Item)
                {
                    case null:
                        return 0;
                    case Image _:
                    case GalleryImage _:
                        return 0;
                    case Album a:
                        return a.ImageCount;
                    case GalleryAlbum ga:
                        return ga.ImageCount;
                    default:
                        return 0;
                }
            }
        }
        public string Thumbnail => _thumbnail;
        public Image DisplayImage
        {
            get => _displayImage;
            set
            {
                _displayImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayImage)));
            }
        }

        public ItemViewModel() { }
        public ItemViewModel(IItem item) : this()
        {
            Item = item;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
