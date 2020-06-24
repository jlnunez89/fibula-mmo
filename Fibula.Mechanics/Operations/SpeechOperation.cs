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

namespace Fibula.Mechanics.Operations
{
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;

    /// <summary>
    /// Class that represents a speech operation.
    /// </summary>
    public class SpeechOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature speaking.</param>
        /// <param name="speechType">The type of speech.</param>
        /// <param name="channelType">The type of channel where the speech happens.</param>
        /// <param name="content">The content of the speech.</param>
        /// <param name="receiver">The receiver of the speech, if applicable.</param>
        public SpeechOperation(uint requestorId, SpeechType speechType, ChatChannelType channelType, string content, string receiver)
            : base(requestorId)
        {
            // this.ExhaustionCost = TimeSpan.FromSeconds(1);
            this.Type = speechType;
            this.ChannelId = channelType;
            this.Content = content;
            this.Receiver = receiver;
        }

        ///// <summary>
        ///// Gets the type of exhaustion that this operation produces.
        ///// </summary>
        // public override ExhaustionType ExhaustionType => ExhaustionType.Speech;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the type of speech.
        /// </summary>
        public SpeechType Type { get; }

        /// <summary>
        /// Gets the type of channel where the speech happens.
        /// </summary>
        public ChatChannelType ChannelId { get; }

        /// <summary>
        /// Gets the content of the speech.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the receiver of the speech, if any.
        /// </summary>
        public string Receiver { get; }

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
                new CreatureSpeechNotification(
                    () => context.CreatureFinder.PlayersThatCanSee(context.TileAccessor, requestor.Location),
                    new CreatureSpeechNotificationArguments(requestor, this.Type, this.ChannelId, this.Content)));
        }
    }
}