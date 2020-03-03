// -----------------------------------------------------------------
// <copyright file="ElevatedOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents an elevated base between game operations, for which the context changes.
    /// </summary>
    public abstract class ElevatedOperation : BaseOperation, IElevatedOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatedOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationContext">A reference to this operation's context.</param>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        protected ElevatedOperation(ILogger logger, IElevatedOperationContext operationContext, uint requestorId)
            : base(logger, operationContext, requestorId)
        {
            operationContext.ThrowIfNull(nameof(operationContext));

            this.Context = operationContext;
        }

        /// <summary>
        /// Gets a reference to this operation's context.
        /// </summary>
        public new IElevatedOperationContext Context { get; }
    }
}
