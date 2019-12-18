// -----------------------------------------------------------------
// <copyright file="MoveUseEventRulesLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events.MoveUseFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Parsing.Grammar;
    using Serilog;
    using Sprache;

    /// <summary>
    /// Class that represents an item event loader that reads from the move/use file.
    /// </summary>
    /// <remarks>
    ///
    /// A move use definition is in the form:
    ///     Type, ContitionFunctions -> ActionFunctions
    /// such as:
    ///
    ///     Use, IsType(Obj1,2487), IsHouse(Obj1), HasRight(User, PREMIUM_ACCOUNT), MayLogout(User) -> MoveRel(User, Obj1, [0,0,0]), Change(Obj1,2495,0), WriteName(User,"%N", Obj1), ChangeRel(Obj1, [0,1,0],2488,2496,0), Logout(User)
    /// .
    /// </remarks>
    public class MoveUseEventRulesLoader : IEventRulesLoader
    {
        /// <summary>
        /// Character for comments.
        /// </summary>
        public const char CommentSymbol = '#';

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseEventRulesLoader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger instance.</param>
        /// <param name="options">The options for this loader.</param>
        /// <param name="scriptFactory">A reference to the script factory in use.</param>
        public MoveUseEventRulesLoader(
            ILogger logger,
            IOptions<MoveUseEventRulesLoaderOptions> options,
            IScriptApi scriptFactory)
        {
            logger.ThrowIfNull(nameof(logger));
            options.ThrowIfNull(nameof(options));
            scriptFactory.ThrowIfNull(nameof(scriptFactory));

            this.LoaderOptions = options.Value;
            this.Logger = logger.ForContext<MoveUseEventRulesLoader>();

            this.ItemEventFactory = new MoveUseItemEventFactory(logger, scriptFactory);
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the reference to the item event factory.
        /// </summary>
        public MoveUseItemEventFactory ItemEventFactory { get; }

        /// <summary>
        /// Gets the loader options.
        /// </summary>
        public MoveUseEventRulesLoaderOptions LoaderOptions { get; }

        /// <summary>
        /// Loads all the event rules.
        /// </summary>
        /// <returns>A mapping between <see cref="EventRuleType"/> and a set of <see cref="IEventRule"/>s of such type.</returns>
        public IDictionary<EventRuleType, ISet<IEventRule>> LoadEventRules()
        {
            var moveUseFilePath = Path.Combine(Environment.CurrentDirectory, this.LoaderOptions.FilePath);

            var assembly = Assembly.GetExecutingAssembly();

            var eventDictionary = new Dictionary<EventRuleType, ISet<IEventRule>>
            {
                { EventRuleType.Use, new HashSet<IEventRule>() },
                { EventRuleType.MultiUse, new HashSet<IEventRule>() },
                { EventRuleType.Movement, new HashSet<IEventRule>() },
                { EventRuleType.Collision, new HashSet<IEventRule>() },
                { EventRuleType.Separation, new HashSet<IEventRule>() },
            };

            foreach (var readLine in File.ReadLines(moveUseFilePath))
            {
                if (readLine == null)
                {
                    continue;
                }

                var inLine = readLine?.Split(new[] { CommentSymbol }, 2).FirstOrDefault();

                // Ignore comments and empty lines.
                if (string.IsNullOrWhiteSpace(inLine) || inLine.StartsWith("BEGIN") || inLine.StartsWith("END"))
                {
                    continue;
                }

                try
                {
                    var moveUseEventParsed = CipGrammar.EventRule.Parse(inLine);

                    if (Enum.TryParse(moveUseEventParsed.Type, out EventRuleType itemEventType))
                    {
                        eventDictionary[itemEventType].Add(this.ItemEventFactory.Create(moveUseEventParsed));
                    }
                }
                catch (ParseException parseEx)
                {
                    this.Logger.Error(parseEx.Message);
                    this.Logger.Error(parseEx.StackTrace);
                }
            }

            return eventDictionary;
        }
    }
}
