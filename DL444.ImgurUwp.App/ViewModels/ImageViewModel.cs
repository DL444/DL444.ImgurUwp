using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        Image _image;

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Id => Image.Id;
        public string Title => Image.Title;
        public string Description => Image.Description;
        public DateTime DateTime => Convert.ToDateTime(Image.DateTime);
        public string Type => Image.Type;
        public bool IsAnimated => Image.Animated;
        public int Width => Image.Width;
        public int Height => Image.Height;
        public int Size => Image.Size;
        public int Views => Image.Views;
        public long Bandwidth => Image.Bandwidth;
        public string DeleteHash => Image.DeleteHash;
        public string Name => Image.Name;
        public string Section => Image.Section;
        public string Link => Image.Link;
        public string Gifv => Image.Gifv;
        public string Mp4 => Image.Mp4;
        public int Mp4Size => Image.Mp4Size;
        public bool Looping => Image.Looping;
        public bool Favorite => Image.Favorite;
        public bool Nsfw => Image.Nsfw == true;
        public string Vote => Image.Vote;
        public bool InGallery => Image.InGallery;
        
        public ImageViewModel() { }
        public ImageViewModel(Image image) => Image = image;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
