using System;

namespace OpenTibia.Communications
{
    public interface IOpenTibiaListener
    {
        int Port { get; }

        void OnAccept(IAsyncResult ar);

        void BeginListening();

        void EndListening();
    }
}