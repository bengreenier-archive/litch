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
    public class NanoleafLightSystem : ILightSystem
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
            var parsedData = JObject.Parse(rawData);
            var mainLightName = parsedData["name"].ToString();

            // see http://forum.nanoleaf.me/docs/openapi for more info
            return parsedData["panelLayout"]["layout"]["positionData"]
                .ToArray()
                .Select(t => new NanoleafLight(mainLightName, t["panelId"].ToString()) as ILight)
                .ToList();
        }
    }
}
