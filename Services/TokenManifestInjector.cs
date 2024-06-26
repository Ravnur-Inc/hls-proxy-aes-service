using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hls.Proxy.Aes.Services
{
    public class TokenManifestInjector : ITokenManifestInjector
    {
        public string InjetTokenToManifestChunks(string playbackUrl, string armoredAuthToken, string content)
        {
            const string fragmentsRegex = @"(seg-[-\w]+?\.ts)";
            const string urlRegex =
                @"("")(/vod/keyservice/\w{8}-[\w-]{27}\+[^/""]+)("")";
            var baseUrl = playbackUrl.Substring(0,
                              playbackUrl.IndexOf(".ism", StringComparison.OrdinalIgnoreCase)) +
                          ".ism";
            var hostNameWithSchema = new Uri(playbackUrl).GetLeftPart(UriPartial.Authority);
            var newContent = Regex.Replace(content, urlRegex,
                string.Format(CultureInfo.InvariantCulture, "$1{0}$2?token={1}$3", hostNameWithSchema, armoredAuthToken));
            var match = Regex.Match(playbackUrl, fragmentsRegex);
            newContent = Regex.Replace(newContent, fragmentsRegex,
                    m => string.Format(CultureInfo.InvariantCulture,
                        baseUrl + "/" + m.Value));

            return newContent;
        }
    }
}
