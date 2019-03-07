using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using System.ComponentModel;


namespace DL444.ImgurUwp.App.ViewModels
{
    class TrophyViewModel : INotifyPropertyChanged
    {
        Trophy _trophy;

        public Trophy Trophy
        {
            get => _trophy;
            set
            {
                _trophy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Name => Trophy.Name;
        public string DisplayName => Trophy.DisplayName;
        public int Id => Trophy.Id;
        public string Description => Trophy.Description;
        public DateTime DateTime => Convert.ToDateTime(Trophy.DateTime);
        public string Image => Trophy.Image;
        public int ImageHeight => Trophy.ImageHeight;
        public int ImageWidth => Trophy.ImageWidth;
        public string Data => Trophy.Data;
        public string DataLink => Trophy.DataLink;

        public event PropertyChangedEventHandler PropertyChanged;

        public TrophyViewModel() { }
        public TrophyViewModel(Trophy trophy) : this() => Trophy = trophy;
    }
}
