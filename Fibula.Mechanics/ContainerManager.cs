// -----------------------------------------------------------------
// <copyright file="ContainerManager.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Mechanics.Notifications;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a manager for all item container operations by creatures.
    /// </summary>
    public class ContainerManager : IContainerManager
    {
        private readonly ILogger logger;
        private readonly ICreatureFinder creatureFinder;
        private readonly IScheduler scheduler;

        private readonly object internalDictionariesLock;
        private readonly IDictionary<uint, IContainerItem[]> creaturesToContainers;
        private readonly IDictionary<Guid, IDictionary<byte, uint>> containersToCreatureIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerManager"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="scheduler">A reference to the scheduler in use.</param>
        public ContainerManager(ILogger logger, ICreatureFinder creatureFinder, IScheduler scheduler)
        {
            this.logger = logger.ForContext<ContainerManager>();
            this.creatureFinder = creatureFinder;
            this.scheduler = scheduler;

            this.internalDictionariesLock = new object();
            this.creaturesToContainers = new Dictionary<uint, IContainerItem[]>();
            this.containersToCreatureIds = new Dictionary<Guid, IDictionary<byte, uint>>();
        }

        /// <summary>
        /// Performs a container open action for a player.
        /// </summary>
        /// <param name="forCreatureId">The id of the creature for which the container is being opened.</param>
        /// <param name="container">The container to open.</param>
        /// <param name="atPosition">The position in which to open the container, for the player.</param>
        public void OpenContainer(uint forCreatureId, IContainerItem container, byte atPosition)
        {
            container.ThrowIfNull(nameof(container));

            // Check if this creature already has this container open at the specified position.
            // If so, we got nothing more to do.
            if (this.IsContainerOpen(container.UniqueId, forCreatureId, out IEnumerable<byte> openPositions) && openPositions.Contains(atPosition))
            {
                return;
            }

            // Otherwise, let's check if the player has something else open at the desired container position.
            var currentContainer = this.GetContainerAt(forCreatureId, atPosition);

            if (currentContainer != null)
            {
                // In this case, we need to close this container first.
                this.CloseContainerInternal(forCreatureId, currentContainer, atPosition);
            }

            // Now, actually open the container for this creature.
            byte containerId = this.OpenContainerInternal(forCreatureId, container, atPosition);

            if (this.creatureFinder.FindPlayerById(forCreatureId) is IPlayer player)
            {
                this.scheduler.ScheduleEvent(
                    new GenericNotification(
                        () => player.YieldSingleItem(),
                        new ContainerOpenPacket(
                            containerId,
                            container.TypeId,
                            container.Type.Name,
                            container.Capacity,
                            container.ParentContainer is IContainerItem parentContainer && parentContainer.Type.TypeId != 0,
                            container.Content)));
            }
        }

        /// <summary>
        /// Performs a container close action for a player.
        /// </summary>
        /// <param name="forCreatureId">The id of the creature for which the container is being closed.</param>
        /// <param name="container">The container being closed.</param>
        /// <param name="atPosition">The position of the container being closed, as seen by the player.</param>
        public void CloseContainer(uint forCreatureId, IContainerItem container, byte atPosition)
        {
            container.ThrowIfNull(nameof(container));

            // Check if this creature doesn't have this container open, or it's no longer at the position specified.
            if (!this.IsContainerOpen(container.UniqueId, forCreatureId, out IEnumerable<byte> openPositions) || !openPositions.Contains(atPosition))
            {
                return;
            }

            this.CloseContainerInternal(forCreatureId, container, atPosition);

            if (this.creatureFinder.FindPlayerById(forCreatureId) is IPlayer player)
            {
                this.scheduler.ScheduleEvent(new GenericNotification(() => player.YieldSingleItem(), new ContainerClosePacket(atPosition)));
            }
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
                if (this.creaturesToContainers.ContainsKey(creatureId) && atPosition < ItemConstants.MaxContainersPerCreature)
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
        /// <returns>The position of container found, or <see cref="ItemConstants.UnsetContainerPosition"/>> if not found.</returns>
        public byte FindForCreature(uint creatureId, IContainerItem container)
        {
            if (container == null)
            {
                return ItemConstants.UnsetContainerPosition;
            }

            lock (this.internalDictionariesLock)
            {
                if (this.creaturesToContainers.ContainsKey(creatureId))
                {
                    for (byte i = 0; i < ItemConstants.MaxContainersPerCreature; i++)
                    {
                        if (this.creaturesToContainers[creatureId][i] != null && this.creaturesToContainers[creatureId][i].UniqueId == container.UniqueId)
                        {
                            return i;
                        }
                    }
                }

                return ItemConstants.UnsetContainerPosition;
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

        /// <summary>
        /// Checks if the specified container is open by a creature and retrieves any positions where it is.
        /// </summary>
        /// <param name="containerUniqueId">The unique id of the container.</param>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="openPositions">A collection of positions in which the container is open.</param>
        /// <returns>True if the container is open for the creature, false otherwise.</returns>
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
                }
            }

            return openPositions != null;
        }

        /// <summary>
        /// Gets the container for a creature at a given position.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="atPosition">The position to check.</param>
        /// <returns>The container found at the position, or null if there is none.</returns>
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

        /// <summary>
        /// Closes a container for a creature at a given position.
        /// </summary>
        /// <param name="forCreatureId">The creature to check for.</param>
        /// <param name="container">The container to close.</param>
        /// <param name="atPosition">The position at which to close the container.</param>
        private void CloseContainerInternal(uint forCreatureId, IContainerItem container, byte atPosition)
        {
            lock (this.internalDictionariesLock)
            {
                if (!this.creaturesToContainers.ContainsKey(forCreatureId) ||
                    atPosition >= this.creaturesToContainers[forCreatureId].Length ||
                    container.UniqueId != this.creaturesToContainers[forCreatureId][atPosition].UniqueId)
                {
                    return;
                }

                // For the per-creature map, we need only close the one at the specific position.
                this.creaturesToContainers[forCreatureId][atPosition] = null;

                // For the containers map, we need to get a bit fancy.
                if (this.containersToCreatureIds.ContainsKey(container.UniqueId))
                {
                    if (this.containersToCreatureIds[container.UniqueId][atPosition] == forCreatureId)
                    {
                        this.containersToCreatureIds[container.UniqueId].Remove(atPosition);
                    }

                    // Clean up if this list is now empty.
                    if (this.containersToCreatureIds[container.UniqueId].Count == 0)
                    {
                        this.containersToCreatureIds.Remove(container.UniqueId);

                        // Clean up events because no one else cares about this container.
                        container.ContentAdded -= this.OnContainerContentAdded;
                        container.ContentRemoved -= this.OnContainerContentRemoved;
                        container.ContentUpdated -= this.OnContainerContentUpdated;
                        container.LocationChanged -= this.OnContainerLocationChanged;
                    }
                }

                this.logger.Verbose($"Creature with id {forCreatureId} closed a {container.Type.Name} at position {atPosition}.");
            }
        }

        /// <summary>
        /// Opens a container for a given creature at the specified position.
        /// </summary>
        /// <param name="forCreatureId">The id of the creature for which to open the container.</param>
        /// <param name="container">The container to open.</param>
        /// <param name="atPosition">The position at which to open the container.</param>
        /// <returns>The position at which the container was actually opened, which may not be the given <paramref name="atPosition"/>.</returns>
        private byte OpenContainerInternal(uint forCreatureId, IContainerItem container, byte atPosition)
        {
            lock (this.internalDictionariesLock)
            {
                byte openedAt = ItemConstants.UnsetContainerPosition;

                if (!this.creaturesToContainers.ContainsKey(forCreatureId))
                {
                    this.creaturesToContainers.Add(forCreatureId, new IContainerItem[ItemConstants.MaxContainersPerCreature]);
                }

                if (atPosition >= ItemConstants.MaxContainersPerCreature)
                {
                    // Find any available position.
                    for (byte i = 0; i < ItemConstants.MaxContainersPerCreature; i++)
                    {
                        if (this.creaturesToContainers[forCreatureId][i] != null)
                        {
                            continue;
                        }

                        openedAt = i;
                        this.creaturesToContainers[forCreatureId][i] = container;

                        break;
                    }
                }
                else
                {
                    openedAt = atPosition;
                    this.creaturesToContainers[forCreatureId][atPosition] = container;
                }

                // Now add to the other index per container.
                if (!this.containersToCreatureIds.ContainsKey(container.UniqueId))
                {
                    this.containersToCreatureIds.Add(container.UniqueId, new Dictionary<byte, uint>());

                    // This container is not being tracked at all, let's start tracking it.
                    container.ContentAdded += this.OnContainerContentAdded;
                    container.ContentRemoved += this.OnContainerContentRemoved;
                    container.ContentUpdated += this.OnContainerContentUpdated;
                    container.LocationChanged += this.OnContainerLocationChanged;
                }

                this.containersToCreatureIds[container.UniqueId][openedAt] = forCreatureId;

                this.logger.Verbose($"Creature with id {forCreatureId} opened a {container.Type.Name} at position {openedAt}.");

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
                if (!this.containersToCreatureIds.ContainsKey(container.UniqueId))
                {
                    return;
                }

                // The request has to be sent this way since the container id may be different for each player.
                foreach (var (containerPosition, creatureId) in this.containersToCreatureIds[container.UniqueId].ToList())
                {
                    if (!(this.creatureFinder.FindPlayerById(creatureId) is IPlayer player))
                    {
                        continue;
                    }

                    var notification = new GenericNotification(() => player.YieldSingleItem(), new ContainerAddItemPacket(containerPosition, addedItem));

                    this.scheduler.ScheduleEvent(notification);
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
                if (!this.containersToCreatureIds.ContainsKey(container.UniqueId))
                {
                    return;
                }

                // The request has to be sent this way since the container id may be different for each player.
                foreach (var (containerId, creatureId) in this.containersToCreatureIds[container.UniqueId].ToList())
                {
                    if (!(this.creatureFinder.FindPlayerById(creatureId) is IPlayer player))
                    {
                        continue;
                    }

                    var notification = new GenericNotification(() => player.YieldSingleItem(), new ContainerRemoveItemPacket(indexRemoved, containerId));

                    this.scheduler.ScheduleEvent(notification);
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
                if (!this.containersToCreatureIds.ContainsKey(container.UniqueId))
                {
                    return;
                }

                // The request has to be sent this way since the container id may be different for each player.
                foreach (var (containerId, creatureId) in this.containersToCreatureIds[container.UniqueId].ToList())
                {
                    if (!(this.creatureFinder.FindPlayerById(creatureId) is IPlayer player))
                    {
                        continue;
                    }

                    var notification = new GenericNotification(() => player.YieldSingleItem(), new ContainerUpdateItemPacket(indexOfUpdated, containerId, updatedItem));

                    this.scheduler.ScheduleEvent(notification);
                }
            }
        }

        /// <summary>
        /// Handles a change event from a container.
        /// </summary>
        /// <param name="containerThatChangedAsThing">The container that changed.</param>
        /// <param name="previousLocation">The container's previous location.</param>
        private void OnContainerLocationChanged(IThing containerThatChangedAsThing, Location previousLocation)
        {
            lock (this.internalDictionariesLock)
            {
                if (!(containerThatChangedAsThing is IContainerItem containerItem) ||
                    this.containersToCreatureIds.ContainsKey(containerItem.UniqueId))
                {
                    return;
                }

                if (containerItem.CarryLocation != null)
                {
                    // Container is held by a creature, which is the only one that should have access now.
                    var creatureHoldingTheContainer = containerItem.GetCarrier();

                    if (creatureHoldingTheContainer != null && this.containersToCreatureIds.ContainsKey(containerItem.UniqueId))
                    {
                        foreach (var (containerId, creatureId) in this.containersToCreatureIds[containerItem.UniqueId].ToList())
                        {
                            if (creatureHoldingTheContainer.Id == creatureId)
                            {
                                continue;
                            }

                            this.CloseContainer(creatureId, containerItem, containerId);
                        }
                    }
                }
                else if (containerItem.Location.Type == LocationType.Map && this.containersToCreatureIds.ContainsKey(containerItem.UniqueId))
                {
                    // Container was dropped or placed in a container that ultimately sits on the map, figure out which creatures are still in range.
                    foreach (var (containerId, creatureId) in this.containersToCreatureIds[containerItem.UniqueId].ToList())
                    {
                        if (!(this.creatureFinder.FindPlayerById(creatureId) is IPlayer player))
                        {
                            continue;
                        }

                        var locationDiff = containerItem.Location - player.Location;

                        if (locationDiff.MaxValueIn2D > 1 || locationDiff.Z != 0)
                        {
                            this.CloseContainer(creatureId, containerItem, containerId);
                        }
                    }
                }
            }
        }
    }
}
