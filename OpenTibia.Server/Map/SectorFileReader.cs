using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Models.Structs;
using OpenTibia.Utilities;

namespace OpenTibia.Server.Map
{
    public class SectorFileReader
    {
        public const char CommentSymbol = '#';
        public const char SectorSeparator = ':';
        public const char PositionSeparator = '-';

        public const string AttributeSeparator = ",";
        public const string AttributeDefinition = "=";
        
        public static IList<Tile> ReadSector(string fileName, string sectorFileContents, ushort xOffset, ushort yOffset, sbyte z)
        {
            var loadedTilesList = new List<Tile>();

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
                    throw new Exception($"Malformed line [{inLine}] in sector file: [{fileName}]");
                }

                var tileInfo = data[0].Split(new[] { PositionSeparator }, 2);
                var tileData = data[1];
                    
                var newTile = new Tile(new Location
                {
                    X = (ushort)(xOffset + Convert.ToUInt16(tileInfo[0])),
                    Y = (ushort)(yOffset + Convert.ToUInt16(tileInfo[1])),
                    Z = z
                });

                // load and add tile flags and contents.
                foreach (var element in CipReader.Parse(tileData))
                {
                    foreach(var attribute in element.Attributes)
                    {
                        if (attribute.Name.Equals("Content"))
                        {
                            newTile.AddContent(attribute.Value);
                        }
                        else
                        {
                            // it's a flag
                            TileFlag flagMatch;

                            if (Enum.TryParse(attribute.Name, out flagMatch))
                            {
                                newTile.SetFlag(flagMatch);
                            }
                            else
                            {
                                // TODO: proper logging.
                                Console.WriteLine($"Unknown flag [{attribute.Name}] found on tile at location {newTile.Location}.");
                            }
                        }
                    }
                }

                loadedTilesList.Add(newTile);
            }

            // TODO: proper logging.
            //Console.WriteLine($"Sector file {sectorFileContents.Name}: {loadedTilesList.Count} tiles loaded.");

            return loadedTilesList;
        }
    }
}
