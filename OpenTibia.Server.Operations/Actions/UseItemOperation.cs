// -----------------------------------------------------------------
// <copyright file="UseItemOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using Serilog;

    /// <summary>
    /// Class that represents an event for an item use.
    /// </summary>
    public class UseItemOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The id of the item being used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromStackPos">The position in the stack from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        public UseItemOperation(ILogger logger, IOperationContext context, uint requestorId, ushort typeId, Location fromLocation, byte fromStackPos = byte.MaxValue, byte index = 1)
            : base(logger, context, requestorId)
        {
            this.ActionsOnPass.Add(() =>
            {
                var fromCylinder = fromLocation.GetCyclinder(this.Context.TileAccessor, this.Context.ContainerManager, ref fromStackPos, ref index, this.Requestor);
                var item = fromLocation.FindItemById(this.Context.TileAccessor, this.Context.ContainerManager, typeId, this.Requestor);

                bool successfulUse = this.PerformItemUse(item, fromCylinder, index, this.Requestor, fromLocation.Type == LocationType.InsideContainer ? (byte?)fromLocation.ContainerId : null);

                if (!successfulUse)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }
            });
        }

        /// <summary>
        /// Immediately attempts to perform an item use in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="item">The item being used.</param>
        /// <param name="fromCylinder">The cylinder from which the use is happening.</param>
        /// <param name="index">The index within the cylinder from which to use the item.</param>
        /// <param name="requestor">The creature requesting the use.</param>
        /// <param name="containerId">Optional. The id of the container as which to open, in case this is a container.</param>
        /// <returns>True if the item was successfully used, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        private bool PerformItemUse(IItem item, ICylinder fromCylinder, byte index, ICreature requestor, byte? containerId = null)
        {
            if (item == null || fromCylinder == null)
            {
                return false;
            }

            if (this.TriggerUseEventRules(new UseEventRuleArguments(item, requestor)))
            {
                requestor.AddExhaustion(this.ExhaustionType, this.Context.Scheduler.CurrentTime, DefaultPlayerActionDelay);

                return true;
            }

            // Change this item into the target.
            if (item.ChangesOnUse && this.PerformItemChange(item, item.ChangeOnUseTo, fromCylinder, index, requestor))
            {
                requestor.AddExhaustion(this.ExhaustionType, this.Context.Scheduler.CurrentTime, DefaultPlayerActionDelay);

                return true;
            }

            if (item is IContainerItem container && requestor is IPlayer player)
            {
                var currentPosition = this.Context.ContainerManager.FindForCreature(player.Id, container);

                if (currentPosition == IContainerManager.UnsetContainerPosition)
                {
                    // Player doesn't have this container open, so open.
                    this.Context.ContainerManager.OpenContainer(player, container, containerId ?? IContainerManager.UnsetContainerPosition);
                }
                else
                {
                    // Close the container for this player.
                    this.Context.ContainerManager.CloseContainer(player, container, currentPosition);
                }

                return true;
            }

            return false;
        }
    }
}
