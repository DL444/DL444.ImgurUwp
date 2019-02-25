using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        public async Task<Stream> DownloadMediaAsync(string url)
        {
            HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, new Uri(url, UriKind.Absolute));
            var response = await client.SendAsync(requestMsg);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
