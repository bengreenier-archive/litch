using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace Litch.Lib.Protocols.Twitch
{
    /// <summary>
    /// A protocol implementation that can interface with twitch chat for lighting controls
    /// </summary>
    public class TwitchLightProtocol
    {
        /// <summary>
        /// The valid colors
        /// </summary>
        /// <remarks>
        /// these must be lowercase
        /// </remarks>
        public static readonly string[] ValidColors = new string[] { "red", "orange", "yellow", "green", "aqua", "blue", "violet", "fuchsia" };

        /// <summary>
        /// The help text
        /// </summary>
        public static readonly string HelpText = $"color1 [colorN] - sets the colors of the lights. Max of 5 colors. Colors can be any of the following: {string.Join(", ", ValidColors)}. See https://bot.docs/lightware for more info.";

        /// <summary>
        /// Internal twitch client
        /// </summary>
        private TwitchClient client;

        /// <summary>
        /// The backing twitch client
        /// </summary>
        /// <remarks>
        /// Odds are if you are using this then we should probably add a feature to this API for you
        /// </remarks>
        public TwitchClient RawClient => client;

        /// <summary>
        /// The command we listen for in chat
        /// </summary>
        public string Command
        {
            get;
            set;
        }

        /// <summary>
        /// Emitted when valid color data is received
        /// </summary>
        public event EventHandler<string[]> OnColorData;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="username">twitch username</param>
        /// <param name="oauth">twitch oauth token</param>
        /// <param name="channel">twitch channel</param>
        /// <param name="command">activation command</param>
        public TwitchLightProtocol(string username, string oauth, string channel, string command = "lights")
        {
            this.Command = command;

            this.client = new TwitchClient(new ConnectionCredentials(username, oauth), channel);

            this.client.OnChatCommandReceived += OnCommandReceived;
        }

        /// <summary>
        /// Connect to twitch
        /// </summary>
        public void Connect()
        {
            client.Connect();
        }

        /// <summary>
        /// Disconnect from twitch
        /// </summary>
        public void Disconnect()
        {
            client.Disconnect();
        }

        /// <summary>
        /// Handler for when commands come in
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the arguments</param>
        private void OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            if (string.Equals(e.Command.CommandText, Command, StringComparison.OrdinalIgnoreCase))
            {
                var args = e.Command.ArgumentsAsList;

                // massive condition that indicates the following (failure commands):
                // no args
                // help arg
                // invalid colors
                // too many colors
                if (args.Count == 0 ||
                    (args.Count == 1 && string.Equals(args[0], "help", StringComparison.OrdinalIgnoreCase)) ||
                    args.Any(a => !ValidColors.Contains(a.ToLower())) ||
                    args.Count > ValidColors.Length)
                {
                    // send back the help text
                    client.SendMessage($"try @{e.Command.ChatMessage.Username} !{Command} {HelpText}");
                }
                // otherwise, we good
                else
                {
                    // invoke the event that colors were given
                    this?.OnColorData(this, args.ToArray());
                }
            }
        }
    }
}
