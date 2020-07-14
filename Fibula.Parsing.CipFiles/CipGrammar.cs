// -----------------------------------------------------------------
// <copyright file="CipGrammar.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.CipFiles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Parsing.Contracts;
    using Sprache;

    /// <summary>
    /// Static class that contains common grammar used to parse CipSoft files.
    /// </summary>
    /// <remarks>
    /// NOTE: Members of this class must remain public in order for it to work correctly, because Sprache.
    /// </remarks>
    public static class CipGrammar
    {
        /// <summary>
        /// The equals sign character.
        /// </summary>
        public static readonly Parser<char> EqualSign = Parse.Char('=');

        /// <summary>
        /// The double quote character.
        /// </summary>
        public static readonly Parser<char> DoubleQuote = Parse.Char('"');

        /// <summary>
        /// The comma character.
        /// </summary>
        public static readonly Parser<char> Comma = Parse.Char(',');

        /// <summary>
        /// The backslash character.
        /// </summary>
        public static readonly Parser<char> Backslash = Parse.Char('\\');

        /// <summary>
        /// The open parenthesis character.
        /// </summary>
        public static readonly Parser<char> OpenParenthesis = Parse.Char('(');

        /// <summary>
        /// The close parenthesis character.
        /// </summary>
        public static readonly Parser<char> CloseParenthesis = Parse.Char(')');

        /// <summary>
        /// The open bracket character.
        /// </summary>
        public static readonly Parser<char> OpenBracket = Parse.Char('[');

        /// <summary>
        /// The close bracket character.
        /// </summary>
        public static readonly Parser<char> CloseBracket = Parse.Char(']');

        /// <summary>
        /// The open curly brace character.
        /// </summary>
        public static readonly Parser<char> OpenCurly = Parse.Char('{');

        /// <summary>
        /// The close curly brace character.
        /// </summary>
        public static readonly Parser<char> CloseCurly = Parse.Char('}');

        /// <summary>
        /// The colon character.
        /// </summary>
        public static readonly Parser<char> Colon = Parse.Char(':');

        /// <summary>
        /// The dash character.
        /// </summary>
        public static readonly Parser<char> Dash = Parse.Char('-');

        /// <summary>
        /// The zero character.
        /// </summary>
        public static readonly Parser<char> Zero = Parse.Char('0');

        /// <summary>
        /// A message enclosed in double quotes.
        /// </summary>
        public static readonly Parser<string> QuotedMessage =
            from open in DoubleQuote
            from text in Quoted.Or(Escaped).Many().Text()
            from close in DoubleQuote
            select open + text + close;

        /// <summary>
        /// The separator between a rule's conditions and actions.
        /// </summary>
        public static readonly Parser<IEnumerable<char>> ConditionsActionsSeparator = Parse.String("->");

        /// <summary>
        /// The 'greater than' comparison.
        /// </summary>
        public static readonly Parser<string> GreaterThanComparison = Parse.String(">").Text();

        /// <summary>
        /// The 'less than' comparison.
        /// </summary>
        public static readonly Parser<string> LessThanComparison = Parse.String("<").Text();

        /// <summary>
        /// The 'greater than or equal' comparison.
        /// </summary>
        public static readonly Parser<string> GreaterThanOrEqualToComparison = Parse.String(">=").Text();

        /// <summary>
        /// The 'less than or equal' comparison.
        /// </summary>
        public static readonly Parser<string> LessThanOrEqualToComparison = Parse.String("<=").Text();

        /// <summary>
        /// The 'equals' comparison.
        /// </summary>
        public static readonly Parser<string> EqualToComparison = Parse.String("==").Text();

        /// <summary>
        /// Any character except for double quotes.
        /// </summary>
        public static readonly Parser<char> Quoted = Parse.AnyChar.Except(DoubleQuote);

        /// <summary>
        /// An escaped character, preceded by '\'.
        /// </summary>
        public static readonly Parser<char> Escaped =
            from blackSlash in Backslash
            from c in Parse.AnyChar
            select c;

        /// <summary>
        /// Any text, except for special characters.
        /// </summary>
        public static readonly Parser<string> Text =
            from text in Parse.AnyChar.Except(ConditionsActionsSeparator).Except(Comma).Except(EqualSign).AtLeastOnce().Text()
            select text.Trim();

        /// <summary>
        /// Parses a location string, in the form [x, y, z].
        /// </summary>
        public static readonly Parser<string> LocationString =
            from leadingSpaces in Parse.WhiteSpace.Many().Text()
            from open in OpenBracket
            from negX in Parse.Char('-').Optional()
            from x in Parse.Number
            from firstComma in Comma
            from negY in Parse.Char('-').Optional()
            from y in Parse.Number
            from secondComma in Comma
            from negZ in Parse.Char('-').Optional()
            from z in Parse.Number
            from close in CloseBracket
            select $"{open}{(negX.IsEmpty ? string.Empty : "-")}{x},{(negY.IsEmpty ? string.Empty : "-")}{y},{(negZ.IsEmpty ? string.Empty : "-")}{z}{close}";

        /// <summary>
        /// Parses a function in the form: Func(arg0, arg1, ..., argN).
        /// </summary>
        public static readonly Parser<string> FunctionOrComparisonString =
            from functionName in Parse.AnyChar.Except(OpenParenthesis).Except(Comma).Except(EqualSign).Many().Text()
            from open in OpenParenthesis
            from oneOrMoreArguments in Arguments
            from close in CloseParenthesis
            from functionComparison in Parse.AnyChar.Except(Parse.WhiteSpace).Except(Comma).Many().Text()
            select functionName.Trim() + open + oneOrMoreArguments.Aggregate((e, str) => str = $"{e},{str}") + close + functionComparison.Trim();

        /// <summary>
        /// Parses a function arguments that are only text and numbers.
        /// </summary>
        public static readonly Parser<string> Argument =
            QuotedMessage.Or(
                Parse.AnyChar.Except(Comma)
                             .Except(Colon)
                             .Except(OpenParenthesis)
                             .Except(CloseParenthesis)
                             .Except(OpenBracket)
                             .Except(CloseBracket)
                             .AtLeastOnce().Text());

        /// <summary>
        /// Parses a multiple arguments separated by a comma: val0, val1, .. valN.
        /// </summary>
        public static readonly Parser<IEnumerable<string>> OneOrMoreArguments = Argument.DelimitedBy(Comma);

        /// <summary>
        /// Parses a parenthesis tuple argument in the form: (val0, val1, .. valN).
        /// </summary>
        public static readonly Parser<string> ParenthesizedTupleArgument =
            from open in OpenParenthesis
            from oneOrMoreArguments in OneOrMoreArguments
            from close in CloseParenthesis
            select open + oneOrMoreArguments.Aggregate((e, str) => str = $"{e},{str}") + close;

        /// <summary>
        /// Parses a bracket tuple argument in the form: [val0, val1, .. valN].
        /// </summary>
        public static readonly Parser<string> BracketedTupleArgument =
            from open in OpenBracket
            from oneOrMoreArguments in OneOrMoreArguments
            from close in CloseBracket
            select open + oneOrMoreArguments.Aggregate((e, str) => str = $"{e},{str}") + close;

        /// <summary>
        /// Parses multiple arguments.
        /// </summary>
        public static readonly Parser<IEnumerable<string>> Arguments = Argument.Or(ParenthesizedTupleArgument).Or(BracketedTupleArgument).DelimitedBy(Comma);

        /// <summary>
        /// Parses a Key/Value pair in the form: key=value.
        /// </summary>
        public static readonly Parser<string> KeyValStr =
            from key in Text
            from eq in EqualSign
            from value in Text
            select key + eq + value;

        /// <summary>
        /// Parses a collection of condition functions.
        /// </summary>
        public static readonly Parser<IEnumerable<string>> Conditions = FunctionOrComparisonString.Or(QuotedMessage).Or(KeyValStr).Or(Text).DelimitedBy(Comma);

        /// <summary>
        /// Parses a collection of action functions.
        /// </summary>
        public static readonly Parser<IEnumerable<string>> Actions = FunctionOrComparisonString.Or(QuotedMessage).Or(KeyValStr).Or(Text).DelimitedBy(Comma);

        ///// <summary>
        ///// Parses the raw event rules.
        ///// </summary>
        // public static readonly Parser<ParsedEventRule> EventRule =
        //    from conditions in Conditions
        //    from leading in Parse.WhiteSpace.Optional().Many()
        //    from separator in ConditionsActionsSeparator
        //    from trailing in Parse.WhiteSpace.Many()
        //    from actions in Actions
        //    select new ParsedEventRule(conditions, actions);

        /// <summary>
        /// Parses monster spells.
        /// </summary>
        public static readonly Parser<(IEnumerable<string> conditions, IEnumerable<string> effects, string chance)> MonsterSpellRule =
            from spellCondition in Conditions
            from lws in Parse.WhiteSpace.Optional().Many()
            from separator in ConditionsActionsSeparator
            from tws in Parse.WhiteSpace.Optional().Many()
            from spellEffect in Actions
            from lws2 in Parse.WhiteSpace.Optional().Many()
            from chanceSeparator in Colon
            from tws2 in Parse.WhiteSpace.Optional().Many()
            from chance in Parse.Number
            select (spellCondition, spellEffect, chance);

        /// <summary>
        /// The outfit lookType for the normal outfit.
        /// </summary>
        public static readonly Parser<(ushort lookTypeId, byte headColor, byte bodyColor, byte legsColor, byte feetColor)> Outfit =
            from looktypeStr in Parse.Number
            from comma in Comma
            from mws in Parse.WhiteSpace.Optional().Many()
            from headColorStr in Parse.Number
            from d0 in Dash
            from bodyColorStr in Parse.Number
            from d1 in Dash
            from legsColorStr in Parse.Number
            from d2 in Dash
            from feetColorStr in Parse.Number
            select (Convert.ToUInt16(looktypeStr), Convert.ToByte(headColorStr), Convert.ToByte(bodyColorStr), Convert.ToByte(legsColorStr), Convert.ToByte(feetColorStr));

        /// <summary>
        /// The outfit lookType for the invisible outfit.
        /// </summary>
        public static readonly Parser<(ushort lookTypeId, byte headColor, byte bodyColor, byte legsColor, byte feetColor)> OutfitInvisible =
            from firstZero in Zero
            from comma in Comma
            from mws in Parse.WhiteSpace.Optional().Many()
            from secondZero in Zero
            select (ushort.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);

        /// <summary>
        /// Parses a monster outfit.
        /// </summary>
        public static readonly Parser<(ushort lookTypeId, byte headColor, byte bodyColor, byte legsColor, byte feetColor)> MonsterOutfit =
            from open in OpenParenthesis
            from outfit in OutfitInvisible.Or(Outfit)
            from close in CloseParenthesis
            select outfit;

        /// <summary>
        /// Parses monster spells.
        /// </summary>
        public static readonly Parser<IEnumerable<(IEnumerable<string> conditions, IEnumerable<string> effects, string chance)>> MonsterSpellRules =
            from open in OpenCurly
            from spells in MonsterSpellRule.DelimitedBy(Comma)
            from close in CloseCurly
            select spells;

        /// <summary>
        /// Parses action functions.
        /// </summary>
        public static readonly Parser<ActionFunction> ActionFunction =
            from functionName in Parse.AnyChar.Except(OpenParenthesis).Except(Comma).Except(EqualSign).Many().Text()
            from open in OpenParenthesis
            from oneOrMoreArguments in LocationString.Or(QuotedMessage).Or(Parse.AnyChar.Except(CloseParenthesis).Except(Comma).Many().Text()).DelimitedBy(Comma)
            from close in CloseParenthesis
            select new ActionFunction(functionName.Trim(), oneOrMoreArguments.ToArray());

        /// <summary>
        /// Parses comparison functions.
        /// </summary>
        public static readonly Parser<ComparisonFunction> ComparisonFunction =
            from functionName in Parse.AnyChar.Except(OpenParenthesis).Except(Comma).Except(EqualSign).Many().Text()
            from open in OpenParenthesis
            from oneOrMoreArguments in LocationString.Or(QuotedMessage).Or(Parse.AnyChar.Except(CloseParenthesis).Except(Comma).Many().Text()).DelimitedBy(Comma)
            from close in CloseParenthesis
            from functionComparison in GreaterThanComparison.Or(GreaterThanOrEqualToComparison).Or(LessThanComparison).Or(LessThanOrEqualToComparison).Or(EqualToComparison)
            from identifier in Parse.AnyChar.Except(Comma).Many().Text()
            select new ComparisonFunction(functionName.Trim(), functionComparison, identifier, oneOrMoreArguments.ToArray());

        /// <summary>
        /// Parses monster inventory entries.
        /// </summary>
        public static readonly Parser<(ushort, byte, ushort)> MonsterInventoryEntry =
            from open in OpenParenthesis
            from content in OneOrMoreArguments
            from close in CloseParenthesis
            select (Convert.ToUInt16(content.ElementAt(0)), Convert.ToByte(content.ElementAt(1)), Convert.ToUInt16(content.ElementAt(2)));

        /// <summary>
        /// Parses a monster inventory.
        /// </summary>
        public static readonly Parser<IEnumerable<(ushort, byte, ushort)>> MonsterInventory =
            from open in OpenCurly
            from inventoryItems in MonsterInventoryEntry.DelimitedBy(Comma)
            from close in CloseCurly
            select inventoryItems;

        /// <summary>
        /// Parses monster skill entries, in the form (skillName, currentLevel, minimumLevel, maximumLevel, currentCount, countForNextLevel, addOnLevel).
        /// </summary>
        public static readonly Parser<(string, int, int, int, uint, uint, byte)> MonsterSkillEntry =
            from open in OpenParenthesis
            from content in OneOrMoreArguments
            from close in CloseParenthesis
            select (content.ElementAt(0),
                Convert.ToInt32(content.ElementAt(1)),
                Convert.ToInt32(content.ElementAt(2)),
                Convert.ToInt32(content.ElementAt(3)),
                Convert.ToUInt32(content.ElementAt(4)),
                Convert.ToUInt32(content.ElementAt(5)),
                Convert.ToByte(content.ElementAt(6)));

        /// <summary>
        /// Parses a monster's skills.
        /// </summary>
        public static readonly Parser<IEnumerable<(string, int, int, int, uint, uint, byte)>> MonsterSkills =
            from open in OpenCurly
            from skillEntries in MonsterSkillEntry.DelimitedBy(Comma)
            from close in CloseCurly
            select skillEntries;

        /// <summary>
        /// Parses a monster strategy.
        /// </summary>
        public static readonly Parser<(byte closest, byte lowestHp, byte mostDamage, byte random)> MonsterStrategy =
            from open in OpenParenthesis
            from content in OneOrMoreArguments
            from close in CloseParenthesis
            select (Convert.ToByte(content.ElementAt(0)), Convert.ToByte(content.ElementAt(1)), Convert.ToByte(content.ElementAt(2)), Convert.ToByte(content.ElementAt(3)));

        /// <summary>
        /// Parses a creature phrases in the form { "phrase0", "phrase1", .. "phraseN" }.
        /// </summary>
        public static readonly Parser<IEnumerable<string>> CreaturePhrases =
            from open in OpenCurly
            from phrases in QuotedMessage.DelimitedBy(Comma)
            from close in CloseCurly
            select phrases;
    }
}
