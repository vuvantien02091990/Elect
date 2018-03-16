﻿#region	License
//--------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Elect </Project>
//     <File>
//         <Name> StringHelper.cs </Name>
//         <Created> 16/03/2018 8:45:29 AM </Created>
//         <Key> 55b0f67d-198e-40fb-8750-62900e039f2c </Key>
//     </File>
//     <Summary>
//         StringHelper.cs is a part of Elect
//     </Summary>
// <License>
//--------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Elect.Core.StringUtils
{
    public class StringHelper
    {
        #region Random

        public static readonly char[] UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static readonly char[] LowerChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        public static readonly char[] NumberChars = "0123456789".ToCharArray();
        public static readonly char[] SpecialChars = "!@#$%^*&".ToCharArray();

        public static string Generate(int length, bool isIncludeUpper = true, bool isIncludeLower = true, bool isIncludeNumber = true, bool isIncludeSpecial = false)
        {
            var chars = new List<char>();

            if (isIncludeUpper)
            {
                chars.AddRange(UpperChars);
            }

            if (isIncludeLower)
            {
                chars.AddRange(LowerChars);
            }

            if (isIncludeNumber)
            {
                chars.AddRange(NumberChars);
            }

            if (isIncludeSpecial)
            {
                chars.AddRange(SpecialChars);
            }

            return GenerateRandom(length, chars.ToArray());
        }

        public static string GenerateRandom(int length, params char[] chars)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"{length} cannot be less than zero.");
            }

            if (chars?.Any() != true)
            {
                throw new ArgumentOutOfRangeException(nameof(chars), $"{nameof(chars)} cannot be empty.");
            }

            chars = chars.Distinct().ToArray();

            const int maxLength = 256;

            if (maxLength < chars.Length)
            {
                throw new ArgumentException(string.Format("{0} may contain more than {1} chars.", nameof(chars), maxLength), nameof(chars));
            }

            var outOfRangeStart = maxLength - (maxLength % chars.Length);

            using (var rng = RandomNumberGenerator.Create())
            {
                var sb = new StringBuilder();

                var buffer = new byte[128];

                while (sb.Length < length)
                {
                    rng.GetBytes(buffer);

                    for (var i = 0; i < buffer.Length && sb.Length < length; ++i)
                    {
                        if (outOfRangeStart <= buffer[i])
                        {
                            continue;
                        }

                        sb.Append(chars[buffer[i] % chars.Length]);
                    }
                }

                return sb.ToString();
            }
        }

        #endregion

        #region Norm

        /// <summary>
        ///     Normalize: UPPER case with remove all diacritic (accents) and convert edge case to
        ///                normal char in string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         If value is is <c> null </c> or <c> whitespace </c> will return <c> empty string </c>
        ///     </para>
        ///     <para> See more: https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1308-normalize-strings-to-uppercase </para>
        /// </remarks>
        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.Trim();

            // Convert Edge case
            value = string.Join(string.Empty, value.Select(ConvertEdgeCases));

            var normalizedString = RemoveAccents(value);

            return normalizedString.ToUpperInvariant();
        }

        public static string ConvertEdgeCases(char c)
        {
            string swap;
            switch (c)
            {
                case 'ı':
                    swap = "i";
                    break;

                case 'ł':
                case 'Ł':
                    swap = "l";
                    break;

                case 'đ':
                    swap = "d";
                    break;

                case 'Đ':
                    swap = "D";
                    break;

                case 'ß':
                    swap = "ss";
                    break;

                case 'ø':
                    swap = "o";
                    break;

                case 'Þ':
                    swap = "th";
                    break;

                default:
                    swap = c.ToString();
                    break;
            }

            return swap;
        }

        /// <summary>
        ///     Remove all diacritics (accents) in string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks> Already handle edge case <see cref="ConvertEdgeCases" /> </remarks>
        public static string RemoveAccents(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var normalizedString = value.Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                var edgeCases = ConvertEdgeCases(c);

                stringBuilder.Append(edgeCases);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        #endregion

        #region Base 64

        public static bool IsBase64(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            try
            {
                var byteArray = Convert.FromBase64String(value);

                return byteArray != null;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}