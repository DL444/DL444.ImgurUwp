using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels.MessageBus
{
    class FavoriteChangedMessage : Message
    {
        public FavoriteChangedMessage(string id, bool isAlbum, bool favorite)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            IsAlbum = isAlbum;
            Favorite = favorite;
        }

        public string Id { get; }
        public bool IsAlbum { get; }
        public bool Favorite { get; }
    }
    class FavoriteChangedMessageListener : MessageListener<FavoriteChangedMessage>
    {
        public FavoriteChangedMessageListener(Func<FavoriteChangedMessage, bool> handler) : base(handler) { }
    }
}
