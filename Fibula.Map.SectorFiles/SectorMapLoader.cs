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

namespace Fibula.Map.SectorFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Delegates;
    using Fibula.Map.Contracts.Enumerations;
    using Fibula.Parsing.CipFiles;
    using Fibula.Parsing.CipFiles.Enumerations;
    using Fibula.Parsing.CipFiles.Extensions;
    using Fibula.Parsing.Contracts.Abstractions;
    using Microsoft.Extensions.Options;
    using Serilog;

    /// <summary>
    /// Class that represents a map loader that loads from the sector files from CipSoft.
    /// </summary>
    public class SectorMapLoader : IMapLoader
    {
        /// <summary>
        /// The symbol used for comments.
        /// </summary>
        public const char CommentSymbol = '#';

        /// <summary>
        /// The separator used for sector files.
        /// </summary>
        public const char SectorSeparator = ':';

        /// <summary>
        /// The separator used for positions of sectors.
        /// </summary>
        public const char PositionSeparator = '-';

        /// <summary>
        /// The minimum X value for sectors known.
        /// </summary>
        public const int SectorXMin = 996;

        /// <summary>
        /// The maximum X value for sectors known.
        /// </summary>
        public const int SectorXMax = 1043;

        /// <summary>
        /// The minimum Y value for sectors known.
        /// </summary>
        public const int SectorYMin = 984;

        /// <summary>
        /// The maximum Y value for sectors known.
        /// </summary>
        public const int SectorYMax = 1031;

        /// <summary>
        /// The minimum Z value for sectors known.
        /// </summary>
        public const int SectorZMin = 0;

        /// <summary>
        /// The maximum Z value for sectors known.
        /// </summary>
        public const int SectorZMax = 15;

        /// <summary>
        /// The size of a square Sector.
        /// </summary>
        private const int SquareSectorSize = 32;

        /// <summary>
        /// Holds the map directory info.
        /// </summary>
        private readonly DirectoryInfo mapDirInfo;

        /// <summary>
        /// The Z length of sectors known.
        /// </summary>
        private readonly int sectorsLengthZ;

        /// <summary>
        /// The Y length of sectors known.
        /// </summary>
        private readonly int sectorsLengthY;

        /// <summary>
        /// The X length of sectors known.
        /// </summary>
        private readonly int sectorsLengthX;

        /// <summary>
        /// Holds the loaded sectors.
        /// </summary>
        private readonly bool[,,] sectorsLoaded;

        /// <summary>
        /// An object used as a lock to semaphore loading into the sectors' dictionary.
        /// </summary>
        private readonly object loadLock;

        /// <summary>
        /// The total known tile count.
        /// </summary>
        private long totalTileCount;

        /// <summary>
        /// The total loaded tiles count.
        /// </summary>
        private long totalLoadedCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectorMapLoader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger instance in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="itemFactory">A reference to the item factory.</param>
        /// <param name="sectorMapLoaderOptions">The options for this map loader.</param>
        public SectorMapLoader(
            ILogger logger,
            ICreatureFinder creatureFinder,
            IItemFactory itemFactory,
            IOptions<SectorMapLoaderOptions> sectorMapLoaderOptions)
        {
            logger.ThrowIfNull(nameof(logger));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            sectorMapLoaderOptions.ThrowIfNull(nameof(sectorMapLoaderOptions));

            DataAnnotationsValidator.ValidateObjectRecursive(sectorMapLoaderOptions.Value);

            this.mapDirInfo = new DirectoryInfo(sectorMapLoaderOptions.Value.LiveMapDirectory);

            if (!this.mapDirInfo.Exists)
            {
                throw new ApplicationException($"The map directory '{sectorMapLoaderOptions.Value.LiveMapDirectory}' could not be found.");
            }

            this.Logger = logger.ForContext<SectorMapLoader>();
            this.CreatureFinder = creatureFinder;
            this.ItemFactory = itemFactory;

            this.totalTileCount = 1;
            this.totalLoadedCount = default;

            this.loadLock = new object();

            this.sectorsLengthX = 1 + SectorXMax - SectorXMin;
            this.sectorsLengthY = 1 + SectorYMax - SectorYMin;
            this.sectorsLengthZ = 1 + SectorZMax - SectorZMin;

            this.sectorsLoaded = new bool[this.sectorsLengthX, this.sectorsLengthY, this.sectorsLengthZ];
        }

        /// <summary>
        /// Event invoked when a window of coordinates in the map is loaded.
        /// </summary>
        public event WindowLoaded WindowLoaded;

        /// <summary>
        /// Gets the logger to use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the creature finder instance.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the item factory instance.
        /// </summary>
        public IItemFactory ItemFactory { get; }

        /// <summary>
        /// Gets the percentage completed loading the map [0, 100].
        /// </summary>
        public byte PercentageComplete => (byte)Math.Floor(Math.Min(100, Math.Max(0M, this.totalLoadedCount * 100 / (this.totalTileCount + 1))));

        /// <summary>
        /// Gets a value indicating whether this loader has previously loaded the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>True if the loader has previously loaded the given coordinates, false otherwise.</returns>
        public bool HasLoaded(int x, int y, sbyte z)
        {
            var convertToSector = x > SectorXMax;

            var probeX = convertToSector ? (x / SquareSectorSize) - SectorXMin : x - SectorXMin;
            var probeY = convertToSector ? (y / SquareSectorSize) - SectorYMin : y - SectorYMin;
            var probeZ = z - SectorZMin;

            if (probeX >= 0 && probeX < this.sectorsLengthX && probeY >= 0 && probeY < this.sectorsLengthY && probeZ >= 0 && probeZ < this.sectorsLengthZ)
            {
                lock (this.loadLock)
                {
                    return this.sectorsLoaded[probeX, probeY, probeZ];
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to load all tiles within a 3 dimensional coordinates window.
        /// </summary>
        /// <param name="fromX">The start X coordinate for the load window.</param>
        /// <param name="toX">The end X coordinate for the load window.</param>
        /// <param name="fromY">The start Y coordinate for the load window.</param>
        /// <param name="toY">The end Y coordinate for the load window.</param>
        /// <param name="fromZ">The start Z coordinate for the load window.</param>
        /// <param name="toZ">The end Z coordinate for the load window.</param>
        /// <returns>A collection of ordered pairs containing the <see cref="Location"/> and its corresponding <see cref="ITile"/>.</returns>
        public IEnumerable<(Location Location, ITile Tile)> Load(int fromX, int toX, int fromY, int toY, sbyte fromZ, sbyte toZ)
        {
            var fromSectorX = fromX / SquareSectorSize;
            var toSectorX = toX / SquareSectorSize;
            var fromSectorY = fromY / SquareSectorSize;
            var toSectorY = toY / SquareSectorSize;

            if (toSectorX < fromSectorX || toSectorY < fromSectorY || toZ < fromZ)
            {
                throw new InvalidOperationException("Bad range supplied.");
            }

            var tuplesAdded = new List<(Location loc, ITile tile)>();

            this.totalTileCount = (toSectorX - fromSectorX + 1) * SquareSectorSize * (toSectorY - fromSectorY + 1) * SquareSectorSize * (toZ - fromZ + 1);
            this.totalLoadedCount = default;

            lock (this.loadLock)
            {
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

                                var tuplesLoaded = this.ReadSector(sectorFileName, streamReader.ReadToEnd(), (ushort)(sectorX * SquareSectorSize), (ushort)(sectorY * SquareSectorSize), (sbyte)sectorZ);

                                tuplesAdded.AddRange(tuplesLoaded);
                            }

                            // 1024 per sector file, regardless if there is a tile or not...
                            Interlocked.Add(ref this.totalLoadedCount, 1024);

                            this.sectorsLoaded[sectorX - SectorXMin, sectorY - SectorYMin, sectorZ - SectorZMin] = true;

                            this.Logger.Debug($"Loaded sector {sectorFileName} [{this.totalLoadedCount} out of {this.totalTileCount}].");
                        });
                    });
                });
            }

            this.totalLoadedCount = this.totalTileCount;

            this.WindowLoaded?.Invoke(fromSectorX * SquareSectorSize, (toSectorX * SquareSectorSize) + SquareSectorSize, fromSectorY * SquareSectorSize, (toSectorY * SquareSectorSize) + SquareSectorSize, fromZ, toZ);

            return tuplesAdded;
        }

        private IList<(Location, ITile)> ReadSector(string fileName, string sectorFileContents, ushort xOffset, ushort yOffset, sbyte z)
        {
            var loadedTilesList = new List<(Location, ITile)>();

            var lines = sectorFileContents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var readLine in lines)
            {
                var inLine = readLine?.Split(new[] { CommentSymbol }, 2).FirstOrDefault();

                // ignore comments and empty lines.
                if (string.IsNullOrWhiteSpace(inLine))
                {
                    continue;
                }

                var data = inLine.Split(new[] { SectorSeparator }, 2);

                if (data.Length != 2)
                {
                    throw new InvalidDataException($"Malformed line [{inLine}] in sector file: [{fileName}]");
                }

                var tileInfo = data[0].Split(new[] { PositionSeparator }, 2);
                var tileData = data[1];

                var location = new Location
                {
                    X = (ushort)(xOffset + Convert.ToUInt16(tileInfo[0])),
                    Y = (ushort)(yOffset + Convert.ToUInt16(tileInfo[1])),
                    Z = z,
                };

                // start off with a tile that has no ground in it.
                ITile newTile = new Tile(location, null);

                this.AddContent(newTile, CipFileParser.Parse(tileData));

                loadedTilesList.Add((location, newTile));
            }

            this.Logger.Verbose($"Sector file {fileName}: {loadedTilesList.Count} tiles loaded.");

            return loadedTilesList;
        }

        /// <summary>
        /// Forcefully adds parsed content elements to this container.
        /// </summary>
        /// <param name="tile">The tile to add content to.</param>
        /// <param name="contentElements">The content elements to add.</param>
        private void AddContent(ITile tile, IEnumerable<IParsedElement> contentElements)
        {
            contentElements.ThrowIfNull(nameof(contentElements));

            // load and add tile flags and contents.
            foreach (var e in contentElements)
            {
                foreach (var attribute in e.Attributes)
                {
                    if (attribute.Name.Equals("Content"))
                    {
                        if (attribute.Value is IEnumerable<IParsedElement> elements)
                        {
                            var thingStack = new Stack<IThing>();

                            foreach (var element in elements)
                            {
                                if (element.IsFlag)
                                {
                                    // A flag is unexpected in this context.
                                    this.Logger.Warning($"Unexpected flag {element.Attributes?.First()?.Name}, ignoring.");

                                    continue;
                                }

                                IItem item = this.ItemFactory.CreateItem(new ItemCreationArguments() { TypeId = (ushort)element.Id });

                                if (item == null)
                                {
                                    this.Logger.Warning($"Item with id {element.Id} not found in the catalog, skipping.");

                                    continue;
                                }

                                this.SetItemAttributes(item, element.Attributes);

                                thingStack.Push(item);
                            }

                            // Add them in reversed order.
                            while (thingStack.Count > 0)
                            {
                                var thing = thingStack.Pop();

                                tile.AddContent(this.ItemFactory, thing);

                                if (thing is IContainedThing thingWithParentCylinder)
                                {
                                    thingWithParentCylinder.ParentContainer = tile;
                                }
                            }
                        }
                    }
                    else
                    {
                        // it's a flag
                        if (Enum.TryParse(attribute.Name, out TileFlag flagMatch))
                        {
                            tile.SetFlag(flagMatch);
                        }
                        else
                        {
                            this.Logger.Warning($"Unknown flag [{attribute.Name}] found on tile at location {tile.Location}.");
                        }
                    }
                }
            }
        }

        private void SetItemAttributes(IItem item, IList<IParsedAttribute> attributes)
        {
            if (attributes == null)
            {
                return;
            }

            foreach (var attribute in attributes)
            {
                if ("Content".Equals(attribute.Name) && this is IContainerItem containerItem)
                {
                    if (!(attribute.Value is IEnumerable<IParsedElement> contentElements) || !contentElements.Any())
                    {
                        continue;
                    }

                    foreach (var element in contentElements)
                    {
                        if (element.IsFlag)
                        {
                            // A flag is unexpected in this context.
                            this.Logger.Warning($"Unexpected flag {element.Attributes?.First()?.Name}, ignoring.");

                            continue;
                        }

                        IItem contentItem = this.ItemFactory.CreateItem(new ItemCreationArguments() { TypeId = (ushort)element.Id });

                        if (contentItem == null)
                        {
                            this.Logger.Warning($"Item with id {element.Id} not found in the catalog, skipping.");

                            continue;
                        }

                        this.SetItemAttributes(contentItem, element.Attributes);

                        // TODO: we should be able to go over capacity here.
                        containerItem.AddContent(this.ItemFactory, contentItem, 0xFF);
                    }

                    continue;
                }

                // These are safe to add as Attributes of the item.
                if (!Enum.TryParse(attribute.Name, out CipItemAttribute cipAttr) || !(cipAttr.ToItemAttribute() is ItemAttribute itemAttribute))
                {
                    this.Logger.Warning($"Unsupported attribute {attribute.Name} on {item.Type.Name}, ignoring.");

                    continue;
                }

                try
                {
                    item.Attributes[itemAttribute] = attribute.Value as IConvertible;
                }
                catch
                {
                    this.Logger.Warning($"Unexpected attribute {attribute.Name} with illegal value {attribute.Value} on item {item.Type.Name}, ignoring.");
                }
            }
        }
    }
}
