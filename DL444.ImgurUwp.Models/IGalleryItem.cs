using System;
using System.Collections.Generic;
using System.Text;

namespace DL444.ImgurUwp.Models
{
    public interface IGalleryItem
    {
        string Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        int DateTime { get; set; }

        string Link { get; set; }

        string AccountUrl { get; set; }
        string AccountId { get; set; }

        string Topic { get; set; }
        int TopicId { get; set; }

        bool? Nsfw { get; set; }

        int CommentCount { get; set; }

        int Ups { get; set; }
        int Downs { get; set; }
        int Points { get; set; }
        int Score { get; set; }
        int Views { get; set; }

        bool IsAlbum { get; set; }

        bool InMostViral { get; set; }

        bool Favorite { get; set; }
        bool InGallery { get; set; }

        string Vote { get; set; }
    }
}
