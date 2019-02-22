﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    static class Convert
    {
        static DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime ToDateTime(int epoch)
        {
            return baseTime.AddSeconds(epoch);
        }
        public static int ToEpoch(DateTime dateTime)
        {
            return (int)(dateTime - baseTime).TotalSeconds;
        }
        public static string Capitalize(this string str)
        {
            if(str == null) { return ""; }
            if (str.Length > 0)
            {
                return $"{char.ToUpper(str[0])}{str.Substring(1)}";
            }
            else { return ""; }
        }
    }

    static class ImageExtensions
    {
        public static Image ToImage(this GalleryImage galleryImage)
        {
            return new Image()
            {
                Id = galleryImage.Id,
                Title = galleryImage.Title,
                Description = galleryImage.Description,
                DateTime = galleryImage.DateTime,
                Type = galleryImage.Type,
                Animated = galleryImage.Animated,
                Width = galleryImage.Width,
                Height = galleryImage.Height,
                Size = galleryImage.Size,
                Views = galleryImage.Views,
                Bandwidth = galleryImage.Bandwidth,
                DeleteHash = galleryImage.DeleteHash,
                Section = galleryImage.Section,
                Link = galleryImage.Link,
                Gifv = galleryImage.Gifv,
                Mp4 = galleryImage.Mp4,
                Mp4Size = galleryImage.Mp4Size,
                Looping = galleryImage.Looping,
                Favorite = galleryImage.Favorite,
                Nsfw = galleryImage.Nsfw,
                Vote = galleryImage.Vote,
                IsAlbum = galleryImage.IsAlbum
            };
        }
    }
}
