// -----------------------------------------------------------------
// <copyright file="ObjectsFileItemTypeLoader.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Loaders.ObjectsFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fibula.Common.Utilities;
    using Fibula.Data.Entities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Parsing.CipFiles;
    using Fibula.Parsing.CipFiles.Enumerations;
    using Fibula.Parsing.CipFiles.Extensions;
    using Microsoft.Extensions.Options;
    using Serilog;

    /// <summary>
    /// Class that represents an item type loader that reads from the objects file.
    /// </summary>
    /// <remarks>
    ///
    /// An item definition starts and ends with blank lines.
    ///
    ///     TypeID      = 1 # body container
    ///     Name        = ""
    ///     Flags       = {Container,Take}
    ///     Attributes  = {Capacity=1,Weight=0}
    /// .
    /// </remarks>
    public class ObjectsFileItemTypeLoader : IItemTypeLoader
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
        /// Initializes a new instance of the <see cref="ObjectsFileItemTypeLoader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger instance.</param>
        /// <param name="options">The options for this loader.</param>
        public ObjectsFileItemTypeLoader(
            ILogger logger,
            IOptions<ObjectsFileItemTypeLoaderOptions> options)
        {
            logger.ThrowIfNull(nameof(logger));
            options.ThrowIfNull(nameof(options));

            DataAnnotationsValidator.ValidateObjectRecursive(options.Value);

            this.LoaderOptions = options.Value;
            this.Logger = logger.ForContext<ObjectsFileItemTypeLoader>();
        }

        /// <summary>
        /// Gets the loader options.
        /// </summary>
        public ObjectsFileItemTypeLoaderOptions LoaderOptions { get; }

        /// <summary>
        /// Gets the logger to use in this handler.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Attempts to load the item catalog.
        /// </summary>
        /// <returns>The catalog, containing a mapping of loaded id to the item types.</returns>
        public IDictionary<ushort, IItemTypeEntity> LoadTypes()
        {
            var itemDictionary = new Dictionary<ushort, IItemTypeEntity>();
            var objectsFilePath = Path.Combine(Environment.CurrentDirectory, this.LoaderOptions.FilePath);

            var currentType = new ItemTypeEntity();

            foreach (var readLine in File.ReadLines(objectsFilePath))
            {
                if (readLine == null)
                {
                    continue;
                }

                var inLine = readLine.Split(new[] { CommentSymbol }, 2).FirstOrDefault();

                // ignore comments and empty lines.
                if (string.IsNullOrWhiteSpace(inLine))
                {
                    // wrap up the current ItemType and add it if it has enough properties set:
                    if (currentType.TypeId == 0 || string.IsNullOrWhiteSpace(currentType.Name))
                    {
                        continue;
                    }

                    currentType.LockChanges();
                    itemDictionary.Add(currentType.TypeId, currentType);

                    currentType = new ItemTypeEntity();
                    continue;
                }

                var data = inLine.Split(new[] { PropertyValueSeparator }, 2);

                if (data.Length != 2)
                {
                    throw new InvalidDataException($"Malformed line [{inLine}] in objects file: [{objectsFilePath}]");
                }

                var propName = data[0].ToLower().Trim();
                var propData = data[1].Trim();

                switch (propName)
                {
                    case "typeid":
                        currentType.TypeId = Convert.ToUInt16(propData);
                        break;
                    case "name":
                        currentType.Name = propData.Substring(Math.Min(1, propData.Length), Math.Max(0, propData.Length - 2));
                        break;
                    case "description":
                        currentType.Description = propData;
                        break;
                    case "flags":
                        foreach (var element in CipFileParser.Parse(propData))
                        {
                            var flagName = element.Attributes.First().Name;

                            if (Enum.TryParse(flagName, out CipItemFlag flagMatch))
                            {
                                if (flagMatch.ToItemFlag() is ItemFlag itemflag)
                                {
                                    currentType.SetItemFlag(itemflag);
                                }

                                continue;
                            }

                            this.Logger.Warning($"Unknown flag [{flagName}] found on item with TypeID [{currentType.TypeId}].");
                        }

                        break;
                    case "attributes":
                        foreach (var attrStr in propData.Substring(Math.Min(1, propData.Length), Math.Max(0, propData.Length - 2)).Split(','))
                        {
                            var attrPair = attrStr.Split('=');

                            if (attrPair.Length != 2)
                            {
                                this.Logger.Error($"Invalid attribute {attrStr}.");

                                continue;
                            }

                            var attributeName = attrPair[0];
                            var attributeValue = Convert.ToInt32(attrPair[1]);

                            if (Enum.TryParse(attributeName, out CipItemAttribute cipAttribute))
                            {
                                if (cipAttribute.ToItemAttribute() is ItemAttribute itemAttribute)
                                {
                                    currentType.SetAttribute(itemAttribute, attributeValue);
                                }

                                continue;
                            }

                            this.Logger.Warning($"Attempted to set an unknown item attribute [{attributeName}].");
                        }

                        break;
                }
            }

            // wrap up the last ItemType and add it if it has enough properties set:
            if (currentType.TypeId != 0 && !string.IsNullOrWhiteSpace(currentType.Name))
            {
                currentType.LockChanges();
                itemDictionary.Add(currentType.TypeId, currentType);
            }

            return itemDictionary;
        }
    }
}
