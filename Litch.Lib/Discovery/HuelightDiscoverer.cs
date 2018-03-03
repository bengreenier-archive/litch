using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litch.Lib.LightSystems;
using Q42.HueApi;
using Litch.Lib.LightSystems.Hue;
using System.Net;

namespace Litch.Lib.Discovery
{
    public class HuelightDiscoverer : IDiscoverer
    {
        public async Task<IList<ILightSystem>> DiscoverAsync(int timeoutInMs = 5000)
        {
            var locator = new HttpBridgeLocator();

            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromMilliseconds(timeoutInMs));

            return bridges.Select(b => new HueLightSystem(new IPEndPoint(IPAddress.Parse(b.IpAddress), 0)) as ILightSystem).ToList();
        }
    }
}
