using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Color
{
    public class HexColor
    {
        public string Hex
        {
            get;
            private set;
        }

        public HexColor(string hexColor)
        {
            this.Hex = hexColor;
        }
        
        public bool Equals(HexColor other)
        {
            return this.Hex == other.Hex;
        }
    }
}
