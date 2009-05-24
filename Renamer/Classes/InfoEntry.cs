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
using Renamer.Classes.Configuration.Keywords;
using System.Text.RegularExpressions;
using System.IO;
using System.Timers;
using Renamer.Logging;

namespace Renamer.Classes
{
    /// <summary>
    /// Contains all information about a single video or subtitle file, including scheduled renaming info
    /// </summary>
    public class InfoEntry
    {
        /// <summary>
        /// what to do with Umlauts
        /// </summary>
        public enum UmlautAction : int { Unset, Ignore, Use, Dont_Use };

        /// <summary>
        /// what to do with casing of filenames
        /// </summary>
        public enum Case : int { Unset, Ignore, small, Large, CAPSLOCK };

        /// <summary>
        /// if file should be moved
        /// </summary>
        public enum DirectoryStructure : int { Unset, CreateDirectoryStructure, NoDirectoryStructure };

        private string _Filename = "";
        /// <summary>
        /// Old filename with extension
        /// </summary>
        public string Filename {
            get { return _Filename; }
            set {
                if (_Filename != value) {
                    _Filename = value;
                    if (_Path != "" && _Filename != "" && _Extension != "") {
                        ExtractName();
                    }
                }
            }
        }
        private string _Extension = "";
        /// <summary>
        /// Extension of the file without dot, i.e. "avi" or "srt"
        /// </summary>
        public string Extension {
            get { return _Extension; }
            set {
                if (_Extension != value) {
                    _Extension = value;
                    if (_Path != "" && _Filename != "" && _Extension != "") {
                        ExtractName();
                    }
                }
            }
        }
        private string _Path = "";
        /// <summary>
        /// Path of the file
        /// </summary>
        public string Path {
            get { return _Path; }
            set {
                if (_Path != value) {
                    _Path = value;
                    if (_Path != "" && _Filename != "" && _Extension != "") {
                        ExtractName();
                    }
                }
            }
        }

        /// <summary>
        /// number of the season
        /// </summary>
        private string _Season = "";
        public string Season {
            get { return _Season; }
            set {
                if (_Season != value) {
                    _Season = value;
                    CreateNewName();
                    SetPath();
                    SetupRelation();
                }
            }
        }

        private string _Episode = "";
        /// <summary>
        /// number of the episode
        /// </summary>
        public string Episode {
            get { return _Episode; }
            set {
                if (_Episode != value) {
                    _Episode = value;
                    SetupRelation();
                }
            }
        }
        public string _Name = "";
        /// <summary>
        /// name of the episode
        /// </summary>
        public string Name {
            get { return _Name; }
            set {
                if (_Name != value) {
                    _Name = value;
                    CreateNewName();
                    if (Movie == true) {
                        SetPath();
                    }
                }
            }
        }
        /// <summary>
        /// new filename with extension
        /// </summary>
        public string NewFileName = "";

        /// <summary>
        /// destination directory
        /// </summary>
        public string Destination = "";

        /// <summary>
        /// If file is to be processed
        /// </summary>
        public bool Process = true;

        private bool _Movie = false;
        /// <summary>
        /// If file is a movie.
        /// </summary>
        public bool Movie {
            get { return _Movie; }
            set {
                if (_Movie != value) {
                    _Movie = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }
        public string _Showname = "";
        /// <summary>
        /// Name of the show this file belongs to.
        /// </summary>
        public string Showname {
            get { return _Showname; }
            set {
                if (_Showname != value) {
                    _Showname = value;
                    if (_Showname == null) _Showname = "";
                    SetupRelation();
                    if (Movie == false) {
                        SetPath();
                    }
                }
            }
        }

        private UmlautAction _UmlautUsage = UmlautAction.Unset;
        /// <summary>
        /// Option indicates the using of umlaute
        /// </summary>
        public UmlautAction UmlautUsage {
            get { return _UmlautUsage; }
            set {
                if (_UmlautUsage != value) {
                    _UmlautUsage = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }
        private Case _Casing = Case.Unset;
        /// <summary>
        /// Option indicates the use of UPPER and lowercase
        /// </summary>
        public Case Casing {
            get { return _Casing; }
            set {
                if (_Casing != value) {
                    _Casing = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }

        private DirectoryStructure _CreateDirectoryStructure = DirectoryStructure.Unset;
        public DirectoryStructure CreateDirectoryStructure {
            get { return _CreateDirectoryStructure; }
            set {
                if (_CreateDirectoryStructure != value) {
                    _CreateDirectoryStructure = value;
                    SetPath();
                }
            }
        }

        private Helper.Languages _Language = Helper.Languages.None;
        public Helper.Languages Language {
            get { return _Language; }
            set {
                if (_Language != value) {
                    _Language = value;
                    if (_Path != "") SetPath();
                    if (NewFileName != "") CreateNewName();
                }
            }
        }

        private string[] toBeReplaced = { "ä", "Ä", "ö", "Ö", "ü", "Ü", "ß", "É", "È", "Ê", "Ë", "Á", "À", "Â", "Ã", "Å", "Í", "Ì", "Î", "Ï", "Ú", "Ù", "Û", "Ó", "Ò", "Ô", "Ý", "Ç", "é", "è", "ê", "ë", "á", "à", "â", "ã", "å", "í", "ì", "î", "ï", "ú", "ù", "û", "ó", "ò", "ô", "ý", "ÿ", "ç" };
        private string[] toBeReplacedWith = { "ae", "Ae", "oe", "Oe", "ue", "Ue", "ss", "E", "E", "E", "E", "A", "A", "A", "A", "A", "I", "I", "I", "I", "U", "U", "U", "O", "O", "O", "Y", "C", "e", "e", "e", "e", "a", "a", "a", "a", "a", "i", "i", "i", "i", "u", "u", "u", "o", "o", "o", "y", "y", "c" };

        /// <summary>
        /// Extracts the name from a file and sets it as title
        /// </summary>
        /// <returns>extracted name or null</returns>
        private void ExtractName() {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            Showname = SeriesNameExtractor.Instance.ExtractSeriesName(_Filename, _Path);
            stopWatch.Stop();
            Info.timeextractname += stopWatch.Elapsed.TotalSeconds;
        }

        public void SetPath() {
            string basepath = Helper.ReadProperty(Config.LastDirectory);
            DateTime dt = DateTime.Now;
            if (_Showname == null || _Showname == "") {
                Destination = "";
            }
            if (!_Movie) {
                //for placing files in directory structure, figure out if selected directory is show name, otherwise create one
                string[] dirs = Path.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                bool InShowDir = false;
                bool InSeasonDir = false;
                string showname = _Showname;
                bool UseSeasonSubDirs = Helper.ReadProperty(Config.UseSeasonSubDir) == "1";
                //fix invalid paths
                if (showname.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) {
                    string pattern = "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
                    string replace = Helper.ReadProperty(Config.InvalidCharReplace);
                    showname = Regex.Replace(showname, pattern, replace);
                }
                //figure out if we are in a season dir
                string[] seasondirs = Helper.ReadProperties(Config.Extract);
                int season = -1;
                try {
                    Int32.TryParse(_Season, out season);
                }
                catch (Exception) { };

                string seasondir = "";
                //loop backwards so first entry is used if nothing is recognized and folder has to be created
                for (int i = seasondirs.Length - 1; i >= 0; i--) {
                    seasondir = seasondirs[i].Replace("%S", _Season);
                    seasondir = seasondir.Replace("%T", showname);

                    if (dirs.Length > 0 && dirs[dirs.Length - 1] == showname) {
                        InShowDir = true;
                        seasondir = seasondirs[0].Replace("%S", _Season);
                        seasondir = seasondir.Replace("%T", showname);
                        break;
                    }
                    else if (dirs.Length > 1 && Regex.Match(dirs[dirs.Length - 1], seasondir).Success) {
                        InSeasonDir = true;
                        break;
                    }
                }
                //if we want to move the files to a directory structure, set destination property to it here, otherwise use same dir
                DirectoryStructure ds = _CreateDirectoryStructure;
                if (ds == DirectoryStructure.Unset) {
                    bool cds = Helper.ReadProperty(Config.CreateDirectoryStructure) == "1";
                    if (cds) {
                        ds = DirectoryStructure.CreateDirectoryStructure;
                    }
                    else {
                        ds = DirectoryStructure.NoDirectoryStructure;
                    }
                }
                if (ds == DirectoryStructure.CreateDirectoryStructure) {
                    //if there is a valid season, add season dir
                    if (season >= 0) {
                        if (InShowDir) {
                            if (UseSeasonSubDirs) {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else {
                                Destination = basepath;
                            }
                        }
                        else if (InSeasonDir) {
                            if (UseSeasonSubDirs) {
                                //Go up two dirs and add proper show and season dir
                                Destination = Directory.GetParent(Directory.GetParent(Path).FullName).FullName + System.IO.Path.DirectorySeparatorChar + showname + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else {
                                Destination = Directory.GetParent(Directory.GetParent(Path).FullName).FullName + System.IO.Path.DirectorySeparatorChar + showname;
                            }
                        }
                        else {
                            if (UseSeasonSubDirs) {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + showname + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + showname;
                            }
                        }
                    }
                    //no valid season found, this could be a movie or so and probably shouldn't be moved
                    else {
                        Destination = Path;
                    }
                }
                //We don't want to move anything at all, so lets just leave it where it is
                else {
                    Destination = Path;
                }
                if (Destination == Path) {
                    Destination = "";
                }
            }
            //if this is a movie
            else {
                if (Helper.ReadInt(Config.CreateDirectoryStructure) > 0) {
                    //Multiple file version, own folder
                    if (Regex.Match(_Name, "CD\\d", RegexOptions.IgnoreCase).Success) {
                        if (Path != "") {
                            Destination = Path + System.IO.Path.DirectorySeparatorChar + _Name.Substring(0, _Name.Length - 3);
                        }
                        else {
                            Destination = _Name.Substring(0, _Name.Length - 3);
                        }
                    }
                    else {
                        Destination = "";
                    }
                }
            }
            Info.timesetpath += (DateTime.Now - dt).TotalSeconds;
        }

        /// <summary>
        /// This function tries to find an episode name which matches the showname, episode and season number by looking at previously downloaded relations
        /// </summary>
        public void SetupRelation() {
            int seasonmatch = -1;
            int epmatch = -1;
            //Clear old name
            Name = "";
            RelationCollection rc = Info.GetRelationCollectionByName(_Showname);
            //if there is no data available, don't do anything
            if (rc == null) return;
            for (int i = 0; i < rc.Relations.Count; i++) {
                string season = rc.Relations[i].Season;
                string episode = rc.Relations[i].Episode;
                string name = rc.Relations[i].Name;
                if (season == _Season && episode == _Episode) {
                    Name = name;
                    seasonmatch = i;
                    epmatch = i;
                    break;
                }
                //if there is none or invalid episode info, try to match by season
                if (season == _Season && !Helper.IsNumeric(_Episode)) seasonmatch = i;
                //if there is none or invalid season info, try to match by episode
                if (episode == _Episode && !Helper.IsNumeric(_Season)) epmatch = i;
            }
            //episode match, but no season match
            if (seasonmatch == -1 && epmatch != -1) {
                Name = rc.Relations[epmatch].Name;

            }
            else if (seasonmatch != -1 && epmatch == -1) {
                Name = rc.Relations[seasonmatch].Name;
            }
        }

        public string AdjustSpelling(string input, bool extension) {
            //treat umlaute and case
            UmlautAction ua = _UmlautUsage;
            if (ua == UmlautAction.Unset) {
                ua = (UmlautAction)Enum.Parse(typeof(UmlautAction), Helper.ReadProperty(Config.Umlaute), true);
                //Fallback default value
                if (ua == UmlautAction.Unset) ua = UmlautAction.Ignore;
            }
            Case c = _Casing;
            if (c == Case.Unset) {
                c = (Case)Enum.Parse(typeof(Case), Helper.ReadProperty(Config.Case), true);
                //Fallback default value
                if (c == Case.Unset) c = Case.Ignore;
            }

            if (ua == UmlautAction.Use && _Language == Helper.Languages.German) {
                input = input.Replace("ae", "ä");
                input = input.Replace("Ae", "Ä");
                input = input.Replace("oe", "ö");
                input = input.Replace("Oe", "Ö");
                input = input.Replace("ue", "ü");
                input = input.Replace("Ue", "Ü");
                input = input.Replace("eü", "eue");
            }
            else if (ua == UmlautAction.Dont_Use) {
                for (int index = 0; index < toBeReplaced.Length; index++) {
                    input.Replace(toBeReplaced[index], toBeReplacedWith[index]);
                }
            }
            if (c == Case.small) {
                input = input.ToLower();
            }
            else if (c == Case.Large) {
                if (!extension) {
                    Regex r = new Regex(@"\b(\w)(\w+)?\b", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    input = Helper.UpperEveryFirst(input);
                }
                else {
                    input = input.ToLower();
                }
            }
            else if (c == Case.CAPSLOCK) {
                input = input.ToUpper();
            }
            return input;
        }
        /// <summary>
        /// This function generates a new filename from the Target Pattern, episode, season, title, showname,... values
        /// </summary>
        public void CreateNewName() {
            DateTime dt = DateTime.Now;
            if (_Name == "") {
                NewFileName = "";
            }
            else if (_Name != "") {
                //Target Filename format
                string name;

                //Those 3 strings need case/Umlaut processing
                string epname = _Name;
                string seriesname = _Showname;
                string extension = Extension;

                if (!_Movie) {
                    name = Helper.ReadProperty(Config.TargetPattern);
                    name = name.Replace("%e", _Episode);
                    string episode = _Episode;
                    if (episode.Length == 1) episode = "0" + episode;
                    name = name.Replace("%E", episode);
                    name = name.Replace("%s", _Season);
                    string season = _Season;
                    if (season.Length == 1) season = "0" + season;
                    name = name.Replace("%S", season);
                }
                else {
                    name = "%N";
                }


                AdjustSpelling(epname, false);
                AdjustSpelling(seriesname, false);
                AdjustSpelling(extension, true);



                //Now that series title, episode title and extension are properly processed, add them to the filename

                //Remove extension from target filename (if existant) and add properly cased one
                name = Regex.Replace(name, "\\." + extension, "", RegexOptions.IgnoreCase);
                name += "." + extension;

                name = name.Replace("%T", seriesname);
                name = name.Replace("%N", epname);

                //string replace function
                List<string> replace = new List<string>(Helper.ReadProperties(Config.Replace));
                List<string> from = new List<string>();
                List<string> to = new List<string>();
                foreach (string s in replace) {
                    if (!s.StartsWith(Settings.getInstance().Comment)) {
                        int pos = s.IndexOf("->");
                        if (pos > 0 && pos < s.Length - 2) {
                            string f = s.Substring(0, pos);
                            string t = s.Substring(pos + 2);
                            name = Regex.Replace(name, f, t, RegexOptions.IgnoreCase);
                        }
                    }
                }

                //Invalid character replace
                if (Helper.ReadProperty(Config.InvalidCharReplace) != null && (Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction), Helper.ReadProperty(Config.InvalidFilenameAction)) == Helper.InvalidFilenameAction.Replace) {
                    string pattern = "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
                    name = Regex.Replace(name, pattern, Helper.ReadProperty(Config.InvalidCharReplace));
                }

                //set new filename if renaming process is required
                if (Filename == name) {
                    NewFileName = "";
                }
                else {
                    NewFileName = name;
                }
            }
            Info.timecreatenewname += (DateTime.Now - dt).TotalSeconds;
        }
    }
}
