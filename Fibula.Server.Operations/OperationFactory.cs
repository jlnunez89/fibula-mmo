// -----------------------------------------------------------------
// <copyright file="OperationFactory.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Server.Operations.Arguments;
    using Fibula.Server.Operations.Contracts.Abstractions;
    using Fibula.Server.Operations.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents an operation factory.
    /// </summary>
    public class OperationFactory : IOperationFactory
    {
        private const int NoRequestorId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFactory"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public OperationFactory(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Creates a new <see cref="IOperation"/> based on the type specified with the given arguments.
        /// </summary>
        /// <param name="type">The type of operation to create.</param>
        /// <param name="arguments">The arguments for creation.</param>
        /// <returns>A new instance of <see cref="IOperation"/>.</returns>
        public IOperation Create(OperationType type, IOperationCreationArguments arguments)
        {
            switch (type)
            {
                //case OperationType.AutoAttack:
                //    if (arguments is AutoAttackCombatOperationCreationArguments autoAttackOpArgs)
                //    {
                //        return new AutoAttackCombatOperation(
                //            autoAttackOpArgs.Attacker,
                //            autoAttackOpArgs.Target,
                //            autoAttackOpArgs.ExhaustionCost);
                //    }

                //    break;
                //case OperationType.AutoWalk:
                //    if (arguments is AutoWalkOperationCreationArguments autoWalkOpArgs)
                //    {
                //        return new AutoWalkOperation(
                //            autoWalkOpArgs.RequestorId,
                //            autoWalkOpArgs.Creature,
                //            autoWalkOpArgs.Directions);
                //    }

                //    break;
                //case OperationType.ChangeItem:
                //    if (arguments is ChangeItemOperationCreationArguments changeItemOpArgs)
                //    {
                //        return new ChangeItemOperation(
                //            changeItemOpArgs.RequestorId,
                //            changeItemOpArgs.ItemTypeId,
                //            changeItemOpArgs.FromLocation,
                //            changeItemOpArgs.ToTypeId,
                //            changeItemOpArgs.Carrier);
                //    }

                //    break;
                //case OperationType.ContainerOpen:
                //    if (arguments is OpenContainerOperationCreationArguments openContainerOpArgs)
                //    {
                //        return new OpenContainerOperation(
                //            openContainerOpArgs.Player,
                //            openContainerOpArgs.Container,
                //            openContainerOpArgs.ContainerId);
                //    }

                //    break;
                //case OperationType.ContainerClose:
                //    if (arguments is CloseContainerOperationCreationArguments closeContainerOpArgs)
                //    {
                //        return new CloseContainerOperation(
                //            closeContainerOpArgs.Player,
                //            closeContainerOpArgs.Container,
                //            closeContainerOpArgs.ContainerId);
                //    }

                //    break;
                //case OperationType.ContainerMoveUp:
                //    if (arguments is MoveUpContainerOperationCreationArguments moveUpContainerOpArgs)
                //    {
                //        return new MoveUpContainerOperation(
                //            moveUpContainerOpArgs.Player,
                //            moveUpContainerOpArgs.Container,
                //            moveUpContainerOpArgs.AsContainerId);
                //    }

                //    break;
                //case OperationType.CreateItem:
                //    if (arguments is CreateItemOperationCreationArguments createItemOpArgs)
                //    {
                //        return new CreateItemOperation(
                //            createItemOpArgs.RequestorId,
                //            createItemOpArgs.ItemTypeId,
                //            createItemOpArgs.AtLocation);
                //    }

                //    break;
                //case OperationType.DeleteItem:
                //    if (arguments is DeleteItemOperationCreationArguments deleteItemOpArgs)
                //    {
                //        return new DeleteItemOperation(
                //            deleteItemOpArgs.RequestorId,
                //            deleteItemOpArgs.ItemTypeId,
                //            deleteItemOpArgs.AtLocation);
                //    }

                //    break;
                case OperationType.LogIn:
                    if (arguments is LogInOperationCreationArguments logInOpArgs)
                    {
                        return new LogInOperation(
                            NoRequestorId,
                            logInOpArgs.CreationMetadata,
                            logInOpArgs.WorldLightLevel,
                            logInOpArgs.WorldLightColor);
                    }

                    break;
                //case OperationType.LogOut:
                //    if (arguments is LogOutOperationCreationArguments logOutOpArgs)
                //    {
                //        return new LogOutOperation(logOutOpArgs.RequestorId, logOutOpArgs.Player);
                //    }

                //    break;
                //case OperationType.Movement:
                //    if (arguments is MovementOperationCreationArguments movementOpArgs)
                //    {
                //        return new MovementOperation(
                //            movementOpArgs.RequestorId,
                //            movementOpArgs.ThingId,
                //            movementOpArgs.FromLocation,
                //            movementOpArgs.FromIndex,
                //            movementOpArgs.FromCreatureId,
                //            movementOpArgs.ToLocation,
                //            movementOpArgs.ToCreatureId,
                //            movementOpArgs.Amount);
                //    }

                //    break;
                //case OperationType.PlaceCreature:
                //    if (arguments is PlaceCreatureOperationCreationArguments placeCreatureOpArgs)
                //    {
                //        return new PlaceCreatureOperation(
                //            placeCreatureOpArgs.RequestorId,
                //            placeCreatureOpArgs.AtTile,
                //            placeCreatureOpArgs.Creature);
                //    }

                //    break;
                //case OperationType.RemoveCreature:
                //    if (arguments is RemoveCreatureOperationCreationArguments removeCreatureOpArgs)
                //    {
                //        return new RemoveCreatureOperation(
                //            removeCreatureOpArgs.RequestorId,
                //            removeCreatureOpArgs.Creature);
                //    }

                //    break;
                //case OperationType.RestoreCombatCredit:
                //    if (arguments is RestoreCombatCreditOperationCreationArguments restoreCreditOpArgs)
                //    {
                //        return new RestoreCombatCreditOperation(restoreCreditOpArgs.Combatant, restoreCreditOpArgs.CreditType);
                //    }

                //    break;
                //case OperationType.Speech:
                //    if (arguments is SpeechOperationCreationArguments speechOperationOpArgs)
                //    {
                //        return new SpeechOperation(
                //            speechOperationOpArgs.RequestorId,
                //            speechOperationOpArgs.Type,
                //            speechOperationOpArgs.ChannelId,
                //            speechOperationOpArgs.Content,
                //            speechOperationOpArgs.Receiver);
                //    }

                //    break;
                //case OperationType.SpawnMonsters:
                //    if (arguments is SpawnMonstersOperationCreationArguments spawnMonstersOpArgs)
                //    {
                //        return new SpawnMonstersOperation(
                //            NoRequestorId,
                //            spawnMonstersOpArgs.Spawn,
                //            spawnMonstersOpArgs.MonsterCreationMetadata);
                //    }

                //    break;
                //case OperationType.Thinking:
                //    if (arguments is ThinkingOperationCreationArguments thinkingOpArgs)
                //    {
                //        return new ThinkingOperation(thinkingOpArgs.Creature, thinkingOpArgs.Cadence);
                //    }

                //    break;
                //case OperationType.Turn:
                //    if (arguments is TurnToDirectionOperationCreationArguments turnToDirectionOpArgs)
                //    {
                //        return new TurnToDirectionOperation(turnToDirectionOpArgs.Creature, turnToDirectionOpArgs.Direction);
                //    }

                //    break;
                //case OperationType.UseItem:
                //    if (arguments is UseItemOperationCreationArguments useItemOpArgs)
                //    {
                //        return new UseItemOperation(
                //            useItemOpArgs.RequestorId,
                //            useItemOpArgs.ItemTypeId,
                //            useItemOpArgs.FromLocation,
                //            useItemOpArgs.FromIndex);
                //    }

                //    break;
                //case OperationType.UseItemOn:
                //    if (arguments is UseItemOnOperationCreationArguments useItemOnOpArgs)
                //    {
                //        return new UseItemOnOperation(
                //            useItemOnOpArgs.RequestorId,
                //            useItemOnOpArgs.FromItemTypeId,
                //            useItemOnOpArgs.FromLocation,
                //            useItemOnOpArgs.FromIndex,
                //            useItemOnOpArgs.ToThingId,
                //            useItemOnOpArgs.ToLocation,
                //            useItemOnOpArgs.ToIndex);
                //    }

                //    break;

                default:
                    throw new NotSupportedException($"Unsupported operation type {type}.");
            }

            throw new NotSupportedException($"Unsupported operation arguments of type '{arguments.GetType().Name}' for operation type {type}.");
        }
    }
}
