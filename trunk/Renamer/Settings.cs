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
using Renamer.Logging;

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
        public static Settings Instance {
            get {
                if (instance == null) {
                    instance = new Settings();
                }
                return instance;
            }
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

        private string currentlyReadingFile;

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

            this.currentlyReadingFile = "";
            files = new Hashtable();
            SetupDefaults();
        }
        public ConfigFile this[string name] {
            get {
                if (!this.fileLoaded(name)) {
                    // Load a file because it's not available yet
                    if (this.IsMonoCompatibilityMode) {
                        Logger.Instance.LogMessage(name + " not loaded yet", LogLevel.LOG);
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
            defaults[Config.Comment] = "//";
            defaults[Config.Delimiter] = "^";
            defaults[Config.Extensions] = new List<string>(new string[] { "avi", "mpg", "mpeg", "mp4", "divx", "mkv", "wmv" });
            defaults[Config.SubtitleExtensions] = new List<string>(new string[] { "srt", "sub" });
            defaults[Config.LogFileLevel] = "DEBUG";
            defaults[Config.LogTextBoxLevel] = "LOG";
            defaults[Config.LogMessageBoxLevel] = "CRITICAL";
            defaults[Config.LogName] = "Renamer.log";
            defaults[Config.MaxDepth] = "0";
            defaults[Config.InvalidCharReplace] = "-";
            defaults[Config.EpIdentifier] = new List<string>(new string[] { "S%SE%E", "%Sx%E", "S%S.E%E", "- %S%E -", "-E%E-", ".%S%E.", "%S.%E", "%S%E" });
            defaults[Config.TargetPattern] = "S%sE%E - %N";
            defaults[Config.LastTitles] = new List<string>(new string[] { "The Simpsons", "Heroes", "Scrubs", "Supernatural", "Eureka", "Chuck", "Stargate", "Stargate Atlantis", "Dexter", "Doctor Who", "24", "Firefly", "Kyle XY", "Prison Break", "Moonlight", "Monk", "House M.D.", "Gilmore Girls", "Friends", "Sex and the City" });
            defaults[Config.LastProvider] = "";
            defaults[Config.LastDirectory] = "";
            defaults[Config.LastSubProvider] = "";
            defaults[Config.Timeout] = "10000";
            defaults[Config.InvalidFilenameAction] = "Ask";
            defaults[Config.Umlaute] = Renamer.Classes.InfoEntry.UmlautAction.Dont_Use.ToString();
            defaults[Config.Case] = Renamer.Classes.InfoEntry.Case.UpperFirst.ToString();
            defaults[Config.Extract] = new List<string>(new string[] { "Season %S", "Season_%S", "Season%S", "Staffel %S", "Staffel_%S", "Staffel%S" });
            defaults[Config.CreateDirectoryStructure] = "1";
            defaults[Config.DeleteEmptyFolders] = "1";
            defaults[Config.DeleteAllEmptyFolders] = "1";
            defaults[Config.IgnoreFiles] = new List<string>(new string[] { "nfo", "diz" });
            defaults[Config.ColumnOrder] = new List<string>(new string[] { "0", "1", "2", "3", "4", "6", "5" });
            defaults[Config.ColumnWidths] = new List<string>(new string[] { "209", "137", "55", "57", "144", "188", "188" });
            defaults[Config.WindowSize] = new List<string>(new string[] { "1024", "600" });
            defaults[Config.UseSeasonSubDir] = "1";
            defaults[Config.TitleHistorySize] = "100";
            defaults[Config.ShownameExtractionRegex] = "((?<pos>\\.S\\d+E\\d+\\.)|(?<pos>\\.\\d+\\.)|(?<pos>[^\\. _-]\\d{3,}))";
            defaults[Config.CleanupRegex] = "[.-_;,!='+]";
            defaults[Config.Replace] = new List<string>(new string[] { "//Comments are indicated by //", "//Format used here is \"From->To\",", "//where \"From\" is a c# regular expression, see", "//http://www.radsoftware.com.au/articles/regexlearnsyntax.aspx for details", "//example: \"\\s->.\" replaces whitespaces with dots", "cd->CD", " i\\.-> I.", " ii\\.-> II.", " iii\\.-> III.", " iv\\.-> IV.", " v\\.-> V.", " vi\\.-> VI.", " vii\\.-> VII.", " i -> I ", " ii -> II ", " iii -> III ", " iv -> IV ", " v -> V ", " vi -> VI ", " vii -> VII ", "\\[\\d+\\]->", " +-> " });
            defaults[Config.Tags] = new List<string>(new string[] { "Xvid", "DivX", "R5", "R3", "GERMAN", "DVD", "INTERNAL", "PROPER", "TELECINE", "LINE", "LD", "MD", "AC3", "SVCD", "XSVCD", "VCD", "Dubbed", "HD", "720P", "720", "SCREENER", "RSVCD", "\\d\\d\\d\\d", "TS", "GER" });
            defaults[Config.ResizeColumns] = "1";
            defaults[Config.ShownameBlacklist] = "Season^Staffel";
            defaults[Config.FindMissingEpisodes] = "1";
            defaults[Config.DeleteSampleFiles] = "1";
            defaults[Config.DestinationDirectory] = "";
        }

        public bool fileLoaded(string filePath) {
            return this.files.ContainsKey(filePath);
        }

        public ConfigFile addConfigFile(string filePath) {
            if (filePath == currentlyReadingFile) {
                return null;
            }
            currentlyReadingFile = filePath;
            this[filePath] = new ConfigFile(filePath);
            currentlyReadingFile = "";
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