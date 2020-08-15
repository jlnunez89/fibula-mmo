// -----------------------------------------------------------------
// <copyright file="TurnToDirectionOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Notifications;

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
            this.Creature.Direction = this.Direction;

            // Send the notification if applicable.
            if (context.Map.GetTileAt(this.Creature.Location, out ITile playerTile))
            {
                var playerStackPos = playerTile.GetStackOrderOfThing(this.Creature);

                this.SendNotification(
                    context,
                    new CreatureTurnedNotification(
                        () => context.Map.PlayersThatCanSee(this.Creature.Location),
                        this.Creature,
                        playerStackPos));
            }
        }
    }
}
