// -----------------------------------------------------------------
// <copyright file="TurnToDirectionOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Actions
{
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;

    /// <summary>
    /// Class that represents an event for a creature turning.
    /// </summary>
    public class TurnToDirectionOperation : BaseActionOperation
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
                        () => context.ConnectionFinder.PlayersThatCanSee(context.CreatureFinder, this.Creature.Location),
                        new CreatureTurnedNotificationArguments(this.Creature, playerStackPos)));
            }
        }
    }
}
