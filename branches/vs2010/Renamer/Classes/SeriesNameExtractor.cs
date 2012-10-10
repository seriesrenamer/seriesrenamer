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
        private string[] MovieTagPatterns;
        private bool seriesNameFromDirectory;
        private string pathBlacklist;
        private string filenameBlacklist;
        private string name;
        private bool filenameBlacklisted;
        private InfoEntry ie;
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

        private SeriesNameExtractor()
        {
            string[] tags = Helper.ReadProperties(Config.Tags);  
            List<string> MovieRegexes = new List<string>();
            foreach (string s in tags)
            {
                MovieRegexes.Add("[^A-Za-z0-9]+" + s);
            }
            MovieTagPatterns = MovieRegexes.ToArray();
            string[] blacklist = Helper.ReadProperties(Config.PathBlacklist);
            pathBlacklist = String.Join("|", blacklist);
            blacklist = Helper.ReadProperties(Config.FilenameBlacklist);
            filenameBlacklist = String.Join("|", blacklist);
            extractPatterns = (string[])Helper.ReadProperties(Config.Extract);
            for (int index = 0; index < extractPatterns.Length; index++) {
                extractPatterns[index] = transformPlaceholderToRegex(extractPatterns[index]);
            }
            filenameBlacklisted = false;
            shownamePatterns = Helper.ReadProperties(Config.ShownameExtractionRegex);
        }

        private void reset() {
            seriesNameFromDirectory = false;
            name = null;
            ie = null;
            filenameBlacklisted = false;
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
                    if (Regex.IsMatch(matchedname, pathBlacklist, RegexOptions.IgnoreCase)) {
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
                        //try to use the part of the name that goes from 0 to pos as showname
                        string matchedname = str.Substring(0, m.Groups["pos"].Index);
                        //if pos is 0, this means that there is no name :(
                        if (m.Groups["pos"].Index != 0)
                        {
                            if (Regex.Match(matchedname, pathBlacklist, RegexOptions.IgnoreCase).Success)
                            {
                                matchedname = null;
                                seriesNameFromDirectory = false;
                            }
                            if (name == null)
                            {
                                name = matchedname;
                            }
                            break;
                        }
                    }
                }
            }
            //Logger.Instance.LogMessage("SNE: extractNameFromString: " + str + "=>" + name, LogLevel.DEBUG);
        }

        private void fallbackFolderNames() {
            if (folders.Length == 0) {
                return;
            }
            for (int i = 1; i < folders.Length - 1; i++) {
                if (!Regex.Match(folders[folders.Length - i], pathBlacklist, RegexOptions.IgnoreCase).Success)
                {
                    if (name == null || (Helper.InitialsMatch(folders[folders.Length - 1], name) && !Filepath.IsExtractionDirectory(folders[folders.Length-1])))
                    {
                        name = folders[folders.Length - i];
                        break;
                    }
                }
            }
            //Logger.Instance.LogMessage("SNE: fallbackFolderNames: " + name, LogLevel.DEBUG);

        }

        
        public string ExtractSeriesName(InfoEntry ie) {
            reset();
            this.ie = ie;
            // Read plain filename
            string filename = System.IO.Path.GetFileNameWithoutExtension(ie.Filename);            
            filename = NameCleanup.RemoveReleaseGroupTag(filename);
            folders = Filepath.extractFoldernamesFromPath(ie.FilePath.Path);
            if (ie.InSeasonFolder && folders.Length > 2)
            {
                if (!Regex.IsMatch(folders[folders.Length - 2], pathBlacklist, RegexOptions.IgnoreCase))
                {
                    return folders[folders.Length - 2];
                }
            }
            extractNameFromSeasonsFolder();
            extractNameFromString(filename);
            if (folders.Length != 0) {
                extractNameFromString(folders[folders.Length - 1]);
            }
            fallbackFolderNames();
            name=NameCleanup.Postprocessing(name);
            if (name == null) return "";
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
