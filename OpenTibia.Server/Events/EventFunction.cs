// <copyright file="EventFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using OpenTibia.Server.Data.Interfaces;

    internal class EventFunction : IEventFunction
    {
        public string FunctionName { get; }

        public object[] Parameters { get; }

        public EventFunction(string name, params object[] parameters)
        {
            this.FunctionName = name;
            this.Parameters = parameters;
        }
    }
}