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
            this.seriesNameFromDirectory = false;
            this.name = null;
            filenameBlacklisted = false;
        }

        private string removeReleaseGroupTag(string filename) {
            //remove releasegroup tag, 
            // normally 3 to 6 characters at the beginning of the filename seperated by a '-'
            //if filename too short, it might be a part of the real name, so skip it
            if (filename.Length > 5)
            {
                return Regex.Replace(Regex.Replace(filename, "^(([^\\p{Lu}]\\p{Ll}{1,3})|(\\p{Lu}{2,2}\\w))-", ""), "-\\w{2,4}$", "");
            }
            else return filename;
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
                        string matchedname = str.Substring(0, m.Groups["pos"].Index);
                        if (m.Groups["pos"].Index == 0) {
                            int startOfName = m.Groups["pos"].Index + m.Groups["pos"].Length;
                            string seperator = str.Substring(startOfName - 1, 1);
                            matchedname = str.Substring(startOfName, str.IndexOf(seperator, startOfName) - startOfName);
                        }
                        if (Regex.Match(matchedname, pathBlacklist, RegexOptions.IgnoreCase).Success) {
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
            for (int i = 1; i < folders.Length - 1; i++) {
                if (!Regex.Match(folders[folders.Length - i], pathBlacklist, RegexOptions.IgnoreCase).Success)
                {
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
            name = Regex.Replace(name, "(?<pos1>[\\p{Ll}\\d])(?<pos2>[\\p{Lu}\\d][\\p{Ll}\\d])|(?<pos1>[^\\d])(?<pos2>\\d)", new MatchEvaluator(SeriesNameExtractor.InsertSpaces));
            name = Regex.Replace(name, "\\[.+\\]|\\(.+\\)", "");
            name = name.Trim(new char[] { '-', '_', '.', ' ', '(', ')', '[', ']' });
            name = Regex.Replace(name, "[\\._]", " ");
            name = Regex.Replace(name, "\\[.*\\]", "");
            name = name.Replace("  ", " ");
            if (!ContainsLowercaseCharacters(name)||!ContainsUppercaseCharacters(name)) {
                name = Helper.UpperEveryFirst(name);
            }
            name = name.Trim();
        }

        private bool ContainsLowercaseCharacters(string str)
        {
            return Regex.IsMatch(str, "\\p{Ll}");
        }
        private bool ContainsUppercaseCharacters(string str)
        {
            return Regex.IsMatch(str, "\\p{Lu}");
        }
        static string InsertSpaces(Match m){
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
        public string ExtractSeriesName(InfoEntry ie) {
            reset();
            this.ie = ie;
            // Read plain filename
            string filename = System.IO.Path.GetFileNameWithoutExtension(ie.Filename);


            filename = removeReleaseGroupTag(filename);
            folders = extractFoldernamesFromPath(ie.FilePath.Path);

            extractNameFromSeasonsFolder();
            extractNameFromString(filename);
            if (folders.Length != 0) {
                extractNameFromString(folders[folders.Length - 1]);
            }
            fallbackFolderNames();
            postprocessing();
            if (name == null) return "";
            return name;
        }


        public int ProcessMultifiles()
        {
            //figure out if this is a multi file video
            string pattern = "(?<pos>(CD|Cd|cd))\\s?(?<number>(\\d|I|II|II|IV|V))|((?<pos>\\d\\s?of\\s?)(?<number>\\d)|(?<pos> )(?<number>(a|b|c|d|e)))$";
            Match m;
            if (filenameBlacklisted)
            {
                m = Regex.Match(ie.FilePath.Path, pattern);
            }
            else
            {
                m = Regex.Match(name, pattern);
            }
            int part=-1;
            if (m.Success)
            {
                string number = m.Groups["number"].Value;
                if (!int.TryParse(number, out part))
                {
                    if (number == "a" || number == "I")
                    {
                        part = 1;
                    }
                    else if (number == "b" || number == "II")
                    {
                        part = 2;
                    }
                    else if (number == "c" || number == "III")
                    {
                        part = 3;
                    }
                    else if (number == "d" || number == "IV")
                    {
                        part = 4;
                    }
                    else if (number == "e" || number == "V")
                    {
                        part = 5;
                    }
                }
                if (filenameBlacklisted)
                {
                    name = Path.GetFileName(Filepath.goUpwards(ie.FilePath.Path,1));
                    ie.ExtractedNameLevel = 2;
                }
                else
                {
                    name = name.Substring(0, m.Groups["pos"].Index);
                    if (name == "")
                    {
                        name = Path.GetFileName(ie.FilePath.Path);
                        ie.ExtractedNameLevel = 1;
                    }
                }
            }
            return part;
        }
        public string ExtractMovieName(InfoEntry ie)
        {
            reset();
            this.ie = ie;
            name = ie.FilePath.Name;
            if (Regex.IsMatch(name, filenameBlacklist, RegexOptions.IgnoreCase))
            {
                filenameBlacklisted = true;
                name = ie.FilePath.Path;
                //must be atleast 1 then
                ie.ExtractedNameLevel = 1;
            }
            int part = ProcessMultifiles();
            name = removeReleaseGroupTag(name);
            //if part couldn't be extracted yet, maybe it was because of a tag at the end before the part identifier
            if (part == -1)
            {
                part = ProcessMultifiles();
            }
            if (part != -1)
            {
                ie.IsMultiFileMovie = true;
            }
            //try to match tags    
            foreach (string s in MovieTagPatterns)
            {
                Match m = Regex.Match(name, s, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    name = name.Substring(0, m.Index);
                }
            }
           
            postprocessing();

            if (part != -1)
            {
                name += " CD" + part;
            }
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
