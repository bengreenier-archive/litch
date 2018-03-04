namespace Litch.Lib.Protocols.Nanoleaf
{
    public class DisplayTemp : IDisplayCommand
    {
        public string command => "displayTemp";
        public string colorType => "HSB";
        public int duration { get; set; }
        public string animType { get; set; }
        public Palette[] palette { get; set; }
    }
}
