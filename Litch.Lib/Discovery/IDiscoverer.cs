using Litch.Lib.LightSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Discovery
{
    public interface IDiscoverer
    {
        /// <summary>
        /// Discover all the light systems on the network
        /// </summary>
        /// <param name="timeoutInMs">the discover scan timeout</param>
        /// <returns>list of light systems</returns>
        Task<IList<ILightSystem>> DiscoverAsync(int timeoutInMs = 5000);
    }
}
