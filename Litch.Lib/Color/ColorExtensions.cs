using Q42.HueApi.ColorConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Color
{
    internal static class ColorExtensions
    {
        public static RGBColor ToRGB(this HexColor color)
        {
            return new RGBColor(color.Hex);
        }
    }
}
