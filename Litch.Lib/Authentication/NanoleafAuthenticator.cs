using Litch.Lib.Protocols.Nanoleaf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Authentication
{
    public class NanoleafAuthenticator : IAuthenticator
    {
        public async Task<AuthenticationResult> AuthenticateAsync(IPEndPoint endpoint)
        {
            AuthenticationResult result = new AuthenticationResult() { Status = AuthenticationResult.Result.GenericFailure };

            try
            {
                result.Data = await new NanoleafProtocol(endpoint).CreateUserAsync();
            }
            catch (Exception ex)
            {
                if (ex is WebException &&
                    (ex as WebException).Response is HttpWebResponse &&
                    ((ex as WebException).Response as HttpWebResponse).StatusCode == HttpStatusCode.Forbidden)
                {
                    result.Status = AuthenticationResult.Result.UnauthorizedFailure;
                }
                else
                {
                    // TODO(bengreenier): we should not just blindy swallow this ex
                    result.Status = AuthenticationResult.Result.GenericFailure;
                }
            }

            return result;
        }
    }
}
