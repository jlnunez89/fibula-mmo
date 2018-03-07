namespace OpenTibia.Communications
{
    public class GameListener : OpenTibiaListener, IOpenTibiaListener
    {
        public static int ListenPort = 7172;
        public static OpenTibiaProtocolType TypeOfProtocol = OpenTibiaProtocolType.GameProtocol;

        int IOpenTibiaListener.Port => ListenPort;

        OpenTibiaProtocolType ProtocolType => TypeOfProtocol;

        public GameListener(IHandlerFactory hanlderFactory)
            : base(ListenPort, ProtocolFactory.CreateForType(OpenTibiaProtocolType.GameProtocol, hanlderFactory))
        {

        }
    }
}
