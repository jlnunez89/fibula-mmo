// -----------------------------------------------------------------
// <copyright file="BaseActionOperation.cs" company="2Dudes">
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
    using System;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a base combat operation.
    /// </summary>
    public abstract class BaseActionOperation : BaseOperation, IActionOperation
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
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseActionOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="actionExhaustionCost">Optional. The cost of this operation. Defaults to <see cref="DefaultActionExhaustionCost"/>.</param>
        public BaseActionOperation(ILogger logger, IOperationContext context, uint requestorId, TimeSpan? actionExhaustionCost = null)
            : base(logger, context, requestorId)
        {
            this.ExhaustionCost = actionExhaustionCost ?? DefaultActionExhaustionCost;
        }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (this.RequestorId > 0 && this.requestor == null)
                {
                    this.requestor = this.Context.CreatureFinder.FindCreatureById(this.RequestorId);
                }

                return this.requestor;
            }
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Action;

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }
    }
}