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
                //RichContentBox = RichTextParser.GetRichContentBox(Content);
                foreach (var c in _comment.Children)
                {
                    Children.Add(new CommentViewModel(c));
                }
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

        public List<CommentViewModel> Children { get; } = new List<CommentViewModel>();

        public int Level { get; private set; }

        // Caching this will cause problem with virtualization of TreeView. 
        public FrameworkElement RichContentBox => RichTextParser.GetRichContentBox(Content);

        public CommentViewModel()
        {
            ShareCommand = new Command(Share);
            ReportCommand = new AsyncCommand<bool>(Report);
        }
        public CommentViewModel(Comment comment, int level = 0) : this()
        {
            Comment = comment;
            Level = level;
        }

        public Command ShareCommand { get; private set; }
        public AsyncCommand<bool> ReportCommand { get; private set; }
        void Share()
        {
            var transferMgr = Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView();
            transferMgr.DataRequested += TransferMgr_DataRequested;
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }
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

        private void TransferMgr_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetWebLink(new Uri($"https://imgur.com/gallery/{ImageId}/comment/{Id}"));
            request.Data.Properties.Title = $"Comment from Imgur";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    static class CommentViewModelFactory
    {
        public static IEnumerable<CommentViewModel> BuildCommentViewModels(IEnumerable<Comment> comments)
        {
            List<CommentViewModel> result = new List<CommentViewModel>();
            foreach(var c in comments)
            {
                result.Add(new CommentViewModel(c));
            }
            return result;
        }
    }
}
