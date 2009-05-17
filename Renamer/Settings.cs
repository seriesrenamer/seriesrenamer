using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Renamer.Classes;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes.Configuration;

namespace Renamer
{
    /// <summary>
    /// Class used to store settings which can't be stored in config file
    /// </summary>
    class Settings
    {
        /// <summary>
        /// filename of the main config file
        /// </summary>
        public static string ConfigName = "Renamer.cfg";

        /// <summary>
        /// Delimiter used in config file
        /// </summary>        
        public static char Delimiter
        {
            get { return delim; }
            set
            {
                if (delim != Convert.ToChar(0))
                {
                    Helper.WriteProperty(Config.Delimiter, value.ToString());
                }
                delim = value;                
            }
        }
        private static char delim;
        
        
        /// <summary>
        /// Comment string used in config file
        /// </summary>
        public static string Comment
        {
            get { return comment; }
            set
            {
                if (comment != null)
                {
                    Helper.WriteProperty(Config.Comment, value);                    
                }
                comment = value;
            }
        }
        private static string comment;

        /// <summary>
        /// List of cached config files
        /// </summary>
        public static List<ConfigFile> files=new List<ConfigFile>();

        /// <summary>
        /// Cache config file containing default values for main config file
        /// </summary>
        public static ConfigFile Defaults = new ConfigFile("");

        /// <summary>
        /// Mono compatibility mode
        /// </summary>
        public static bool MonoCompatibilityMode = false;

        /// <summary>
        /// if main config file is loaded, used for avoiding some recursion at startup
        /// </summary>
        public static bool ConfigLoaded = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Settings()
        {
            delim = Convert.ToChar(0);
            SetupDefaults();
        }

        /// <summary>
        /// Adds default values to default config file
        /// </summary>
        public void SetupDefaults(){
            Defaults.variables.Add(Config.Comment, "//");
            Defaults.variables.Add(Config.Delimiter, "^");
            Defaults.variables.Add(Config.Extensions, new List<string>(new string[] { "avi", "mpg", "mpeg", "mp4", "divx", "mkv", "wmv" }));
            Defaults.variables.Add(Config.SubtitleExtensions, new List<string>(new string[] { "srt", "sub" }));
            Defaults.variables.Add(Config.LogLevelInfo, "LogFile");
            Defaults.variables.Add(Config.LogLevelWarning, "LogFile");
            Defaults.variables.Add(Config.LogLevelError, "Log_and_Message");
            Defaults.variables.Add(Config.LogLevelStatus, "LogFile");
            Defaults.variables.Add(Config.LogLevelDebug, "None");
            Defaults.variables.Add(Config.LogName, "Renamer.log");
            Defaults.variables.Add(Config.MaxDepth, "0");
            Defaults.variables.Add(Config.InvalidCharReplace, "-");
            Defaults.variables.Add(Config.EpIdentifier, new List<string>(new string[] { "S%SE%E", "%Sx%E", "S%S.E%E", "- %S%E -", "-E%E-", ".%S%E.", "%S.%E", "%S%E" }));
            Defaults.variables.Add(Config.TargetPattern, "S%sE%E - %N");
            Defaults.variables.Add(Config.LastTitles, new List<string>(new string[]{"The Simpsons", "Heroes", "Scrubs", "Supernatural", "Eureka", "Chuck", "Stargate", "Stargate Atlantis", "Dexter", "Doctor Who", "24", "Firefly", "Kyle XY", "Prison Break", "Moonlight", "Monk", "House M.D.", "Gilmore Girls", "Friends", "Sex and the City"}));
            Defaults.variables.Add(Config.LastProvider, "");
            Defaults.variables.Add(Config.LastDirectory, "");
            Defaults.variables.Add(Config.LastSubProvider, "");
            Defaults.variables.Add(Config.Timeout, "10000");
            Defaults.variables.Add(Config.InvalidFilenameAction, "Ask");
            Defaults.variables.Add(Config.Umlaute, "Dont_Use");
            Defaults.variables.Add(Config.Case, "Large");
            Defaults.variables.Add(Config.Extract, new List<string>(new string[] { "Season %S", "Season_%S", "Season%S", "Staffel %S", "Staffel_%S", "Staffel%S" }));
            Defaults.variables.Add(Config.CreateDirectoryStructure, "1");
            Defaults.variables.Add(Config.DeleteEmptyFolders, "1");
            Defaults.variables.Add(Config.DeleteAllEmptyFolders, "1");
            Defaults.variables.Add(Config.IgnoreFiles, new List<string>(new string[]{"nfo","diz"}));
            Defaults.variables.Add(Config.ColumnOrder, new List<string>(new string[]{"0","1","2","3","4","6","5"}));
            Defaults.variables.Add(Config.ColumnWidths, new List<string>(new string[] {"209", "137", "55", "57", "144", "188", "188" }));
            Defaults.variables.Add(Config.WindowSize, new List<string>(new string[] { "1024", "600" }));
            Defaults.variables.Add(Config.UseSeasonSubDir, "1");
            Defaults.variables.Add(Config.TitleHistorySize, "100");
            Defaults.variables.Add(Config.ShownameExtractionRegex, "((?<pos>\\.S\\d+E\\d+\\.)|(?<pos>\\.\\d+\\.)|(?<pos>[^\\. _-]\\d{3,}))");
            Defaults.variables.Add(Config.CleanupRegex, "[.-_;,!='+]");
            Defaults.variables.Add(Config.Replace, new List<string>(new string[]{"//Comments are indicated by //","//Format used here is \"From->To\",","//where \"From\" is a c# regular expression, see","//http://www.radsoftware.com.au/articles/regexlearnsyntax.aspx for details","//example: \"\\s->.\" replaces whitespaces with dots","cd->CD", " i\\.-> I.", " ii\\.-> II.", " iii\\.-> III.", " iv\\.-> IV.", " v\\.-> V.", " vi\\.-> VI.", " vii\\.-> VII.", " i -> I ", " ii -> II ", " iii -> III ", " iv -> IV ", " v -> V ", " vi -> VI ", " vii -> VII ","\\[\\d+\\]->", " +-> "}));
            Defaults.variables.Add(Config.Tags, new List<string>(new string[]{"Xvid", "DivX", "R5", "R3", "GERMAN", "DVD", "INTERNAL", "PROPER", "TELECINE", "LINE", "LD", "MD", "AC3", "SVCD", "XSVCD", "VCD", "Dubbed", "HD", "720P", "720", "SCREENER", "RSVCD","\\d\\d\\d\\d","TS","GER"}));
            Defaults.variables.Add(Config.ResizeColumns, "1");
            Defaults.variables.Add(Config.ShownameBlacklist, "Season^Staffel");
        }
    }    
}