#region SVN Info
/***************************************************************
 * $Author$
 * $Revision$
 * $Date$
 * $LastChangedBy$
 * $LastChangedDate$
 * $URL$
 * 
 * License: GPLv3
 * 
****************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Renamer.Classes;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes.Configuration;
using System.Collections;

namespace Renamer
{
    /// <summary>
    /// Class used to store settings which can't be stored in config file
    /// 
    /// This class is a Singleton, only one instance is available
    /// </summary>
    class Settings : IEnumerable
    {

        private static Settings instance;

        /// <summary>
        /// Get the instance of Settings, create the settings, if required
        /// </summary>
        /// <returns>actual Settings</returns>
        public static Settings getInstance() {
            if (instance == null) {
                instance = new Settings();
            }
            return instance;
        }

        /// <summary>
        /// filename of the main config file
        /// </summary>
        public const string MainConfigFileName = "Renamer.cfg";

        private const string comment = "//";
        private const string beginMultiField = "{";
        private const string endMultiField = "}";

        /// <summary>
        /// List of cached config files
        /// </summary>
        private Hashtable files;

        /// <summary>
        /// Cache config file containing default values for main config file
        /// </summary>
        private ConfigFile defaults;

        /// <summary>
        /// Mono compatibility mode
        /// </summary>
        private bool monoCompatibilityMode;

        /// <summary>
        /// Constructor
        /// </summary>
        protected Settings() {
            if (Type.GetType("Mono.Runtime") != null) {
                monoCompatibilityMode = true;
            }
            else {
                monoCompatibilityMode = false;
            }

            files = new Hashtable();
            SetupDefaults();
        }

        public ConfigFile this[string name] {
            get {
                if (!this.fileLoaded(name)) {
                    // Load a file because it's not available yet
                    if (this.IsMonoCompatibilityMode) {
                        Helper.Log(name + " not loaded yet", Helper.LogType.Plain);
                    }
                    return this.addConfigFile(name);
                }
                return (ConfigFile)this.files[name];
            }
            set {
                if (this.files.ContainsKey(name)) {
                    this.files[name] = value;
                }
                else {
                    this.files.Add(name, value);
                }
            }
        }

        public ConfigFile this[int index] {
            get {
                return (ConfigFile)this.files[index];
            }
            set {
                this.files[index] = value;
            }
        }

        /// <summary>
        /// Comment string used in config file
        /// </summary>
        public string Comment {
            get {
                return comment;
            }
        }

        public string BeginMultiValueField {
            get {
                return beginMultiField;
            }
        }
        public string EndMultiValueField {
            get {
                return endMultiField;
            }
        }

        public ConfigFile Defaults {
            get {
                return defaults;
            }
        }

        public int Count {
            get {
                return this.files.Count;
            }
        }

        public bool IsMonoCompatibilityMode {
            get {
                return this.monoCompatibilityMode;
            }
        }

        /// <summary>
        /// Adds default values to default config file
        /// </summary>
        private void SetupDefaults() {
            defaults = new ConfigFile("");
            defaults.addVariable(Config.Comment, "//");
            defaults.addVariable(Config.Delimiter, "^");
            defaults.addVariable(Config.Extensions, new List<string>(new string[] { "avi", "mpg", "mpeg", "mp4", "divx", "mkv", "wmv" }));
            defaults.addVariable(Config.SubtitleExtensions, new List<string>(new string[] { "srt", "sub" }));
            defaults.addVariable(Config.LogLevelInfo, "LogFile");
            defaults.addVariable(Config.LogLevelWarning, "LogFile");
            defaults.addVariable(Config.LogLevelError, "Log_and_Message");
            defaults.addVariable(Config.LogLevelStatus, "LogFile");
            defaults.addVariable(Config.LogLevelDebug, "None");
            defaults.addVariable(Config.LogName, "Renamer.log");
            defaults.addVariable(Config.MaxDepth, "0");
            defaults.addVariable(Config.InvalidCharReplace, "-");
            defaults.addVariable(Config.EpIdentifier, new List<string>(new string[] { "S%SE%E", "%Sx%E", "S%S.E%E", "- %S%E -", "-E%E-", ".%S%E.", "%S.%E", "%S%E" }));
            defaults.addVariable(Config.TargetPattern, "S%sE%E - %N");
            defaults.addVariable(Config.LastTitles, new List<string>(new string[] { "The Simpsons", "Heroes", "Scrubs", "Supernatural", "Eureka", "Chuck", "Stargate", "Stargate Atlantis", "Dexter", "Doctor Who", "24", "Firefly", "Kyle XY", "Prison Break", "Moonlight", "Monk", "House M.D.", "Gilmore Girls", "Friends", "Sex and the City" }));
            defaults.addVariable(Config.LastProvider, "");
            defaults.addVariable(Config.LastDirectory, "");
            defaults.addVariable(Config.LastSubProvider, "");
            defaults.addVariable(Config.Timeout, "10000");
            defaults.addVariable(Config.InvalidFilenameAction, "Ask");
            defaults.addVariable(Config.Umlaute, "Dont_Use");
            defaults.addVariable(Config.Case, "Large");
            defaults.addVariable(Config.Extract, new List<string>(new string[] { "Season %S", "Season_%S", "Season%S", "Staffel %S", "Staffel_%S", "Staffel%S" }));
            defaults.addVariable(Config.CreateDirectoryStructure, "1");
            defaults.addVariable(Config.DeleteEmptyFolders, "1");
            defaults.addVariable(Config.DeleteAllEmptyFolders, "1");
            defaults.addVariable(Config.IgnoreFiles, new List<string>(new string[] { "nfo", "diz" }));
            defaults.addVariable(Config.ColumnOrder, new List<string>(new string[] { "0", "1", "2", "3", "4", "6", "5" }));
            defaults.addVariable(Config.ColumnWidths, new List<string>(new string[] { "209", "137", "55", "57", "144", "188", "188" }));
            defaults.addVariable(Config.WindowSize, new List<string>(new string[] { "1024", "600" }));
            defaults.addVariable(Config.UseSeasonSubDir, "1");
            defaults.addVariable(Config.TitleHistorySize, "100");
            defaults.addVariable(Config.ShownameExtractionRegex, "((?<pos>\\.S\\d+E\\d+\\.)|(?<pos>\\.\\d+\\.)|(?<pos>[^\\. _-]\\d{3,}))");
            defaults.addVariable(Config.CleanupRegex, "[.-_;,!='+]");
            defaults.addVariable(Config.Replace, new List<string>(new string[] { "//Comments are indicated by //", "//Format used here is \"From->To\",", "//where \"From\" is a c# regular expression, see", "//http://www.radsoftware.com.au/articles/regexlearnsyntax.aspx for details", "//example: \"\\s->.\" replaces whitespaces with dots", "cd->CD", " i\\.-> I.", " ii\\.-> II.", " iii\\.-> III.", " iv\\.-> IV.", " v\\.-> V.", " vi\\.-> VI.", " vii\\.-> VII.", " i -> I ", " ii -> II ", " iii -> III ", " iv -> IV ", " v -> V ", " vi -> VI ", " vii -> VII ", "\\[\\d+\\]->", " +-> " }));
            defaults.addVariable(Config.Tags, new List<string>(new string[] { "Xvid", "DivX", "R5", "R3", "GERMAN", "DVD", "INTERNAL", "PROPER", "TELECINE", "LINE", "LD", "MD", "AC3", "SVCD", "XSVCD", "VCD", "Dubbed", "HD", "720P", "720", "SCREENER", "RSVCD", "\\d\\d\\d\\d", "TS", "GER" }));
            defaults.addVariable(Config.ResizeColumns, "1");
            defaults.addVariable(Config.ShownameBlacklist, "Season^Staffel");
        }

        public bool fileLoaded(string filePath) {
            return this.files.ContainsKey(filePath);
        }

        public ConfigFile addConfigFile(string filePath) {
            this[filePath] = new ConfigFile(filePath);
            return this[filePath];
        }

        public void SetMonoCompatibilityMode(bool p) {
            this.monoCompatibilityMode = true;
        }

        #region IEnumerable Member

        public IEnumerator GetEnumerator() {
            return this.files.GetEnumerator();
        }

        #endregion
    }
}