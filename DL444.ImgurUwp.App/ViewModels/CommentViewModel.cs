using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Windows.UI.Xaml;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class CommentViewModel : INotifyPropertyChanged, IReportable
    {
        Comment _comment;

        public Comment Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                RichContentBox = RichTextParser.GetRichContentBox(Content);
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

        public FrameworkElement RichContentBox { get; private set; }

        public CommentViewModel() { }
        public CommentViewModel(Comment comment, int level = 0)
        {
            Comment = comment;
            Level = level;
            ReportCommand = new AsyncCommand<bool>(Report);
        }

        public AsyncCommand<bool> ReportCommand { get; private set; }
        async Task<bool> Report()
        {
            Controls.ReportConfirmDialog dialog = new Controls.ReportConfirmDialog(this);
            var result = await dialog.ShowAsync();
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                return await ApiClient.Client.ReportCommentAsync(this.Id, dialog.SelectedReason);
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    static class CommentViewModelFactory
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
}
