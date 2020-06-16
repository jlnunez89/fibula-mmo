// -----------------------------------------------------------------
// <copyright file="BaseActionOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Actions
{
    using System;

    /// <summary>
    /// Class that represents a base combat operation.
    /// </summary>
    public abstract class BaseActionOperation : Operation, IActionOperation
    {
        /// <summary>
        /// The default exhaustion cost for action operations.
        /// </summary>
        protected static readonly TimeSpan DefaultActionExhaustionCost = TimeSpan.Zero;

        /// <summary>
        /// Default delay for player actions.
        /// </summary>
        protected static readonly TimeSpan DefaultPlayerActionDelay = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseActionOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="actionExhaustionCost">Optional. The cost of this operation. Defaults to <see cref="DefaultActionExhaustionCost"/>.</param>
        public BaseActionOperation(uint requestorId, TimeSpan? actionExhaustionCost = null)
            : base(requestorId)
        {
            this.ExhaustionCost = actionExhaustionCost ?? DefaultActionExhaustionCost;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Action;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }
    }
}