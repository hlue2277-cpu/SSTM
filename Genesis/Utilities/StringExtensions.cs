using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Genesis.Utilities
{
    public static class StringExtensions
    {
        private static readonly string[] MultiPlatformNewLine = new[] { "\r\n", "\r", "\n" };

        [ExcludeFromCodeCoverage]
        [DebuggerStepThrough]
        public static string DefaultFormat(this string target, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, target, args);
        }

        [ExcludeFromCodeCoverage]
        [DebuggerStepThrough]
        public static string CultureFormat(this string target, params object[] args)
        {
            return string.Format(CultureInfo.CurrentUICulture, target, args);
        }

        public static string ToValue(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var trimed = input.Trim();
            return trimed.Length == 0 ? null : trimed;
        }

        public static string Trim(this string value, string trimString)
        {
            return TrimHelper(value, trimString, "^({0})+|({0})+$");
        }

        public static string TrimEnd(this string value, string trimString)
        {
            return TrimHelper(value, trimString, "({0})+$");
        }

        public static string TrimStart(this string value, string trimString)
        {
            return TrimHelper(value, trimString, "^({0})+");
        }

        [ExcludeFromCodeCoverage]
        [DebuggerStepThrough]
        public static string RemoveNewLine(this string text)
        {
            if (text == null)
                return null;
            return text.Replace(Environment.NewLine, string.Empty);
        }

        [ExcludeFromCodeCoverage]
        [DebuggerStepThrough]
        public static string[] SplitNewLine(this string text, StringSplitOptions option = StringSplitOptions.None)
        {
            if (text == null) return null;
            return text.Split(MultiPlatformNewLine, option);
        }

        private static string TrimHelper(string value, string trimString, string patternFormat)
        {
            var pattern = string.Format(patternFormat, Regex.Escape(trimString));
            return Regex.Replace(value, pattern, string.Empty);
        }
    }
}