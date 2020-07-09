// -----------------------------------------------------------------
// <copyright file="SpeechOperation.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
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

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.Speech;

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

            if (requestor is ICombatant combatant)
            {
                switch (this.Content)
                {
                    case "increase atk speed":
                        combatant.IncreaseAttackSpeed(0.1m);
                        context.Logger.Debug($"Combatant {combatant.Name}'s attack speed is now {combatant.AttackSpeed}.");
                        return;
                    case "decrease atk speed":
                        combatant.DecreaseAttackSpeed(0.1m);
                        context.Logger.Debug($"Combatant {combatant.Name}'s attack speed is now {combatant.AttackSpeed}.");
                        return;
                    case "increase def speed":
                        combatant.IncreaseDefenseSpeed(0.1m);
                        context.Logger.Debug($"Combatant {combatant.Name}'s defense speed is now {combatant.DefenseSpeed}.");
                        return;
                    case "decrease def speed":
                        combatant.DecreaseDefenseSpeed(0.1m);
                        context.Logger.Debug($"Combatant {combatant.Name}'s defense speed is now {combatant.DefenseSpeed}.");
                        return;
                }
            }

            new CreatureSpeechNotification(
                    () => context.Map.PlayersThatCanSee(requestor.Location),
                    new CreatureSpeechNotificationArguments(requestor, this.Type, this.ChannelId, this.Content))
            .Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));
        }
    }
}
