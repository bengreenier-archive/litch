using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Authentication
{
    public interface IAuthenticator
    {
        Task<AuthenticationResult> AuthenticateAsync(IPEndPoint endpoint);
    }
}
