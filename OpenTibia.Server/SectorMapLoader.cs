// -----------------------------------------------------------------
// <copyright file="SectorMapLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class SectorMapLoader : IMapLoader
    {
        public const char CommentSymbol = '#';
        public const char SectorSeparator = ':';
        public const char PositionSeparator = '-';

        public const string AttributeSeparator = ",";
        public const string AttributeDefinition = "=";

        public const int SectorXMin = 996;
        public const int SectorXMax = 1043;

        public const int SectorYMin = 984;
        public const int SectorYMax = 1031;

        public const int SectorZMin = 0;
        public const int SectorZMax = 15;

        private readonly DirectoryInfo mapDirInfo;
        private readonly bool[,,] sectorsLoaded;

        private long totalTileCount;
        private long totalLoadedCount;

        public byte PercentageComplete => (byte)Math.Floor(Math.Min(100, Math.Max(0M, this.totalLoadedCount * 100 / (this.totalTileCount + 1))));

        /// <summary>
        /// Initializes a new instance of the <see cref="SectorMapLoader"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="itemFactory">A reference to the item factory.</param>
        /// <param name="sectorMapLoaderOptions">The options for this map loader.</param>
        public SectorMapLoader(ICreatureFinder creatureFinder, IItemFactory itemFactory, IOptions<SectorMapLoaderOptions> sectorMapLoaderOptions)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            sectorMapLoaderOptions.ThrowIfNull(nameof(sectorMapLoaderOptions));

            this.mapDirInfo = new DirectoryInfo(sectorMapLoaderOptions.Value.LiveMapDirectory);

            this.CreatureFinder = creatureFinder;
            this.ItemFactory = itemFactory;

            this.totalTileCount = 1;
            this.totalLoadedCount = default;
            this.sectorsLoaded = new bool[SectorXMax - SectorXMin, SectorYMax - SectorYMin, SectorZMax - SectorZMin];
        }

        public ICreatureFinder CreatureFinder { get; }

        public IItemFactory ItemFactory { get; }

        public bool HasLoaded(int x, int y, sbyte z)
        {
            if (x > SectorXMax)
            {
                return this.sectorsLoaded[(x / 32) - SectorXMin, (y / 32) - SectorYMin, z - SectorZMin];
            }

            return this.sectorsLoaded[x - SectorXMin, y - SectorYMin, z - SectorZMin];
        }

        public IEnumerable<(Location Location, Tile Tile)> Load(int fromX, int toX, int fromY, int toY, sbyte fromZ, sbyte toZ)
        {
            var fromSectorX = fromX / 32;
            var toSectorX = toX / 32;
            var fromSectorY = fromY / 32;
            var toSectorY = toY / 32;

            if (toSectorX < fromSectorX || toSectorY < fromSectorY || toZ < fromZ)
            {
                throw new InvalidOperationException("Bad range supplied.");
            }

            var tuplesAdded = new List<(Location loc, Tile tile)>();

            this.totalTileCount = ((toSectorX - fromSectorX + 1) * 32) * ((toSectorY - fromSectorY + 1) * 32) * (toZ - fromZ + 1);
            this.totalLoadedCount = default;

            Parallel.For(fromZ, toZ + 1, sectorZ =>
            {
                Parallel.For(fromSectorY, toSectorY + 1, sectorY =>
                {
                    Parallel.For(fromSectorX, toSectorX + 1, sectorX =>
                    {
                        var sectorFileName = $"{sectorX:0000}-{sectorY:0000}-{sectorZ:00}.sec";

                        var fullFilePath = Path.Combine(this.mapDirInfo.FullName, sectorFileName);
                        var sectorFileInfo = new FileInfo(fullFilePath);

                        if (sectorFileInfo.Exists)
                        {
                            using var streamReader = sectorFileInfo.OpenText();

                            var fileContents = streamReader.ReadToEnd();

                            tuplesAdded.AddRange(SectorFileReader.ReadSector(this.ItemFactory, sectorFileName, fileContents, (ushort)(sectorX * 32), (ushort)(sectorY * 32), (sbyte)sectorZ));
                        }

                        Interlocked.Add(ref this.totalLoadedCount, 1024); // 1024 per sector file, regardless if there is a tile or not...
                        this.sectorsLoaded[sectorX - SectorXMin, sectorY - SectorYMin, sectorZ - SectorZMin] = true;
                    });
                });
            });

            this.totalLoadedCount = this.totalTileCount;

            return tuplesAdded;
        }
    }
}
