using DL444.ImgurUwp.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Net.Http;
using System.IO;

namespace DL444.ImgurUwp.App
{
    static class OAuthClient
    {
        public static async Task<UserToken> Authenticate()
        {
            string requestUri = $"https://api.imgur.com/oauth2/authorize?client_id={ApiKey.ClientId}&response_type=token";
            string redirectUri = "https://imgur.com";
            string result = "";

            try
            {
                var webAuthenticationResult =
                    await Windows.Security.Authentication.Web.WebAuthenticationBroker.AuthenticateAsync(
                    Windows.Security.Authentication.Web.WebAuthenticationOptions.None,
                    new Uri(requestUri),
                    new Uri(redirectUri));

                switch (webAuthenticationResult.ResponseStatus)
                {
                    case Windows.Security.Authentication.Web.WebAuthenticationStatus.Success:
                        result = webAuthenticationResult.ResponseData;
                        break;
                    case Windows.Security.Authentication.Web.WebAuthenticationStatus.ErrorHttp:
                        throw new OAuthFailedException(OAuthFailureReason.NetworkException);
                    default:
                        throw new OAuthFailedException(OAuthFailureReason.Others);
                }
            }
            catch (Exception ex)
            {
                throw new OAuthFailedException(OAuthFailureReason.NetworkException, "Authentication cancelled.", ex);
            }

            try
            {
                result = "?" + result.Split('#')[1];
            }
            catch(IndexOutOfRangeException ex)
            {
                throw new OAuthFailedException(OAuthFailureReason.Others, "Authentication result parsing failed.", ex);
            }

            try
            {
                var decoder = new WwwFormUrlDecoder(result);
                string accessToken = decoder.GetFirstValueByName("access_token");
                string refreshToken = decoder.GetFirstValueByName("refresh_token");
                string expireTime = decoder.GetFirstValueByName("expires_in");
                string userId = decoder.GetFirstValueByName("account_id");
                string username = decoder.GetFirstValueByName("account_username");

                return new UserToken()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserId = userId,
                    Username = username,
                    ExpireTime = DateTime.Now + new TimeSpan(0, 0, int.Parse(expireTime))
                };
            }
            catch(ArgumentOutOfRangeException ex)
            {
                throw new OAuthFailedException(OAuthFailureReason.PermissionDenied, "Authentication cancelled.", ex);
            }
        }
        public static async Task<UserToken> RefreshToken(UserToken token)
        {
            var parameters = new RefreshTokenRequestParameters(token.RefreshToken);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.imgur.com");
            string result = "";

            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "oauth2/token"))
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(parameters));
                try
                {
                    var response = await client.SendAsync(message);
                    result = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new OAuthFailedException(OAuthFailureReason.NetworkException, "Network error occured while refreshing access token.", ex);
                }
            }

            string accessToken = null;
            string refreshToken = null;
            int expiresIn = 0;

            using (StringReader stringReader = new StringReader(result))
            {
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(await stringReader.ReadToEndAsync());
                if(obj.ContainsKey("access_token"))
                {
                    accessToken = (string)obj["access_token"];
                    refreshToken = (string)obj["refresh_token"];
                    expiresIn = (int)obj["expires_in"];
                }
            }

            if(accessToken == null)
            {
                throw new OAuthFailedException(OAuthFailureReason.PermissionDenied, "The authorization has been revoked.");
            }
            else
            {
                return new UserToken()
                {
                    UserId = token.UserId,
                    Username = token.Username,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpireTime = DateTime.Now + new TimeSpan(0, 0, expiresIn)
                };
            }
        }

        class RefreshTokenRequestParameters
        {
            [JsonProperty(PropertyName = "refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty(PropertyName = "client_id")]
            public string ClientId => ApiKey.ClientId;

            [JsonProperty(PropertyName = "client_secret")]
            public string ClientSecret => ApiKey.ClientSecret;

            [JsonProperty(PropertyName = "grant_type")]
            public string GrantType => "refresh_token";

            public RefreshTokenRequestParameters(string refreshToken)
            {
                RefreshToken = refreshToken;
            }
        }
    }

    public class OAuthFailedException : Exception
    {
        public OAuthFailureReason FailureReason { get; set; }

        public OAuthFailedException() { }

        public OAuthFailedException(OAuthFailureReason reason) : this() { FailureReason = reason; }

        public OAuthFailedException(string message) : base(message) { }

        public OAuthFailedException(OAuthFailureReason reason, string message) : this(message) { FailureReason = reason; }

        public OAuthFailedException(string message, Exception innerException) : base(message, innerException) { }

        public OAuthFailedException(OAuthFailureReason reason, string message, Exception innerException) : this(message, innerException) { FailureReason = reason; }
    }

    public enum OAuthFailureReason
    {
        PermissionDenied, NetworkException, Others
    }
}
