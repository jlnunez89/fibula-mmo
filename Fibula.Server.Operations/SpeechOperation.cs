// -----------------------------------------------------------------
// <copyright file="SpeechOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations
{
    using System;

    /// <summary>
    /// Class that represents a speech operation.
    /// </summary>
    public class SpeechOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechOperation"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="speechType"></param>
        /// <param name="channelId"></param>
        /// <param name="content"></param>
        /// <param name="receiver"></param>
        public SpeechOperation(uint requestorId, SpeechType speechType, ChatChannelType channelId, string content, string receiver)
            : base(requestorId)
        {
            // this.ExhaustionCost = TimeSpan.FromSeconds(1);
            this.Type = speechType;
            this.ChannelId = channelId;
            this.Content = content;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Speech;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        public SpeechType Type { get; }

        public ChatChannelType ChannelId { get; }

        public string Receiver { get; }

        public string Content { get; }

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
                    new CreatureSpokeNotificationArguments(requestor, this.Type, this.ChannelId, this.Content)));
        }
    }
}