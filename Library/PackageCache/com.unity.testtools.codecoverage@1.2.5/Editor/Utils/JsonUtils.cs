using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.TestTools.CodeCoverage.Utils;

namespace UnityEditor.TestTools.CodeCoverage
{
    internal static class JsonUtils
    {
        internal readonly static string[] m_validKeys = { "assembliesInclude", "assembliesExclude", "pathsInclude", "pathsExclude" };

        // removes whitespace from places it would be problematic
        internal static string CleanJsonString(string jsonString)
        {
            string result = jsonString;

            // removes whitespace before the json key's colons
            string regexSpaceBetweenKeyAndColon = @"(?<=\"")\s+(?=:)";
            result = Regex.Replace(result, regexSpaceBetweenKeyAndColon, string.Empty, RegexOptions.None, TimeSpan.FromSeconds(5));

            // trims whitespace from end of keys + values
            string regexTrailingWhitespace = @"\s+(?=\"")";
            result = Regex.Replace(result, regexTrailingWhitespace, string.Empty, RegexOptions.None, TimeSpan.FromSeconds(5));

            // trims whitespace from beginning of keys + values
            string regexPrecedingWhitespace = @"(?<=\"")\s+";
            result = Regex.Replace(result, regexPrecedingWhitespace, string.Empty, RegexOptions.None, TimeSpan.FromSeconds(5));

            return result;
        }

        internal static void ValidateJsonKeys(string jsonString)
        {
            // grabs all json keys by looking before the colons
            Regex jsonKeyRegex = new Regex(@"[a-zA-Z0-9\s]+(?=\"":)", RegexOptions.None, TimeSpan.FromSeconds(5));
            string[] keys = jsonKeyRegex.Matches(jsonString)
                                        .Cast<Match>()
                                        .Select(m => m.Value)
                                        .ToArray();

            if (!AreAllKeysUnique(keys, out string[] duplicateKeys))
                ResultsLogger.Log(ResultID.Warning_FilterFileContainsDuplicateKeys, string.Join(",", duplicateKeys));

            if (!AreAllKeysValid(keys, m_validKeys, out string[] invalidKeys))
                ResultsLogger.Log(ResultID.Warning_FilterFileContainsInvalidKey, string.Join(",", invalidKeys));
        }

        internal static bool AreAllKeysUnique(string[] keys, out string[] duplicateKeys)
        {
            duplicateKeys = keys.GroupBy(key => key, StringComparer.InvariantCultureIgnoreCase)
                            .Where(keyGroup => keyGroup.Count() > 1)
                            .Select(keyGroup => keyGroup.Key)
                            .ToArray();

            if (duplicateKeys.Length > 0) return false;
            return true;
        }

        internal static bool AreAllKeysValid(string[] keys, string[] validKeys, out string[] invalidKeys)
        {
            invalidKeys = keys.Where(keyToCheck => !validKeys.Any(validKey => keyToCheck.Equals(validKey, StringComparison.InvariantCultureIgnoreCase)))
                                .Distinct()
                                .ToArray();

            if (invalidKeys.Length > 0) return false;
            return true;
        }
    }
}
