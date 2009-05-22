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

        /// <summary>
        /// Old filename with extension
        /// </summary>
        private string _Filename = "";
        public string Filename
        {
            get { return _Filename; }
            set
            {
                if (_Filename != value)
                {
                    _Filename = value;
                    if (_Path != "" && _Filename != "" && _Extension != "")
                    {
                        ExtractName();
                    }
                }
            }
        }
        /// <summary>
        /// Extension of the file without dot, i.e. "avi" or "srt"
        /// </summary>
        private string _Extension = "";
        public string Extension
        {
            get { return _Extension; }
            set
            {
                if (_Extension != value)
                {
                    _Extension = value;
                    if (_Path != "" && _Filename != "" && _Extension != "")
                    {
                        ExtractName();
                    }
                }
            }
        }
        /// <summary>
        /// Path of the file
        /// </summary>
        private string _Path = "";
        public string Path
        {
            get { return _Path; }
            set
            {
                if (_Path != value)
                {
                    _Path = value;
                    if (_Path != "" && _Filename != "" && _Extension != "")
                    {
                        ExtractName();
                    }
                }
            }
        }

        /// <summary>
        /// number of the season
        /// </summary>
        private string _Season = "";
        public string Season
        {
            get { return _Season; }
            set {
                if (_Season != value)
                {
                    _Season = value;
                    CreateNewName();
                    SetPath();
                    SetupRelation();
                }
            }
        }
        
        /// <summary>
        /// number of the episode
        /// </summary>
        private string _Episode = "";
        public string Episode
        {
            get { return _Episode; }
            set
            {
                if (_Episode != value)
                {
                    _Episode = value;
                    SetupRelation();
                }
            }
        }
        /// <summary>
        /// name of the episode
        /// </summary>
        public string _Name = "";
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    CreateNewName();
                    if (Movie == true)
                    {
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

        /// <summary>
        /// If file is a movie.
        /// </summary>
        private bool _Movie = false;
        public bool Movie
        {
            get { return _Movie; }
            set
            {
                if (_Movie != value)
                {
                    _Movie = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }
        /// <summary>
        /// Name of the show this file belongs to.
        /// </summary>
        public string _Showname = "";
        public string Showname
        {
            get { return _Showname; }
            set
            {
                if (_Showname != value)
                {
                    _Showname = value;
                    if (_Showname == null) _Showname = "";
                    SetupRelation();
                    if (Movie == false)
                    {
                        SetPath();
                    }
                }
            }
        }

        private UmlautAction _UmlautUsage = UmlautAction.Unset;
        public UmlautAction UmlautUsage
        {
            get { return _UmlautUsage; }
            set
            {
                if (_UmlautUsage != value)
                {
                    _UmlautUsage = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }

        private Case _Casing = Case.Unset;
        public Case Casing
        {
            get { return _Casing; }
            set
            {
                if (_Casing != value)
                {
                    _Casing = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }

        private DirectoryStructure _CreateDirectoryStructure = DirectoryStructure.Unset;
        public DirectoryStructure CreateDirectoryStructure
        {
            get { return _CreateDirectoryStructure; }
            set
            {
                if (_CreateDirectoryStructure != value)
                {
                    _CreateDirectoryStructure = value;
                    SetPath();
                }
            }
        }

        private Helper.Languages _Language = Helper.Languages.None;
        public Helper.Languages Language
        {
            get { return _Language; }
            set
            {
                if (_Language != value)
                {
                    _Language = value;
                    if (_Path != "") SetPath();
                    if (NewFileName != "") CreateNewName();
                }
            }
        }
        /// <summary>
        /// Extracts the name from a file and sets it as title
        /// </summary>
        /// <param name="path">path to extract name from</param>
        /// <param name="filename">filename to extract name from</param>
        /// <returns>extracted name or null</returns>
        private void ExtractName()
        {
            DateTime dt=DateTime.Now;
            //blabla these comments aren't up to date
            //first, check if the file is located in a directory that matches the showname
            //since we have no way to determine if a directory is a showdir if season dirs aren't used, try to get name from the filename in that case
            //if season dirs are used however, try to extract the showname from the location of the file, otherwise resort to filename again

            //final name which is returned
            string name = null;
            //name recognized from a directory for postprocessing
            string namefromdirectory = null;
            //name recognized from a filename for postprocessing
            string namefromfilename = null;
            string filename = System.IO.Path.GetFileNameWithoutExtension(_Filename);
            //remove releasegroup tag (like "sof-" at the start of the name
            filename = Regex.Replace( filename, "\\w\\w\\w-","");
            string[] folders = _Path.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < folders.Length; i++)
            {
                //Get rid of brackets enclosing the first character
                if (folders[i][0] == '(' && folders[i][2] == ')')
                {
                    folders[i] = folders[i].Replace("(", "").Replace(")", "");
                }
            }
            string[] patterns = Helper.ReadProperties(Config.Extract);
            Match m;
            string[] blacklist = Helper.ReadProperties(Config.ShownameBlacklist);
            //figure out if the file in a season dir, if yes, use the name from the directory above            
            /*for (int i = patterns.Length - 1; i >= 0; i--)
            {
                pattern = patterns[i].Replace("%S", "\\d+");
                m = Regex.Match(folders[folders.Length - 1], pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    name = folders[folders.Length - 2];
                    namefromdirectory = name;
                    break;
                }
            }*/
            //check if file or foldername can be used to extract a name            
            //try to figure out if this file is placed in a season dir(although they aren't meant to be used) and extract the name from the parent directory
            for (int i = patterns.Length - 1; i >= 0; i--)
            {
                //Make regex from placeholder syntax
                string seasondir = patterns[i].Replace("%S", "\\d+");
                seasondir = seasondir.Replace("%T", "(?<pos>.*?)");
                seasondir += ".*";
                //Get rid of brackets enclosing the first character
                if (seasondir[0] == '(' && seasondir[2] == ')')
                {
                    seasondir = seasondir.Replace("(", "").Replace(")", "");
                }
                
                m = Regex.Match(folders[folders.Length - 1], seasondir, RegexOptions.IgnoreCase);
                if (m.Success && seasondir.Contains("(?<pos>.*?)") && m.Groups["pos"].Value != "")
                {                       
                    //yes we use filename so we can check against a top level dir
                    namefromfilename = m.Groups["pos"].Value;
                    name = namefromfilename;
                    break;
                }
                else if (m.Success)
                {
                    string matchedname = folders[folders.Length - 2];
                    foreach (string blacklistentry in blacklist)
                    {
                        if (Regex.Match(matchedname,blacklistentry,RegexOptions.IgnoreCase).Success)
                        {
                            matchedname = null;
                            break;
                        }
                    }
                    namefromdirectory = matchedname;
                    name = namefromdirectory;
                    break;
                }
            }            

            //try to extract the showname from the original filename or an extraction folder
            //<pos> is the position of the season/episode part, we just use the text before it and polish it up a bit
            patterns = Helper.ReadProperties(Config.ShownameExtractionRegex);
            if (namefromdirectory == null)
            {
                foreach (string pattern in patterns)
                {
                    //from filename
                    m = Regex.Match(filename, pattern, RegexOptions.IgnoreCase);
                    string matchedname=filename.Substring(0, m.Groups["pos"].Index);
                    if (m.Success) // && filename.Length != matchedname.Length + m.Groups["pos"].Value.Length)
                    {
                        foreach (string blacklistentry in blacklist)
                        {
                            if (Regex.Match(matchedname, blacklistentry, RegexOptions.IgnoreCase).Success)
                            {
                                matchedname = null;
                                break;
                            }
                        }
                        namefromdirectory = namefromfilename = matchedname;
                        if (name == null)
                        {
                            name = namefromfilename;
                        }
                        break;
                    }
                }
            }
            //from extraction folder    
            foreach (string pattern in patterns)
            {
                m = Regex.Match(folders[folders.Length - 1], pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    string matchedname=folders[folders.Length - 1].Substring(0, m.Groups["pos"].Index);
                    foreach (string blacklistentry in blacklist)
                    {
                        if (Regex.Match(matchedname, blacklistentry, RegexOptions.IgnoreCase).Success)
                        {
                            matchedname = null;
                            break;
                        }
                    }
                    //yes we'll use filename so we can check against a top level dir
                    if (namefromfilename == null)
                    {
                        namefromfilename = matchedname;
                    }
                    if (name == null)
                    {
                        name = namefromfilename;
                    }
                    break;
                }
            }
            //still nothing found, just use a parent directory name that isn't blacklisted or equals the filename(since extraction didn't work) :P
            if (name == null)
            {
                name = folders[folders.Length - 1];
                int i = 2;
                while (folders.Length - i >= 0)
                {
                    foreach (string blacklistentry in blacklist)
                    {
                        if (Regex.Match(name, blacklistentry, RegexOptions.IgnoreCase).Success||name.ToLower()==filename.ToLower())
                        {
                            name = folders[folders.Length - i];
                            break;
                        }
                    }
                    i++;
                }

            }
            //we should definitely have a name by now
            //some postprocessing
            if (name != null)
            {
                //get rid of dots and _
                if (namefromfilename != null)
                {
                    namefromfilename = Regex.Replace(namefromfilename, "[\\._]", " ");
                    namefromfilename = Regex.Replace(namefromfilename, "\\[.+\\]|\\(.+\\)", "");
                    namefromfilename = namefromfilename.TrimEnd(new char[] { '-', '_', '.', ' ', '(',')','[',']' });
                    namefromfilename = Helper.VISSpeak(namefromfilename);
                }
                if (namefromdirectory != null)
                {
                    namefromdirectory = Regex.Replace(namefromdirectory, "[\\._]", " ");
                    namefromdirectory = Regex.Replace(namefromdirectory, "\\[.+\\]|\\(.+\\)", "");
                    namefromdirectory = namefromdirectory.TrimEnd(new char[] { '-', '_', '.', ' ', '(', ')', '[', ']' });
                    namefromdirectory = Helper.VISSpeak(namefromdirectory);
                }
                name = Regex.Replace(name, "\\[.+\\]|\\(.+\\)", "");
                name = name.TrimEnd(new char[] { '-', '_', '.', ' ', '(', ')', '[', ']' });
                //namefromfilename = Helper.VISSpeak(namefromfilename);

                if (namefromfilename != null && namefromdirectory != null)
                {
                    if (namefromdirectory.Length > namefromfilename.Length)
                    {
                        if (namefromdirectory.StartsWith(namefromfilename,StringComparison.CurrentCultureIgnoreCase))
                        {
                            name = namefromfilename;
                        }
                    }
                    else
                    {
                        if (namefromfilename.StartsWith(namefromdirectory, StringComparison.CurrentCultureIgnoreCase))
                        {
                            name = namefromdirectory;
                        }
                    }
                }
                else if (namefromfilename != null)
                {
                    //check all directories for matches starting with the top level directory
                    for (int i = 0; i < folders.Length; i++)
                    {                        
                        string folder = folders[i];
                        //We may not do this if both the file and the folder have same name because we lose preprocessing
                        if (folder.ToLower() == filename.ToLower()) break;
                        folder = Regex.Replace(folder, "[\\._]", " ");
                        folder = Regex.Replace(folder, "\\[.*\\]", "");
                        folder = folder.Trim();                        
                        if (folder.StartsWith(namefromfilename, StringComparison.CurrentCultureIgnoreCase))
                        {
                            name = folder;
                            break;
                        }
                    }
                }
                else if (namefromdirectory != null)
                {
                }
                if (namefromfilename != null)
                {                    
                }
                /*else if (namefromdirectory != null)
                {
                    namefromdirectory = Regex.Replace(namefromdirectory, "[\\._]", " ");
                    namefromdirectory = Regex.Replace(namefromdirectory, "\\[.*\\]", "");
                    namefromdirectory = namefromdirectory.Trim();
                    namefromdirectory = Helper.VISSpeak(namefromdirectory);
                }*/
                /*//we might want to clean the title a bit, because it often might be recognized in lowercase for example. for this we check if there is a folder with that name.
                if (Directory.Exists(_Path + System.IO.Path.DirectorySeparatorChar + name))
                {
                    string[] dirs = Directory.GetDirectories(_Path);
                    foreach (string str in dirs)
                    {
                        string processed = str.Substring(str.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                        if (processed.ToLower() == name.ToLower())
                        {
                            name = processed;
                            break;
                        }
                    }
                }*/
            }
            Showname = name;
            Info.timeextractname += (DateTime.Now - dt).TotalSeconds;
        }

        public void SetPath()
        {
            string basepath = Helper.ReadProperty(Config.LastDirectory);
            DateTime dt = DateTime.Now;
            if (_Showname == null || _Showname == "")
            {
                Destination = "";
            }
            if (!_Movie)
            {
                //for placing files in directory structure, figure out if selected directory is show name, otherwise create one
                string[] dirs = Path.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                bool InShowDir = false;
                bool InSeasonDir = false;
                string showname = _Showname;
                bool UseSeasonSubDirs = Helper.ReadProperty(Config.UseSeasonSubDir)=="1";
                //fix invalid paths
                if (showname.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                {
                    string pattern = "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
                    string replace = Helper.ReadProperty(Config.InvalidCharReplace);
                    showname = Regex.Replace(showname, pattern, replace);
                }
                //figure out if we are in a season dir
                string[] seasondirs = Helper.ReadProperties(Config.Extract);
                int season = -1;
                try
                {
                    Int32.TryParse(_Season, out season);
                }
                catch (Exception) { };

                string seasondir="";
                //loop backwards so first entry is used if nothing is recognized and folder has to be created
                for (int i = seasondirs.Length-1; i >=0; i--)
                {
                    seasondir = seasondirs[i].Replace("%S", _Season);
                    seasondir = seasondir.Replace("%T", showname);

                    if (dirs.Length > 0 && dirs[dirs.Length - 1] == showname)
                    {
                        InShowDir = true;
                        seasondir = seasondirs[0].Replace("%S", _Season);
                        seasondir = seasondir.Replace("%T", showname);
                        break;
                    }
                    else if (dirs.Length > 1 && Regex.Match(dirs[dirs.Length - 1], seasondir).Success)
                    {
                        InSeasonDir = true;
                        break;
                    }
                }
                //if we want to move the files to a directory structure, set destination property to it here, otherwise use same dir
               DirectoryStructure ds=_CreateDirectoryStructure;
                if(ds==DirectoryStructure.Unset){
                    bool cds=Helper.ReadProperty(Config.CreateDirectoryStructure)=="1";
                    if(cds){
                        ds = DirectoryStructure.CreateDirectoryStructure;
                    }else{
                        ds = DirectoryStructure.NoDirectoryStructure;
                    }
                }
                if (ds==DirectoryStructure.CreateDirectoryStructure)
                {
                    //if there is a valid season, add season dir
                    if (season >= 0)
                    {
                        if (InShowDir)
                        {
                            if (UseSeasonSubDirs)
                            {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else
                            {
                                Destination = basepath;
                            }
                        }
                        else if (InSeasonDir)
                        {
                            if (UseSeasonSubDirs)
                            {
                                //Go up two dirs and add proper show and season dir
                                Destination = Directory.GetParent(Directory.GetParent(Path).FullName).FullName + System.IO.Path.DirectorySeparatorChar + showname + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else
                            {
                                Destination = Directory.GetParent(Directory.GetParent(Path).FullName).FullName + System.IO.Path.DirectorySeparatorChar + showname;
                            }
                        }
                        else
                        {
                            if (UseSeasonSubDirs)
                            {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + showname + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else
                            {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + showname;
                            }
                        }
                    }
                    //no valid season found, this could be a movie or so and probably shouldn't be moved
                    else
                    {
                        Destination = Path;
                    }
                }
                //We don't want to move anything at all, so lets just leave it where it is
                else
                {
                    Destination = Path;
                }
                if (Destination == Path)
                {
                    Destination = "";
                }
            }
            //if this is a movie
            else
            {
                if (Helper.ReadInt(Config.CreateDirectoryStructure) > 0)
                {
                    //Multiple file version, own folder
                    if (Regex.Match(_Name, "CD\\d", RegexOptions.IgnoreCase).Success)
                    {
                        if (Path != "")
                        {
                            Destination = Path + System.IO.Path.DirectorySeparatorChar + _Name.Substring(0, _Name.Length - 3);
                        }
                        else
                        {
                            Destination = _Name.Substring(0, _Name.Length - 3);
                        }
                    }
                    else
                    {
                        Destination = "";
                    }
                }
            }
            Info.timesetpath += (DateTime.Now - dt).TotalSeconds;
        }

        /// <summary>
        /// This function tries to find an episode name which matches the showname, episode and season number by looking at previously downloaded relations
        /// </summary>
        public void SetupRelation()
        {            
            int seasonmatch = -1;
            int epmatch = -1;
            //Clear old name
            Name = "";
            RelationCollection rc = Info.GetRelationCollectionByName(_Showname);
            //if there is no data available, don't do anything
            if (rc == null) return;
            for (int i = 0; i < rc.Relations.Count; i++)
            {
                string season = rc.Relations[i].Season;
                string episode = rc.Relations[i].Episode;
                string name = rc.Relations[i].Name;
                if (season == _Season && episode == _Episode)
                {
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
            if (seasonmatch == -1 && epmatch != -1)
            {
                Name = rc.Relations[epmatch].Name;

            }
            else if (seasonmatch != -1 && epmatch == -1)
            {
                Name = rc.Relations[seasonmatch].Name;
            }
        }

        public string AdjustSpelling(string input, bool extension)
        {
            //treat umlaute and case
            UmlautAction ua = _UmlautUsage;
            if (ua == UmlautAction.Unset)
            {
                ua = (UmlautAction)Enum.Parse(typeof(UmlautAction), Helper.ReadProperty(Config.Umlaute), true);
                //Fallback default value
                if (ua == UmlautAction.Unset) ua = UmlautAction.Ignore;
            }
            Case c = _Casing;
            if (c == Case.Unset)
            {
                c = (Case)Enum.Parse(typeof(Case), Helper.ReadProperty(Config.Case), true);
                //Fallback default value
                if (c == Case.Unset) c = Case.Ignore;
            }

            if (ua == UmlautAction.Use && _Language == Helper.Languages.German)
            {
                input = input.Replace("ae", "ä");
                input = input.Replace("Ae", "Ä");
                input = input.Replace("oe", "ö");
                input = input.Replace("Oe", "Ö");
                input = input.Replace("ue", "ü");
                input = input.Replace("Ue", "Ü");
                input = input.Replace("eü", "eue");                
            }
            else if (ua == UmlautAction.Dont_Use)
            {
                input = input.Replace("ä", "ae");
                input = input.Replace("Ä", "Ae");
                input = input.Replace("ö", "oe");
                input = input.Replace("Ö", "Oe");
                input = input.Replace("ü", "ue");
                input = input.Replace("Ü", "Ue");
                input = input.Replace("ß", "ss");

                input = input.Replace("É", "E");
                input = input.Replace("È", "E");
                input = input.Replace("Ê", "E");
                input = input.Replace("Ë", "E");
                input = input.Replace("Á", "A");
                input = input.Replace("À", "A");
                input = input.Replace("Â", "A");
                input = input.Replace("Ã", "A");
                input = input.Replace("Å", "A");
                input = input.Replace("Í", "I");
                input = input.Replace("Ì", "I");
                input = input.Replace("Î", "I");
                input = input.Replace("Ï", "I");
                input = input.Replace("Ú", "U");
                input = input.Replace("Ù", "U");
                input = input.Replace("Û", "U");
                input = input.Replace("Ó", "O");
                input = input.Replace("Ò", "O");
                input = input.Replace("Ô", "O");
                input = input.Replace("Ý", "Y");
                input = input.Replace("Ç", "C");

                input = input.Replace("é", "e");
                input = input.Replace("è", "e");
                input = input.Replace("ê", "e");
                input = input.Replace("ë", "e");
                input = input.Replace("á", "a");
                input = input.Replace("à", "a");
                input = input.Replace("â", "a");
                input = input.Replace("ã", "a");
                input = input.Replace("å", "a");
                input = input.Replace("í", "i");
                input = input.Replace("ì", "i");
                input = input.Replace("î", "i");
                input = input.Replace("ï", "i");
                input = input.Replace("ú", "u");
                input = input.Replace("ù", "u");
                input = input.Replace("û", "u");
                input = input.Replace("ó", "o");
                input = input.Replace("ò", "o");
                input = input.Replace("ô", "o");
                input = input.Replace("ý", "y");
                input = input.Replace("ÿ", "y");
                input = input.Replace("ç", "c");                
            }
            if (c == Case.small)
            {
                input = input.ToLower();
            }
            else if (c == Case.Large)
            {
                if (!extension)
                {
                    Regex r = new Regex(@"\b(\w)(\w+)?\b", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    input = Helper.VISSpeak(input);
                }
                else
                {
                    input = input.ToLower();
                }
            }
            else if (c == Case.CAPSLOCK)
            {
                input = input.ToUpper();
            }
            return input;
        }
        /// <summary>
        /// This function generates a new filename from the Target Pattern, episode, season, title, showname,... values
        /// </summary>
        public void CreateNewName()
        {
            DateTime dt = DateTime.Now;
            if (_Name == "")
            {
                NewFileName = "";
            }
            else if (_Name != "")
            {
                //Target Filename format
                string name;

                //Those 3 strings need case/Umlaut processing
                string epname = _Name;
                string seriesname = _Showname;
                string extension = Extension;

                if (!_Movie)
                {
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
                else
                {
                    name = "%N";
                }


                AdjustSpelling(epname, false);
                AdjustSpelling(seriesname, false);
                AdjustSpelling(extension, true);



                //Now that series title, episode title and extension are properly processed, add them to the filename

                //Remove extension from target filename (if existant) and add properly cased one
                name = Regex.Replace(name, "\\."+extension, "", RegexOptions.IgnoreCase);
                name += "."+extension;

                name = name.Replace("%T", seriesname);
                name = name.Replace("%N", epname);

                //string replace function
                List<string> replace = new List<string>(Helper.ReadProperties(Config.Replace));
                List<string> from = new List<string>();
                List<string> to = new List<string>();
                foreach (string s in replace)
                {
                    if (!s.StartsWith(Settings.getInstance().Comment))
                    {
                        int pos = s.IndexOf("->");
                        if (pos > 0 && pos < s.Length - 2)
                        {
                            string f = s.Substring(0, pos);
                            string t = s.Substring(pos + 2);
                            name = Regex.Replace(name, f, t, RegexOptions.IgnoreCase);
                        }
                    }
                }

                //Invalid character replace
                if (Helper.ReadProperty(Config.InvalidCharReplace) != null && (Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction), Helper.ReadProperty(Config.InvalidFilenameAction)) == Helper.InvalidFilenameAction.Replace)
                {
                    string pattern = "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
                    name = Regex.Replace(name, pattern, Helper.ReadProperty(Config.InvalidCharReplace));
                }

                //set new filename if renaming process is required
                if (Filename == name)
                {
                    NewFileName = "";
                }
                else
                {
                    NewFileName = name;
                }
            }
            Info.timecreatenewname += (DateTime.Now - dt).TotalSeconds;
        }
    }
}
