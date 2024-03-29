﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.LightSystems.Nanoleaf
{
    public class NanoleafLight : ILight
    {
        public NanoleafLight(string name, string id)
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
