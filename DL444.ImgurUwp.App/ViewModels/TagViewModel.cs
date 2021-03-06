﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DL444.ImgurUwp.App.ViewModels
{
    class TagViewModel : INotifyPropertyChanged
    {
        Tag _tag;
        readonly Func<MessageBus.TagFollowChangedMessage, bool> followChangedHandler;
        readonly Func<MessageBus.FavoriteChangedMessage, bool> favoriteChangedHandler;

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

        public IconElement FollowIcon => (Tag != null && Tag.Following) ? new SymbolIcon(Symbol.Accept) : new SymbolIcon(Symbol.Add);
        public string FollowText => (Tag != null && Tag.Following) ? "Following" : "Follow";

        public Visibility DescriptionVisibility => string.IsNullOrEmpty(Description) ? Visibility.Collapsed : Visibility.Visible;

        public Command ShowDetailsCommand { get; private set; }
        void ShowDetails()
        {
            Navigation.ContentFrame.Navigate(typeof(Pages.Tag), this);
        }

        public AsyncCommand<bool> FollowCommand { get; private set; }
        async Task<bool> Follow()
        {
            bool result;
            if(!Following)
            {
                result = await ApiClient.Client.FollowTagAsync(Name);
                if(result == true)
                {
                    Tag.Following = true;
                }
            }
            else
            {
                result = await ApiClient.Client.UnfollowTagAsync(Name);
                if (result == true)
                {
                    Tag.Following = false;
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FollowIcon)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FollowText)));
            if(result == true)
            {
                MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.TagFollowChangedMessage(Name, Following));
            }
            return result;
        }

        public TagViewModel()
        {
            FollowCommand = new AsyncCommand<bool>(Follow);
            ShowDetailsCommand = new Command(ShowDetails);
            followChangedHandler = new Func<MessageBus.TagFollowChangedMessage, bool>(x =>
            {
                if(x.TagName == this.Name)
                {
                    this.Tag.Following = x.Following;
                    return true;
                }
                else { return false; }
            });
            MessageBus.ViewModelMessageBus.Instance.RegisterListener(new MessageBus.TagFollowChangedMessageListener(followChangedHandler));
            favoriteChangedHandler = new Func<MessageBus.FavoriteChangedMessage, bool>(x =>
            {
                var item = Items.FirstOrDefault(i => i.Id == x.Id && i.IsAlbum == x.IsAlbum);
                if(item == null) { return false; }
                item.Favorite = true;
                return true;
            });
        }
        public TagViewModel(Tag tag) : this()
        {
            Tag = tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
