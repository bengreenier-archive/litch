using Litch.Lib.Authentication;
using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems.Hue
{
    public class HueLightSystem : ILightSystem
    {
        public string AccessToken
        {
            get;
            set;
        }

        public IPEndPoint Endpoint
        {
            get;
            set;
        }

        public string AppName
        {
            get;
            set;
        }

        public HueLightSystem(IPEndPoint endpoint, string accessToken = null)
        {
            this.Endpoint = endpoint;
            this.AccessToken = accessToken;
            this.AppName = Guid.NewGuid().ToString().Substring(0, 10);
        }

        public async Task<bool> AuthenticateAsync()
        {
            var token = await new HuelightAuthenticator(this.AppName).AuthenticateAsync(this.Endpoint);

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
            var client = new LocalHueClient(this.Endpoint.Address.ToString());

            client.Initialize(this.AccessToken);

            var lights = await client.GetLightsAsync();

            return lights
                .Select(l => new HueLight(l.Name, l.Id) as ILight)
                .ToList();
        }

        public Task IdentifyAsync()
        {
            throw new NotImplementedException();
        }
    }
}
