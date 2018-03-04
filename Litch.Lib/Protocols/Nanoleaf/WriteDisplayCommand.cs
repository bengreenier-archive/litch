namespace Litch.Lib.Protocols.Nanoleaf
{
    public class WriteDisplayCommand
    {
        public IDisplayCommand Write { get; set; }

        public WriteDisplayCommand(IDisplayCommand displayCommand)
        {
            this.Write = displayCommand;
        }
    }
}
