using Litch.Lib.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems
{
    public class PaintRequest
    {
        public HexColor Color
        {
            get;
            private set;
        }

        public ILight Light
        {
            get;
            private set;
        }

        public PaintRequest(HexColor color, ILight light)
        {
            this.Color = color;
            this.Light = light;
        }
    }
}
