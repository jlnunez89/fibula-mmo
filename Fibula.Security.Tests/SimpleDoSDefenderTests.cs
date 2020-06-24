// -----------------------------------------------------------------
// <copyright file="SimpleDoSDefenderTests.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
