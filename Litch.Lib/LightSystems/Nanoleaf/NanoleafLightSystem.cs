using Litch.Lib.Authentication;
using Litch.Lib.Protocols.Nanoleaf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems.Nanoleaf
{
    class NanoleafLightSystem : ILightSystem
    {
        public NanoleafLightSystem(IPEndPoint endpoint, string accessToken = null)
        {
            this.Endpoint = endpoint;
            this.AccessToken = accessToken;
        }
        
        public IPEndPoint Endpoint
        {
            get;
            private set;
        }

        public string AccessToken
        {
            get;
            set;
        }

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

            var mainLightName = JObject.Parse(rawData)["name"].ToString();

            // nanoleaf lights aren't individually addressable so we just have one
            // see http://forum.nanoleaf.me/docs/openapi for more info
            return new ILight[] { new NanoleafLight(mainLightName)};
        }
    }
}
