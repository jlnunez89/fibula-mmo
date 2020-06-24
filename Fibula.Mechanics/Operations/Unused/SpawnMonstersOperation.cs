// -----------------------------------------------------------------
// <copyright file="SpawnMonstersOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Environment
{
    using System;

    /// <summary>
    /// Class that represents a monsters spawn operation.
    /// </summary>
    public class SpawnMonstersOperation : ElevatedOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnMonstersOperation"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="spawn"></param>
        /// <param name="monsterCreationMetadata"></param>
        public SpawnMonstersOperation(uint requestorId, Spawn spawn, ICreatureCreationMetadata monsterCreationMetadata)
            : base(requestorId)
        {
            this.Spawn = spawn;
            this.MonsterCreationMetadata = monsterCreationMetadata;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        public Spawn Spawn { get; }

        public ICreatureCreationMetadata MonsterCreationMetadata { get; }

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            var rng = new Random();

            for (int i = 0; i < this.Spawn.Count; i++)
            {
                var r = this.Spawn.Radius / 4;
                var newMonster = context.CreatureFactory.Create(CreatureType.Monster, this.MonsterCreationMetadata) as IMonster;

                var randomLoc = this.Spawn.Location + new Location { X = (int)Math.Round(r * Math.Cos(rng.Next(360))), Y = (int)Math.Round(r * Math.Sin(rng.Next(360))), Z = 0 };

                // Need to actually pathfind to avoid placing a monster in unreachable places.
                context.PathFinder.FindBetween(this.Spawn.Location, randomLoc, out Location foundLocation, (i + 1) * 10);

                // TODO: some property of newMonster here to figure out what actually blocks path finding.
                if (context.TileAccessor.GetTileAt(foundLocation, out ITile targetTile) && !targetTile.IsPathBlocking())
                {
                    var placeCreatureOperation = context.OperationFactory.Create(OperationType.PlaceCreature, new PlaceCreatureOperationCreationArguments(requestorId: 0, targetTile, newMonster));

                    context.Scheduler.ScheduleEvent(placeCreatureOperation);
                }
            }
        }
    }
}