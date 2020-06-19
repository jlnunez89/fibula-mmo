// -----------------------------------------------------------------
// <copyright file="RawMonsterSpell.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.Contracts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class that represents a raw monster spell rule.
    /// </summary>
    public class RawMonsterSpell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawMonsterSpell"/> class.
        /// </summary>
        /// <param name="conditions">The conditions for the spell.</param>
        /// <param name="effects">The effects of the spell.</param>
        /// <param name="chance">The chance of the spell.</param>
        public RawMonsterSpell(IEnumerable<string> conditions, IEnumerable<string> effects, string chance)
        {
            this.ConditionSet = conditions.ToList();
            this.EffectSet = effects.ToList();

            this.Chance = Convert.ToByte(chance);
        }

        /// <summary>
        /// Gets the set of conditions for this rule.
        /// </summary>
        public IList<string> ConditionSet { get; }

        /// <summary>
        /// Gets the set of effects for this rule.
        /// </summary>
        public IList<string> EffectSet { get; }

        /// <summary>
        /// Gets the chance of this spell being picked.
        /// </summary>
        public byte Chance { get; }
    }
}
