// -----------------------------------------------------------------
// <copyright file="MonFilesMonsterTypeLoader.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Loaders.MonFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Data.Entities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Structs;
    using Fibula.Parsing.CipFiles;
    using Fibula.Parsing.CipFiles.Enumerations;
    using Fibula.Parsing.CipFiles.Extensions;
    using Microsoft.Extensions.Options;
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

            DataAnnotationsValidator.ValidateObjectRecursive(options.Value);

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
        public IDictionary<ushort, IMonsterTypeEntity> LoadTypes()
        {
            var monsterTypesDictionary = new Dictionary<ushort, IMonsterTypeEntity>();

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
        /// Reads a <see cref="IMonsterTypeEntity"/> out of a monster file.
        /// </summary>
        /// <param name="monsterFileInfo">The information about the monster file.</param>
        /// <returns>The <see cref="IMonsterTypeEntity"/> instance.</returns>
        private IMonsterTypeEntity ReadMonsterFile(FileInfo monsterFileInfo)
        {
            monsterFileInfo.ThrowIfNull(nameof(monsterFileInfo));

            if (!monsterFileInfo.Exists)
            {
                return null;
            }

            var monsterType = new MonsterTypeEntity();

            foreach ((string name, string value) in this.ReadInDataTuples(File.ReadLines(monsterFileInfo.FullName), monsterFileInfo.FullName))
            {
                switch (name)
                {
                    case "racenumber":
                        monsterType.RaceId = Convert.ToUInt16(value);
                        break;
                    case "name":
                        monsterType.Name = value;
                        break;
                    case "article":
                        monsterType.Article = value;
                        break;
                    case "outfit":
                        var (lookTypeId, headColor, bodyColor, legsColor, feetColor) = CipFileParser.ParseMonsterOutfit(value);

                        monsterType.Outfit = new Outfit()
                        {
                            Id = lookTypeId,
                            Head = headColor,
                            Body = bodyColor,
                            Legs = legsColor,
                            Feet = feetColor,
                        };
                        break;
                    case "corpse":
                        monsterType.Corpse = Convert.ToUInt16(value);
                        break;
                    case "blood":
                        if (Enum.TryParse(value, out BloodType bloodType))
                        {
                            monsterType.BloodType = bloodType;
                        }

                        break;
                    case "experience":
                        monsterType.BaseExperienceYield = Convert.ToUInt32(value);
                        break;
                    case "summoncost":
                        monsterType.SummonCost = Convert.ToUInt16(value);
                        break;
                    case "fleethreshold":
                        monsterType.HitpointFleeThreshold = Convert.ToUInt16(value);
                        break;
                    case "attack":
                        monsterType.BaseAttack = Convert.ToUInt16(value);
                        break;
                    case "defend":
                        monsterType.BaseDefense = Convert.ToUInt16(value);
                        break;
                    case "armor":
                        monsterType.BaseArmorRating = Convert.ToUInt16(value);
                        break;
                    case "poison":
                        monsterType.SetConditionInfect(ConditionFlag.Posion, Convert.ToUInt16(value));
                        break;
                    case "losetarget":
                        monsterType.LoseTargetDistance = Convert.ToByte(value);
                        break;
                    case "strategy":
                        monsterType.Strategy = CipFileParser.ParseMonsterStrategy(value);
                        break;
                    case "flags":
                        var parsedElements = CipFileParser.Parse(value);

                        foreach (var element in parsedElements)
                        {
                            if (!element.IsFlag || element.Attributes == null || !element.Attributes.Any())
                            {
                                continue;
                            }

                            if (Enum.TryParse(element.Attributes.First().Name, out CipCreatureFlag flagMatch))
                            {
                                if (flagMatch.ToCreatureFlag() is CreatureFlag creatureFlag)
                                {
                                    monsterType.SetCreatureFlag(creatureFlag);
                                }
                            }
                        }

                        break;
                    case "skills":
                        var skillParsed = CipFileParser.ParseMonsterSkills(value);

                        foreach (var skill in skillParsed)
                        {
                            if (!Enum.TryParse(skill.Name, ignoreCase: true, out CipMonsterSkillType mSkill))
                            {
                                continue;
                            }

                            switch (mSkill)
                            {
                                case CipMonsterSkillType.Hitpoints:
                                    monsterType.MaxHitpoints = skill.CurrentLevel < 0 ? ushort.MaxValue : (ushort)skill.DefaultLevel;
                                    break;
                                case CipMonsterSkillType.GoStrength:
                                    monsterType.BaseSpeed = skill.CurrentLevel < 0 ? ushort.MinValue : (ushort)skill.DefaultLevel;
                                    break;
                                case CipMonsterSkillType.CarryStrength:
                                    monsterType.Capacity = skill.CurrentLevel < 0 ? ushort.MinValue : (ushort)skill.DefaultLevel;
                                    break;
                                case CipMonsterSkillType.FistFighting:
                                    if (skill.CurrentLevel > 0)
                                    {
                                        monsterType.SetSkill(SkillType.NoWeapon, skill.CurrentLevel, skill.DefaultLevel, skill.MaximumLevel, skill.TargetCount, skill.CountIncreaseFactor, skill.IncreaserPerLevel);
                                    }

                                    break;
                            }
                        }

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
