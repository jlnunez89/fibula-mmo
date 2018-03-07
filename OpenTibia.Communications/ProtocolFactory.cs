namespace OpenTibia.Communications
{
    public static class ProtocolFactory
    {
        public static IProtocol CreateForType(OpenTibiaProtocolType type, IHandlerFactory handlerFactory)
        {
            if (type == OpenTibiaProtocolType.LoginProtocol)
            {
                return new LoginProtocol(handlerFactory);
            }

            if (type == OpenTibiaProtocolType.GameProtocol)
            {
                return new GameProtocol(handlerFactory);
            }

            return new ManagementProtocol(handlerFactory);
        }
    }
}
