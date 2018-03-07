namespace OpenTibia.Communications
{
    public interface IHandlerFactory
    {
        IIncomingPacketHandler CreateIncommingForType(byte packeType);
    }
}