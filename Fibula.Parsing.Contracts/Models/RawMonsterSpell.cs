// -----------------------------------------------------------------
// <copyright file="RawMonsterSpell.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
