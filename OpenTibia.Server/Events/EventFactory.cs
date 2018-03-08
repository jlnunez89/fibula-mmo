// <copyright file="EventFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using static OpenTibia.Utilities.Grammar.EventGrammar;

    public static class EventFactory
    {
        public static IEvent Create(MoveUseEvent moveUseEvent)
        {
            if (moveUseEvent == null)
            {
                throw new ArgumentNullException(nameof(moveUseEvent));
            }

            if (moveUseEvent.Rule == null)
            {
                throw new ArgumentNullException(nameof(moveUseEvent.Rule));
            }

            switch (moveUseEvent.Type)
            {
                case EventType.Collision:
                    return new CollisionEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case EventType.Use:
                    return new UseEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case EventType.MultiUse:
                    return new MultiUseEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case EventType.Separation:
                    return new SeparationEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case EventType.Movement:
                    return new MovementEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
            }

            throw new InvalidCastException($"Unsuported type of event on EventFactory {moveUseEvent.Type}");
        }
    }
}
