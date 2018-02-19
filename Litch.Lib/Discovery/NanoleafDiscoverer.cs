using Litch.Lib.LightSystems;
using Litch.Lib.LightSystems.Nanoleaf;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tmds.MDns;

namespace Litch.Lib.Discovery
{
    public class NanoleafDiscoverer : IDiscoverer
    {
        /// <summary>
        /// The service name nanoleaf registers under
        /// </summary>
        /// <remarks>
        /// See http://forum.nanoleaf.me/docs/openapi#_gf9l5guxt8r0
        /// </remarks>
        public static readonly string ServiceName = "_nanoleafapi._tcp";

        /// <summary>
        /// Discover all the nanoleaf systems on the network
        /// </summary>
        /// <param name="timeoutInMs">the discover scan timeout</param>
        /// <returns>list of light systems</returns>
        public async Task<IList<ILightSystem>> DiscoverAsync(int timeoutInMs = 5000)
        {
            var systems = new List<ILightSystem>();
            var systemsLock = new object();
            var discover = new ServiceBrowser();

            discover.ServiceAdded += (object sender, ServiceAnnouncementEventArgs e) =>
            {
                lock (systemsLock)
                {
                    systems.AddRange(e.Announcement.Addresses
                        .Select(addr => new IPEndPoint(addr, e.Announcement.Port))
                        .Select(endpoint => new NanoleafLightSystem(endpoint)));
                }
            };

            discover.StartBrowse(ServiceName);

            await Task.Delay(timeoutInMs);

            discover.StopBrowse();

            return systems;
        }
    }
}
