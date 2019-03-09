﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace DL444.ImgurUwp.App.ViewModels
{
    class TagViewModel : INotifyPropertyChanged
    {
        Tag _tag;
        public Tag Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Name => Tag.Name;
        public int Followers => Tag.Followers;
        public int TotalItems => Tag.TotalItems;
        public bool Following => Tag.Following;
        public List<IGalleryItem> Items => Tag.Items;

        public string DisplayName => Tag.DisplayName;
        public SolidColorBrush Accent => Tag.Accent == null ? null : new SolidColorBrush(Convert.ToColor(Tag.Accent));
        public string BackgroundImage => $"https://i.imgur.com/{Tag.BackgroundImageHash}.png";
        public string Description => Tag.Description;

        public Visibility DescriptionVisibility => string.IsNullOrEmpty(Description) ? Visibility.Collapsed : Visibility.Visible;

        public Command ShowDetailsCommand { get; private set; }
        void ShowDetails()
        {
            Navigation.ContentFrame.Navigate(typeof(Pages.Tag), this);
        }

        public TagViewModel()
        {
            ShowDetailsCommand = new Command(ShowDetails);
        }
        public TagViewModel(Tag tag) : this()
        {
            Tag = tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
