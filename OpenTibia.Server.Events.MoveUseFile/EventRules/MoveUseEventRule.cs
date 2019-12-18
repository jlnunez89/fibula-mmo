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
    using OpenTibia.Server.Parsing.Grammar;
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
        /// A value to indicate whether this event is set up.
        /// </summary>
        private bool isSetup;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseEventRule"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="scriptApi">A reference to the script factory in use.</param>
        /// <param name="conditionSet">The conditions for this event.</param>
        /// <param name="actionSet">The actions of this event.</param>
        public MoveUseEventRule(ILogger logger, IScriptApi scriptApi, IList<string> conditionSet, IList<string> actionSet)
        {
            logger.ThrowIfNull(nameof(logger));
            scriptApi.ThrowIfNull(nameof(scriptApi));

            this.ScriptFactory = scriptApi;
            this.Logger = logger.ForContext(this.GetType());

            this.Conditions = this.ParseRules(conditionSet);
            this.Actions = this.ParseRules(actionSet);

            this.Adapter = new MoveUseScriptApiAdapter(logger, scriptApi);

            this.isSetup = false;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets a reference to the script factory in use.
        /// </summary>
        public IScriptApi ScriptFactory { get; }

        /// <summary>
        /// Gets the type of this event.
        /// </summary>
        public abstract EventRuleType Type { get; }

        /// <summary>
        /// Gets or sets the primary thing involved in the event.
        /// </summary>
        public IThing PrimaryThing { get; protected set; }

        /// <summary>
        /// Gets or sets the secondary thing involved in the event.
        /// </summary>
        public IThing SecondaryThing { get; protected set; }

        /// <summary>
        /// Gets or sets the player involved in the event.
        /// </summary>
        public IPlayer Player { get; protected set; }

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
        /// Gets a value indicating whether this event can be executed.
        /// This generally means the <see cref="Conditions"/> have been passed.
        /// </summary>
        public bool CanBeExecuted
        {
            get
            {
                return this.isSetup && this.Conditions.All(condition => this.InvokeCondition(this.PrimaryThing, this.SecondaryThing, this.Player, condition.FunctionName, condition.Parameters));
            }
        }

        /// <summary>
        /// Sets up this event.
        /// </summary>
        /// <param name="obj1">The primary thing involved in the event.</param>
        /// <param name="obj2">The secondary thing involved in the event.</param>
        /// <param name="user">The player involved in the event.</param>
        /// <returns>True if the event is successfully set up, false otherwise.</returns>
        public bool Setup(IThing obj1, IThing obj2 = null, IPlayer user = null)
        {
            this.PrimaryThing = obj1;
            this.SecondaryThing = obj2;
            this.Player = user;

            this.isSetup = true;

            return true;
        }

        /// <summary>
        /// Executes this event.
        /// This generally means executing the event's <see cref="Actions"/>.
        /// </summary>
        public void Execute()
        {
            if (!this.isSetup)
            {
                throw new InvalidOperationException("Cannot execute event without first doing Setup.");
            }

            foreach (var action in this.Actions)
            {
                var obj1Result = this.PrimaryThing;
                var obj2Result = this.SecondaryThing;
                var userResult = this.Player;

                this.InvokeAction(ref obj1Result, ref obj2Result, ref userResult, action.FunctionName, action.Parameters);

                this.PrimaryThing = obj1Result;
                this.SecondaryThing = obj2Result;
                this.Player = userResult;
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