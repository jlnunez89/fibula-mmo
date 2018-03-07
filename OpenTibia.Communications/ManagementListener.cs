namespace OpenTibia.Communications
{
    public class ManagementListener : OpenTibiaListener, IOpenTibiaListener
    {
        public static int ListenPort = 17778;

        int IOpenTibiaListener.Port => ListenPort;

        public ManagementListener(IHandlerFactory handlerFactory)
            : base(ListenPort, ProtocolFactory.CreateForType(OpenTibiaProtocolType.ManagementProtocol, handlerFactory))
        {

        }
    }
}
