// -----------------------------------------------------------------
// <copyright file="OperationFactory.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Actions;
    using OpenTibia.Server.Operations.Arguments;
    using OpenTibia.Server.Operations.Environment;
    using OpenTibia.Server.Operations.Movements;
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
        /// <param name="defaultElevatedOperationContext">A reference to the default elevated operation context.</param>
        /// <param name="defaultOperationContext">A reference for the default operation context.</param>
        public OperationFactory(ILogger logger, IElevatedOperationContext defaultElevatedOperationContext, IOperationContext defaultOperationContext)
        {
            logger.ThrowIfNull(nameof(logger));
            defaultOperationContext.ThrowIfNull(nameof(defaultOperationContext));
            defaultElevatedOperationContext.ThrowIfNull(nameof(defaultElevatedOperationContext));

            this.Logger = logger;
            this.DefaultElevatedOperationContext = defaultElevatedOperationContext;
            this.DefaultOperationContext = defaultOperationContext;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets a reference to the default elevated operation context.
        /// </summary>
        public IElevatedOperationContext DefaultElevatedOperationContext { get; }

        /// <summary>
        /// Gets the reference to the default operation context.
        /// </summary>
        public IOperationContext DefaultOperationContext { get; }

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
                case OperationType.LogIn:
                    if (arguments is LogInOperationCreationArguments logInOpArgs)
                    {
                        return new LogInOperation(
                            this.Logger,
                            this.DefaultElevatedOperationContext,
                            NoRequestorId,
                            logInOpArgs.CreationMetadata,
                            logInOpArgs.Connection,
                            logInOpArgs.WorldLightLevel,
                            logInOpArgs.WorldLightColor);
                    }

                    break;
                case OperationType.LogOut:
                    if (arguments is LogOutOperationCreationArguments logOutOpArgs)
                    {
                        return new LogOutOperation(
                            this.Logger,
                            this.DefaultElevatedOperationContext,
                            logOutOpArgs.RequestorId,
                            logOutOpArgs.Player);
                    }

                    break;

                case OperationType.PlaceCreature:
                    if (arguments is PlaceCreatureOperationCreationArguments placeCreatureOpArgs)
                    {
                        return new PlaceCreatureOperation(
                            this.Logger,
                            this.DefaultElevatedOperationContext,
                            placeCreatureOpArgs.RequestorId,
                            placeCreatureOpArgs.AtLocation,
                            placeCreatureOpArgs.Creature);
                    }

                    break;
                case OperationType.RemoveCreature:
                    if (arguments is RemoveCreatureOperationCreationArguments removeCreatureOpArgs)
                    {
                        return new RemoveCreatureOperation(
                            this.Logger,
                            this.DefaultElevatedOperationContext,
                            removeCreatureOpArgs.RequestorId,
                            removeCreatureOpArgs.Creature);
                    }

                    break;
                case OperationType.ChangeItem:
                    if (arguments is ChangeItemOperationCreationArguments changeItemOpArgs)
                    {
                        return new ChangeItemOperation(
                            this.Logger,
                            this.DefaultElevatedOperationContext,
                            changeItemOpArgs.RequestorId,
                            changeItemOpArgs.ItemTypeId,
                            changeItemOpArgs.FromLocation,
                            changeItemOpArgs.ToTypeId,
                            changeItemOpArgs.FromStackPos,
                            changeItemOpArgs.Index,
                            changeItemOpArgs.Carrier);
                    }

                    break;
                case OperationType.CreateItem:
                    if (arguments is CreateItemOperationCreationArguments createItemOpArgs)
                    {
                        return new CreateItemOperation(
                            this.Logger,
                            this.DefaultElevatedOperationContext,
                            createItemOpArgs.RequestorId,
                            createItemOpArgs.ItemTypeId,
                            createItemOpArgs.AtLocation);
                    }

                    break;
                case OperationType.DeleteItem:
                    if (arguments is DeleteItemOperationCreationArguments deleteItemOpArgs)
                    {
                        return new DeleteItemOperation(
                            this.Logger,
                            this.DefaultElevatedOperationContext,
                            deleteItemOpArgs.RequestorId,
                            deleteItemOpArgs.ItemTypeId,
                            deleteItemOpArgs.AtLocation);
                    }

                    break;
                case OperationType.ContainerClose:
                    if (arguments is CloseContainerOperationCreationArguments closeContainerOpArgs)
                    {
                        return new CloseContainerOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            closeContainerOpArgs.Player,
                            closeContainerOpArgs.Container,
                            closeContainerOpArgs.ContainerId);
                    }

                    break;
                case OperationType.ContainerMoveUp:
                    if (arguments is MoveUpContainerOperationCreationArguments moveUpContainerOpArgs)
                    {
                        return new MoveUpContainerOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            moveUpContainerOpArgs.Player,
                            moveUpContainerOpArgs.Container,
                            moveUpContainerOpArgs.AsContainerId);
                    }

                    break;
                case OperationType.Turn:
                    if (arguments is TurnToDirectionOperationCreationArguments turnToDirectionOpArgs)
                    {
                        return new TurnToDirectionOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            turnToDirectionOpArgs.Creature,
                            turnToDirectionOpArgs.Direction);
                    }

                    break;
                case OperationType.UseItem:
                    if (arguments is UseItemOperationCreationArguments useItemOpArgs)
                    {
                        return new UseItemOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            useItemOpArgs.RequestorId,
                            useItemOpArgs.ItemTypeId,
                            useItemOpArgs.FromLocation,
                            useItemOpArgs.FromStackPos,
                            useItemOpArgs.Index);
                    }

                    break;
                case OperationType.BodyToBodyMovement:
                    if (arguments is BodyToBodyMovementOperationCreationArguments bodyToBodyOpArgs)
                    {
                        return new BodyToBodyMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            bodyToBodyOpArgs.RequestorId,
                            bodyToBodyOpArgs.ThingMoving,
                            bodyToBodyOpArgs.TargetCreature,
                            bodyToBodyOpArgs.FromSlot,
                            bodyToBodyOpArgs.ToSlot,
                            bodyToBodyOpArgs.Amount);
                    }

                    break;
                case OperationType.BodyToContainerMovement:
                    if (arguments is BodyToContainerMovementOperationCreationArguments bodyToContainerOpArgs)
                    {
                        return new BodyToContainerMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            bodyToContainerOpArgs.RequestorId,
                            bodyToContainerOpArgs.ThingMoving,
                            bodyToContainerOpArgs.TargetCreature,
                            bodyToContainerOpArgs.FromSlot,
                            bodyToContainerOpArgs.ToContainerId,
                            bodyToContainerOpArgs.ToContainerIndex,
                            bodyToContainerOpArgs.Amount);
                    }

                    break;
                case OperationType.BodyToMapMovement:
                    if (arguments is BodyToMapMovementOperationCreationArguments bodyToMapOpArgs)
                    {
                        return new BodyToMapMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            bodyToMapOpArgs.RequestorId,
                            bodyToMapOpArgs.ThingMoving,
                            bodyToMapOpArgs.FromCreature,
                            bodyToMapOpArgs.FromSlot,
                            bodyToMapOpArgs.ToLocation,
                            bodyToMapOpArgs.Amount);
                    }

                    break;
                case OperationType.ContainerToBodyMovement:
                    if (arguments is ContainerToBodyMovementOperationCreationArguments containerToBodyOpArgs)
                    {
                        return new ContainerToBodyMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            containerToBodyOpArgs.RequestorId,
                            containerToBodyOpArgs.ThingMoving,
                            containerToBodyOpArgs.TargetCreature,
                            containerToBodyOpArgs.FromContainerId,
                            containerToBodyOpArgs.FromContainerIndex,
                            containerToBodyOpArgs.ToSlot,
                            containerToBodyOpArgs.Amount);
                    }

                    break;
                case OperationType.ContainerToContainerMovement:
                    if (arguments is ContainerToContainerMovementOperationCreationArguments containerToContainerOpArgs)
                    {
                        return new ContainerToContainerMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            containerToContainerOpArgs.RequestorId,
                            containerToContainerOpArgs.ThingMoving,
                            containerToContainerOpArgs.TargetCreature,
                            containerToContainerOpArgs.FromContainerId,
                            containerToContainerOpArgs.FromContainerIndex,
                            containerToContainerOpArgs.ToContainerId,
                            containerToContainerOpArgs.ToContainerIndex,
                            containerToContainerOpArgs.Amount);
                    }

                    break;
                case OperationType.ContainerToMapMovement:
                    if (arguments is ContainerToMapMovementOperationCreationArguments containerToMapOpArgs)
                    {
                        return new ContainerToMapMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            containerToMapOpArgs.RequestorId,
                            containerToMapOpArgs.ThingMoving,
                            containerToMapOpArgs.FromCreature,
                            containerToMapOpArgs.FromContainerId,
                            containerToMapOpArgs.FromContainerIndex,
                            containerToMapOpArgs.ToLocation,
                            containerToMapOpArgs.Amount);
                    }

                    break;
                case OperationType.MapToBodyMovement:
                    if (arguments is MapToBodyMovementOperationCreationArguments mapToBodyOpArgs)
                    {
                        return new MapToBodyMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            mapToBodyOpArgs.RequestorId,
                            mapToBodyOpArgs.ThingMoving,
                            mapToBodyOpArgs.FromLocation,
                            mapToBodyOpArgs.ToCreature,
                            mapToBodyOpArgs.ToSlot,
                            mapToBodyOpArgs.Amount);
                    }

                    break;
                case OperationType.MapToContainerMovement:
                    if (arguments is MapToContainerMovementOperationCreationArguments mapToContainerOpArgs)
                    {
                        return new MapToContainerMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            mapToContainerOpArgs.RequestorId,
                            mapToContainerOpArgs.ThingMoving,
                            mapToContainerOpArgs.FromLocation,
                            mapToContainerOpArgs.ToCreature,
                            mapToContainerOpArgs.ToContainerId,
                            mapToContainerOpArgs.ToContainerIndex,
                            mapToContainerOpArgs.Amount);
                    }

                    break;
                case OperationType.MapToMapMovement:
                    if (arguments is MapToMapMovementOperationCreationArguments mapToMapOpArgs)
                    {
                        return new MapToMapMovementOperation(
                            this.Logger,
                            this.DefaultOperationContext,
                            mapToMapOpArgs.RequestorId,
                            mapToMapOpArgs.ThingMoving,
                            mapToMapOpArgs.FromLocation,
                            mapToMapOpArgs.ToLocation,
                            mapToMapOpArgs.FromStackPos,
                            mapToMapOpArgs.Amount,
                            mapToMapOpArgs.IsTeleport);
                    }

                    break;

                default:
                    throw new NotSupportedException($"Unsupported operation type {type}.");
            }

            throw new NotSupportedException($"Unsupported operation arguments of type '{arguments.GetType().Name}' for operation type {type}.");
        }
    }
}
