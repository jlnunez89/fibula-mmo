// -----------------------------------------------------------------
// <copyright file="CipFileParser.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Parsing
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;

    /// <summary>
    /// Static class that parses Cip files.
    /// </summary>
    public static class CipFileParser
    {
        /// <summary>
        /// The comma character.
        /// </summary>
        public const char Comma = ',';

        /// <summary>
        /// The space character.
        /// </summary>
        public const char Space = ' ';

        /// <summary>
        /// The equals sign character.
        /// </summary>
        public const char EqualsSign = '=';

        /// <summary>
        /// The open parenthesis character.
        /// </summary>
        public const char OpenParenthesis = '(';

        /// <summary>
        /// The close parenthesis character.
        /// </summary>
        public const char CloseParenthesis = ')';

        /// <summary>
        /// The open curly brace character.
        /// </summary>
        public const char OpenCurly = '{';

        /// <summary>
        /// The close curly brace character.
        /// </summary>
        public const char CloseCurly = '}';

        /// <summary>
        /// The name of the content attribute.
        /// </summary>
        public const string ContentAttributeName = "Content";

        /// <summary>
        /// Attempts to parse a string into a <see cref="CipElement"/>.
        /// </summary>
        /// <param name="inputStr">The input string.</param>
        /// <returns>A collection of <see cref="CipElement"/>s parsed.</returns>
        public static IEnumerable<CipElement> Parse(string inputStr)
        {
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                return null;
            }

            var enclosingChars = new List<(char, char)> { (OpenCurly, CloseCurly), (OpenParenthesis, CloseParenthesis) };

            inputStr = inputStr.Trim(Space); // remove extra leading and trailing spaces.
            inputStr = inputStr.TrimEnclosures(enclosingChars);

            var enclosures = inputStr.GetEnclosedStrings(enclosingChars);
            var pendingContent = new Stack<IParsedAttribute>();

            var root = new CipElement(-1, new List<IParsedAttribute>() { new CipAttribute() });

            // root is guaranteed to have at least one attribute.
            pendingContent.Push(root.Attributes.First());

            foreach (var enclosure in enclosures)
            {
                // comma separate but watch for strings in quotes ("").
                var elements = enclosure.SplitByToken(Comma).Select(ParseElement).ToList();

                var attribute = pendingContent.Pop();

                foreach (var element in elements)
                {
                    foreach (var attr in element.Attributes.Where(a => a.Name.Equals(ContentAttributeName)))
                    {
                        pendingContent.Push(attr);
                    }
                }

                attribute.Value = elements;
            }

            return root.Attributes.First().Value as IEnumerable<CipElement>;
        }

        /// <summary>
        /// Parses an element string value into a <see cref="CipElement"/>.
        /// </summary>
        /// <param name="elementStr">The element string.</param>
        /// <returns>The new instance of <see cref="CipElement"/>.</returns>
        private static CipElement ParseElement(string elementStr)
        {
            elementStr.ThrowIfNullOrWhiteSpace(nameof(elementStr));

            var attrs = elementStr.SplitByToken();
            var attributesList = attrs as IList<string> ?? attrs.ToList();
            var hasIdData = int.TryParse(attributesList.FirstOrDefault(), out int intValue);

            IList<IParsedAttribute> attributes = attributesList.Skip(hasIdData ? 1 : 0).Select(ParseAttribute).ToList();

            var element = new CipElement(hasIdData ? intValue : -1, attributes);

            return element;
        }

        /// <summary>
        /// Parses an attribute string value into a <see cref="IParsedAttribute"/>.
        /// </summary>
        /// <param name="attributeStr">The attribute string.</param>
        /// <returns>The new instance of <see cref="IParsedAttribute"/>.</returns>
        private static IParsedAttribute ParseAttribute(string attributeStr)
        {
            IParsedAttribute attribute = new CipAttribute();

            var sections = attributeStr.Split(new[] { EqualsSign }, 2);

            if (sections.Length < 2)
            {
                attribute.Name = sections[0].EndsWith(EqualsSign) ? sections[0][0..^1] : sections[0];
            }
            else
            {
                attribute.Name = sections[0];
                attribute.Value = int.TryParse(sections[1], out int numericValue) ? (object)numericValue : sections[1];
            }

            return attribute;
        }
    }
}
