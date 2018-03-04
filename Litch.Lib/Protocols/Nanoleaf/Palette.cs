namespace Litch.Lib.Protocols.Nanoleaf
{
    public class Palette
    {
        public double hue { get; set; }
        public double saturation { get; set; }
        public double brightness { get; set; }

        public Palette(double hue, double saturation, double brightness)
        {
            this.hue = hue;
            this.saturation = saturation;
            this.brightness = brightness;
        }
    }
}
