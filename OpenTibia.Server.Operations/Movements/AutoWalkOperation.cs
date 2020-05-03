// -----------------------------------------------------------------
// <copyright file="AutoWalkOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Operations.Arguments;

    /// <summary>
    /// Class that represents a logout operation.
    /// </summary>
    public class AutoWalkOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoWalkOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="creature">The creature walking.</param>
        /// <param name="directions">The directions to walk to.</param>
        /// <param name="stepIndex">Optional. The index of the current direction.</param>
        public AutoWalkOperation(uint requestorId, ICreature creature, Direction[] directions, int stepIndex = 0)
            : base(requestorId)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.StepIndex = stepIndex;
            this.Locations = new Location[directions.Length];

            var location = creature.Location;
            for (int i = 0; i < directions.Length; i++)
            {
                this.Locations[i] = location.LocationAt(directions[i]);

                location = this.Locations[i];
            }
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Movement;

        /// <summary>
        /// Gets the creature that is walking.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the locations at which the walk is supposed to happen.
        /// </summary>
        public Location[] Locations { get; }

        /// <summary>
        /// Gets the index of the current direction.
        /// </summary>
        public int StepIndex { get; private set; }

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            // Cancel any movement waiting to be executed.
            context.Scheduler.CancelAllFor(this.Creature.Id, typeof(IMovementOperation));
            context.EventRulesApi.ClearAllFor(this.GetPartitionKey());

            if (this.StepIndex >= this.Locations.Length)
            {
                return;
            }

            TimeSpan movementDelay = this.Creature.CalculateRemainingCooldownTime(ExhaustionType.Movement, context.Scheduler.CurrentTime);

            var fromTile = context.TileAccessor.GetTileAt(this.Creature.Location);

            var nextLocation = this.Locations[this.StepIndex];

            var movementOperation = context.OperationFactory.Create(
                OperationType.Movement,
                new MovementOperationCreationArguments(
                    this.RequestorId,
                    ICreature.CreatureThingId,
                    this.Creature.Location,
                    fromTile.GetStackPositionOfThing(this.Creature),
                    fromCreatureId: 0,
                    nextLocation,
                    toCreatureId: 0));

            context.Scheduler.ScheduleEvent(movementOperation, movementDelay);

            // Increment the step and check if we need to worry about further steps.
            if (++this.StepIndex < this.Locations.Length)
            {
                // Setup as a movement rule, so that it gets expedited when the combatant is in range from it's target.
                var conditionsForExpedition = new Func<IEventRuleContext, bool>[]
                {
                    (context) =>
                    {
                        if (!(context.Arguments is MovementEventRuleArguments movementEventRuleArguments) ||
                            !(movementEventRuleArguments.ThingMoving is ICreature creature))
                        {
                            return false;
                        }

                        return creature.Location == nextLocation;
                    },
                };

                context.EventRulesApi.SetupRule(new ExpediteOperationMovementEventRule(context.Logger, this, conditionsForExpedition, totalExecutionCount: 1), this.GetPartitionKey());
            }
        }
    }
}
