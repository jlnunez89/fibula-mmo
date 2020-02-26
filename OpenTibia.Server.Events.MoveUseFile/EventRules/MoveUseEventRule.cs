// -----------------------------------------------------------------
// <copyright file="MoveUseEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events.MoveUseFile.EventRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Events.MoveUseFile;
    using OpenTibia.Server.Parsing.CipFiles;
    using Serilog;
    using Sprache;

    /// <summary>
    /// Abstract class that represents a base for all Move/Use event rules.
    /// </summary>
    public abstract class MoveUseEventRule : IEventRule
    {
        /// <summary>
        /// The name of the special function to determine the item event type.
        /// </summary>
        public const string IsTypeFunctionName = "IsType";

        /// <summary>
        /// The identifier for the primary thing involved in scripts.
        /// </summary>
        public const string PrimaryThingIdentifier = "Obj1";

        /// <summary>
        /// The identifier for the secondary thing involved in scripts.
        /// </summary>
        public const string SecondaryThingIdentifier = "Obj2";

        /// <summary>
        /// The identifier for the user involved in scripts.
        /// </summary>
        public const string CurrentUserIdentifier = "User";

        /// <summary>
        /// An identifier for the user's name in scripts.
        /// </summary>
        public const string NameShorthand = "%N";

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseEventRule"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public MoveUseEventRule(ILogger logger, IList<string> conditionSet, IList<string> actionSet)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger.ForContext(this.GetType());

            this.Conditions = this.ParseRules(conditionSet);
            this.Actions = this.ParseRules(actionSet);

            this.Adapter = new MoveUseScriptApiAdapter(logger);
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public abstract EventRuleType Type { get; }

        /// <summary>
        /// Gets the actions to perform when an event is executed.
        /// </summary>
        public IEnumerable<IEventRuleFunction> Actions { get; }

        /// <summary>
        /// Gets the reference to the script API adapter in use.
        /// </summary>
        public MoveUseScriptApiAdapter Adapter { get; }

        /// <summary>
        /// Gets or sets the conditions for the event to happen.
        /// </summary>
        public IEnumerable<IEventRuleFunction> Conditions { get; protected set; }

        /// <summary>
        /// Checks whether this event rule can be executed.
        /// This generally means the <see cref="Conditions"/> all evaluate to true.
        /// </summary>
        /// <param name="gameApi">A reference to the game's api in use.</param>
        /// <param name="primaryThing">The primary thing involved in the event rule.</param>
        /// <param name="secondaryThing">The secondary thing involved in the event rule.</param>
        /// <param name="requestingPlayer">The player requesting the event rule execution.</param>
        /// <returns>True if the rule can be executed, false otherwise.</returns>
        public bool CanBeExecuted(IGame gameApi, IThing primaryThing, IThing secondaryThing = null, IPlayer requestingPlayer = null)
        {
            gameApi.ThrowIfNull(nameof(gameApi));

            // TODO: fix this indirection.
            this.Adapter.Game = gameApi;

            return this.Conditions.All(condition => this.InvokeCondition(primaryThing, secondaryThing, requestingPlayer, condition.FunctionName, condition.Parameters));
        }

        /// <summary>
        /// Executes this event rule.
        /// This generally means executing the event rule's <see cref="Actions"/>.
        /// </summary>
        /// <param name="gameApi">A reference to the game's api in use.</param>
        /// <param name="primaryThing">The primary thing involved in the event rule.</param>
        /// <param name="secondaryThing">The secondary thing involved in the event rule.</param>
        /// <param name="requestingPlayer">The player requesting the event rule execution.</param>
        public void Execute(IGame gameApi, ref IThing primaryThing, ref IThing secondaryThing, ref IPlayer requestingPlayer)
        {
            gameApi.ThrowIfNull(nameof(gameApi));

            // TODO: fix this.
            this.Adapter.Game = gameApi;

            foreach (var action in this.Actions)
            {
                this.InvokeAction(ref primaryThing, ref secondaryThing, ref requestingPlayer, action.FunctionName, action.Parameters);
            }
        }

        /// <summary>
        /// Parses event rules from a collection of strings.
        /// </summary>
        /// <param name="stringSet">The collection of strings to parse.</param>
        /// <returns>A collection of <see cref="IEventRuleFunction"/>s parsed.</returns>
        private IEnumerable<IEventRuleFunction> ParseRules(IList<string> stringSet)
        {
            var functionList = new List<IEventRuleFunction>();

            const string specialCaseNoOperationAction = "NOP";

            if (stringSet != null)
            {
                foreach (var str in stringSet)
                {
                    if (specialCaseNoOperationAction.Equals(str))
                    {
                        continue;
                    }

                    var actionFunctionParseResult = CipGrammar.ActionFunction.TryParse(str);

                    if (actionFunctionParseResult.WasSuccessful)
                    {
                        functionList.Add(new MoveUseActionFunction(actionFunctionParseResult.Value.Name, actionFunctionParseResult.Value.Parameters));

                        continue;
                    }

                    var comparisonFunctionParseResult = CipGrammar.ComparisonFunction.TryParse(str);

                    if (comparisonFunctionParseResult.WasSuccessful)
                    {
                        functionList.Add(
                            new MoveUseComparisonFunction(
                                comparisonFunctionParseResult.Value.Name,
                                comparisonFunctionParseResult.Value.Type,
                                comparisonFunctionParseResult.Value.ConstantOrValue,
                                comparisonFunctionParseResult.Value.Parameters));
                    }

                    throw new ParseException($"Failed to parse '{str}' into an {nameof(CipGrammar.ActionFunction)} or {nameof(CipGrammar.ComparisonFunction)}.");
                }
            }

            return functionList;
        }

        /// <summary>
        /// Invokes a condition function.
        /// </summary>
        /// <param name="obj1">The primary thing involved in the condition.</param>
        /// <param name="obj2">The secondary thing involved in the condition.</param>
        /// <param name="user">The user involved.</param>
        /// <param name="methodName">The name of the condition to invoke.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <returns>True if the condition was found and invoked, false otherwise.</returns>
        private bool InvokeCondition(IThing obj1, IThing obj2, IPlayer user, string methodName, params object[] parameters)
        {
            methodName.ThrowIfNullOrWhiteSpace(nameof(methodName));

            var negateCondition = methodName.StartsWith("!");

            methodName = methodName.TrimStart('!');

            var methodInfo = this.Adapter.GetType().GetMethod(methodName);

            try
            {
                if (methodInfo == null)
                {
                    throw new MissingMethodException(this.Adapter.GetType().Name, methodName);
                }

                var methodParameters = methodInfo.GetParameters();

                var parametersForInvocation = new List<object>();

                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (PrimaryThingIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj1);
                    }
                    else if (SecondaryThingIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj2);
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(user);
                    }
                    else
                    {
                        parametersForInvocation.Add((parameters[i] as string).ConvertStringToNewType(methodParameters[i].ParameterType));
                    }
                }

                var result = (bool)methodInfo.Invoke(this.Adapter, parametersForInvocation.ToArray());

                return negateCondition ? !result : result;
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                this.Logger.Error(ex.Message);
                this.Logger.Error(ex.StackTrace);
            }

            return false;
        }

        /// <summary>
        /// Invokes an action function.
        /// </summary>
        /// <param name="obj1">The primary thing involved in the action.</param>
        /// <param name="obj2">The secondary thing involved in the action.</param>
        /// <param name="user">The user involved.</param>
        /// <param name="methodName">The name of the action to invoke.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        private void InvokeAction(ref IThing obj1, ref IThing obj2, ref IPlayer user, string methodName, params object[] parameters)
        {
            methodName.ThrowIfNullOrWhiteSpace(nameof(methodName));

            var methodInfo = this.Adapter.GetType().GetMethod(methodName);

            try
            {
                if (methodInfo == null)
                {
                    throw new MissingMethodException(this.Adapter.GetType().Name, methodName);
                }

                var methodParameters = methodInfo.GetParameters();

                var parametersForInvocation = new List<object>();

                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (PrimaryThingIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj1);
                    }
                    else if (SecondaryThingIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj2);
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(user);
                    }
                    else
                    {
                        var convertedValue = (parameters[i] as string).ConvertStringToNewType(methodParameters[i].ParameterType);

                        parametersForInvocation.Add(convertedValue);
                    }
                }

                var paramsArray = parametersForInvocation.ToArray();

                methodInfo.Invoke(this.Adapter, paramsArray);

                // update references to special parameters.
                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (PrimaryThingIdentifier.Equals(parameters[i] as string))
                    {
                        obj1 = paramsArray[i] as IThing;
                    }
                    else if (SecondaryThingIdentifier.Equals(parameters[i] as string))
                    {
                        obj2 = paramsArray[i] as IThing;
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        user = paramsArray[i] as IPlayer;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex.Message);
                this.Logger.Error(ex.StackTrace);
            }
        }
    }
}