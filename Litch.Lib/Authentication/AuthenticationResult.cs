using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Authentication
{
    public struct AuthenticationResult
    {
        public enum Result
        {
            Success = 0,
            UnauthorizedFailure,
            GenericFailure
        }

        public Result Status;
        public string Data;
    }
}
