﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Classes
{
    class RegexConverter
    {
        public static string toRegex(string easyRegex) {
            easyRegex = easyRegex.Replace("%T", "*.?");
            if (!easyRegex.Contains("%S")) {
                easyRegex = easyRegex.Replace("%S", "(?<Season>\\d)");
            }else if (easyRegex.Contains("%S%E")) {
                easyRegex = easyRegex.Replace("%S", "(?<Season>\\d)");
                easyRegex  = easyRegex.Replace("%E", "(?<Episode>\\d+)");
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
    }
}