﻿// -----------------------------------------------------------------
// <copyright file="ISkill.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Abstractions
{
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for skills in the game.
    /// </summary>
    public interface ISkill
    {
        ///// <summary>
        ///// Event triggered when this skill advances to the next level.
        ///// </summary>
        //event SkillLevelAdvance OnAdvance;

        /// <summary>
        /// Gets this skill's type.
        /// </summary>
        SkillType Type { get; }

        /// <summary>
        /// Gets this skill's level.
        /// </summary>
        uint Level { get; }

        /// <summary>
        /// Gets this skill's maximum level.
        /// </summary>
        uint MaxLevel { get; }

        /// <summary>
        /// Gets this skill's default level.
        /// </summary>
        uint DefaultLevel { get; }

        /// <summary>
        /// Gets this skill's current count.
        /// </summary>
        double Count { get; }

        /// <summary>
        /// Gets this skill's target count.
        /// </summary>
        double Target { get; }

        /// <summary>
        /// Gets this skill's target base increase level over level.
        /// </summary>
        double BaseTargetIncrease { get; }

        /// <summary>
        /// Gets this skill's rate of target count increase.
        /// </summary>
        double Rate { get; }

        /// <summary>
        /// Increases this skill's counter.
        /// </summary>
        /// <param name="value">The amount by which to increase this skills counter.</param>
        void IncreaseCounter(double value);
    }
}