using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using DL444.ImgurUwp.Models;
using Windows.UI;
using Windows.UI.Xaml;

namespace DL444.ImgurUwp.App.ViewModels
{
    static class Convert
    {
        static DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime ToDateTime(int epoch, bool utc = false)
        {
            var time = baseTime.AddSeconds(epoch);
            if (!utc) { time = time.ToLocalTime(); }
            return time;
        }
        public static int ToEpoch(DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();
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
        public static Color ToColor(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            if(hex.Length == 8)
            {
                byte a = (byte)System.Convert.ToUInt32(hex.Substring(0, 2), 16);
                byte r = (byte)System.Convert.ToUInt32(hex.Substring(2, 2), 16);
                byte g = (byte)System.Convert.ToUInt32(hex.Substring(4, 2), 16);
                byte b = (byte)System.Convert.ToUInt32(hex.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            else if(hex.Length == 6)
            {
                byte r = (byte)System.Convert.ToUInt32(hex.Substring(0, 2), 16);
                byte g = (byte)System.Convert.ToUInt32(hex.Substring(2, 2), 16);
                byte b = (byte)System.Convert.ToUInt32(hex.Substring(4, 2), 16);
                return Color.FromArgb(255, r, g, b);
            }
            else { throw new ArgumentException($"Specified string \"{hex}\" is not a valid color representation."); }
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
                Favorite = galleryImage.Favorite == true,
                Nsfw = galleryImage.Nsfw,
                Vote = galleryImage.Vote,
                IsAlbum = galleryImage.IsAlbum
            };
        }
    }

    public class NumberStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int number = (int)value;
            int abs = number < 0 ? -number : number;
            if(abs < 1_000)
            {
                return number.ToString();
            }
            else if(abs < 1_000_000)
            {
                return $"{GetApprox(number, 1000)}K";
            }
            else if(abs < 1_000_000_000)
            {
                return $"{GetApprox(number, 1000000)}M";
            }
            else
            {
                return $"{GetApprox(number, 1000000000)}B";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        string GetApprox(int number, int baseValue)
        {
            int intDigit = number / baseValue;
            if (Abs(intDigit) < 10)
            {
                int decDigit = (number % baseValue) / (baseValue / 10);
                return $"{intDigit}.{decDigit}";
            }
            else
            {
                return intDigit.ToString();
            }


            int Abs(int value) => value < 0 ? -value : value;
        }
    }

    public class DateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime date = (DateTime)value;
            return date.ToString("d");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public class FriendlyTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = ((DateTime)value).ToLocalTime();
            var currTime = DateTime.Now;
            return GetFriendlyTimeString(time, currTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        public static string GetFriendlyTimeString(DateTime time, DateTime currTime)
        {
            TimeSpan diff = currTime - time;

            var yesterday = DateTime.Today.AddDays(-1);

            double hourDiff = diff.TotalHours;
            double minDiff = diff.TotalMinutes;

            if (diff.TotalHours > 6.0)
            {
                // Today: More than 6 hours before
                if (time.Date == currTime.Date)
                {
                    return time.ToShortTimeString();
                }
                // Yesterday
                else if (time.Date == yesterday.Date)
                {
                    return $"{time.ToShortTimeString()} yesterday";
                }
                // Earlier
                else
                {
                    return time.ToString("g");
                }
            }
            else if (diff.TotalHours >= 1.0)
            {
                // Today: 1 - 6 hours before
                double hours = (int)(diff.TotalHours / 0.5) * 0.5;
                return $"{hours:0.#} hr";
            }
            else if (minDiff > 1.0)
            {
                // Today: Within an hour
                return $"{diff.Minutes} min";
            }
            else if (minDiff > -2.0)
            {
                // Today: Within a minute, tolerate to 2 minutes of inaccuracy.
                return "Moments ago";
            }
            else
            {
                if (hourDiff > -1.0)
                {
                    return "Minutes in the future";
                }
                else
                {
                    return "From the future";
                }
            }

            // TODO: Localize.
        }
    }

    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value == true) { return Visibility.Visible; }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((Visibility)value == Visibility.Visible) { return true; }
            return false;
        }
    }

    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }

    public class InvertedBoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value == true) { return Visibility.Collapsed; }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((Visibility)value == Visibility.Visible) { return false; }
            return true;
        }
    }

    static class CommonOperations
    {
        public static async Task Save(Windows.Storage.Streams.IInputStream inputStream, string filename)
        {
            var pictureLib = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            var defaultFolder = await pictureLib.SaveFolder.GetFolderAsync("Imgur");
            if (defaultFolder == null)
            {
                defaultFolder = await pictureLib.SaveFolder.CreateFolderAsync("Imgur");
            }

            var file = await defaultFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
            using (var fileStream = (await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)))
            {
                await Windows.Storage.Streams.RandomAccessStream.CopyAsync(inputStream, fileStream);
            }
        }
    }

    public interface IReportable { }
}
