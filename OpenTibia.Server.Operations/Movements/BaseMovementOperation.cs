// -----------------------------------------------------------------
// <copyright file="BaseMovementOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations;
    using Serilog;

    /// <summary>
    /// Class that represents a common base between movements.
    /// </summary>
    public abstract class BaseMovementOperation : BaseOperation, IMovementOperation
    {
        private static readonly TimeSpan DefaultMovementExhaustionCost = TimeSpan.Zero;

        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="fromCylinder">The cyclinder from which the movement is happening.</param>
        /// <param name="toCylinder">The cyclinder to which the movement is happening.</param>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="movementExhaustionCost">Optional. The cost of this operation. Defaults to <see cref="DefaultMovementExhaustionCost"/>.</param>
        protected BaseMovementOperation(
            ILogger logger,
            IOperationContext context,
            ICylinder fromCylinder,
            ICylinder toCylinder,
            uint requestorId,
            TimeSpan? movementExhaustionCost = null)
            : base(logger, context, requestorId)
        {
            fromCylinder.ThrowIfNull(nameof(fromCylinder));
            toCylinder.ThrowIfNull(nameof(toCylinder));

            this.FromCylinder = fromCylinder;
            this.ToCylinder = toCylinder;
            this.ExhaustionCost = movementExhaustionCost ?? DefaultMovementExhaustionCost;
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
        public override ExhaustionType ExhaustionType => ExhaustionType.Movement;

        /// <summary>
        /// Gets the cylinder from which the movement happens.
        /// </summary>
        public ICylinder FromCylinder { get; }

        /// <summary>
        /// Gets the cylinder to which the movement happens.
        /// </summary>
        public ICylinder ToCylinder { get; }

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }
    }
}
