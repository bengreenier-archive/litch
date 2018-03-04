using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems
{
    public class PaintRequestException : Exception
    {
        public PaintRequestException(Exception ex) : base("Failed to complete paint request", ex)
        {
        }
    }
}
