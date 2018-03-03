using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Q42.HueApi;

namespace Litch.Lib.Authentication
{
    public class HuelightAuthenticator : IAuthenticator
    {
        public static readonly string DeviceName = "LitchAPI";

        public string AppName
        {
            get;
            private set;
        }

        public HuelightAuthenticator(string appName)
        {
            this.AppName = appName;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(IPEndPoint endpoint)
        {
            var client = new LocalHueClient(endpoint.Address.ToString());
            try
            {
                var registration = await client.RegisterAsync(this.AppName, DeviceName);

                return new AuthenticationResult()
                {
                    Status = AuthenticationResult.Result.Success,
                    Data = registration
                };
            }
            catch (Exception)
            {
                return new AuthenticationResult()
                {
                    Status = AuthenticationResult.Result.GenericFailure
                };
            }
        }
    }
}
