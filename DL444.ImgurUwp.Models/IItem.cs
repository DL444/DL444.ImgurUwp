namespace DL444.ImgurUwp.Models
{
    public interface IItem
    {
        string Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        int DateTime { get; set; }

        int Views { get; set; }
        string DeleteHash { get; set; }
        string Section { get; set; }
        string Link { get; set; }
        bool Favorite { get; set; }
        bool? Nsfw { get; set; }
        bool InGallery { get; set; }

        bool? IsAlbum { get; set; }

        string AccountUrl { get; set; }


        // Non-gallery items generally do not have points. But since account favorites endpoint
        // does not correctly indicate whether item is in gallery or not, this is used to determine.
        int? Points { get; set; }
    }
}
