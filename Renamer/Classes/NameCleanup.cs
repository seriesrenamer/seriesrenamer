using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Renamer.Classes
{
    class NameCleanup
    {
        public static string Postprocessing(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return name;
            }
            name = Regex.Replace(name, "(?<pos1>[\\p{Ll}\\d])(?<pos2>[\\p{Lu}\\d][\\p{Ll}\\d])|(?<pos1>[^\\d])(?<pos2>\\d)", new MatchEvaluator(NameCleanup.InsertSpaces));
            name = Regex.Replace(name, "\\[.+\\]|\\(.+\\)", "");
            name = name.Trim(new char[] { '-', '_', '.', ' ', '(', ')', '[', ']' });
            name = Regex.Replace(name, "[\\._]", " ");
            name = Regex.Replace(name, "\\[.*\\]", "");
            name = name.Replace("  ", " ");
            if (!StringTools.ContainsLowercaseCharacters(name) || !StringTools.ContainsUppercaseCharacters(name))
            {
                name = Helper.UpperEveryFirst(name);
            }
            name = name.Trim();
            return name;
        }

        private static string InsertSpaces(Match m)
        {
            //if we only found numbers, we want to skip this tag (i.e. 007)
            foreach (char c in m.Groups["pos1"].Value + m.Groups["pos2"].Value)
            {
                if (!Char.IsDigit(c))
                {
                    return m.Groups["pos1"].Value + " " + m.Groups["pos2"].Value;
                }
            }
            return m.Groups["pos1"].Value + m.Groups["pos2"].Value;
        }
        public static string RemoveReleaseGroupTag(string filename)
        {
            //remove releasegroup tag, 
            // normally 3 to 6 characters at the beginning of the filename seperated by a '-'
            //if filename too short, it might be a part of the real name, so skip it
            if (filename.Length > 5)
            {
                return Regex.Replace(Regex.Replace(filename, "^(([^\\p{Lu}]\\p{Ll}{1,3})|(\\p{Lu}{2,2}\\w))-", ""), "-\\w{2,4}$", "");
            }
            else return filename;
        }   
    }
}
