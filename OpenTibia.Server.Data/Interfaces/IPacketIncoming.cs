namespace OpenTibia.Server.Data.Interfaces
{
    public enum LoginOrManagementIncomingPacketType : byte
    {
        AuthenticationRequest = 0x00,
        LoginServerRequest = 0x01,
        ServerStatusRequest = 0xFF,
        PlayerLogIn = 0x14,
        PlayerLogOut = 0x15,
        NameLock = 0x17,
        Banishment = 0x19,
        Notation = 0x1A,
        ReportStatement = 0x1B,
        CharacterDeath = 0x1D,
        CreatePlayerUnsure = 0x20,
        FinishAuctions = 0x21,
        TransferHouses = 0x23,
        EvictFreeAccounts = 0x24,
        EvictDeleted = 0x25,
        EvictDelinquentGuildhouse = 0x26,
        GetHouseOwners = 0x2A,
        InsertHouseOwner = 0x27,
        UpdateHouseOwner = 0x28,
        DeleteHouseOwner = 0x29,
        BanishIpAddress = 0x1C,
        AddVip = 0x1E,
        RemoveVip = 0x1F,
        GetAuctions = 0x2B,
        StartAuction = 0x2C,
        InsertHouses = 0x2D,
        ClearIsOnline = 0x2E,
        CreatePlayerList = 0x2F,
        LogKilledCreatures = 0x30,
        LoadPlayers = 0x32,
        ExcludeFromAuctions = 0x33,
        LoadWorld = 0x35,
        HighscoreUnsure = 0xCB,
        CreateHighscores = 0xCC,
        Any = 0xFF // Do not send
    }

    public enum GameIncomingPacketType : byte
    {
        PlayerLoginRequest = 0x0A,
        PlayerLogOut = 0x14, // logout
        Ping = 0x1E, // keep alive / ping response
        AutoMove = 0x64, // move with autowalk
        WalkNorth = 0x65, // move north
        WalkEast = 0x66, // move east
        WalkSouth = 0x67, // move south
        WalkWest = 0x68, // move west
        CancelAutoWalk = 0x69, // stop-autowalk
        WalkNorteast = 0x6A,
        WalkSoutheast = 0x6B,
        WalkSouthwest = 0x6C,
        WalkNorthwest = 0x6D,
        TurnNorth = 0x6F, // turn north
        TurnEast = 0x70, // turn east
        TurnSouth = 0x71, // turn south
        TurnWest = 0x72, // turn west
        ItemThrow = 0x78, // throw item
        TradeRequest = 0x7D, // Request trade
        TradeLook = 0x7E, // Look at an item in trade
        TradeAccept = 0x7F, // Accept trade
        TradeCancel = 0x80, // Close/cancel trade
        ItemUse = 0x82, // use item
        ItemUseOn = 0x83, // use item
        ItemBattleWindow = 0x84, // battle window
        ItemRotate = 0x85,	//rotate item
        ContainerClose = 0x87, // close container
        ContainerUp = 0x88, //"up-arrow" - container
        WindowText = 0x89,
        WindowHouse = 0x8A,
        ItemLook = 0x8C, // look at item
        Speech = 0x96,  // say something
        ChannelListRequest = 0x97, // request Channels
        ChannelOpen = 0x98, // open Channel
        ChannelClose = 0x99, // close Channel
        ChannelOpenPrivate = 0x9A, // open priv
        ReportProcess = 0x9B, //process report
        ReportClose = 0x9C, //gm closes report
        ReportCancel = 0x9D, //player cancels report
        ChangeModes = 0xA0, // set attack and follow mode
        Attack = 0xA1, // attack
        Follow = 0xA2, //follow
        PartyInvite = 0xA3,
        PartyJoin = 0xA4,
        PartyRevoke = 0xA5,
        PartyPassLeadership = 0xA6,
        PartyLeave = 0xA7,
        ChannelCreatePrivate = 0xAA,
        ChannelInvite = 0xAB,
        ChannelExclude = 0xAC,
        StopAllActions = 0xBE, // cancel move
        ResendTile = 0xC9, //client request to resend the tile
        ResentContainer = 0xCA, //client request to resend the container (happens when you store more than container maxsize)
        OutfitChangeRequest = 0xD2, // request outfit
        OutfitChangeCompleted = 0xD3, // set outfit
        AddVip = 0xDC,
        RemoveVip = 0xDD,
        ReportBug = 0xE6,
        ReportViolation = 0xE7,
        ReportDebugAssertion = 0xE8,
        Any = 0xFF // Do not send
    }

    public interface IPacketIncoming
    {
        NetworkMessage NetworkMessage { get; }

        void Parse(NetworkMessage message);
    }
}