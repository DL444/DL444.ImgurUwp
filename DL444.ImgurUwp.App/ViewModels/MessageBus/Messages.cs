using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels.MessageBus
{
    class FavoriteChangedMessage : Message
    {
        public FavoriteChangedMessage(string id, bool isAlbum, bool favorite, Models.IItem item = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            IsAlbum = isAlbum;
            Favorite = favorite;
            Item = item;
        }

        public string Id { get; }
        public bool IsAlbum { get; }
        public bool Favorite { get; }
        public Models.IItem Item { get; }
    }
    class FavoriteChangedMessageListener : MessageListener<FavoriteChangedMessage>
    {
        public FavoriteChangedMessageListener(Func<FavoriteChangedMessage, bool> handler) : base(handler) { }
    }

    class TagFollowChangedMessage : Message
    {
        public TagFollowChangedMessage(string tagName, bool following)
        {
            TagName = tagName ?? throw new ArgumentNullException(nameof(tagName));
            Following = following;
        }

        public string TagName { get; }
        public bool Following { get; }
    }
    class TagFollowChangedMessageListener : MessageListener<TagFollowChangedMessage>
    {
        public TagFollowChangedMessageListener(Func<TagFollowChangedMessage, bool> handler) : base(handler) { }
    }

    abstract class ItemMessage : Message
    {
        protected ItemMessage(string id, bool isAlbum)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            IsAlbum = isAlbum;
        }

        public string Id { get; }
        public bool IsAlbum { get; }
    }

    class ItemDeleteMessage : ItemMessage
    {
        public ItemDeleteMessage(string id, bool isAlbum) : base(id, isAlbum) { }
    }
    class ItemDeleteMessageListener : MessageListener<ItemDeleteMessage>
    {
        public ItemDeleteMessageListener(Func<ItemDeleteMessage, bool> handler) : base(handler) { }
    }

    class GalleryRemoveMessage : ItemMessage
    {
        public GalleryRemoveMessage(string id, bool isAlbum) : base(id, isAlbum) { }
    }
    class GalleryRemoveMessageListener : MessageListener<GalleryRemoveMessage>
    {
        public GalleryRemoveMessageListener(Func<GalleryRemoveMessage, bool> handler) : base(handler) { }
    }

    class BioChangedMessage : Message
    {
        public BioChangedMessage(string newBio = null)
        {
            NewBio = newBio;
        }

        public string NewBio { get; }
    }
    class BioChangedMessageListener : MessageListener<BioChangedMessage>
    {
        public BioChangedMessageListener(Func<BioChangedMessage, bool> handler) : base(handler) { }
    }

    class ItemUploadMessage : Message
    {
        public ItemUploadMessage(IItem item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }
        public static async Task<ItemUploadMessage> CreateFromId(string id, bool isAlbum)
        {
            if(id == null) { throw new ArgumentNullException(nameof(id)); }
            IItem item;
            if(isAlbum)
            {
                item = await ApiClient.Client.GetAlbumAsync(id);
            }
            else
            {
                item = await ApiClient.Client.GetImageAsync(id);
            }
            return new ItemUploadMessage(item);
        }

        public IItem Item { get; }
    }
    class ItemUploadMessageListener : MessageListener<ItemUploadMessage>
    {
        public ItemUploadMessageListener(Func<ItemUploadMessage, bool> handler) : base(handler) { }
    }

    class GalleryPostMessage : Message
    {
        public GalleryPostMessage(IGalleryItem item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public IGalleryItem Item { get; }
    }
    class GalleryPostMessageListener : MessageListener<GalleryPostMessage>
    {
        public GalleryPostMessageListener(Func<GalleryPostMessage, bool> handler) : base(handler) { }
    }

    class CommentPostMessage : Message
    {
        public Comment Comment { get; }
        public CommentPostMessage(Comment comment) => Comment = comment;
        public CommentPostMessage(int id, string imageId, string content, string author, string albumCover, int dateTime, int parentId = 0)
        {
            Comment = new Comment()
            {
                Id = id,
                ImageId = imageId,
                Content = content,
                Author = author,
                AlbumCover = albumCover,
                DateTime = dateTime,
                ParentId = parentId,
                Children = new List<Comment>()
            };
        }
    }
    class CommentPostMessageListener : MessageListener<CommentPostMessage>
    {
        public CommentPostMessageListener(Func<CommentPostMessage, bool> handler) : base(handler) { }
    }

    class CommentDeleteMessage : Message
    {
        public CommentDeleteMessage(int id) => Id = id;
        public int Id { get; }
    }
    class CommentDeleteMessageListener : MessageListener<CommentDeleteMessage>
    {
        public CommentDeleteMessageListener(Func<CommentDeleteMessage, bool> handler) : base(handler) { }
    }
}
