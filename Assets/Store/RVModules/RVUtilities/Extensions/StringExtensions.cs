// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.IO;
using System.Linq;

namespace RVModules.RVUtilities.Extensions
{
    public static class StringExtensions
    {
        #region Public methods

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static string GetWithoutIllegal(this string _path)
        {
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (var c in invalid)
                _path = _path.Replace(c.ToString(), "");
            return _path;
        }

        public static string RemoveAfter(this string t, string _after)
        {
            t = t.Remove(t.IndexOf(_after));
            return t;
        }

        /// <summary>
        /// Separates words from text by whitespaces
        /// </summary>
        public static string[] GetWords(this string text, string[] separators)
        {
            var s = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return s;
        }

        public static string RemoveMany(this string text, params string[] remove)
        {
            var t = text;
            foreach (var s in remove)
                while (t.Contains(s))
                    t = t.Replace(s, "");
            return t;
        }

        public static string[] GetWordsByWhitespaces(this string text, string[] separators) => text.GetWords(new[] {" "});

        public static int GetWordCount(this string text)
        {
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }

            return wordCount;
        }

        #endregion
    }
}