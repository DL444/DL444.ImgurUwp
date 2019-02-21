using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
