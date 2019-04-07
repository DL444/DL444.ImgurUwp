using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        public async Task<(string id, string deleteHash)> CreateAlbumAsync(IEnumerable<string> imageIds = null, string title = null, string description = null,
            AlbumPrivacy? privacy = null, string coverId = null, IEnumerable<string> deleteHashes = null)
        {
            if(coverId != null)
            {
                if(imageIds == null && deleteHashes == null)
                {
                    // All null
                    throw new ArgumentException("The specified cover is not in this album.");
                }
                else if(deleteHashes == null)
                {
                    // imageIds not null
                    if(!imageIds.Contains(coverId))
                    {
                        throw new ArgumentException("The specified cover is not in this album.");
                    }
                }
                else if(imageIds == null)
                {
                    // deleteHashes not null
                    if (!deleteHashes.Contains(coverId))
                    {
                        throw new ArgumentException("The specified cover is not in this album.");
                    }
                }
                else
                {
                    // All not null
                    if((!imageIds.Contains(coverId)) && (!deleteHashes.Contains(coverId)))
                    {
                        throw new ArgumentException("The specified cover is not in this album.");
                    }
                }
            }

            AlbumCreateParams album = new AlbumCreateParams(title, description, privacy, imageIds, deleteHashes, coverId);

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "/3/album");
            msg.Content = new StringContent(JsonConvert.SerializeObject(album));
            msg.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await client.SendAsync(msg);

            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                JObject obj = JObject.Parse(dataJson);
                return (obj["id"].ToObject<string>(), obj["deletehash"].ToObject<string>());
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<bool> UpdateAlbumInfoAsync(string id, IEnumerable<string> imageIds = null, string title = null, string description = null,
            AlbumPrivacy? privacy = null, string coverId = null, IEnumerable<string> deleteHashes = null)
        {
            if(id == null) { throw new ArgumentNullException(nameof(id)); }
            
            // Cover not in album is not important after all.
            //if (coverId != null)
            //{
            //    if (imageIds == null && deleteHashes == null)
            //    {
            //        // All null
            //        throw new ArgumentException("The specified cover is not in this album.");
            //    }
            //    else if (deleteHashes == null)
            //    {
            //        // imageIds not null
            //        if (!imageIds.Contains(coverId))
            //        {
            //            throw new ArgumentException("The specified cover is not in this album.");
            //        }
            //    }
            //    else if (imageIds == null)
            //    {
            //        // deleteHashes not null
            //        if (!deleteHashes.Contains(coverId))
            //        {
            //            throw new ArgumentException("The specified cover is not in this album.");
            //        }
            //    }
            //    else
            //    {
            //        // All not null
            //        if ((!imageIds.Contains(coverId)) && (!deleteHashes.Contains(coverId)))
            //        {
            //            throw new ArgumentException("The specified cover is not in this album.");
            //        }
            //    }
            //}

            AlbumCreateParams album = new AlbumCreateParams(title, description, privacy, imageIds, deleteHashes, coverId);

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/album/{id}");
            msg.Content = new StringContent(JsonConvert.SerializeObject(album));
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

        public async Task<Album> GetAlbumAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/album/{id}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<Album>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<bool> FavoriteAlbumAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/album/{id}/favorite");
            var response = await client.SendAsync(msg);
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return string.Equals(dataJson, "favorited", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<bool> EditAlbumImageAsync(string id, IEnumerable<string> imageIds, AlbumEditMode editMode = AlbumEditMode.Add)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            if (imageIds == null) { throw new ArgumentNullException(nameof(imageIds)); }

            string url;
            if (editMode == AlbumEditMode.Remove)
            {
                // See https://stackoverflow.com/questions/23859696/imgur-api-wont-remove-image-from-album
                if (!imageIds.Any()) { return true; }
                StringBuilder urlBuilder = new StringBuilder($"/3/album/{id}/remove_images?");
                foreach(var i in imageIds)
                {
                    urlBuilder.Append($"&ids[]={i}");
                }
                url = urlBuilder.ToString();
            }
            else
            {
                url = $"/3/album/{id}";
            }

            if (editMode == AlbumEditMode.Add) { url += "/add"; }

            HttpRequestMessage msg = new HttpRequestMessage(editMode == AlbumEditMode.Remove ? HttpMethod.Delete : HttpMethod.Post, url);
            if(editMode != AlbumEditMode.Remove)
            {
                string contentStr = $"{{\"ids\":{JsonConvert.SerializeObject(imageIds)}}}";
                msg.Content = new StringContent(contentStr);
                msg.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            }

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

        public async Task<bool> DeleteAlbumAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Delete, $"/3/album/{id}");
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
