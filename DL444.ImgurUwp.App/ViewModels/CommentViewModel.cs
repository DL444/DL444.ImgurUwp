using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Windows.UI.Xaml;
using System.Collections.ObjectModel;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class CommentViewModel : INotifyPropertyChanged, IReportable
    {
        Comment _comment;
        string _reply = "";
        Visibility _replyFieldVisibility = Visibility.Collapsed;
        bool _upvoted;
        bool _downvoted;
        IEnumerable<RichTextComponent> components;

        public Comment Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                //RichContentBox = RichTextParser.GetRichContentBox(Content);

                SetComponents();

                if (_comment.Children != null)
                {
                    foreach (var c in _comment.Children)
                    {
                        Children.Add(new CommentViewModel(c));
                    }
                }
                _upvoted = _comment.Vote == "up";
                _downvoted = _comment.Vote == "down";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public int Id => Comment.Id;
        public string ImageId => Comment.ImageId;
        public string ImageUrl
        {
            get
            {
                if(AlbumCover == null)
                {
                    return $"https://imgur.com/{ImageId}l.jpg";
                }
                else
                {
                    return $"https://imgur.com/{AlbumCover}l.jpg";
                }
            }
        }
        public string Content => Comment.Content;
        public string Author => Comment.Author;
        public bool OnAlbum => Comment.OnAlbum;
        public string AlbumCover => Comment.AlbumCover;
        public int Ups
        {
            get => _comment.Ups;
            set
            {
                _comment.Ups = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ups)));
            }
        }
        public int Downs
        {
            get => _comment.Downs;
            set
            {
                _comment.Downs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Downs)));
            }
        }
        public int Points => Comment.Points;
        public DateTime DateTime => Convert.ToDateTime(Comment.DateTime);
        public int ParentId => Comment.ParentId;
        public bool Deleted => Comment.Deleted;

        public bool Upvoted
        {
            get => _upvoted;
            set
            {
                _upvoted = value;
                if (value == true)
                {
                    Downvoted = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Upvoted)));
            }
        }
        public bool Downvoted
        {
            get => _downvoted;
            set
            {
                _downvoted = value;
                if (value == true)
                {
                    Upvoted = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Downvoted)));
            }
        }

        public ObservableCollection<CommentViewModel> Children { get; } = new ObservableCollection<CommentViewModel>();

        public int Level { get; private set; }
        public bool HasChildren => Children == null ? false : Children.Count > 0;
        public bool IsOwner => Author == ApiClient.OwnerAccount && !Deleted;

        public string Reply {
            get => _reply;
            set
            {
                _reply = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Reply)));
                SendReplyCommand.RaiseCanExecuteChanged();
            }
        }
        public Visibility ReplyFieldVisibilty
        {
            get => _replyFieldVisibility;
            set
            {
                _replyFieldVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplyFieldVisibilty)));
            }
        }

        public CommentReactionImage ReactionImage { get; private set; } = null;

        // Caching this will cause problem with virtualization of TreeView. 
        public FrameworkElement RichContentBox => RichTextParser.GetRichContentBox(components);

        public CommentViewModel()
        {
            Children.CollectionChanged += Children_CollectionChanged;
            ShowReplyFieldCommand = new Command(ShowReplyField);
            ShareCommand = new Command(Share);
            ReportCommand = new AsyncCommand<bool>(Report);
            SendReplyCommand = new AsyncCommand<int>(SendReply, () => !string.IsNullOrWhiteSpace(Reply));
            UpvoteCommand = new AsyncCommand<bool>(() => Vote(ImgurUwp.ApiClient.Vote.Up));
            DownvoteCommand = new AsyncCommand<bool>(() => Vote(ImgurUwp.ApiClient.Vote.Down));
            DeleteCommand = new AsyncCommand<bool>(Delete);
        }

        public CommentViewModel(Comment comment) : this() => Comment = comment;

        public Command ShowReplyFieldCommand { get; private set; }
        public Command ShareCommand { get; private set; }
        public AsyncCommand<bool> ReportCommand { get; private set; }
        public AsyncCommand<int> SendReplyCommand { get; private set; }
        public AsyncCommand<bool> UpvoteCommand { get; private set; }
        public AsyncCommand<bool> DownvoteCommand { get; private set; }
        public AsyncCommand<bool> DeleteCommand { get; private set; }

        void ShowReplyField()
        {
            if(ReplyFieldVisibilty == Visibility.Collapsed)
            {
                ReplyFieldVisibilty = Visibility.Visible;
            }
            else
            {
                ReplyFieldVisibilty = Visibility.Collapsed;
            }
        }
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
        async Task<int> SendReply()
        {
            ReplyFieldVisibilty = Visibility.Collapsed;
            int newId = await ApiClient.Client.PostCommentAsync(ImageId, Reply, Id.ToString());
            MessageBus.ViewModelMessageBus.Instance.SendMessage(
                new MessageBus.CommentPostMessage(newId, ImageId, Reply, ApiClient.OwnerAccount, AlbumCover, Convert.ToEpoch(DateTime.Now), Id));
            Reply = "";
            return newId;
        }
        async Task<bool> Vote(ImgurUwp.ApiClient.Vote vote)
        {
            bool result = await ApiClient.Client.VoteCommentAsync(Id, vote);
            Comment comment = await ApiClient.Client.GetCommentAsync(Id);
            Ups = comment.Ups;
            Downs = comment.Downs;
            return result;
        }
        async Task<bool> Delete()
        {
            var result = await ApiClient.Client.DeleteCommentAsync(Id);
            if(result == true)
            {
                if (Children.Count != 0)
                {
                    SetVirtualDeleteState();
                }

                MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.CommentDeleteMessage(Id, ImageId, Children.Count != 0));
            }
            return result;
        }

        void SetVirtualDeleteState()
        {
            Comment.Content = "[deleted]";
            Comment.Author = "[deleted]";
            ReactionImage = null;
            Comment.Deleted = true;
            SetComponents();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        private void SetComponents()
        {
            components = RichTextParser.ParseComment(Content);
            foreach (var c in components)
            {
                if (c is UriComponent uri && uri.Type != UriType.Normal)
                {
                    ReactionImage = new CommentReactionImage(uri.Type == UriType.Video, uri.Text);
                    break;
                }
            }
        }

        private void TransferMgr_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetWebLink(new Uri($"https://imgur.com/gallery/{ImageId}/comment/{Id}"));
            request.Data.Properties.Title = $"Comment from Imgur";
        }
        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChildren)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class CommentReactionImage
    {
        public CommentReactionImage(bool isVideo, string url)
        {
            IsVideo = isVideo;
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public bool IsVideo { get; set; }
        public string Url { get; set; }
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
