// -----------------------------------------------------------------
// <copyright file="EventExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using Fibula.Common.Utilities;
    using Fibula.Scheduling.Contracts.Abstractions;

    public static class EventExtensions
    {
        public static string GetPartitionKey(this IEvent evt)
        {
            evt.ThrowIfNull(nameof(evt));

            return $"{evt.GetType().Name}:{evt.RequestorId}";
        }
    }
}
