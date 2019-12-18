// -----------------------------------------------------------------
// <copyright file="ContentElement.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Parsing.Contracts
{
    using System.Text;

    /// <summary>
    /// Class that represents a parsed content element.
    /// </summary>
    public class ContentElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentElement"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attributes"></param>
        public ContentElement(string id, params object[] attributes)
        {
            this.Id = id;
            this.Attributes = attributes;
        }

        public string Id { get; }

        public object[] Attributes { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var obj in this.Attributes)
            {
                sb.Append(obj);
            }

            return $"{this.Id} {sb}";
        }
    }
}
