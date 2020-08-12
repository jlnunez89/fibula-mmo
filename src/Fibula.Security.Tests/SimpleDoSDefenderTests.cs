// -----------------------------------------------------------------
// <copyright file="SimpleDoSDefenderTests.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security.Tests
{
    using System;
    using Fibula.Common.TestingUtilities;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the <see cref="SimpleDosDefender"/> class.
    /// </summary>
    [TestClass]
    public class SimpleDoSDefenderTests
    {
        /// <summary>
        /// Checks <see cref="SimpleDosDefender"/> initialization.
        /// </summary>
        [TestMethod]
        public void LoginListener_Initialization()
        {
            SimpleDosDefenderOptions defenderOptions = new SimpleDosDefenderOptions()
            {
                TimeframeInSeconds = 1,
                ListSizeLimit = 100,
                BlockAtCount = 5,
            };

            // Initialize with null parameters, should throw.
            ExceptionAssert.Throws<ArgumentNullException>(() => new SimpleDosDefender(null), $"Value cannot be null. (Parameter 'options')");

            // And initialize with all good values.
            new SimpleDosDefender(Options.Create(defenderOptions));
        }
    }
}
