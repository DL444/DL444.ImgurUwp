﻿using System;
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
        string _reply = "";
        Visibility _replyFieldVisibility = Visibility.Collapsed;
        bool _upvoted;
        bool _downvoted;

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

                _upvoted = _comment.Vote == "up";
                _downvoted = _comment.Vote == "down";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public int Id => Comment.Id;
        public string ImageId => Comment.ImageId;
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
        //public string Vote => Comment.Vote;

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

        public List<CommentViewModel> Children { get; } = new List<CommentViewModel>();

        public int Level { get; private set; }

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


        // Caching this will cause problem with virtualization of TreeView. 
        public FrameworkElement RichContentBox => RichTextParser.GetRichContentBox(Content);

        public CommentViewModel()
        {
            ShowReplyFieldCommand = new Command(ShowReplyField);
            ShareCommand = new Command(Share);
            ReportCommand = new AsyncCommand<bool>(Report);
            SendReplyCommand = new AsyncCommand<int>(SendReply, () => !string.IsNullOrWhiteSpace(Reply));
            UpvoteCommand = new AsyncCommand<bool>(() => Vote(ImgurUwp.ApiClient.Vote.Up));
            DownvoteCommand = new AsyncCommand<bool>(() => Vote(ImgurUwp.ApiClient.Vote.Down));
        }
        public CommentViewModel(Comment comment, int level = 0) : this()
        {
            Comment = comment;
            Level = level;
        }

        public Command ShowReplyFieldCommand { get; private set; }
        public Command ShareCommand { get; private set; }
        public AsyncCommand<bool> ReportCommand { get; private set; }
        public AsyncCommand<int> SendReplyCommand { get; private set; }
        public AsyncCommand<bool> UpvoteCommand { get; private set; }
        public AsyncCommand<bool> DownvoteCommand { get; private set; }


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
            Reply = "";
            return newId;
        }
        public async Task<bool> Vote(ImgurUwp.ApiClient.Vote vote)
        {
            bool result = await ApiClient.Client.VoteCommentAsync(Id, vote);
            Comment comment = await ApiClient.Client.GetCommentAsync(Id);
            Ups = comment.Ups;
            Downs = comment.Downs;
            return result;
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
