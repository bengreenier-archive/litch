using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Protocols.ColourLovers
{
    /// <summary>
    /// Defines the protocol for communicating with ColourLovers
    /// </summary>
    public class ColourLoversProtocol
    {
        /// <summary>
        /// Represents a ColourLovers Palette
        /// </summary>
        public struct Palette
        {
            /// <summary>
            /// The Pallete id
            /// </summary>
            public string Id;

            /// <summary>
            /// The Pallete title
            /// </summary>
            public string Title;

            /// <summary>
            /// The username of the user who created the Pallete
            /// </summary>
            public string Username;

            /// <summary>
            /// The Pallete id
            /// </summary>
            public int NumViews;

            /// <summary>
            /// The number of votes for the Pallete
            /// </summary>
            public int NumVotes;

            /// <summary>
            /// The number of comments for the Pallete
            /// </summary>
            public int NumComments;

            /// <summary>
            /// The number of hearts for the Pallete
            /// </summary>
            public float NumHearts;

            /// <summary>
            /// The Pallete rank
            /// </summary>
            public int Rank;

            /// <summary>
            /// The Date of creation for the Pallete
            /// </summary>
            public DateTime DateCreated;

            /// <summary>
            /// The Pallete colors, as hex codes
            /// </summary>
            public string[] Colors;

            /// <summary>
            /// The Pallete color widths
            /// </summary>
            public string ColorWidths;

            /// <summary>
            /// The Pallete description
            /// </summary>
            public string Description;

            /// <summary>
            /// The Pallete url
            /// </summary>
            public Uri Url;

            /// <summary>
            /// The Pallete image preview url
            /// </summary>
            public Uri ImageUrl;

            /// <summary>
            /// The Pallete badge url
            /// </summary>
            public Uri BadgeUrl;

            /// <summary>
            /// The Pallete api url
            /// </summary>
            public Uri ApiUrl;
        }

        /// <summary>
        /// A collection of <see cref="Palette"/>
        /// </summary>
        public class PaletteCollection : List<Palette>
        {
        }

        /// <summary>
        /// Internal helper to issue requests
        /// </summary>
        /// <param name="uri">the url to hit</param>
        /// <param name="method">the http method</param>
        /// <param name="body">the http body</param>
        /// <returns></returns>
        private async Task<HttpWebResponse> IssueRequestAsync(string uri, string method = "GET", string body = null)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = method;

            if (!string.IsNullOrEmpty(body))
            {
                using (var sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(body);
                }
            }

            return (HttpWebResponse)await req.GetResponseAsync();
        }

        /// <summary>
        /// Faciliates communication with http://www.colourlovers.com/api/palettes/top?format=json
        /// </summary>
        /// <param name="hueOption">hue option values</param>
        /// <param name="numResults">the number of results</param>
        /// <returns>a collection of Palettes</returns>
        public async Task<PaletteCollection> GetTopPaletteAsync(string[] hueOption = null, int numResults = 5)
        {
            if (hueOption == null)
            {
                hueOption = new string[] { };
            }

            // the {"http:"} part is just to make vs intelisense highlighting work better
            var res = await this.IssueRequestAsync($"{"http:"}//www.colourlovers.com/api/palettes/top?{nameof(hueOption)}={string.Join(",", hueOption)}&{nameof(numResults)}={numResults}&format=json");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string rawData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                rawData = sr.ReadToEnd();
            }

            res.Close();

            return JsonConvert.DeserializeObject<PaletteCollection>(rawData);
        }

        /// <summary>
        /// Faciliates communication with http://www.colourlovers.com/api/palettes/random?format=json
        /// </summary>
        /// <returns>a Palette</returns>
        public async Task<Palette> GetRandomPaletteAsync()
        {
            // the {"http:"} part is just to make vs intelisense highlighting work better
            var res = await this.IssueRequestAsync($"{"http:"}//www.colourlovers.com/api/palettes/random?format=json");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string rawData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                rawData = sr.ReadToEnd();
            }

            res.Close();

            // this is...kind of annoying. The API returns an array with only ever one element
            // we save the caller some time here by parsing as a collection but then only returning
            // the one element (so they don't need to work with the single element array)
            return JsonConvert.DeserializeObject<PaletteCollection>(rawData)[0];
        }
    }
}
