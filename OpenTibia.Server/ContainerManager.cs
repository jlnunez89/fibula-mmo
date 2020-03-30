// -----------------------------------------------------------------
// <copyright file="ContainerManager.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using Serilog;

    /// <summary>
    /// Class that contains helper methods for container operations.
    /// </summary>
    public class ContainerManager : IContainerManager
    {
        /// <summary>
        /// The maximum number of containers to maintain per creature.
        /// </summary>
        private const int MaxContainersPerCreature = 16;

        private readonly ILogger logger;
        private readonly IConnectionFinder connectionFinder;
        private readonly IScheduler scheduler;

        private readonly object internalDictionariesLock;
        private readonly IDictionary<uint, IContainerItem[]> creaturesToContainers;
        private readonly IDictionary<Guid, IDictionary<byte, ICreature>> containersToCreatures;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerManager"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="connectionFinder">A reference to the connection finder in use.</param>
        /// <param name="scheduler">A reference to the scheduler in use.</param>
        public ContainerManager(
            ILogger logger,
            IConnectionFinder connectionFinder,
            IScheduler scheduler)
        {
            this.logger = logger.ForContext<ContainerManager>();
            this.scheduler = scheduler;
            this.connectionFinder = connectionFinder;

            this.internalDictionariesLock = new object();
            this.creaturesToContainers = new Dictionary<uint, IContainerItem[]>();
            this.containersToCreatures = new Dictionary<Guid, IDictionary<byte, ICreature>>();
        }

        /// <summary>
        /// Performs a container open action for a player.
        /// </summary>
        /// <param name="forCreature">The creature for which the container is being opened.</param>
        /// <param name="container">The container to open.</param>
        /// <param name="atPosition">The position in which to open the container, for the player.</param>
        public void OpenContainer(ICreature forCreature, IContainerItem container, byte atPosition)
        {
            forCreature.ThrowIfNull(nameof(forCreature));
            container.ThrowIfNull(nameof(container));

            if (this.IsContainerOpen(container.UniqueId, forCreature.Id, out IEnumerable<byte> openPositions) && openPositions.Contains(atPosition))
            {
                // This player is has this container open already, at this same position.
                return;
            }

            // Now, let's check if the player has something else open at the desired location.
            var currentContainer = this.GetContainerAt(forCreature.Id, atPosition);

            if (currentContainer != null)
            {
                // In this case, we need to close this container first.
                this.CloseContainerInternal(forCreature, currentContainer, atPosition);
            }

            // Now actually open the container for this player.
            byte containerId = this.OpenContainerInternal(forCreature, container, atPosition);

            this.scheduler.ScheduleEvent(
                new GenericNotification(
                    () => this.connectionFinder.FindByPlayerId(forCreature.Id).YieldSingleItem(),
                    new GenericNotificationArguments(
                        new ContainerOpenPacket(
                            containerId,
                            container.ThingId,
                            container.Type.Name,
                            container.Capacity,
                            (container.ParentCylinder is IContainerItem parentContainer) && parentContainer.Type.TypeId != 0,
                            container.Content))));
        }

        /// <summary>
        /// Performs a container close action for a player.
        /// </summary>
        /// <param name="forCreature">The creature for which the container is being closed.</param>
        /// <param name="container">The container being closed.</param>
        /// <param name="atPosition">The position of the container being closed, as seen by the player.</param>
        public void CloseContainer(ICreature forCreature, IContainerItem container, byte atPosition)
        {
            forCreature.ThrowIfNull(nameof(forCreature));
            container.ThrowIfNull(nameof(container));

            if (!this.IsContainerOpen(container.UniqueId, forCreature.Id, out IEnumerable<byte> openPositions) || !openPositions.Contains(atPosition))
            {
                // This player is has this container open already, at this same position.
                return;
            }

            this.CloseContainerInternal(forCreature, container, atPosition);

            this.scheduler.ScheduleEvent(
                new GenericNotification(
                    () => this.connectionFinder.FindByPlayerId(forCreature.Id).YieldSingleItem(),
                    new GenericNotificationArguments(new ContainerClosePacket(atPosition))));
        }

        /// <summary>
        /// Finds a container for a specific creature at the specified position.
        /// </summary>
        /// <param name="creatureId">The id of the creature for which to find the container.</param>
        /// <param name="atPosition">The position at which to look for the container.</param>
        /// <returns>The container found, or null if not found.</returns>
        public IContainerItem FindForCreature(uint creatureId, byte atPosition)
        {
            lock (this.internalDictionariesLock)
            {
                if (this.creaturesToContainers.ContainsKey(creatureId) && atPosition < MaxContainersPerCreature)
                {
                    return this.creaturesToContainers[creatureId][atPosition];
                }

                return null;
            }
        }

        /// <summary>
        /// Finds the position of a specified container as seen by a specific creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature for which to find the container.</param>
        /// <param name="container">The container to look for.</param>
        /// <returns>The position of container found, or <see cref="IContainerManager.UnsetContainerPosition"/>> if not found.</returns>
        public byte FindForCreature(uint creatureId, IContainerItem container)
        {
            if (container == null)
            {
                return IContainerManager.UnsetContainerPosition;
            }

            lock (this.internalDictionariesLock)
            {
                if (this.creaturesToContainers.ContainsKey(creatureId))
                {
                    for (byte i = 0; i < MaxContainersPerCreature; i++)
                    {
                        if (this.creaturesToContainers[creatureId][i] != null && this.creaturesToContainers[creatureId][i].UniqueId == container.UniqueId)
                        {
                            return i;
                        }
                    }
                }

                return IContainerManager.UnsetContainerPosition;
            }
        }

        /// <summary>
        /// Finds all the containers known by the specified creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <returns>A collection of containers that the creature knows.</returns>
        public IEnumerable<IContainerItem> FindAllForCreature(uint creatureId)
        {
            lock (this.internalDictionariesLock)
            {
                if (!this.creaturesToContainers.ContainsKey(creatureId))
                {
                    return Enumerable.Empty<IContainerItem>();
                }

                return this.creaturesToContainers[creatureId].Where(i => i != null).ToList();
            }
        }

        private bool IsContainerOpen(Guid containerUniqueId, uint creatureId, out IEnumerable<byte> openPositions)
        {
            openPositions = null;

            lock (this.internalDictionariesLock)
            {
                if (this.creaturesToContainers.ContainsKey(creatureId))
                {
                    var found = new List<byte>();

                    for (byte i = 0; i < this.creaturesToContainers[creatureId].Length; i++)
                    {
                        if (this.creaturesToContainers[creatureId][i] != null && this.creaturesToContainers[creatureId][i].UniqueId == containerUniqueId)
                        {
                            found.Add(i);
                        }
                    }

                    openPositions = found;

                    return true;
                }
            }

            return false;
        }

        private IContainerItem GetContainerAt(uint creatureId, byte atPosition)
        {
            lock (this.internalDictionariesLock)
            {
                if (this.creaturesToContainers.ContainsKey(creatureId) && atPosition < this.creaturesToContainers[creatureId].Length)
                {
                    return this.creaturesToContainers[creatureId][atPosition];
                }
            }

            return null;
        }

        private void CloseContainerInternal(ICreature forCreature, IContainerItem container, byte atPosition)
        {
            lock (this.internalDictionariesLock)
            {
                if (!this.creaturesToContainers.ContainsKey(forCreature.Id) ||
                    atPosition >= this.creaturesToContainers[forCreature.Id].Length ||
                    container.UniqueId != this.creaturesToContainers[forCreature.Id][atPosition].UniqueId)
                {
                    return;
                }

                // For the per-creature map, we need only close the one at the specific position.
                this.creaturesToContainers[forCreature.Id][atPosition] = null;

                // For the containers map, we need to get a bit fancy.
                if (this.containersToCreatures.ContainsKey(container.UniqueId))
                {
                    if (this.containersToCreatures[container.UniqueId][atPosition].Id == forCreature.Id)
                    {
                        this.containersToCreatures[container.UniqueId].Remove(atPosition);
                    }

                    // Clean up if this list is now empty.
                    if (this.containersToCreatures[container.UniqueId].Count == 0)
                    {
                        this.containersToCreatures.Remove(container.UniqueId);

                        // Clean up events because no one else cares about this container.
                        container.ContentAdded -= this.OnContainerContentAdded;
                        container.ContentRemoved -= this.OnContainerContentRemoved;
                        container.ContentUpdated -= this.OnContainerContentUpdated;
                        container.ThingChanged -= this.OnContainerChanged;
                    }
                }
            }
        }

        private byte OpenContainerInternal(ICreature forCreature, IContainerItem container, byte atPosition)
        {
            lock (this.internalDictionariesLock)
            {
                byte openedAt = IContainerManager.UnsetContainerPosition;

                if (!this.creaturesToContainers.ContainsKey(forCreature.Id))
                {
                    this.creaturesToContainers.Add(forCreature.Id, new IContainerItem[MaxContainersPerCreature]);
                }

                if (atPosition >= MaxContainersPerCreature)
                {
                    // Find any available position.
                    for (byte i = 0; i < MaxContainersPerCreature; i++)
                    {
                        if (this.creaturesToContainers[forCreature.Id][i] != null)
                        {
                            continue;
                        }

                        openedAt = i;
                        this.creaturesToContainers[forCreature.Id][i] = container;

                        break;
                    }
                }
                else
                {
                    openedAt = atPosition;
                    this.creaturesToContainers[forCreature.Id][atPosition] = container;
                }

                // Now add to the other index per container.
                if (!this.containersToCreatures.ContainsKey(container.UniqueId))
                {
                    this.containersToCreatures.Add(container.UniqueId, new Dictionary<byte, ICreature>());

                    // This container is not being tracked at all, let's start tracking it.
                    container.ContentAdded += this.OnContainerContentAdded;
                    container.ContentRemoved += this.OnContainerContentRemoved;
                    container.ContentUpdated += this.OnContainerContentUpdated;
                    container.ThingChanged += this.OnContainerChanged;
                }

                this.containersToCreatures[container.UniqueId][openedAt] = forCreature;

                return openedAt;
            }
        }

        /// <summary>
        /// Handles an event from a container content added.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="addedItem">The item that was added.</param>
        private void OnContainerContentAdded(IContainerItem container, IItem addedItem)
        {
            lock (this.internalDictionariesLock)
            {
                if (!this.containersToCreatures.ContainsKey(container.UniqueId))
                {
                    return;
                }

                // The request has to be sent this way since the container id may be different for each player.
                foreach (var (containerPosition, creature) in this.containersToCreatures[container.UniqueId].ToList())
                {
                    if (!(creature is IPlayer player))
                    {
                        continue;
                    }

                    this.scheduler.ScheduleEvent(
                        new GenericNotification(
                            () => this.connectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                            new GenericNotificationArguments(new ContainerAddItemPacket(containerPosition, addedItem))));
                }
            }
        }

        /// <summary>
        /// Handles an event from a container content removed.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexRemoved">The index that was removed.</param>
        private void OnContainerContentRemoved(IContainerItem container, byte indexRemoved)
        {
            lock (this.internalDictionariesLock)
            {
                if (!this.containersToCreatures.ContainsKey(container.UniqueId))
                {
                    return;
                }

                // The request has to be sent this way since the container id may be different for each player.
                foreach (var (containerId, creature) in this.containersToCreatures[container.UniqueId].ToList())
                {
                    if (!(creature is IPlayer player))
                    {
                        continue;
                    }

                    this.scheduler.ScheduleEvent(
                        new GenericNotification(
                            () => this.connectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                            new GenericNotificationArguments(new ContainerRemoveItemPacket(indexRemoved, containerId))));
                }
            }
        }

        /// <summary>
        /// Handles an event from a container content updated.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexOfUpdated">The index that was updated.</param>
        /// <param name="updatedItem">The updated item.</param>
        private void OnContainerContentUpdated(IContainerItem container, byte indexOfUpdated, IItem updatedItem)
        {
            if (updatedItem == null)
            {
                return;
            }

            lock (this.internalDictionariesLock)
            {
                if (!this.containersToCreatures.ContainsKey(container.UniqueId))
                {
                    return;
                }

                // The request has to be sent this way since the container id may be different for each player.
                foreach (var (containerId, creature) in this.containersToCreatures[container.UniqueId].ToList())
                {
                    if (!(creature is IPlayer player))
                    {
                        continue;
                    }

                    this.scheduler.ScheduleEvent(
                        new GenericNotification(
                            () => this.connectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                            new GenericNotificationArguments(new ContainerUpdateItemPacket((byte)indexOfUpdated, containerId, updatedItem))));
                }
            }
        }

        /// <summary>
        /// Handles a change event from a container.
        /// </summary>
        /// <param name="containerThatChangedAsThing">The container that changed.</param>
        /// <param name="eventArgs">The event arguments of the change.</param>
        private void OnContainerChanged(IThing containerThatChangedAsThing, ThingStateChangedEventArgs eventArgs)
        {
            lock (this.internalDictionariesLock)
            {
                if (!(containerThatChangedAsThing is IContainerItem containerItem) ||
                !eventArgs.PropertyChanged.Equals(nameof(containerItem.Location)) ||
                this.containersToCreatures.ContainsKey(containerItem.UniqueId))
                {
                    return;
                }

                if (containerItem.CarryLocation != null)
                {
                    // Container is held by a creature, which is the only one that should have access now.
                    var creatureHoldingTheContainer = containerItem.Carrier;

                    if (creatureHoldingTheContainer != null && this.containersToCreatures.ContainsKey(containerItem.UniqueId))
                    {
                        foreach (var (containerId, creature) in this.containersToCreatures[containerItem.UniqueId].ToList())
                        {
                            if (creatureHoldingTheContainer.Id == creature.Id || !(creature is IPlayer player))
                            {
                                continue;
                            }

                            this.CloseContainer(player, containerItem, containerId);
                        }
                    }
                }
                else if (containerItem.Location.Type == LocationType.Map && this.containersToCreatures.ContainsKey(containerItem.UniqueId))
                {
                    // Container was dropped or placed in a container that ultimately sits on the map, figure out which creatures are still in range.
                    foreach (var (containerId, creature) in this.containersToCreatures[containerItem.UniqueId].ToList())
                    {
                        if (!(creature is IPlayer player))
                        {
                            continue;
                        }

                        var locationDiff = containerItem.Location - player.Location;

                        if (locationDiff.MaxValueIn2D > 1 || locationDiff.Z != 0)
                        {
                            this.CloseContainer(player, containerItem, containerId);
                        }
                    }
                }
            }
        }
    }
}
