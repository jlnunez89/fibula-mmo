// -----------------------------------------------------------------
// <copyright file="Rsa.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Security.Encryption
{
    using System;
    using OpenTibia.Common.Utilities;

    public static class Rsa
    {
        static readonly BigInteger OtServerP = new BigInteger("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113", 10);
        static readonly BigInteger OtServerQ = new BigInteger("7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101", 10);
        static readonly BigInteger OtServerE = new BigInteger("65537", 10);

        static readonly BigInteger OtServerD = new BigInteger("46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073", 10);

        static readonly BigInteger OtServerM = new BigInteger("109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413", 10);
        static readonly BigInteger OtServerDp = new BigInteger("11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937", 10);
        static readonly BigInteger OtServerDq = new BigInteger("4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973", 10);
        static readonly BigInteger OtServerInverseQ = new BigInteger("5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418", 10);

        static readonly BigInteger CipsoftP = new BigInteger("12017580013707233233987537782574702577133548287527131234152948150506251412291888866940292054989907714155267326586216043845592229084368540020196135619327879", 10);
        static readonly BigInteger CipsoftQ = new BigInteger("11898921368616868351880508246112101394478760265769325412746398405473436969889506919017477758618276066588858607419440134394668095105156501566867770737187273", 10);
        static readonly BigInteger CipsoftE = new BigInteger("65537", 10);

        static readonly BigInteger CipsoftD = new BigInteger("76986729922013555803005346411676553796216642757744136918874786680847350484561355561990197463851432043966925732503405503392169872913316040431385473364992747280187301473917937217770571147715209164630731036001536097640055859779719441340893852413578783372753311287389876226475916787249536441608198161537606056385", 10);

        static readonly BigInteger CipsoftM = new BigInteger("142996239624163995200701773828988955507954033454661532174705160829347375827760388829672133862046006741453928458538592179906264509724520840657286865659265687630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212883967", 10);
        static readonly BigInteger CipsoftDp = new BigInteger("8600096169230651977875330302008843109503996439950294564624155336050523997688169856104180804416233147594214529455018271455182195462973351342710205846262073", 10);
        static readonly BigInteger CipsoftDq = new BigInteger("2519694687789567709651001624113763876170350107089837772221714711859870886188360422694425383128682046052156487995590127487208810654582326452919586222907441", 10);
        static readonly BigInteger CipsoftInverseQ = new BigInteger("1174780978094632975480997011665925784916890194200219560434769607142991788322744284466767726580919222136359133434371639500067898638577680086982263461836866", 10);

        public static bool Encrypt(ref byte[] buffer, int position, bool useCipValues = true)
        {
            return useCipValues ? Encrypt(CipsoftE, CipsoftM, ref buffer, position) : Encrypt(OtServerE, OtServerM, ref buffer, position);
        }

        public static bool Encrypt(BigInteger e, BigInteger m, ref byte[] buffer, int position)
        {
            byte[] temp = new byte[128];

            Array.Copy(buffer, position, temp, 0, 128);

            BigInteger input = new BigInteger(temp);
            BigInteger output = input.ModPow(e, m);

            Array.Copy(GetPaddedValue(output), 0, buffer, position, 128);

            return true;
        }

        public static bool Decrypt(ref byte[] buffer, int position, int length, bool useCipValues = true)
        {
            if (length - position != 128)
            {
                return false;
            }

            position = length - 128;

            byte[] temp = new byte[128];
            Array.Copy(buffer, position, temp, 0, 128);

            BigInteger input = new BigInteger(temp);
            BigInteger output;

            BigInteger m1 = useCipValues ? input.ModPow(CipsoftDp, CipsoftP) : input.ModPow(OtServerDp, OtServerP);
            BigInteger m2 = useCipValues ? input.ModPow(CipsoftDq, CipsoftQ) : input.ModPow(OtServerDq, OtServerQ);
            BigInteger h;

            if (useCipValues)
            {
                if (m2 > m1)
                {
                    h = CipsoftP - (((m2 - m1) * CipsoftInverseQ) % CipsoftP);
                }
                else
                {
                    h = ((m1 - m2) * CipsoftInverseQ) % CipsoftP;
                }

                output = m2 + (CipsoftQ * h);
            }
            else
            {
                if (m2 > m1)
                {
                    h = OtServerP - (((m2 - m1) * OtServerInverseQ) % OtServerP);
                }
                else
                {
                    h = ((m1 - m2) * OtServerInverseQ) % OtServerP;
                }

                output = m2 + (OtServerQ * h);
            }

            Array.Copy(GetPaddedValue(output), 0, buffer, position, 128);
            return true;
        }

        private static byte[] GetPaddedValue(BigInteger value)
        {
            byte[] result = value.GetBytes();

            int length = 1024 >> 3;
            if (result.Length >= length)
            {
                return result;
            }

            // left-pad 0x00 value on the result (same integer, correct length)
            byte[] padded = new byte[length];
            Buffer.BlockCopy(result, 0, padded, length - result.Length, result.Length);
            // temporary result may contain decrypted (plaintext) data, clear it
            Array.Clear(result, 0, result.Length);
            return padded;
        }
    }
}
