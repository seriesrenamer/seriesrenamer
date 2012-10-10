using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Classes
{
    class RegexConverter
    {
        public static string toRegex(string easyRegex) {
            easyRegex = easyRegex.Replace("%T", "*.?");
            if (easyRegex.Contains("%S%E")) {
                easyRegex = easyRegex.Replace("%S", "(?<Season>\\d)");
                easyRegex  = easyRegex.Replace("%E", "(?<Episode>\\d\\d+)");
            }
            else {
                easyRegex = easyRegex.Replace("%S", "(?<Season>\\d+)");
                easyRegex = easyRegex.Replace("%E", "(?<Episode>\\d+)");
            }
            return easyRegex;
        }
        public static string toEasyRegex(string regex) {
            return null;
        }

        public static string replaceSeriesname(string regex, string showname) {
            return regex.Replace("%S", "(?<Season>\\d+)").Replace("%T", showname);
        }
        public static string replaceSeriesnameAndSeason(string regex, string showname, int season)
        {
            return regex.Replace("%S", season.ToString()).Replace("%T", showname);
        }
        public static string replaceSeason(string regex, string season) {
            return regex.Replace("%S", season);
        }
    }
}
