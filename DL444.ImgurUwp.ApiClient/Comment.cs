using DL444.ImgurUwp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        public async Task<Comment> GetCommentAsync(int id, bool includeReplies = false)
        {
            var response = await client.GetAsync($"/3/comment/{id}{(includeReplies ? "/replies" : "")}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<Comment>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<int> PostCommentAsync(string imageId, string content, string parentId = null)
        {
            if(imageId == null) { throw new ArgumentNullException(nameof(imageId)); }
            if(content == null) { throw new ArgumentNullException(nameof(content)); }
            PostCommentParams comment = new PostCommentParams(imageId, content, parentId);

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/gallery/{imageId}/comment");
            msg.Content = new StringContent(JsonConvert.SerializeObject(comment));
            msg.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await client.SendAsync(msg);

            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JObject.Parse(dataJson)["id"].ToObject<int>();
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<bool> DeleteCommentAsync(int id)
        {
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Delete, $"/3/comment/{id}");
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
        public async Task<bool> VoteCommentAsync(int id, Vote vote)
        {
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/comment/{id}/vote/{vote.ToString().ToLower()}");
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
        public async Task<bool> ReportCommentAsync(int id, ReportReason reason = ReportReason.Unspecified)
        {
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/comment/{id}/report");
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
