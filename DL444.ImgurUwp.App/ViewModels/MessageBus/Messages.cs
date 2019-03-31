using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    class ItemDeleteMessage : Message
    {
        public ItemDeleteMessage(string id, bool isAlbum)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            IsAlbum = isAlbum;
        }

        public string Id { get; }
        public bool IsAlbum { get; }
    }
    class ItemDeleteMessageListener : MessageListener<ItemDeleteMessage>
    {
        public ItemDeleteMessageListener(Func<ItemDeleteMessage, bool> handler) : base(handler) { }
    }
}
