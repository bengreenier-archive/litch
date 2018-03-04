using Litch.Lib.Authentication;
using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Colorspace;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Litch.Lib.Color;
using Q42.HueApi.Models.Groups;

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

        public async Task PaintAsync(IEnumerable<PaintRequest> paintRequests)
        {
            var client = new LocalHueClient(this.Endpoint.Address.ToString());

            client.Initialize(this.AccessToken);

            var colorBuckets = paintRequests
                .GroupBy(r => r.Color)
                .Select(g => g.ToList())
                .ToList();

            var pendingCommands = new List<Task<HueResults>>();

            foreach (var colorBucket in colorBuckets)
            {
                var color = colorBucket[0].Color;
                var lightIds = colorBucket.Select(b => b.Light.Id);

                var cmd = new LightCommand().TurnOn().SetColor(color.ToRGB());

                pendingCommands.Add(client.SendCommandAsync(cmd, lightIds));
            }

            var commandResults = await Task.WhenAll(pendingCommands);
            var errorResults = commandResults
                .Where(r => r.HasErrors())
                .SelectMany(r => r
                    .Errors
                    .Select(e => e.Error));

            if (errorResults.Count() > 0)
            {
                throw new PaintRequestException(new AggregateException(errorResults
                    .Select(e => new Exception(e.Description))));
            }
        }
    }
}
