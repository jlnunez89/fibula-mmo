// -----------------------------------------------------------------
// <copyright file="Player.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class Player : Creature, IPlayer
    {
        private const int KnownCreatureLimit = 100; // TODO: not sure of the number for this version... debugs will tell :|

        private const sbyte MaxContainers = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="gameInstance"></param>
        /// <param name="characterId"></param>
        /// <param name="name"></param>
        /// <param name="maxHitpoints"></param>
        /// <param name="maxManapoints"></param>
        /// <param name="corpse"></param>
        /// <param name="hitpoints"></param>
        /// <param name="manapoints"></param>
        public Player(
            string characterId,
            string name,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort corpse,
            ushort hitpoints = 0,
            ushort manapoints = 0)
            : base(name, string.Empty, maxHitpoints, maxManapoints, corpse, hitpoints, manapoints)
        {
            characterId.ThrowIfNullOrWhiteSpace(nameof(characterId));

            this.CharacterId = characterId;

            // TODO: implement, Temp values
            this.Speed = 420;

            this.Outfit = new Outfit
            {
                Id = 128,
                Head = 114,
                Body = 114,
                Legs = 114,
                Feet = 114,
            };

            this.EmittedLightLevel = (byte)LightLevels.Torch;
            this.EmittedLightColor = (byte)LightColors.Orange;
            this.CarryStrength = 150;

            this.SoulPoints = 0;

            this.Skills[SkillType.Experience] = new Skill(SkillType.Experience, 1, 1.0, 10, 1, 150);
            this.Skills[SkillType.Magic] = new Skill(SkillType.Magic, 1, 1.0, 10, 1, 150);
            this.Skills[SkillType.NoWeapon] = new Skill(SkillType.NoWeapon, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Axe] = new Skill(SkillType.Axe, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Club] = new Skill(SkillType.Club, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Sword] = new Skill(SkillType.Sword, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Shield] = new Skill(SkillType.Shield, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Ranged] = new Skill(SkillType.Ranged, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Fishing] = new Skill(SkillType.Fishing, 10, 1.0, 10, 10, 150);

            // this.OpenContainers = new IContainer[MaxContainers];
            this.Inventory = new PlayerInventory(this);
            this.KnownCreatures = new Dictionary<uint, long>();
            this.VipList = new Dictionary<string, bool>();

            // this.OnThingChanged += this.CheckInventoryContainerProximity;
        }

        // ~PlayerId()
        // {
        //    OnLocationChanged -= CheckInventoryContainerProximity;
        // }

        /// <summary>
        /// Gets the player's character id.
        /// </summary>
        public string CharacterId { get; }

        /// <summary>
        /// Gets the player's permissions level.
        /// </summary>
        public byte PermissionsLevel { get; }

        public override bool CanBeMoved => this.PermissionsLevel == 0;

        public override string Description => this.Name;

        public override string InspectionText => this.Description;

        public byte SoulPoints { get; } // TODO: nobody likes soulpoints... figure out what to do with them :)

        // public IAction PendingAction { get; private set; }

        public bool IsLogoutAllowed => true; // this.AutoAttackTargetId == 0;

        private IDictionary<uint, long> KnownCreatures { get; }

        private IDictionary<string, bool> VipList { get; }

        public Location LocationInFront
        {
            get
            {
                switch (this.Direction)
                {
                    case Direction.North:
                        return new Location
                        {
                            X = this.Location.X,
                            Y = this.Location.Y - 1,
                            Z = this.Location.Z,
                        };
                    case Direction.East:
                        return new Location
                        {
                            X = this.Location.X + 1,
                            Y = this.Location.Y,
                            Z = this.Location.Z,
                        };
                    case Direction.West:
                        return new Location
                        {
                            X = this.Location.X - 1,
                            Y = this.Location.Y,
                            Z = this.Location.Z,
                        };
                    case Direction.South:
                        return new Location
                        {
                            X = this.Location.X,
                            Y = this.Location.Y + 1,
                            Z = this.Location.Z,
                        };
                    default:
                        return this.Location; // should not happen.
                }
            }
        }

        public sealed override IInventory Inventory { get; protected set; }

        // private IContainerItem[] OpenContainers { get; }

        public bool KnowsCreatureWithId(uint creatureId)
        {
            return this.KnownCreatures.ContainsKey(creatureId);
        }

        public void AddKnownCreature(uint creatureId)
        {
            try
            {
                this.KnownCreatures[creatureId] = DateTimeOffset.UtcNow.Ticks;
            }
            catch
            {
                // happens when 2 try to add at the same time, which we don't care about.
            }
        }

        public uint ChooseToRemoveFromKnownSet()
        {
            // If the buffer is full we need to choose a victim.
            while (this.KnownCreatures.Count == KnownCreatureLimit)
            {
                // ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                foreach (var candidate in this.KnownCreatures.OrderBy(kvp => kvp.Value).ToList())
                {
                    if (this.KnownCreatures.Remove(candidate.Key))
                    {
                        return candidate.Key;
                    }
                }
            }

            return uint.MinValue;
        }

        // public void SetPendingAction(IAction action)
        // {
        //    action.ThrowIfNull(nameof(action));

        // this.PendingAction = action;
        // }

        // public void ClearPendingActions()
        // {
        //    this.PendingAction = null;
        // }

        // protected override void CheckPendingActions(IThing thingChanged, ThingStateChangedEventArgs eventArgs)
        // {
        //    if (this.PendingAction == null || thingChanged != this || eventArgs.PropertyChanged != nameof(this.Location))
        //    {
        //        return;
        //    }

        // if (this.Location == this.PendingAction.RetryLocation)
        //    {
        //        Task.Delay(this.CalculateRemainingCooldownTime(ExhaustionType.Action, DateTimeOffset.UtcNow) + TimeSpan.FromMilliseconds(500))
        //            .ContinueWith(previous =>
        //            {
        //                this.PendingAction.Perform();
        //            });
        //    }
        // }

        // public sbyte GetContainerId(IContainer thingAsContainer)
        // {
        //    for (sbyte i = 0; i < this.OpenContainers.Length; i++)
        //    {
        //        if (this.OpenContainers[i] == thingAsContainer)
        //        {
        //            return i;
        //        }
        //    }

        // return -1;
        // }

        // public void CloseContainerWithId(byte openContainerId)
        // {
        //    try
        //    {
        //        this.OpenContainers[openContainerId].Close(this.Id);
        //        this.OpenContainers[openContainerId] = null;
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        // }

        // public sbyte OpenContainer(IContainer container)
        // {
        //    container.ThrowIfNull(nameof(container));

        // for (byte i = 0; i < this.OpenContainers.Length; i++)
        //    {
        //        if (this.OpenContainers[i] != null)
        //        {
        //            continue;
        //        }

        // this.OpenContainers[i] = container;
        //        this.OpenContainers[i].Open(this.Id, i);

        // return (sbyte)i;
        //    }

        // var lastIdx = (sbyte)(this.OpenContainers.Length - 1);

        // this.OpenContainers[lastIdx] = container;

        // return lastIdx;
        // }

        // public void OpenContainerAt(IContainerItem thingAsContainer, byte index)
        // {
        //    this.OpenContainers[index]?.Close(this.Id);
        //    this.OpenContainers[index] = thingAsContainer;
        //    this.OpenContainers[index].Open(this.Id, index);
        // }

        // public IContainer GetContainer(byte index)
        // {
        //    try
        //    {
        //        var container = this.OpenContainers[index];

        // container.Open(this.Id, index);

        // return container;
        //    }
        //    catch
        //    {
        //        // ignored
        //    }

        // return null;
        // }

        // public void CheckInventoryContainerProximity(IThing thingChanging, ThingStateChangedEventArgs eventArgs)
        // {
        //    for (byte i = 0; i < this.OpenContainers.Length; i++)
        //    {
        //        if (this.OpenContainers[i] == null)
        //        {
        //            continue;
        //        }

        // var containerSourceLoc = this.OpenContainers[i].Location;

        // switch (containerSourceLoc.Type)
        //        {
        //            case LocationType.Ground:
        //                var locDiff = this.Location - containerSourceLoc;

        // if (locDiff.MaxValueIn2D > 1)
        //                {
        //                    var container = this.GetContainer(i);
        //                    this.CloseContainerWithId(i);

        // if (container != null)
        //                    {
        //                        container.OnThingChanged -= this.CheckInventoryContainerProximity;
        //                    }

        // this.Game.NotifySinglePlayer(this, conn => new GenericNotification(conn, new ContainerClosePacket { ContainerId = i }));
        //                }

        // break;
        //            case LocationType.Container:
        //                break;
        //            case LocationType.Slot:
        //                break;
        //        }
        //    }
        // }
    }
}
