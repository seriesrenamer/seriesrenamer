using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Renamer.Classes
{
    class StringTools
    {

        public static bool ContainsLowercaseCharacters(string str)
        {
            return Regex.IsMatch(str, "\\p{Ll}");
        }
        public static bool ContainsUppercaseCharacters(string str)
        {
            return Regex.IsMatch(str, "\\p{Lu}");
        }
    }
}
