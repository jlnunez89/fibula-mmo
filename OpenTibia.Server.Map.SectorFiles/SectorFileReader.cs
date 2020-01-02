// -----------------------------------------------------------------
// <copyright file="SectorFileReader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Map.SectorFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Map;
    using OpenTibia.Server.Parsing;
    using Serilog;

    public class SectorFileReader
    {
        public const char CommentSymbol = '#';

        public const char SectorSeparator = ':';

        public const char PositionSeparator = '-';

        public static IList<(Location, ITile)> ReadSector(ILogger logger, IItemFactory itemFactory, string fileName, string sectorFileContents, ushort xOffset, ushort yOffset, sbyte z)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));

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

                newTile.AddContent(logger, itemFactory, CipFileParser.Parse(tileData));

                loadedTilesList.Add((location, newTile));
            }

            // TODO: proper logging.
            // Console.WriteLine($"Sector file {sectorFileContents.Name}: {loadedTilesList.Count} tiles loaded.");
            return loadedTilesList;
        }
    }
}
