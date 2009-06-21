using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Classes.Configuration.Keywords;
using System.Text.RegularExpressions;
using System.IO;

namespace Renamer.Classes
{
    class SeriesNameExtractor
    {
        private string[] folders;
        private string[] extractPatterns;
        private string[] shownamePatterns;

        private bool seriesNameFromDirectory;
        private string shownameBlacklist;
        private string name;


        private static SeriesNameExtractor instance = null;
        private static object m_lock = new object();

        public static SeriesNameExtractor Instance {
            get {
                if (instance == null) {
                    lock (m_lock) { if (instance == null) instance = new SeriesNameExtractor(); }
                }
                return instance;
            }
        }

        private SeriesNameExtractor() {

            string[] blacklist = Helper.ReadProperties(Config.ShownameBlacklist);
            shownameBlacklist = String.Join("|", blacklist);
            extractPatterns = (string[])Helper.ReadProperties(Config.Extract);
            for (int index = 0; index < extractPatterns.Length; index++) {
                extractPatterns[index] = transformPlaceholderToRegex(extractPatterns[index]);
            }

            shownamePatterns = Helper.ReadProperties(Config.ShownameExtractionRegex);
        }

        private void reset() {
            this.seriesNameFromDirectory = false;
            this.name = null;

        }

        private string removeReleaseGroupTag(string filename) {
            //remove releasegroup tag, 
            // normally 3 to 6 characters at the beginning of the filename seperated by a '-'

            //Logger.Instance.LogMessage("SNE: removeReleasGroupTag: " + filename + "=>" + Regex.Replace(filename, "^\\w{3,6}-", ""), LogLevel.DEBUG);
            return Regex.Replace(filename, "^\\w{3,6}-", "");
        }

        private string[] extractFoldernamesFromPath(string path) {
            string[] folders = path.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < folders.Length; i++) {
                folders[i] = Regex.Replace(folders[i], "\\((?<letter>\\w)\\)", "${letter}");//.Replace("(", "").Replace(")", "");
            }
            return folders;
        }

        private string transformPlaceholderToRegex(string placeholderString) {
            placeholderString = placeholderString.Replace("%S", "\\d+");
            placeholderString = placeholderString.Replace("%T", "(?<pos>.*?)");
            //Don't matter what comes next
            placeholderString += ".*";
            return placeholderString;
        }

        private void extractNameFromSeasonsFolder() {
            if (!String.IsNullOrEmpty(name) || folders.Length == 0) {
                return;
            }
            string logname = name;
            Match m;
            foreach (string s in folders)
            {
                if (s.ToLower().Contains("sample"))
                {
                    name = "Sample";
                    return;
                }
            }
            foreach (string pattern in extractPatterns) {
                m = Regex.Match(folders[folders.Length - 1], pattern, RegexOptions.IgnoreCase);
                if (m.Success && pattern.Contains("(?<pos>.*?)") && m.Groups["pos"].Value != "") {
                    //yes we use filename so we can check against a top level dir
                    name = m.Groups["pos"].Value;
                }
                else if (m.Success) {
                    string matchedname = folders[folders.Length - 2];
                    if (Regex.IsMatch(matchedname, shownameBlacklist, RegexOptions.IgnoreCase)) {
                        matchedname = null;
                    }
                    else {
                        seriesNameFromDirectory = true;
                    }
                    name = matchedname;
                }
            }
            //Logger.Instance.LogMessage("SNE: extractNameFromSeasonsFolder: " + logname + "=>" + name, LogLevel.DEBUG);
        }

        private void extractNameFromString(string str) {
            if (!String.IsNullOrEmpty(name)) {
                return;
            }
            if(str.ToLower().Contains("sample")){
                name="Sample";
                return;
            }
            if (seriesNameFromDirectory == false) {
                Match m;
                foreach (string pattern in shownamePatterns) {
                    //from filename
                    m = Regex.Match(str, pattern, RegexOptions.IgnoreCase);
                    if (m.Success) // && filename.Length != matchedname.Length + m.Groups["pos"].Value.Length)
                    {
                        string matchedname = str.Substring(0, m.Groups["pos"].Index);
                        if (m.Groups["pos"].Index == 0) {
                            int startOfName = m.Groups["pos"].Index + m.Groups["pos"].Length;
                            string seperator = str.Substring(startOfName - 1, 1);
                            matchedname = str.Substring(startOfName, str.IndexOf(seperator, startOfName) - startOfName);
                        }
                        if (Regex.Match(matchedname, shownameBlacklist, RegexOptions.IgnoreCase).Success) {
                            matchedname = null;
                            seriesNameFromDirectory = false;
                        }
                        if (name == null) {
                            name = matchedname;
                        }
                        break;
                    }
                }
            }
            //Logger.Instance.LogMessage("SNE: extractNameFromString: " + str + "=>" + name, LogLevel.DEBUG);
        }

        private void fallbackFolderNames() {
            if (!String.IsNullOrEmpty(name) || folders.Length == 0) {
                return;
            }
            name = folders[folders.Length - 1];
            for (int i = 2; i < folders.Length - i; i++) {
                if (Regex.Match(name, shownameBlacklist, RegexOptions.IgnoreCase).Success) {
                    name = folders[folders.Length - i];
                    break;
                }
            }
            //Logger.Instance.LogMessage("SNE: fallbackFolderNames: " + name, LogLevel.DEBUG);

        }

        private void postprocessing() {
            if (String.IsNullOrEmpty(name)) {
                return;
            }
            name = Regex.Replace(name, "\\[.+\\]|\\(.+\\)", "");
            name = name.Trim(new char[] { '-', '_', '.', ' ', '(', ')', '[', ']' });
            name = Regex.Replace(name, "[\\._]", " ");
            name = Regex.Replace(name, "\\[.*\\]", "");
            name = name.Replace("  ", " ");
            if (!this.seriesNameFromDirectory) {
                name = Helper.UpperEveryFirst(name);
            }
            name = name.Trim();
        }


        internal string ExtractSeriesName(Filepath filepath) {
            return this.ExtractSeriesName(filepath.Filename, filepath.Path);
        }
        public string ExtractSeriesName(string file, string path) {
            reset();
            // Read plain filename
            string filename = System.IO.Path.GetFileNameWithoutExtension(file);


            filename = removeReleaseGroupTag(filename);
            folders = extractFoldernamesFromPath(path);

            extractNameFromSeasonsFolder();
            extractNameFromString(filename);
            if (folders.Length != 0) {
                extractNameFromString(folders[folders.Length - 1]);
            }
            fallbackFolderNames();
            postprocessing();
            return name;
        }



        /// <summary>
        /// Extracts season from directory name
        /// </summary>
        /// <param name="path">path from which to extract the data (NO FILEPATH, JUST FOLDER)</param>
        /// <returns>recognized season, -1 if not recognized</returns>
        public int ExtractSeasonFromDirectory(string path) {
            string[] patterns = Helper.ReadProperties(Config.Extract);
            string[] folders = path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = patterns.Length - 1; i >= 0; i--) {
                string pattern = RegexConverter.toRegex(patterns[i]);
                Match m = Regex.Match(folders[folders.Length - 1], pattern, RegexOptions.IgnoreCase);

                if (m.Success) {
                    try {
                        return Int32.Parse(m.Groups["Season"].Value);
                    }
                    catch (Exception) {
                        return -1;
                    }
                }
            }
            return -1;
        }
    }

}
