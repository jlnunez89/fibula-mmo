// -----------------------------------------------------------------
// <copyright file="UseEventRuleArguments.cs" company="2Dudes">
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
    /// Class that represents arguments for a use item event rule.
    /// </summary>
    public class UseEventRuleArguments : IEventRuleArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseEventRuleArguments"/> class.
        /// </summary>
        /// <param name="itemBeingUsed">The item being used.</param>
        /// <param name="requestingCreature">Optional. The creature requesting the event.</param>
        public UseEventRuleArguments(IItem itemBeingUsed, ICreature requestingCreature = null)
        {
            itemBeingUsed.ThrowIfNull(nameof(itemBeingUsed));

            this.ItemUsed = itemBeingUsed;
            this.Requestor = requestingCreature;
        }

        /// <summary>
        /// Gets the item being used.
        /// </summary>
        public IItem ItemUsed { get; }

        /// <summary>
        /// Gets the creature who requested the use, if any.
        /// </summary>
        public ICreature Requestor { get; }
    }
}