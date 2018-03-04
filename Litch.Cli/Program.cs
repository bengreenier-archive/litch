using Litch.Lib.Authentication;
using Litch.Lib.Discovery;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Litch.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncEntryPoint(args).Wait();
        }

        static async Task AsyncEntryPoint(string[] args)
        {
            // see if we have an AT as a cli arg
            string accessToken = null;
            if (args.Length > 0)
            {
                accessToken = args[0];
            }

            // discover nanos, and select the first one
            var disc = new NanoleafDiscoverer();
            var system = (await disc.DiscoverAsync())[0];

            // if we had an at, we use it
            if (!string.IsNullOrEmpty(accessToken))
            {
                system.AccessToken = accessToken;
            }
            // otherwise we try to aquire a new one
            else
            {
                // (you must have hit the button already)
                // if you haven't hit the button, this will be false..indicating auth failed
                if (await system.AuthenticateAsync())
                {
                    Console.WriteLine(system.AccessToken);
                }
                else
                {
                    Console.WriteLine("Unable to auth, probably hit the button");
                    return;
                }
            }

            // if we're still going, enumerate lights
            var lights = await system.GetLightsAsync();

            // print light names
            // note: for aurora, there is currently just one light
            // since their api doesn't make it easy (possible?) to individually address lights
            // see http://forum.nanoleaf.me/docs/openapi for more info
            foreach (var light in lights)
            {
                Console.Write(light.Name);
            }
        }
    }
}
