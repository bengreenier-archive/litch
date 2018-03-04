using Colorspace;
using Litch.Lib.Authentication;
using Litch.Lib.Protocols.Nanoleaf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems.Nanoleaf
{
    class NanoleafLightSystem : ILightSystem
    {
        private const double RGBModifier = 255;

        public NanoleafLightSystem(IPEndPoint endpoint, string accessToken = null)
        {
            this.Endpoint = endpoint;
            this.AccessToken = accessToken;
            this.NanoleafProtocol = new NanoleafProtocol(endpoint);
        }

        public IPEndPoint Endpoint { get; }

        public string AccessToken { get; set; }

        private NanoleafProtocol NanoleafProtocol { get; }

        public async Task<bool> AuthenticateAsync()
        {
            var token = await new NanoleafAuthenticator().AuthenticateAsync(this.Endpoint);


            if (token.Status == AuthenticationResult.Result.Success)
            {
                this.AccessToken = token.Data;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<ILight>> GetLightsAsync()
        {
            var rawData = await new NanoleafProtocol(Endpoint).GetRawDataAsync(AccessToken);
            var parsedData = JObject.Parse(rawData);
            var mainLightName = parsedData["name"].ToString();

            // see http://forum.nanoleaf.me/docs/openapi for more info
            return parsedData["panelLayout"]["layout"]["positionData"]
                .ToArray()
                .Select(t => new NanoleafLight(mainLightName, t["panelId"].ToString()) as ILight)
                .ToList();
        }

        public Task IdentifyAsync()
        {
            // currently hardcoded for testing, but just to verify the RGB -> HSL converter works
            // properly we'll use it to identify the lights rather than hardcoding the HSL value directly.
            var rgbColor = ColorTranslator.FromHtml("#a7f442");

            var convertedRgbValue = new ColorRGB(rgbColor.R / RGBModifier, rgbColor.G / RGBModifier, rgbColor.B / RGBModifier);
            var hslConversion = new ColorHSV(convertedRgbValue);

            var displayTemp = new DisplayTemp
            {
                duration = 3,
                animType = "random",
                palette = new[]
                {
                    new Palette(hslConversion.H, hslConversion.S, hslConversion.V)
                }
            };

            return this.NanoleafProtocol.WriteDisplayCommandAsync(this.AccessToken, displayTemp);
        }
    }
}
