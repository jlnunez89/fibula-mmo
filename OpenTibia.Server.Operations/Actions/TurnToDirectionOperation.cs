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
    using OpenTibia.Server.Operations.Notifications;
    using OpenTibia.Server.Operations.Notifications.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents an event for a creature turning.
    /// </summary>
    public class TurnToDirectionOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnToDirectionOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation's context.</param>
        /// <param name="creature">The creature which is turning.</param>
        /// <param name="direction">The direction to which the creature is turning.</param>
        public TurnToDirectionOperation(ILogger logger, IOperationContext context, ICreature creature, Direction direction)
            : base(logger, context, creature.Id)
        {
            this.ActionsOnPass.Add(() =>
            {
                creature.TurnToDirection(direction);

                if (this.Context.TileAccessor.GetTileAt(creature.Location, out ITile playerTile))
                {
                    var playerStackPos = playerTile.GetStackPositionOfThing(creature);

                    this.Context.Scheduler.ImmediateEvent(
                        new CreatureTurnedNotification(
                            this.Logger,
                            () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, creature.Location),
                            new CreatureTurnedNotificationArguments(creature, playerStackPos)));
                }
            });
        }
    }
}
