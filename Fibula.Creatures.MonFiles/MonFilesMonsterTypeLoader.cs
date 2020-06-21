// -----------------------------------------------------------------
// <copyright file="MonFilesMonsterTypeLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Monsters.MonFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Monsters;
    using OpenTibia.Server.Parsing.CipFiles;
    using Serilog;

    /// <summary>
    /// Class that represents a monster type loader that reads from the .mon files.
    /// </summary>
    public class MonFilesMonsterTypeLoader : IMonsterTypeLoader
    {
        /// <summary>
        /// Character for comments.
        /// </summary>
        public const char CommentSymbol = '#';

        /// <summary>
        /// Separator used for property and value pairs.
        /// </summary>
        public const char PropertyValueSeparator = '=';

        /// <summary>
        /// The extension for monster files.
        /// </summary>
        private const string MonsterFileExtension = "mon";

        /// <summary>
        /// The directory information for the monster files directory.
        /// </summary>
        private readonly DirectoryInfo monsterFilesDirInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonFilesMonsterTypeLoader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger instance.</param>
        /// <param name="options">The options for this loader.</param>
        public MonFilesMonsterTypeLoader(
            ILogger logger,
            IOptions<MonFilesMonsterTypeLoaderOptions> options)
        {
            logger.ThrowIfNull(nameof(logger));
            options.ThrowIfNull(nameof(options));

            this.LoaderOptions = options.Value;
            this.Logger = logger.ForContext<MonFilesMonsterTypeLoader>();

            options.Value.MonsterFilesDirectory.ThrowIfNullOrWhiteSpace(nameof(options.Value.MonsterFilesDirectory));

            this.monsterFilesDirInfo = new DirectoryInfo(options.Value.MonsterFilesDirectory);
        }

        /// <summary>
        /// Gets the loader options.
        /// </summary>
        public MonFilesMonsterTypeLoaderOptions LoaderOptions { get; }

        /// <summary>
        /// Gets the logger to use in this handler.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Attempts to load the monster catalog.
        /// </summary>
        /// <returns>The catalog, containing a mapping of loaded id to the monster types.</returns>
        public IDictionary<ushort, IMonsterType> LoadTypes()
        {
            var monsterTypesDictionary = new Dictionary<ushort, IMonsterType>();

            if (!this.monsterFilesDirInfo.Exists)
            {
                throw new InvalidDataException($"The specified {nameof(this.LoaderOptions.MonsterFilesDirectory)} could not be found.");
            }

            foreach (var monsterFileInfo in this.monsterFilesDirInfo.GetFiles($"*.{MonsterFileExtension}"))
            {
                var monsterType = this.ReadMonsterFile(monsterFileInfo);

                if (monsterType != null)
                {
                    monsterTypesDictionary.Add(monsterType.RaceId, monsterType);
                }
            }

            return monsterTypesDictionary;
        }

        /// <summary>
        /// Reads a <see cref="IMonsterType"/> out of a monster file.
        /// </summary>
        /// <param name="monsterFileInfo">The information about the monster file.</param>
        /// <returns>The <see cref="IMonsterType"/> instance.</returns>
        private IMonsterType ReadMonsterFile(FileInfo monsterFileInfo)
        {
            monsterFileInfo.ThrowIfNull(nameof(monsterFileInfo));

            if (!monsterFileInfo.Exists)
            {
                return null;
            }

            var monsterType = new MonsterType();

            foreach ((string name, string value) in this.ReadInDataTuples(File.ReadLines(monsterFileInfo.FullName), monsterFileInfo.FullName))
            {
                switch (name)
                {
                    case "racenumber":
                        monsterType.SetId(Convert.ToUInt16(value));
                        break;
                    case "name":
                        monsterType.SetName(value.Trim('\"'));
                        break;
                    case "article":
                        monsterType.SetArticle(value.Trim('\"'));
                        break;
                    case "outfit":
                        monsterType.SetOutfit(value.Trim('(', ')'));
                        break;
                    case "corpse":
                        monsterType.SetCorpse(Convert.ToUInt16(value));
                        break;
                    case "blood":
                        monsterType.SetBlood(value);
                        break;
                    case "experience":
                        monsterType.SetExperience(Convert.ToUInt32(value));
                        break;
                    case "summoncost":
                        monsterType.SetSummonCost(Convert.ToUInt16(value));
                        break;
                    case "fleethreshold":
                        monsterType.SetFleeTreshold(Convert.ToUInt16(value));
                        break;
                    case "attack":
                        monsterType.SetAttack(Convert.ToUInt16(value));
                        break;
                    case "defend":
                        monsterType.SetDefense(Convert.ToUInt16(value));
                        break;
                    case "armor":
                        monsterType.SetArmor(Convert.ToUInt16(value));
                        break;
                    case "poison":
                        monsterType.SetConditionInfect(ConditionFlag.Posion, Convert.ToUInt16(value));
                        break;
                    case "losetarget":
                        monsterType.SetLoseTarget(Convert.ToByte(value));
                        break;
                    case "strategy":
                        monsterType.SetStrategy(CipFileParser.ParseMonsterStrategy(value));
                        break;
                    case "flags":
                        monsterType.SetFlags(CipFileParser.Parse(value));
                        break;
                    case "skills":
                        monsterType.SetSkills(CipFileParser.ParseMonsterSkills(value));
                        break;
                    case "spells":
                        monsterType.SetSpells(CipFileParser.ParseMonsterSpells(value));
                        break;
                    case "inventory":
                        monsterType.SetInventory(CipFileParser.ParseMonsterInventory(value));
                        break;
                    case "talk":
                        monsterType.SetPhrases(CipFileParser.ParsePhrases(value));
                        break;
                }
            }

            monsterType.Lock();

            return monsterType;
        }

        /// <summary>
        /// Reads data out of multiple lines in the input files.
        /// </summary>
        /// <param name="fileLines">The file's lines.</param>
        /// <param name="monsterFileName">The current monster file name, for logging purposes.</param>
        /// <returns>A collection of mappings of properties names to values.</returns>
        private IEnumerable<(string propName, string propValue)> ReadInDataTuples(IEnumerable<string> fileLines, string monsterFileName)
        {
            fileLines.ThrowIfNull(nameof(fileLines));

            var propName = string.Empty;
            var propData = string.Empty;

            foreach (var readLine in fileLines)
            {
                var inLine = readLine.TrimStart();

                // ignore comments and empty lines.
                if (string.IsNullOrWhiteSpace(inLine) || inLine.StartsWith(CommentSymbol))
                {
                    continue;
                }

                var data = inLine.Split(new[] { PropertyValueSeparator }, 2);

                if (data.Length > 2)
                {
                    throw new Exception($"Malformed line [{inLine}] in objects file: [{monsterFileName}]");
                }

                if (data.Length == 1)
                {
                    // line is a continuation of the last prop.
                    propData += data[0].Trim();
                }
                else
                {
                    if (propName.Length > 0 && propData.Length > 0)
                    {
                        yield return (propName, propData);
                    }

                    propName = data[0].ToLower().Trim();
                    propData = data[1].Trim();
                }
            }

            if (propName.Length > 0 && propData.Length > 0)
            {
                yield return (propName, propData);
            }
        }
    }
}
