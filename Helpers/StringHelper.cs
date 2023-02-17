﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AstraBlog.Helpers
{
    public static class StringHelper
    {
        public static string BlogSlug(string title)
        {
            // Remove all accents and make the string lower case.
            string output = RemoveAccents(title).ToLower();

            //Remove special characters
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

            //Remove all additional spaces in favor of just one
            output = Regex.Replace(output, @"\s+", " ").Trim();

            //Replace all spaces with the hypen
            output = Regex.Replace(output, @"\s", "-");

            return output;
        }

        private static string RemoveAccents(string title)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                return title;
            }


            // Convert to Unicode
            title = title.Normalize(NormalizationForm.FormD);


            // Format unicode/asscii
            char[] chars = title.Where(c=> CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();


            // Convert and return the new title
            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}
