using DL444.ImgurUwp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    static class Defaults
    {
        public static Account DefaultAccount =>
            new Account()
            {
                Id = 0, Url = "", Bio = "", Reputation = 0,
                CreatedTime = 0, ReputationName = "neutral",
                IsBlocked = false, Avatar = "https://i.imgur.com/XzARrBw_d.png?maxwidth=290", AvatarName = "default/default",
                Cover = "", CoverName = "", UserFollow = false
            };

        public static GalleryImage DefaultImage =>
            new GalleryImage()
            {
                IsAlbum = false,
                Id = "",
                Title = "",
                Description = "",
                DateTime = 0,
                Link = "https://i.imgur.com/HFoOCeg.jpg",
                AccountUrl = "",
                AccountId = "",
                Topic = "",
                TopicId = 0,
                Nsfw = false,
                CommentCount = 0,
                Ups = 0,
                Downs = 0,
                Points = 0,
                Score = 0,
                Views = 0,
                InMostViral = false,
                Favorite = false
            };
    }
}
