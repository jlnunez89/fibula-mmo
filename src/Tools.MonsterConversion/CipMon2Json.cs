// -----------------------------------------------------------------
// <copyright file="CipMon2Json.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Tools.MonsterConversion
{
    /// <summary>
    /// Static class with methods to convert from CipSoft formatted monster files (.mon) to JSON format.
    /// </summary>
    public static class CipMon2Json
    {
        /// <summary>
        /// The extension for monster files.
        /// </summary>
        private const string MonsterFileExtension = "mon";

        public string Convert(string monFileContent)
        {
            foreach (var monsterFileInfo in this.monsterFilesDirInfo.GetFiles($"*.{MonsterFileExtension}"))
            {
                var monsterType = this.ReadMonsterFile(monsterFileInfo);

                if (monsterType != null)
                {
                    monsterTypesDictionary.Add(monsterType.RaceId, monsterType);
                }
            }
        }
    }
}
