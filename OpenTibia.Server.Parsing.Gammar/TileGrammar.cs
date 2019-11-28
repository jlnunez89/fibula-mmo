// -----------------------------------------------------------------
// <copyright file="TileGrammar.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Parsing.Grammar
{
    using System.Linq;
    using System.Text;
    using Sprache;

    public class TileGrammar
    {
        private static readonly Parser<string> Text =
            from text in Parse.AnyChar.Except(Parse.WhiteSpace).Except(CipGrammar.OpenCurly).Except(CipGrammar.CloseCurly).Except(CipGrammar.Comma).Except(CipGrammar.EqualSign).AtLeastOnce().Text()
            select text.Trim();

        private static readonly Parser<string> EnclosedInCurly =
            from openCurly in CipGrammar.OpenCurly
            from content in Content
            from closeCurly in CipGrammar.CloseCurly
            select openCurly + content.ToString() + closeCurly;

        private static readonly Parser<string> KeyValPair =
            from key in Text
            from eq in CipGrammar.EqualSign
            from value in EnclosedInCurly.Or(CipGrammar.QuotedMessage).Or(Text).Once()
            select key + eq + value; // we want the whole thing .. key=val

        public static readonly Parser<ContentElement> Content =
            from leadingWs in Parse.WhiteSpace.Many()
            from id in Parse.Number.Optional()
            from ws in Parse.WhiteSpace.Many()
            from attrs in KeyValPair.Or(Text).Optional().DelimitedBy(CipGrammar.Comma)
            select new ContentElement(id.IsEmpty ? "0" : id.Get(), attrs.Select(i => i.IsEmpty ? string.Empty : i.Get()).ToArray());

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
}
