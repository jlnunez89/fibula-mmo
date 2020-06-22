// -----------------------------------------------------------------
// <copyright file="TurnToDirectionOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations
{
    using System;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Notifications.Arguments;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Contracts.Extensions;
    using Fibula.Server.Notifications;
    using Fibula.Server.Operations.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an event for a creature turning.
    /// </summary>
    public class TurnToDirectionOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnToDirectionOperation"/> class.
        /// </summary>
        /// <param name="creature">The creature which is turning.</param>
        /// <param name="direction">The direction to which the creature is turning.</param>
        public TurnToDirectionOperation(ICreature creature, Direction direction)
            : base(creature.Id)
        {
            this.Creature = creature;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets a reference to the creature turning.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the direction in which the creature is turning.
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            // Perform the actual, internal turn.
            this.Creature.TurnToDirection(this.Direction);

            // Send the notification if applicable.
            if (context.TileAccessor.GetTileAt(this.Creature.Location, out ITile playerTile))
            {
                var playerStackPos = playerTile.GetStackPositionOfThing(this.Creature);

                context.Scheduler.ScheduleEvent(
                    new CreatureTurnedNotification(
                        () => context.CreatureFinder.PlayersThatCanSee(context.TileAccessor, this.Creature.Location),
                        new CreatureTurnedNotificationArguments(this.Creature, playerStackPos)));
            }
        }
    }
}
