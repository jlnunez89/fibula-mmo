// -----------------------------------------------------------------
// <copyright file="ElevatedOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Mechanics.Contracts.Abstractions;

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
