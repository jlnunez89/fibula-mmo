// -----------------------------------------------------------------
// <copyright file="LookAtOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Common.Utilities.Extensions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;

    /// <summary>
    /// Class that represents an event for a thing description.
    /// </summary>
    public class LookAtOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookAtOperation"/> class.
        /// </summary>
        /// <param name="thingId">The id of the thing to describe.</param>
        /// <param name="location">The location where the thing to describe is.</param>
        /// <param name="stackPosition">The position in the stack at the location of the thing to describe is.</param>
        /// <param name="playerToDescribeFor">The player to describe the thing for.</param>
        public LookAtOperation(ushort thingId, Location location, byte stackPosition, IPlayer playerToDescribeFor)
            : base(playerToDescribeFor?.Id ?? 0)
        {
            this.ThingId = thingId;
            this.Location = location;
            this.StackPosition = stackPosition;
            this.PlayerToDescribeFor = playerToDescribeFor;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the id of the thing to describe.
        /// </summary>
        public ushort ThingId { get; }

        /// <summary>
        /// Gets the location where the thing to describe is.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the position in the stack at the location of the thing to describe is.
        /// </summary>
        public byte StackPosition { get; }

        /// <summary>
        /// Gets the player to describe for.
        /// </summary>
        public IPlayer PlayerToDescribeFor { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            IThing thing = null;

            if (this.Location.Type != LocationType.Map || this.PlayerToDescribeFor.CanSee(this.Location))
            {
                IContainerItem container;

                // Get thing at location
                switch (this.Location.Type)
                {
                    case LocationType.Map:
                        thing = context.Map.GetTileAt(this.Location, out ITile targetTile) ? targetTile.TopThing : null;
                        break;
                    case LocationType.InsideContainer:
                        container = context.ContainerManager.FindForCreature(this.PlayerToDescribeFor.Id, this.Location.ContainerId);

                        thing = container?[this.Location.ContainerIndex];
                        break;
                    case LocationType.InventorySlot:
                        container = this.PlayerToDescribeFor.Inventory[(byte)this.Location.Slot] as IContainerItem;

                        thing = container?.Content.FirstOrDefault();
                        break;
                }
            }

            if (thing == null)
            {
                return;
            }

            if (thing != null)
            {
                context.Logger.Debug($"Player {this.PlayerToDescribeFor.Name} looking at {thing}. {this.Location} sector: {this.Location.X / 32}-{this.Location.Y / 32}-{this.Location.Z:00}");
            }

            string description = string.Empty;

            if (thing is IItem itemToDescribe)
            {
                description = this.DescribeItem(itemToDescribe);
            }
            else if (thing is ICreature creatureToDescribe)
            {
                description = $"{creatureToDescribe.Article} {creatureToDescribe.Name}.";
            }

            // TODO: support other things.
            if (string.IsNullOrWhiteSpace(description))
            {
                return;
            }

            description = $"You see {description}";

            new TextMessageNotification(
                () => this.PlayerToDescribeFor.YieldSingleItem(),
                new TextMessageNotificationArguments(MessageType.DescriptionGreen, description))
            .Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));
        }

        private string DescribeItem(IItem itemToDescribe)
        {
            string description = $"{itemToDescribe.Type.Name}";

            if (itemToDescribe.Type.Flags.Contains(ItemFlag.IsLiquidPool) && itemToDescribe.Attributes.ContainsKey(ItemAttribute.LiquidType))
            {
                var liquidTypeName = ((LiquidType)itemToDescribe.Attributes[ItemAttribute.LiquidType]).ToString().ToLower();

                description += $" of {liquidTypeName}";
            }

            if (itemToDescribe.Type.Flags.Contains(ItemFlag.IsLiquidContainer) && itemToDescribe.Attributes.ContainsKey(ItemAttribute.LiquidType))
            {
                var liquidTypeName = ((LiquidType)itemToDescribe.Attributes[ItemAttribute.LiquidType]).ToString().ToLower();

                description += $" of {liquidTypeName}";
            }

            description += $".";

            if (itemToDescribe.Amount > 1)
            {
                // TODO: naive solution, add S to pluralize will produce some spelling errors.
                description = $"{itemToDescribe.Amount} {description.TrimStartArticles().TrimEnd('.')}s.";
            }

            if (!string.IsNullOrWhiteSpace(itemToDescribe.Type.Description))
            {
                description += $"\n{itemToDescribe.Type.Description}";
            }

            if (itemToDescribe.Type.Flags.Contains(ItemFlag.IsReadable) && itemToDescribe.Attributes.ContainsKey(ItemAttribute.Text) && itemToDescribe.Attributes.ContainsKey(ItemAttribute.ReadRange))
            {
                var text = itemToDescribe.Attributes[ItemAttribute.Text] as string;
                var fontSize = itemToDescribe.Attributes[ItemAttribute.ReadRange] as int? ?? 0;

                var locationDiff = itemToDescribe.Location - this.PlayerToDescribeFor.Location;
                var readingDistance = itemToDescribe.CarryLocation != null ? 0 : itemToDescribe.Location.Type != LocationType.Map ? 0 : locationDiff.MaxValueIn2D + Math.Abs(locationDiff.Z * 10);

                switch (fontSize)
                {
                    case 0:
                        description += readingDistance <= 1 ? $"\n{Regex.Unescape(text).Trim('"')}." : string.Empty;
                        break;
                    case 1:
                        // Only on use, so nothing to add here.
                        break;
                    default:
                        // Distance calculation.
                        description += readingDistance <= fontSize ? $" It reads:\n{Regex.Unescape(text).Trim('"')}" : " You are too far away to read it.";
                        break;
                }
            }

            return Regex.Replace(description, @"[^\u0000-\u007F]+", string.Empty);
        }
    }
}
