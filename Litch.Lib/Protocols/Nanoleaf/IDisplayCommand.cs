namespace Litch.Lib.Protocols.Nanoleaf
{
    public interface IDisplayCommand
    {
        string command { get; }
        string colorType { get; }
        int duration { get; set; }
        string animType { get; set; }
        Palette[] palette { get; set; }
    }
}
