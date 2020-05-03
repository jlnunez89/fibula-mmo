﻿// -----------------------------------------------------------------
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
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an elevated base between game operations, for which the context changes.
    /// </summary>
    public abstract class ElevatedOperation : Operation, IElevatedOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatedOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        protected ElevatedOperation(uint requestorId)
            : base(requestorId)
        {
        }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">The execution context for this operation.</param>
        protected override void Execute(IOperationContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!typeof(IElevatedOperationContext).IsAssignableFrom(context.GetType()))
            {
                throw new ArgumentException($"{nameof(context)} must be an {nameof(IElevatedOperationContext)}.");
            }

            this.Execute(context as IElevatedOperationContext);
        }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">The execution context for this operation.</param>
        protected abstract void Execute(IElevatedOperationContext context);
    }
}