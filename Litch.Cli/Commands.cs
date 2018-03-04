using Litch.Lib.Color;
using Litch.Lib.LightSystems;
using Litch.Lib.LightSystems.Hue;
using Litch.Lib.LightSystems.Nanoleaf;
using Litch.Lib.Protocols.ColourLovers;
using Litch.Lib.Protocols.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Litch.Cli
{
    public class Commands
    {
        public static void Run(
            string twitchUser,
            string twitchAuth,
            string twitchChannel,
            string[] nanoAddr = null,
            string[] nanoToken = null,
            string[] hueAddr = null,
            string[] hueApp = null,
            string[] hueToken = null,
            string hueSelector = null)
        {
            if (nanoToken == null &&
                hueToken == null)
            {
                Console.WriteLine($"Error. At least one {nameof(nanoToken)} or {nameof(hueToken)} must be specified.");
                return;
            }

            if (nanoAddr == null &&
                hueAddr == null)
            {
                Console.WriteLine($"Error. At least one {nameof(nanoAddr)} or {nameof(hueAddr)} must be specified.");
                return;
            }

            var systems = new List<ILightSystem>();

            if (nanoAddr != null &&
                nanoToken != null &&
                nanoAddr.Length == nanoToken.Length)
            {
                // note: they say not to hardcode the port but we do that here :(
                systems.AddRange(nanoAddr
                    .Zip(nanoToken, (addr, token) => new Tuple<string, string>(addr, token))
                    .Select(t => new NanoleafLightSystem(new IPEndPoint(IPAddress.Parse(t.Item1), 16021), t.Item2)));
            }

            if (hueAddr != null &&
                hueToken != null &&
                hueApp != null &&
                hueAddr.Length == hueToken.Length &&
                hueToken.Length == hueApp.Length)
            {
                // note: we use port zero bc the hue sdk doesn't care about our given port
                systems.AddRange(hueApp
                    .Zip(hueToken, (app, token) => new Tuple<string, string>(app, token))
                    .Zip(hueAddr, (t, addr) => new Tuple<string, string, string>(addr, t.Item1, t.Item2))
                    .Select(t => {
                        var sys = new HueLightSystem(new IPEndPoint(IPAddress.Parse(t.Item1), 0), t.Item3)
                        {
                            AppName = t.Item2
                        };
                        return sys;
                    }));
            }

            var colorApi = new ColourLoversProtocol();
            var chat = new TwitchLightProtocol(twitchUser, twitchAuth, twitchChannel);

            chat.OnError += (object sender, Exception ex) =>
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            };

            chat.OnColorData += async (object sender, string[] colorData) =>
            {
                var palettes = await colorApi.GetTopPaletteAsync(hueOption: colorData);
                
                // for now we always use the first one
                var palette = palettes[0];

                Console.WriteLine($"Setting Palette {palette.BadgeUrl}");

                var pendingSystemPaints = new List<Task>();
                
                foreach (var system in systems)
                {
                    var lights = await system.GetLightsAsync();

                    // apply hueSelector if needed
                    if (system is HueLightSystem &&
                        hueSelector != null)
                    {
                        // TODO(bengreenier): if we're really ignoring case here we should document that...
                        var re = new Regex(hueSelector, RegexOptions.IgnoreCase);

                        lights = lights.Where(l => re.IsMatch(l.Name));
                    }

                    var paintRequests = new List<PaintRequest>();

                    // formulate the paints for the system
                    for (var i = 0; i < lights.Count(); i++)
                    {
                        var lightColor = palette.Colors[i % palette.Colors.Length];
                        paintRequests.Add(new PaintRequest(new HexColor(lightColor), lights.ElementAt(i)));
                    }

                    // begin the work to paint the system, tracking that task for future resolution
                    pendingSystemPaints.Add(system.PaintAsync(paintRequests));
                }

                // finally, we wait for all paints to resolve (complete) across all systems
                await Task.WhenAll(pendingSystemPaints);
            };

            chat.Connect();
            Console.WriteLine("Connecting to Twitch...");

            Console.ReadKey();

            chat.Disconnect();
        }
    }
}
