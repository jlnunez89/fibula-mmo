namespace OpenTibia.Security
{
    internal interface IDoSDefender
    {
        void AddToBlocked(string addessStr);

        bool IsBlockedAddress(string addressStr);
    }
}