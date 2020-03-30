// -----------------------------------------------------------------
// <copyright file="MultiUseEventRuleArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for a use item on event rule.
    /// </summary>
    public class MultiUseEventRuleArguments : IEventRuleArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiUseEventRuleArguments"/> class.
        /// </summary>
        /// <param name="itemBeingUsed">The item being used.</param>
        /// <param name="useOnThing">The thing to use the item on.</param>
        /// <param name="requestingCreature">Optional. The creature requesting the event.</param>
        public MultiUseEventRuleArguments(IItem itemBeingUsed, IThing useOnThing, ICreature requestingCreature = null)
        {
            itemBeingUsed.ThrowIfNull(nameof(itemBeingUsed));

            this.ItemUsed = itemBeingUsed;
            this.UseOnThing = useOnThing;
            this.Requestor = requestingCreature;
        }

        /// <summary>
        /// Gets or sets the item being used.
        /// </summary>
        public IItem ItemUsed { get; set; }

        /// <summary>
        /// Gets or sets the thing to use on.
        /// </summary>
        public IThing UseOnThing { get; set; }

        /// <summary>
        /// Gets or sets the creature who requested the use, if any.
        /// </summary>
        public ICreature Requestor { get; set; }
    }
}