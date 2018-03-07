namespace OpenTibia.Communications
{
    public class LoginListener : OpenTibiaListener, IOpenTibiaListener
    {
        public static int ListenPort = 7171;
        //public static OpenTibiaProtocolType TypeOfProtocol = OpenTibiaProtocolType.LoginProtocol;

        int IOpenTibiaListener.Port => ListenPort;

        //OpenTibiaProtocolType ProtocolType
        //{
        //    get
        //    {
        //        return TypeOfProtocol;
        //    }
        //}
        
        public LoginListener(IHandlerFactory handlerFactory)
            : base(ListenPort, ProtocolFactory.CreateForType(OpenTibiaProtocolType.LoginProtocol, handlerFactory))
        {

        }
    }
}
