using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Scripting;
using OpenTibia.Utilities.Grammar;
using Sprache;

namespace OpenTibia.Server.Events
{
    internal abstract class BaseEvent : IEvent
    {
        public const string IsTypeFunctionName = "IsType";

        private bool _isSetup;

        public abstract EventType Type { get; }

        public IThing Obj1 { get; protected set; }
        public IThing Obj2 { get; protected set; }
        public IPlayer User { get; protected set; }

        public IEnumerable<IEventFunction> Actions { get; }

        public IEnumerable<IEventFunction> Conditions { get; protected set; }

        public bool CanBeExecuted
        {
            get
            {
                return _isSetup && Conditions.All(condition => Functions.InvokeCondition(Obj1, Obj2, User, condition.FunctionName, condition.Parameters));
            }
        }

        public BaseEvent(IList<string> conditionSet, IList<string> actionSet)
        {
            Conditions = ParseFunctions(conditionSet);
            Actions = ParseFunctions(actionSet);

            _isSetup = false;
        }

        private IEnumerable<IEventFunction> ParseFunctions(IList<string> stringSet)
        {
            var functionList = new List<IEventFunction>();

            const string specialCaseNoOperationAction = "NOP";

            foreach(var str in stringSet)
            {
                if (specialCaseNoOperationAction.Equals(str))
                {
                    continue;
                }

                var resultFunction = CipGrammar.Function.TryParse(str);

                if (resultFunction.WasSuccessful)
                {
                    functionList.Add(new EventFunction(resultFunction.Value.Name, resultFunction.Value.Parameters));

                    continue;
                }

                var resultComparison = CipGrammar.Comparison.TryParse(str);

                if(!resultComparison.WasSuccessful)
                {
                    throw new ParseException("Failed to parse string {str} into a function or function comparison.");
                }

                functionList.Add(new EventFunctionComparison(resultComparison.Value.Name, resultComparison.Value.Type, resultComparison.Value.CompareToIdentifier, resultComparison.Value.Parameters));
            }

            return functionList;
        }

        public bool Setup(IThing obj1, IThing obj2 = null, IPlayer user = null)
        {
            Obj1 = obj1;
            Obj2 = obj2;
            User = user;

            _isSetup = true;

            return true; // TODO: make this abstract/virtual and let subclasses override.
        }

        public void Execute()
        {
            if (!_isSetup)
            {
                throw new InvalidOperationException("Cannot execute event without first doing Setup.");
            }

            foreach(var action in Actions)
            {
                var obj1Result = Obj1;
                var obj2Result = Obj2;
                var userResult = User;

                Functions.InvokeAction(ref obj1Result, ref obj2Result, ref userResult, action.FunctionName, action.Parameters);

                Obj1 = obj1Result;
                Obj2 = obj2Result;
                User = userResult;
            }
        }
    }
}