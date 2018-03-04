using Colorspace;
using Q42.HueApi.ColorConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems
{
    public interface ILightSystem
    {
        string AccessToken { get; set; }
        IPEndPoint Endpoint { get; }

        Task<bool> AuthenticateAsync();
        Task IdentifyAsync();
        Task<IEnumerable<ILight>> GetLightsAsync();
        Task PaintAsync(IEnumerable<PaintRequest> paintRequests);
    }
}
