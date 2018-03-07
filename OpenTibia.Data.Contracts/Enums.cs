namespace OpenTibia.Data.Contracts
{
    public enum LoginFailureReason : byte
    {
        None = 0x00,
        WrongClientVersion = 0x01,
        //0x02 = doesnt exist
        //0x03 = doesnt live on this world
        //0x04 = Private
        //0x05 = ???
        AccountOrPasswordIncorrect = 0x06,
        //0x07 = Account disabled for 5 minutes
        //0x08 = 0x06
        //0x09 = IP Address blocked for 30 minutes
        Bannished = 0x0A,
        //0x0B = Banished for name
        //0x0C = IP Banned
        AnotherCharacterIsLoggedIn = 0x0D,
        //0x0E = May only login with GM acc
        InternalServerError = 0x0F
    }

    public enum CooldownType
    {
        Move,
        Action,
        Combat,
        Talk
    }

    public enum BloodType : byte
    {
        Blood,
        Fire,
        Slime,
        Bones
    }

    public enum SkillType : byte
    {
        Level,
        Magic,
        Fist,
        Axe,
        Club,
        Sword,
        Shield,
        Ranged,
        Fishing
        //// monster skills
        //HitPoints,
        //GoStrength,
        //CarryStrength,
        //FistFighting
    }

    public enum Direction : byte
    {
        North,
        East,
        South,
        West,
        NorthEast,
        SouthEast,
        NorthWest,
        SouthWest
    }

    public enum CreatureProperty : byte
    {
        Hitpoints,
        Mana
    }

    public enum CreatureFlag : uint
    {
        None = 0,
        KickBoxes = 1 >> 1,
        KickCreatures = 1 >> 2,
        SeeInvisible = 1 >> 3,
        Unpushable = 1 >> 4,
        DistanceFighting = 1 >> 5,
        NoSummon = 1 >> 6,
        NoIllusion = 1 >> 7,
        NoConvince = 1 >> 8,
        NoBurning = 1 >> 9,
        NoPoison = 1 >> 10,
        NoEnergy = 1 >> 11,
        NoParalyze = 1 >> 12,
        NoHit = 1 >> 13,
        NoLifeDrain = 1 >> 14
    }

    public enum ItemFlag : byte
    {
        Container,
        Take,
        Unpass,
        Bank,
        Unmove,
        Unthrow,
        Unlay,
        CollisionEvent,
        Avoid,
        Expire,
        LiquidSource,
        SeparationEvent,
        Disguise,
        UseEvent,
        ForceUse,
        RopeSpot,
        Bottom,
        Light,
        Height,
        Clip,
        Top,
        HookEast,
        HookSouth,
        KeyDoor,
        ChangeUse,
        NameDoor,
        Text,
        QuestDoor,
        LevelDoor,
        Cumulative,
        Throw,
        Hang,
        Rotate,
        Destroy,
        MagicField,
        Special,
        Information,
        Chest,
        Bed,
        MultiUse,
        LiquidContainer,
        Write,
        WriteOnce,
        Clothes,
        LiquidPool,
        ExpireStop,
        MovementEvent,
        Key,
        Shield,
        Protection,
        WearOut,
        ShowDetail,
        Armor,
        RestrictLevel,
        RestrictProfession,
        Wand,
        SkillBoost,
        DistUse,
        Rune,
        Weapon,
        Food,
        Bow,
        Ammo,
        Corpse,
        TeleportAbsolute,
        TeleportRelative
    }

    public enum ItemAttribute : byte
    {
        Capacity,
        Weight,
        Waypoints,
        AvoidDamageTypes,
        ExpireTarget,
        TotalExpireTime,
        SourceLiquidType,
        DisguiseTarget,
        Brightness,
        LightColor,
        Elevation,
        KeydoorTarget,
        ChangeTarget,
        NamedoorTarget,
        FontSize,
        QuestdoorTarget,
        LeveldoorTarget,
        ThrowRange,
        ThrowAttackValue,
        ThrowDefendValue,
        ThrowMissile,
        ThrowSpecialEffect,
        ThrowEffectStrength,
        ThrowFragility,
        RotateTarget,
        DestroyTarget,
        Meaning,
        InformationType,
        MaxLength,
        MaxLengthOnce,
        BodyPosition,
        ShieldDefendValue,
        ProtectionDamageTypes,
        DamageReduction,
        WearoutTarget,
        TotalUses,
        ArmorValue,
        MinimumLevel,
        Professions,
        WandRange,
        WandManaConsumption,
        WandAttackStrength,
        WandAttackVariation,
        WandDamageType,
        WandMissile,
        SkillNumber,
        SkillModification,
        WeaponType,
        WeaponAttackValue,
        WeaponDefendValue,
        Nutrition,
        BowRange,
        BowAmmoType,
        AmmoType,
        AmmoAttackValue,
        AmmoMissile,
        AmmoSpecialEffect,
        AmmoEffectStrength,
        CorpseType,
        AbsTeleportEffect,
        RelTeleportEffect,
        RelTeleportDisplacement,
        Amount,
        ContainerLiquidType,
        PoolLiquidType,
        String,
        SavedExpireTime
    }

    public enum TileFlag : byte
    {
        None = 0,
        Refresh = 1 << 0,
        ProtectionZone = 1 << 1,
        NoLogout = 1 << 2
    }

    public enum WorldState : byte
    {
        Creating,   // Represents a world in loading state
        Open,     // The normal, open public state
        Closed      // Testing or closed beta state
    }

    public enum WorldType : byte
    {
        Safe,       // No PvP
        Normal,     // Normal Pvp & punishment
        Hardcore    // PvP is encouraged
    }

    public enum EffectT : byte
    {
        XBlood = 0x01,
        RingsBlue = 0x02,
        Puff = 0x03,
        SparkYellow = 0x04,
        DamageExplosion = 0x05,
        DamageMagicMissile = 0x06,
        AreaFlame = 0x07,
        RingsYellow = 0x08,
        RingsGreen = 0x09,
        XGray = 0x0A,
        BubbleBlue = 0x0B,
        DamageEnergy = 0x0C,
        GlitterBlue = 0x0D,
        GlitterRed = 0x0E,
        GlitterGreen = 0x0F,
        Flame = 0x10,
        XPoison = 0x11,
        BubbleBlack = 0x12,
        SoundGreen = 0x13,
        SoundRed = 0x14,
        DamageVenomMissile = 0x15,
        SoundYellow = 0x16,
        SoundPurple = 0x17,
        SoundBlue = 0x18,
        SoundWhite = 0x19,
        None = 0xFF //Don't send to client.
    }

    public enum LiquidSourceType : byte
    {
        Water = 1,
        Wine = 2,
        Beer = 3,
        Mud = 4,
        Blood = 5,
        Slime = 6,
        Lemonade = 12
    }

    public enum Slot : byte
    {
        TwoHanded = 0x00,
        Head = 0x01,
        Necklace = 0x02,
        Backpack = 0x03,
        Body = 0x04,
        Right = 0x05,
        Left = 0x06,
        Legs = 0x07,
        Feet = 0x08,
        Ring = 0x09,
        Ammo = 0x0A,
        WhereEver = 0x0B
    }

    public enum LightLevels : byte
    {
        None = 0,
        Torch = 7,
        Full = 27,
        World = 255
    }

    public enum LightColors : byte
    {
        None = 0,
        Default = 206, // default light color
        Orange = Default,
        White = 215
    }

    public enum MessageType : byte
    {
        ConsoleYellow = 0x01, //Yellow message in the console
        ConsoleLightBlue = 0x04, //Light blue message in the console
        ConsoleOrange = 0x11, //Orange message in the console
        Warning = 0x12, //Red message in game window and in the console
        EventAdvance = 0x13, //White message in game window and in the console
        EventDefault = 0x14, //White message at the bottom of the game window and in the console
        StatusDefault = 0x15, //White message at the bottom of the game window and in the console
        DescriptionGreen = 0x16, //Green message in game window and in the console
        StatusSmall = 0x17, //White message at the bottom of the game window"
        ConsoleBlue = 0x18, //Blue message in the console
        ConsoleRed = 0x19 //Red message in the console
    }

    public enum LocationType : byte
    {
        Container,
        Slot,
        Ground
    }

    public enum SpeechType : byte
    {
        Say = 0x01,	//normal talk
        Whisper = 0x02,	//whispering - #w text
        Yell = 0x03,	//yelling - #y text
        Private = 0x04, //Players speaking privately to players
        ChannelYellow = 0x05,	//Yellow message in chat
        RuleViolationReport = 0x06, //Reporting rule violation - Ctrl+R
        RuleViolationAnswer = 0x07, //Answering report
        RuleViolationContinue = 0x08, //Answering the answer of the report
        Broadcast = 0x09,	//Broadcast a message - #b
        //ChannelRed = 0x05,	//Talk red on chat - #c
        //PrivateRed = 0x04,	//Red private - @name@ text
        //ChannelOrange = 0x05,	//Talk orange on text
        //ChannelRedAnonymous = 0x05,	//Talk red anonymously on chat - #d
        MonsterSay = 0x0E	//Talk orange
        //MonsterYell = 0x0E,	//Yell orange
    }

    public enum ChatChannel : ushort
    {
        RuleViolations = 0x03,
        Game = 0x04,
        Trade = 0x05,
        RealLife = 0x06,
        Help = 0x08,
        Private = 0xFFFF,
        None = 0xAAAA
    }

    public enum SpellsT : byte
    {
        AnimateDead,
        Antidote,
        AntidoteRune,
        Berserk,
        CancelInvisibility,
        Challenge,
        Chameleon,
        ConjureArrow,
        ConjureBolt,
        ConvinceCreature,
        CreatureIllusion,
        Desintegrate,
        DestroyField,
        EnchantStaff,
        EnergyBeam,
        Energybomb,
        EnergyField,
        EnergyStrike,
        EnergyWall,
        EnergyWave,
        Envenom,
        Explosion,
        ExplosiveArrow,
        FindPerson,
        Fireball,
        Firebomb,
        FireField,
        FireWall,
        FireWave,
        FlameStrike,
        Food,
        ForceStrike,
        GreatEnergyBeam,
        GreatFireball,
        GreatLight,
        Haste,
        HealFriend,
        HeavyMagicMissile,
        IntenseHealing,
        IntenseHealingRune,
        Invisible,
        Levitate,
        Light,
        LightHealing,
        LightMagicMissile,
        MagicRope,
        MagicShield,
        MagicWall,
        MassHealing,
        Paralyze,
        PoisonBomb,
        PoisonedArrow,
        PoisonField,
        PoisonStorm,
        PoisonWall,
        PowerBolt,
        Soulfire,
        StrongHaste,
        SuddenDeath,
        SummonCreature,
        UltimateExplosion,
        UltimateHealing,
        UltimateHealingRune,
        UltimateLight,
        UndeadLegion,
        WildGrowth
    }

    public enum AttackType : byte
    {
        Physical,
        Posion,
        Fire,
        Energy,
        Drain
    }

    public enum ConditionType : uint
    {
        None = 0,
        Posion = 1 << 0,
        Fire = 1 << 1,
        Energy = 1 << 2,
        LifeDrain = 1 << 3,
        Haste = 1 << 4,
        Paralyze = 1 << 5,
        Outfit = 1 << 6,
        Invisible = 1 << 7,
        Light = 1 << 8,
        ManaShield = 1 << 9,
        InFight = 1 << 10,
        PzLocked = 1 << 11,
        Drunk = 1 << 12,
        Muted = 1 << 13,
        ExhaustCombat = 1 << 14,
        ExhaustHeal = 1 << 15,
        ExhaustYell = 1 << 16
    }

    public enum Gender : byte
    {
        Male = 0x00,
        Female = 0x01
    }

    public enum VocationType : byte
    {
        None = 0,
        Knight = 1,
        Paladin = 2,
        Sorcerer = 3,
        Druid = 4
    }

    public enum FightMode : byte
    {
        FullAttack = 0x01,
        Balanced = 0x02,
        FullDefense = 0x03
    }

    public enum ChaseMode : byte
    {
        Stand = 0x00,
        Chase = 0x01,
        KeepDistance = 0x02
    }

    public enum TextColor : byte
    {
        Blue = 5,
        Green = 30,
        LightBlue = 35,
        Crystal = 65,
        Purple = 83,
        Platinum = 89,
        LightGrey = 129,
        DarkRed = 144,
        Red = 180,
        Orange = 198,
        Gold = 210,
        White = 215,
        None = 255
    }

    public enum EventType : byte
    {
        Use,
        MultiUse,
        Movement,
        Collision,
        Separation
    }
    
    public enum FunctionComparisonType : byte
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Equal
    }

    public enum ShootTypeT : byte
    {
        Spear = 0x00,
        Bolt = 0x01,
        Arrow = 0x02,
        Fire = 0x03,
        Energy = 0x04,
        PoisonArrow = 0x05,
        BurstArrow = 0x06,
        ThrowingStar = 0x07,
        ThrowingKnife = 0x08,
        SmallStone = 0x09,
        SuddenDeath = 0x0A,
        LargeRock = 0x0B,
        Snowball = 0x0C,
        PowerBolt = 0x0D,
        None = 0xFF //Don't send to client.
    }

    // ********** old implementation enums ******** //

    //   public enum PlayerFlag : byte
    //{
    //	None = 0,
    //	Promoted = 1 << 0,
    //	Save = 1 << 1,
    //	Online = 1 << 2,
    //	HideChar = 1 << 3,
    //	VerifyName = 1 << 4
    //}

    //   //public enum TileFlag : ulong
    //   //{
    //   //    IsProtectionZone = 1 << 0,
    //   //    IsHouse = 1 << 1,
    //   //    IsSpawn = 1 << 2
    //   //};

    ////public enum ItemFlag : ulong
    ////{
    ////	Ground = 			1<< 0,
    ////	TopOrder1 =		 	1<< 1,
    ////	TopOrder2 = 		1<< 2,
    ////	TopOrder3 = 		1<< 3,
    ////	IsContainer = 		1<< 4,
    ////	IsStackable = 		1<< 5,
    ////	IsCorpse = 			1<< 6,
    ////	IsUsableWith = 		1<< 7,
    ////	IsWritable =		1<< 8,
    ////	IsReadable = 		1<< 9,
    ////	IsFluidContainer = 	1<< 10,
    ////	IsSplash = 			1<< 11,
    ////	Blocking = 			1<< 12,
    ////	IsImmovable = 		1<< 13,
    ////	BlocksMissiles = 	1<< 14,
    ////	IsPickupable = 		1<< 15,
    ////	IsLadder = 			1<< 16,
    ////	IsLightSource = 	1<< 17,
    ////	FloorchangeDown = 	1<< 18,
    ////	HasHeight = 		1<< 19,
    ////	IsFloor = 			1<< 20,
    ////	HasAutoMapColor = 	1<< 21,
    ////	HasInfo = 			1<< 22,
    ////       //new in 7.4
    ////	Rotateable = 		1<< 23,
    ////	Hangable = 		    1<< 24,
    ////	AllowsHangableH = 	1<< 25,
    ////	AllowsHangableV = 	1<< 26,
    ////	IsIdleAnimation = 	1<< 26
    ////};

    //public enum EquipFlag : byte
    //{
    //	ShowCharges = 1 << 0,
    //	ShowDuration = 1 << 1,
    //	Invisible = 1 << 2,
    //	ManaShield = 1 << 3
    //}

    //public enum ItemType_t : byte
    //{
    //	None = 			0x00,
    //	Equipment = 	0x01,
    //	Rune = 		    0x02,
    //	Trashholder = 	0x03,
    //	Door = 			0x04,
    //	Teleport = 		0x05,
    //	MagicField = 	0x06,
    //	Mailbox = 		0x07,
    //	Bed = 			0x08,
    //	Key = 			0x09
    //}

    //public enum WeaponType_t : byte
    //{
    //	None = 		 0x00,
    //	Club = 		 0x01,
    //	Axe = 		 0x02,
    //	Sword = 	 0x03,
    //	Shield = 	 0x04,
    //	Distance = 	 0x05,
    //	Ammunition = 0x06
    //}

    //public enum ShootType_t : byte
    //{
    //	Spear			= 0x00,
    //	Bolt           	= 0x01,
    //	Arrow          	= 0x02,
    //	Fire		   	= 0x03,
    //	Energy         	= 0x04,
    //	PoisonArrow    	= 0x05,
    //	BurstArrow     	= 0x06,
    //	ThrowingStar   	= 0x07,
    //	ThrowingKnife  	= 0x08,
    //	SmallStone     	= 0x09,
    //	SuddenDeath    	= 0x0A,
    //	LargeRock      	= 0x0B,
    //	Snowball       	= 0x0C,
    //	PowerBolt      	= 0x0D,
    //	None 			= 0xFF //Don't send to client.
    //}

    //public enum AmmoType_t : byte
    //{
    //	None 			= 0x00,
    //	Bolt 			= 0x01,
    //	Arrow 			= 0x02,
    //	Spear 			= 0x03,
    //	ThrowingStar 	= 0x04,
    //	ThrowingKnife 	= 0x05,
    //	Stone 			= 0x06,
    //	Snowball 		= 0x07
    //}

    //public enum FieldType_t : byte
    //{
    //	ReducingDamage 			= 0x00,
    //	CustomDamage 			= 0x01
    //}

    //   public enum PositionType
    //   {
    //       Container,
    //       Slot,
    //       Map
    //   }

    //public enum FloorChangeDirection : byte
    //   {
    //       None = 0,
    //       Up = 1 << 0,
    //       Down = 1 << 1,
    //       North = 1 << 2,
    //       South = 1 << 3,
    //       West = 1 << 4,
    //       East = 1 << 5
    //   }

    //   #region version 7.1
    //   /*public enum MessageType : byte
    //   {
    //       EventAdvance = 0x10,        //White message in game window and in the console
    //       EventDefault = 0x11,        //White message at the bottom of the game window and in the console
    //       StatusDefault = 0x12,       //White message at the bottom of the game window and in the console
    //       DescriptionGreen = 0x13,    //Green message in game window and in the console
    //       StatusSmall = 0x14,         //White message at the bottom of the game window
    //       Warning = 0x15              //Red message in game window and in the console
    //   }*/
    //   #endregion
    //   #region version 7.4
    //   //public enum MessageType : byte
    //   //{
    //   //    ConsoleYellow       = 0x01, //Yellow message in the console
    //   //    ConsoleLightBlue    = 0x04, //Light blue message in the console
    //   //    ConsoleOrange       = 0x11, //Orange message in the console
    //   //    Warning             = 0x12, //Red message in game window and in the console
    //   //    EventAdvance        = 0x13, //White message in game window and in the console
    //   //    EventDefault        = 0x14, //White message at the bottom of the game window and in the console
    //   //    StatusDefault       = 0x15, //White message at the bottom of the game window and in the console
    //   //    DescriptionGreen    = 0x16, //Green message in game window and in the console
    //   //    StatusSmall         = 0x17, //White message at the bottom of the game window"
    //   //    ConsoleBlue         = 0x18, //Blue message in the console
    //   //    ConsoleRed          = 0x19, //Red message in the console
    //   //}
    //   #endregion

    //   //public enum Skill_t : byte
    //   //{
    //   //    First       = 0x00,
    //   //    Fist        = First,
    //   //    Club        = 0x01,
    //   //    Axe         = 0x02,
    //   //    Sword       = 0x03,
    //   //    Shield      = 0x04,
    //   //    Distance    = 0x05,
    //   //    Fishing     = 0x06,
    //   //    Last        = Fishing
    //   //}

    //   public enum Fluid : byte
    //   {
    //       Empty       = 0x00,
    //       Water       = 0x01,
    //       Blood       = 0x02,
    //       Beer        = 0x03,
    //       Slime       = 0x04,
    //       Lemonade    = 0x05,
    //       Milk        = 0x06,
    //       Mana        = 0x07,
    //       Life        = 0x09,
    //       Oil         = 0x11,
    //       Urine       = 0x13,
    //       Wine        = 0x15,
    //       Mud         = 0x19      //TODO: check
    //   }

    //   public enum FluidColor : byte
    //   {
    //       Empty       = 0,
    //       Blue        = 1,
    //       Red         = 2,
    //       Brown       = 3,
    //       Green       = 4,
    //       Yellow      = 5,
    //       White       = 6,
    //       Purple      = 7
    //   }


    //   //public enum OutfitTypes : byte
    //   //{
    //   //    OutfitMale_1   = 0x80,
    //   //    OutfitMale_2 = 0x81,
    //   //    OutfitMale_3 = 0x82,
    //   //    OutfitMale_4 = 0x83,
    //   //    OutfitFemale_1 = 0x88,
    //   //    OutfitFemale_2 = 0x89,
    //   //    OutfitFemale_3 = 0x8A,
    //   //    OutfitFemale_4 = 0x8B,
    //   //}
}