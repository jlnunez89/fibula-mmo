// -----------------------------------------------------------------
// <copyright file="BaseMovementOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Movements
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations;
    using Serilog;

    /// <summary>
    /// Class that represents a common base between movements.
    /// </summary>
    public abstract class BaseMovementOperation : BaseOperation, IMovementOperation
    {
        private static readonly TimeSpan DefaultMovementExhaustionCost = TimeSpan.Zero;

        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="fromCylinder">The cyclinder from which the movement is happening.</param>
        /// <param name="toCylinder">The cyclinder to which the movement is happening.</param>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="movementExhaustionCost">Optional. The cost of this operation. Defaults to <see cref="DefaultMovementExhaustionCost"/>.</param>
        protected BaseMovementOperation(
            ILogger logger,
            IOperationContext context,
            ICylinder fromCylinder,
            ICylinder toCylinder,
            uint requestorId,
            TimeSpan? movementExhaustionCost = null)
            : base(logger, context, requestorId)
        {
            this.FromCylinder = fromCylinder;
            this.ToCylinder = toCylinder;
            this.ExhaustionCost = movementExhaustionCost ?? DefaultMovementExhaustionCost;
        }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (this.RequestorId > 0 && this.requestor == null)
                {
                    this.requestor = this.Context.CreatureFinder.FindCreatureById(this.RequestorId);
                }

                return this.requestor;
            }
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Movement;

        /// <summary>
        /// Gets the cylinder from which the movement happens.
        /// </summary>
        public ICylinder FromCylinder { get; }

        /// <summary>
        /// Gets the cylinder to which the movement happens.
        /// </summary>
        public ICylinder ToCylinder { get; }

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Immediately attempts to perform an item movement between two cylinders.
        /// </summary>
        /// <param name="item">The item being moved.</param>
        /// <param name="fromCylinder">The cylinder from which the movement is being performed.</param>
        /// <param name="toCylinder">The cylinder to which the movement is being performed.</param>
        /// <param name="fromIndex">Optional. The index within the cylinder to move the item from.</param>
        /// <param name="toIndex">Optional. The index within the cylinder to move the item to.</param>
        /// <param name="amountToMove">Optional. The amount of the thing to move. Defaults to 1.</param>
        /// <param name="requestorCreature">Optional. The creature that this movement is being performed in behalf of, if any.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        protected bool PerformItemMovement(IItem item, ICylinder fromCylinder, ICylinder toCylinder, byte fromIndex = 0xFF, byte toIndex = 0xFF, byte amountToMove = 1, ICreature requestorCreature = null)
        {
            const byte FallbackIndex = 0xFF;

            if (item == null || fromCylinder == null || toCylinder == null)
            {
                return false;
            }

            var sameCylinder = fromCylinder == toCylinder;

            if (sameCylinder && fromIndex == toIndex)
            {
                // no change at all.
                return true;
            }

            // Edge case, check if the moving item is the target container.
            if (item is IContainerItem containerItem && toCylinder is IContainerItem targetContainer && targetContainer.IsChildOf(containerItem))
            {
                return false;
            }

            IThing itemAsThing = item as IThing;

            (bool removeSuccessful, IThing removeRemainder) = fromCylinder.RemoveContent(this.Context.ItemFactory, ref itemAsThing, fromIndex, amount: amountToMove);

            if (!removeSuccessful)
            {
                // Failing to remove the item from the original cylinder stops the entire operation.
                return false;
            }

            if (fromCylinder is ITile fromTile)
            {
                this.Context.Scheduler.ScheduleEvent(
                    new TileUpdatedNotification(
                        this.Logger,
                        this.Context.CreatureFinder,
                        () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, fromTile.Location),
                        new TileUpdatedNotificationArguments(fromTile.Location, this.Context.MapDescriptor.DescribeTile)));
            }

            this.TriggerSeparationEventRules(new SeparationEventRuleArguments(fromCylinder.Location, item, requestorCreature));

            IThing addRemainder = itemAsThing;

            if (sameCylinder && removeRemainder == null && fromIndex < toIndex)
            {
                // If the move happens within the same cylinder, we need to adjust the index of where we're adding, depending if it is before or after.
                toIndex--;
            }

            if (!this.AddContentToCylinderChain(toCylinder.GetCylinderHierarchy(includeTiles: false), toIndex, ref addRemainder, requestorCreature) || addRemainder != null)
            {
                // There is some rollback to do, as we failed to add the entire thing.
                IThing rollbackRemainder = addRemainder ?? item;

                if (!this.AddContentToCylinderChain(fromCylinder.GetCylinderHierarchy(), FallbackIndex, ref rollbackRemainder, requestorCreature))
                {
                    this.Logger.Error($"Rollback failed on {nameof(this.PerformItemMovement)}. Thing: {rollbackRemainder.DescribeForLogger()}");
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a throw between two map locations is valid.
        /// </summary>
        /// <param name="fromLocation">The first location.</param>
        /// <param name="toLocation">The second location.</param>
        /// <param name="checkLineOfSight">Optional. A value indicating whether to consider line of sight.</param>
        /// <returns>True if the throw is valid, false otherwise.</returns>
        protected bool CanThrowBetweenMapLocations(Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            if (fromLocation.Type != LocationType.Map || toLocation.Type != LocationType.Map)
            {
                return false;
            }

            if (fromLocation == toLocation)
            {
                return true;
            }

            // Cannot throw across the surface boundary (floor 7).
            if ((fromLocation.Z >= 8 && toLocation.Z <= 7) || (toLocation.Z >= 8 && fromLocation.Z <= 7))
            {
                return false;
            }

            var deltaX = Math.Abs(fromLocation.X - toLocation.X);
            var deltaY = Math.Abs(fromLocation.Y - toLocation.Y);
            var deltaZ = Math.Abs(fromLocation.Z - toLocation.Z);

            // distance checks
            if (deltaX - deltaZ >= (IMap.DefaultWindowSizeX / 2) || deltaY - deltaZ >= (IMap.DefaultWindowSizeY / 2))
            {
                return false;
            }

            return !checkLineOfSight || this.InLineOfSight(fromLocation, toLocation) || this.InLineOfSight(toLocation, fromLocation);
        }

        /// <summary>
        /// Checks if a map location is in the line of sight of another map location.
        /// </summary>
        /// <param name="firstLocation">The first location.</param>
        /// <param name="secondLocation">The second location.</param>
        /// <returns>True if the second location is considered within the line of sight of the first location, false otherwise.</returns>
        protected bool InLineOfSight(Location firstLocation, Location secondLocation)
        {
            if (firstLocation.Type != LocationType.Map || secondLocation.Type != LocationType.Map)
            {
                return false;
            }

            if (firstLocation == secondLocation)
            {
                return true;
            }

            // Normalize so that the check always happens from 'high to low' floors.
            var origin = firstLocation.Z > secondLocation.Z ? secondLocation : firstLocation;
            var target = firstLocation.Z > secondLocation.Z ? firstLocation : secondLocation;

            // Define positive or negative steps, depending on where the target location is wrt the origin location.
            var stepX = (sbyte)(origin.X < target.X ? 1 : origin.X == target.X ? 0 : -1);
            var stepY = (sbyte)(origin.Y < target.Y ? 1 : origin.Y == target.Y ? 0 : -1);

            var a = target.Y - origin.Y;
            var b = origin.X - target.X;
            var c = -((a * target.X) + (b * target.Y));

            while ((origin - target).MaxValueIn2D != 0)
            {
                var moveHorizontal = Math.Abs((a * (origin.X + stepX)) + (b * origin.Y) + c);
                var moveVertical = Math.Abs((a * origin.X) + (b * (origin.Y + stepY)) + c);
                var moveCross = Math.Abs((a * (origin.X + stepX)) + (b * (origin.Y + stepY)) + c);

                if (origin.Y != target.Y && (origin.X == target.X || moveHorizontal > moveVertical || moveHorizontal > moveCross))
                {
                    origin.Y += stepY;
                }

                if (origin.X != target.X && (origin.Y == target.Y || moveVertical > moveHorizontal || moveVertical > moveCross))
                {
                    origin.X += stepX;
                }

                if (this.Context.TileAccessor.GetTileAt(origin, out ITile tile) && tile.BlocksThrow)
                {
                    return false;
                }
            }

            while (origin.Z != target.Z)
            {
                // now we need to perform a jump between floors to see if everything is clear (literally)
                if (this.Context.TileAccessor.GetTileAt(origin, out ITile tile) && tile.Ground != null)
                {
                    return false;
                }

                origin.Z++;
            }

            return true;
        }
    }
}
