// -----------------------------------------------------------------
// <copyright file="SpeechOperation.cs" company="2Dudes">
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
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;

    /// <summary>
    /// Class that represents a speech operation.
    /// </summary>
    public class SpeechOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechOperation"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="speechInfo"></param>
        public SpeechOperation(uint requestorId, ISpeechInfo speechInfo)
            : base(requestorId)
        {
            // this.ExhaustionCost = TimeSpan.FromSeconds(1);
            this.SpeechInfo = speechInfo;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Speech;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        public ISpeechInfo SpeechInfo { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            var requestor = this.GetRequestor(context.CreatureFinder);

            if (requestor == null)
            {
                return;
            }

            context.Scheduler.ScheduleEvent(
                new CreatureSpokeNotification(
                    () => context.ConnectionFinder.GetAllActive(),
                    new CreatureSpokeNotificationArguments(requestor, this.SpeechInfo.Type, this.SpeechInfo.ChannelId, this.SpeechInfo.Content)));
        }
    }
}