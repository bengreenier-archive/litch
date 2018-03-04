using Litch.Lib.Authentication;
using Litch.Lib.Discovery;
using Litch.Lib.LightSystems.Hue;
using Litch.Lib.Protocols.ColourLovers;
using Litch.Lib.Protocols.Twitch;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Litch.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO(bengreenier): fix this cli tool parsing
            // this is gross bc the cli tool didn't work as expected so i just did it myself for now
            Commands.Run(
                CommandLine.Arguments["twitchUser"] as string,
                CommandLine.Arguments["twitchAuth"] as string,
                CommandLine.Arguments["twitchChannel"] as string,
                CommandLine.Arguments.ContainsKey("nanoAddr") ? 
                    CommandLine.Arguments["nanoAddr"] is string ?
                        new string[] { CommandLine.Arguments["nanoAddr"] as string } :
                        CommandLine.Arguments["nanoAddr"] as string[] : null,
                CommandLine.Arguments.ContainsKey("nanoToken") ?
                    CommandLine.Arguments["nanoToken"] is string ?
                        new string[] { CommandLine.Arguments["nanoToken"] as string } :
                        CommandLine.Arguments["nanoToken"] as string[] : null,
                CommandLine.Arguments.ContainsKey("hueAddr") ?
                    CommandLine.Arguments["hueAddr"] is string ?
                            new string[] { CommandLine.Arguments["hueAddr"] as string } :
                            CommandLine.Arguments["hueAddr"] as string[] : null,
                CommandLine.Arguments.ContainsKey("hueApp") ?
                    CommandLine.Arguments["hueApp"] is string ?
                            new string[] { CommandLine.Arguments["hueApp"] as string } :
                            CommandLine.Arguments["hueApp"] as string[] : null,
                CommandLine.Arguments.ContainsKey("hueToken") ?
                    CommandLine.Arguments["hueToken"] is string ?
                            new string[] { CommandLine.Arguments["hueToken"] as string } :
                            CommandLine.Arguments["hueToken"] as string[] : null,
                CommandLine.Arguments.ContainsKey("hueSelector") ?
                    CommandLine.Arguments["hueSelector"] as string : null);
        }
    }
}
