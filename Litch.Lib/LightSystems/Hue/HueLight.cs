using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems.Hue
{
    public class HueLight : ILight
    {
        public HueLight(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Id
        {
            get;
            private set;
        }
    }
}
