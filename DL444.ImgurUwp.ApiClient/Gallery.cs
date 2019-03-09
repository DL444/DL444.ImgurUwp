using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static DL444.ImgurUwp.ApiClient.DisplayParams;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        public async Task<IEnumerable<IGalleryItem>> GetGalleryItemsAsync(Sort sort = Sort.Viral, int page = 0,
            Section section = Section.Hot, Window window = Window.Day, bool showViral = true)
        {
            List<IGalleryItem> result = new List<IGalleryItem>();
            var response = await client.GetAsync($"/3/gallery/{section}/{sort}/{window}/{page}/{showViral}".ToLower());
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                JArray imgJsonArray = JArray.Parse(dataJson);
                foreach (var i in imgJsonArray)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<GalleryAlbum>());
                    }
                    else
                    {
                        result.Add(i.ToObject<GalleryImage>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<IGalleryItem> GetGalleryItemAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/{id}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                JObject obj = JObject.Parse(dataJson);
                if (obj["is_album"].ToObject<bool>())
                {
                    return obj.ToObject<GalleryAlbum>();
                }
                else
                {
                    return obj.ToObject<GalleryImage>();
                }
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<GalleryImage> GetGalleryImageAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/image/{id}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<GalleryImage>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<GalleryAlbum> GetGalleryAlbumAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/album/{id}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<GalleryAlbum>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<int> GetGalleryCommentCountAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/{id}/comments/count");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<int>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<int>> GetGalleryCommentIdsAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/{id}/comments/ids");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<int>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<Comment>> GetGalleryCommentsAsync(string id, CommentDisplayParams.Sort sort = CommentDisplayParams.Sort.Best)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/{id}/comments/{sort.ToString().ToLower()}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<Comment>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<Votes> GetGalleryVotesAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/{id}/votes");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<Votes>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<IEnumerable<IGalleryItem>> GallerySearchAsync(string query, Sort sort = Sort.Time, Window window = Window.All, int page = 0)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            List<IGalleryItem> result = new List<IGalleryItem>();
            var response = await client.GetAsync($"{$"/3/gallery/search/{sort}/{window}/{page}".ToLower()}?q={query}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var items = JArray.Parse(dataJson);
                foreach (var i in items)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<GalleryAlbum>());
                    }
                    else
                    {
                        result.Add(i.ToObject<GalleryImage>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        /// <summary>
        /// Perform advanced search in gallery.
        /// </summary>
        /// <param name="qAll">Search for all of these words (and)</param>
        /// <param name="qAny">Search for any of these words (or)</param>
        /// <param name="qExactly">Search for exactly this word or phrase</param>
        /// <param name="qNot">Exclude results matching this (not)</param>
        /// <param name="qType">Show results for any file type, jpg | png | gif | anigif (animated gif) | album</param>
        /// <param name="qSizePx">Size ranges, small (500 pixels square or less) | med (500 to 2,000 pixels square) | big(2,000 to 5,000 pixels square)
        /// | lrg(5,000 to 10,000 pixels square) | huge(10,000 square pixels and above)</param>
        public async Task<IEnumerable<IGalleryItem>> GallerySearchAsync(string qAll, string qAny, string qExactly, string qNot, string qType, string qSizePx, Sort sort = Sort.Time, Window window = Window.All, int page = 0)
        {
            throw new NotImplementedException($"Method {nameof(GallerySearchAsync)} is not yet implemented.");
        }

        public async Task<IEnumerable<IGalleryItem>> GetMemeGalleryItemsAsync(Sort sort = Sort.Viral, Window window = Window.Week, int page = 0)
        {
            var response = await client.GetAsync($"/3/g/memes/{sort}/{window}/{page}".ToLower());
            List<IGalleryItem> result = new List<IGalleryItem>();
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var items = JArray.Parse(dataJson);
                foreach (var i in items)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<GalleryAlbum>());
                    }
                    else
                    {
                        result.Add(i.ToObject<GalleryImage>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<IEnumerable<IGalleryItem>> GetRandomGalleryItemsAsync(int page = 0)
        {
            List<IGalleryItem> result = new List<IGalleryItem>();
            var response = await client.GetAsync($"/3/gallery/random/random/{page}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var items = JArray.Parse(dataJson);
                foreach (var i in items)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<GalleryAlbum>());
                    }
                    else
                    {
                        result.Add(i.ToObject<GalleryImage>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<bool> VoteGalleryItemAsync(string id, Vote vote)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/gallery/{id}/vote/{vote.ToString().ToLower()}");
            var response = await client.SendAsync(msg);
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<bool>(dataJson.ToLower());
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<bool> RemoveGalleryItemAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Delete, $"/3/gallery/{id}");
            var response = await client.SendAsync(msg);
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<bool>(dataJson.ToLower());
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<bool> ReportGalleryItemAsync(string id, ReportReason reason = ReportReason.Unspecified)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/gallery/{id}/report");
            msg.Content = new StringContent($"{{ \"reason\": {(int)reason} }}");
            msg.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await client.SendAsync(msg);
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<bool>(dataJson.ToLower());
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
    }
}
