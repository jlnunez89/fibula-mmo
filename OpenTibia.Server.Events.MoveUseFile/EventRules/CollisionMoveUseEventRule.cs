// -----------------------------------------------------------------
// <copyright file="CollisionMoveUseEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events.MoveUseFile.EventRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a collision event rule.
    /// </summary>
    internal class CollisionMoveUseEventRule : MoveUseEventRule, ICollisionEventRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionMoveUseEventRule"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameApi">A reference to the game API.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public CollisionMoveUseEventRule(ILogger logger, IGameApi gameApi, IList<string> conditionSet, IList<string> actionSet)
            : base(logger, gameApi, conditionSet, actionSet)
        {
            var isTypeCondition = this.Conditions.FirstOrDefault(func => IsTypeFunctionName.Equals(func.FunctionName));

            if (isTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find {IsTypeFunctionName} function.");
            }

            this.CollidingThingId = Convert.ToUInt16(isTypeCondition.Parameters[1]);
        }

        /// <summary>
        /// Gets the id of the thing involved in the collision.
        /// </summary>
        public ushort CollidingThingId { get; }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public override EventRuleType Type => EventRuleType.Collision;

        /// <summary>
        /// Checks whether this event rule can be executed.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        /// <returns>True if the rule can be executed, false otherwise.</returns>
        public override bool CanBeExecuted(IEventRuleContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!(context.Arguments is CollisionEventRuleArguments collisionRuleArgs) ||
                collisionRuleArgs.ThingMoving == null ||
                !context.TileAccessor.GetTileAt(collisionRuleArgs.AtLocation, out ITile toTile) ||
                !toTile.HasCollisionEvents)
            {
                return false;
            }

            foreach (var item in toTile.ItemsWithCollision.Where(i => i.Type.TypeId == this.CollidingThingId))
            {
                return this.Conditions.All(condition =>
                    this.InvokeCondition(
                        item,
                        collisionRuleArgs.ThingMoving,
                        collisionRuleArgs.Requestor as IPlayer,
                        condition.FunctionName,
                        condition.Parameters));
            }

            return false;
        }

        /// <summary>
        /// Executes this event rule.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        public override void Execute(IEventRuleContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!(context.Arguments is CollisionEventRuleArguments collisionRuleArgs) ||
                collisionRuleArgs.ThingMoving == null ||
                !context.TileAccessor.GetTileAt(collisionRuleArgs.AtLocation, out ITile toTile) ||
                !toTile.HasCollisionEvents)
            {
                return;
            }

            foreach (var item in toTile.ItemsWithCollision.Where(i => i.Type.TypeId == this.CollidingThingId))
            {
                IThing primaryThing = item;
                IThing secondaryThing = collisionRuleArgs.ThingMoving;
                IPlayer user = collisionRuleArgs.Requestor as IPlayer;

                foreach (var action in this.Actions)
                {
                    this.InvokeAction(ref primaryThing, ref secondaryThing, ref user, action.FunctionName, action.Parameters);
                }

                collisionRuleArgs.ThingMoving = secondaryThing;
                collisionRuleArgs.Requestor = user;
            }

            context.Arguments = collisionRuleArgs;

            if (this.RemainingExecutionCount != IEventRule.UnlimitedExecutions && this.RemainingExecutionCount > 0)
            {
                this.RemainingExecutionCount--;
            }
        }
    }
}