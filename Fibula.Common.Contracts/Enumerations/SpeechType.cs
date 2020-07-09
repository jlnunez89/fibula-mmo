// -----------------------------------------------------------------
// <copyright file="SpeechType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible speech types.
    /// </summary>
    public enum SpeechType : byte
    {
        /// <summary>
        /// A normal talk.
        /// </summary>
        Say = 0x01,

        /// <summary>
        /// Whispering (#w).
        /// </summary>
        Whisper = 0x02,

        /// <summary>
        /// Yelling (#y).
        /// </summary>
        Yell = 0x03,

        /// <summary>
        /// Players speaking privately to other players.
        /// </summary>
        Private = 0x04,

        /// <summary>
        /// Yellow message in chat.
        /// </summary>
        ChannelYellow = 0x05,

        /// <summary>
        /// Rule violation report.
        /// </summary>
        RuleViolationReport = 0x06,

        /// <summary>
        /// Rule violation reply.
        /// </summary>
        RuleViolationAnswer = 0x07,

        /// <summary>
        /// Rule violation continuance.
        /// </summary>
        RuleViolationContinue = 0x08,

        /// <summary>
        /// Broadcast a message (#b).
        /// </summary>
        Broadcast = 0x09,

        /// <summary>
        /// Talk in organge.
        /// </summary>
        MonsterSay = 0x0E,

        // ChannelRed = 0x05,   //Talk red on chat - #c
        // PrivateRed = 0x04,   //Red private - @name@ text
        // ChannelOrange = 0x05,    //Talk orange on text
        // ChannelRedAnonymous = 0x05,  //Talk red anonymously on chat - #d
        // MonsterYell = 0x0E,  //Yell orange
    }
}
