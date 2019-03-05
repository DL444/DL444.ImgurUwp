using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class CommentViewModel : INotifyPropertyChanged
    {
        Comment _comment;

        public Comment Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public int Id => Comment.Id;
        public string ImageId => Comment.ImageId;
        public string Content => Comment.Content;
        public string Author => Comment.Author;
        public bool OnAlbum => Comment.OnAlbum;
        public string AlbumCover => Comment.AlbumCover;
        public int Ups => Comment.Ups;
        public int Downs => Comment.Downs;
        public int Points => Comment.Points;
        public DateTime DateTime => Convert.ToDateTime(Comment.DateTime);
        public int ParentId => Comment.ParentId;
        public bool Deleted => Comment.Deleted;
        public string Vote => Comment.Vote;

        public List<Comment> Children => Comment.Children;

        public int Level { get; private set; }

        public CommentViewModel() { }
        public CommentViewModel(Comment comment, int level = 0)
        {
            Comment = comment;
            Level = level;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public static class CommentViewModelFactory
    {
        public static IEnumerable<CommentViewModel> BuildCommentViewModels(IEnumerable<Comment> comments, int baseLevel = 0)
        {
            List<CommentViewModel> result = new List<CommentViewModel>();
            foreach(var c in comments)
            {
                result.AddRange(BuildCommentViewModels(c, baseLevel));
            }
            return result;
        }
        public static IEnumerable<CommentViewModel> BuildCommentViewModels(Comment comment, int level = 0)
        {
            List<CommentViewModel> result = new List<CommentViewModel>();
            result.Add(new CommentViewModel(comment, level));
            foreach(var c in comment.Children)
            {
                result.AddRange(BuildCommentViewModels(c, level + 1));
            }
            return result;
        }
    }

    static class CommentContentParser
    {
        public static IEnumerable<CommentComponent> ParseComment(string comment)
        {
            List<CommentComponent> result = new List<CommentComponent>();
            StringBuilder strBuilder = new StringBuilder();
            string[] words = comment.Split(' ');
            foreach (var w in words)
            {
                if (IsUri(w))
                {
                    if (strBuilder.Length != 0)
                    {
                        result.Add(new TextComponent(strBuilder.ToString()));
                        strBuilder.Clear();
                    }
                    result.Add(new UriComponent(w));
                }
                else
                {
                    strBuilder.Append($"{w} ");
                }
            }
            result.Add(new TextComponent(strBuilder.ToString()));
            return result;
        }

        static bool IsUri(string str)
        {
            if (Uri.TryCreate(str, UriKind.Absolute, out System.Uri uri) == true)
            {
                if (uri.Scheme != "http" && uri.Scheme != "https") { return false; }
                return true;
            }
            else { return false; }
        }
    }

    abstract class CommentComponent
    {
        public string Text { get; set; }
    }
    class TextComponent : CommentComponent
    {
        public override string ToString() => Text;
        public TextComponent(string text) => Text = text;
    }
    class UriComponent : CommentComponent
    {
        public override string ToString() => $"{Type}: {Text}";
        public UriType Type { get; set; }
        public UriComponent(string uri)
        {
            Text = uri;
            var ext = uri.Substring(uri.LastIndexOf('.') + 1).ToLower();
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                case "apng":
                case "tif":
                case "tiff":
                    Type = UriType.Image;
                    break;
                case "mp4":
                case "gifv":
                    Type = UriType.Video;
                    break;
                default:
                    Type = UriType.Normal;
                    break;
            }
        }
    }
    enum UriType
    {
        Normal, Image, Video
    }

    // There are also @user, #tag, and #imageId.
}
