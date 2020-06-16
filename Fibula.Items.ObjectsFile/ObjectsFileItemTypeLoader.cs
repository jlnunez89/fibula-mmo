// -----------------------------------------------------------------
// <copyright file="ObjectsFileItemTypeLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.ObjectsFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fibula.Common.Utilities;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Parsing.CipFiles;
    using Fibula.Server.Items;
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
        public IDictionary<ushort, IItemType> LoadTypes()
        {
            var itemDictionary = new Dictionary<ushort, IItemType>();
            var objectsFilePath = Path.Combine(Environment.CurrentDirectory, this.LoaderOptions.FilePath);

            var current = new ItemType();

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
                    if (current.TypeId == 0 || string.IsNullOrWhiteSpace(current.Name))
                    {
                        continue;
                    }

                    current.LockChanges();
                    itemDictionary.Add(current.TypeId, current);

                    current = new ItemType();
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
                        current.SetId(Convert.ToUInt16(propData));
                        break;
                    case "name":
                        current.SetName(propData.Substring(Math.Min(1, propData.Length), Math.Max(0, propData.Length - 2)));
                        break;
                    case "description":
                        current.SetDescription(propData);
                        break;
                    case "flags":
                        foreach (var element in CipFileParser.Parse(propData))
                        {
                            var flagName = element.Attributes.First().Name;

                            if (Enum.TryParse(flagName, out ItemFlag flagMatch))
                            {
                                current.SetFlag(flagMatch);
                                continue;
                            }

                            this.Logger.Warning($"Unknown flag [{flagName}] found on item with TypeID [{current.TypeId}].");
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

                            current.SetAttribute(attrPair[0], Convert.ToInt32(attrPair[1]));
                        }

                        break;
                }
            }

            // wrap up the last ItemType and add it if it has enough properties set:
            if (current.TypeId != 0 && !string.IsNullOrWhiteSpace(current.Name))
            {
                current.LockChanges();
                itemDictionary.Add(current.TypeId, current);
            }

            return itemDictionary;
        }
    }
}
