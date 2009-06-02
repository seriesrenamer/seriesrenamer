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
using System.Data;
using System.Timers;
using Renamer.Logging;
using Renamer.Dialogs;
using System.Windows.Forms;

namespace Renamer.Classes
{
    /// <summary>
    /// Contains all information about a single video or subtitle file, including scheduled renaming info
    /// </summary>
    public class InfoEntry
    {

        #region Enumerations
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
        #endregion


        #region members
        private string filename = "";
        private string extension = "";
        private string filepath = "";
        private string newFileName = "";
        private string destination = "";
        private string showname = "";
        private string name = "";

        private int season = -1;
        private int episode = -1;
        
        private bool isVideofile;
        private bool isSubtitle;
        private bool processingRequested = true;
        private bool isMovie = false;
        
        private UmlautAction umlautUsage = UmlautAction.Unset;
        private Case casing = Case.Unset;
        private DirectoryStructure createDirectoryStructure = DirectoryStructure.Unset;
        private Helper.Languages language = Helper.Languages.None;
        #endregion

        #region Static Members
        private static string[] toBeReplaced = { "ä", "Ä", "ö", "Ö", "ü", "Ü", "ß", "É", "È", "Ê", "Ë", "Á", "À", "Â", "Ã", "Å", "Í", "Ì", "Î", "Ï", "Ú", "Ù", "Û", "Ó", "Ò", "Ô", "Ý", "Ç", "é", "è", "ê", "ë", "á", "à", "â", "ã", "å", "í", "ì", "î", "ï", "ú", "ù", "û", "ó", "ò", "ô", "ý", "ÿ", "ç" };
        private static string[] toBeReplacedWith = { "ae", "Ae", "oe", "Oe", "ue", "Ue", "ss", "E", "E", "E", "E", "A", "A", "A", "A", "A", "I", "I", "I", "I", "U", "U", "U", "O", "O", "O", "Y", "C", "e", "e", "e", "e", "a", "a", "a", "a", "a", "i", "i", "i", "i", "u", "u", "u", "o", "o", "o", "y", "y", "c" };
        #endregion

        #region Properties

        /// <summary>
        /// destination directory
        /// </summary>
        public string Destination {
            get { return destination; }
            set { destination = value; }
        }
        /// <summary>
        /// new filename with extension
        /// </summary>
        public string NewFileName {
            get { return this.newFileName; }
            set { this.newFileName = value; }
        }
        /// <summary>
        /// Old filename with extension
        /// </summary>
        public string Filename {
            get { return filename; }
            set {
                if (filename != value) {
                    filename = value;
                    ExtractName();
                }
            }
        }
        /// <summary>
        /// Extension of the file without dot, i.e. "avi" or "srt"
        /// </summary>
        public string Extension {
            get { return extension; }
            set {
                if (extension != value) {
                    extension = value;
                    CheckExtension();
                    ExtractName();
                }
            }
        }
        /// <summary>
        /// Path of the file
        /// </summary>
        public string Filepath {
            get { return filepath; }
            set {
                if (filepath != value) {
                    filepath = value;
                    ExtractName(); 
                }
            }
        }
        /// <summary>
        /// Name of the show this file belongs to.
        /// </summary>
        public string Showname {
            get { return showname; }
            set {
                if (showname != value) {
                    if (value == null) value = "";
                    showname = value;
                    SetPath();
                }
            }
        }
        /// <summary>
        /// number of the season
        /// </summary>
        public int Season {
            get { return season; }
            set {
                if (season != value) {
                    season = value;
                    CreateNewName();
                    SetPath();
                    SetupRelation();
                }
            }
        }
        /// <summary>
        /// number of the episode
        /// </summary>
        public int Episode {
            get { return episode; }
            set {
                if (episode != value) {
                    episode = value;
                    SetupRelation();
                }
            }
        }
        /// <summary>
        /// name of the episode
        /// </summary>
        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    if (Movie == true) {
                        SetPath();
                    }
                }
            }
        }

        public bool IsVideofile {
            get {
                return this.isVideofile;
            }
        }
        public bool IsSubtitle {
            get {
                return this.isSubtitle;
            }
        }
        /// <summary>
        /// If file is a movie.
        /// </summary>
        public bool Movie {
            get { return isMovie; }
            set {
                if (isMovie != value) {
                    isMovie = value;
                    CreateNewName();
                    if (Movie == true) {
                        SetPath();
                    }
                }
            }
        }
        /// <summary>
        /// If file is to be processed
        /// </summary>
        public bool ProcessingRequested {
            get { return processingRequested; }
            set { processingRequested = value; }
        }

        /// <summary>
        /// Option indicates the using of umlaute
        /// </summary>
        public UmlautAction UmlautUsage {
            get { return umlautUsage; }
            set {
                if (umlautUsage != value) {
                    umlautUsage = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }
        /// <summary>
        /// Option indicates the use of UPPER and lowercase
        /// </summary>
        public Case Casing {
            get { return casing; }
            set {
                if (casing != value) {
                    casing = value;
                    CreateNewName();
                    SetPath();
                }
            }
        }

        public DirectoryStructure CreateDirectoryStructure {
            get { return createDirectoryStructure; }
            set {
                if (createDirectoryStructure != value) {
                    createDirectoryStructure = value;
                    SetPath();
                }
            }
        }
        public Helper.Languages Language {
            get { return language; }
            set {
                if (language != value) {
                    language = value;
                    if (filepath != "") SetPath();
                    if (NewFileName != "") CreateNewName();
                }
            }
        }
        #endregion

        #region private Methods
        /// <summary>
        /// Extracts the name from a file and sets it as title
        /// </summary>
        /// <returns>extracted name or null</returns>
        private void ExtractName() {
            if (filepath == "" && filename == "" && extension == "") {
                return;
            }
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            Showname = SeriesNameExtractor.Instance.ExtractSeriesName(filename, filepath);
            stopWatch.Stop();
        }

        /// <summary>
        /// Checks the Extension
        /// </summary>
        private void CheckExtension() {
            this.isVideofile = (new List<string>(Helper.ReadProperties(Config.Extensions, true))).Contains(this.extension);
            this.isSubtitle = (new List<string>(Helper.ReadProperties(Config.SubtitleExtensions, true))).Contains(this.extension);
        }
        #endregion
        #region public Methods

        public void SetPath() {
            string basepath = Helper.ReadProperty(Config.LastDirectory);
            DateTime dt = DateTime.Now;
            if (showname == null || showname == "") {
                Destination = "";
            }
            if (!isMovie) {
                //for placing files in directory structure, figure out if selected directory is show name, otherwise create one
                string[] dirs = Filepath.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                bool InShowDir = false;
                bool InSeasonDir = false;
                string tmpshowname = showname;
                bool UseSeasonSubDirs = Helper.ReadProperty(Config.UseSeasonSubDir) == "1";
                //fix invalid paths
                if (tmpshowname.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) {
                    string pattern = "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
                    string replace = Helper.ReadProperty(Config.InvalidCharReplace);
                    tmpshowname = Regex.Replace(tmpshowname, pattern, replace);
                }
                //figure out if we are in a season dir
                string[] seasondirs = Helper.ReadProperties(Config.Extract);

                string seasondir = "";
                //loop backwards so first entry is used if nothing is recognized and folder has to be created
                for (int i = seasondirs.Length - 1; i >= 0; i--) {
                    seasondir = seasondirs[i].Replace("%S", season.ToString());
                    seasondir = seasondir.Replace("%T", tmpshowname);

                    if (dirs.Length > 0 && dirs[dirs.Length - 1] == tmpshowname) {
                        InShowDir = true;
                        seasondir = seasondirs[0].Replace("%S", season.ToString());
                        seasondir = seasondir.Replace("%T", tmpshowname);
                        break;
                    }
                    else if (dirs.Length > 1 && Regex.Match(dirs[dirs.Length - 1], seasondir).Success) {
                        InSeasonDir = true;
                        break;
                    }
                }
                //if we want to move the files to a directory structure, set destination property to it here, otherwise use same dir
                DirectoryStructure ds = createDirectoryStructure;
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
                                Destination = Directory.GetParent(Directory.GetParent(Filepath).FullName).FullName + System.IO.Path.DirectorySeparatorChar + tmpshowname + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else {
                                Destination = Directory.GetParent(Directory.GetParent(Filepath).FullName).FullName + System.IO.Path.DirectorySeparatorChar + tmpshowname;
                            }
                        }
                        else {
                            if (UseSeasonSubDirs) {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + tmpshowname + System.IO.Path.DirectorySeparatorChar + seasondir;
                            }
                            else {
                                Destination = basepath + System.IO.Path.DirectorySeparatorChar + tmpshowname;
                            }
                        }
                    }
                    //no valid season found, this could be a movie or so and probably shouldn't be moved
                    else {
                        Destination = Filepath;
                    }
                }
                //We don't want to move anything at all, so lets just leave it where it is
                else {
                    Destination = Filepath;
                }
                if (Destination == Filepath) {
                    Destination = "";
                }
            }
            //if this is a movie
            else {
                if (Helper.ReadInt(Config.CreateDirectoryStructure) > 0) {
                    //Multiple file version, own folder
                    if (Regex.Match(name, "CD\\d", RegexOptions.IgnoreCase).Success) {
                        if (Filepath != "") {
                            Destination = Filepath + System.IO.Path.DirectorySeparatorChar + name.Substring(0, name.Length - 3);
                        }
                        else {
                            Destination = name.Substring(0, name.Length - 3);
                        }
                    }
                    else {
                        Destination = "";
                    }
                }
            }
        }
        
        /// <summary>
        /// This function tries to find an episode name which matches the showname, episode and season number by looking at previously downloaded relations
        /// </summary>
        public void SetupRelation() {
            SetupRelation(RelationManager.Instance.GetRelationCollection(this.Showname));
        }
        /// <summary>
        /// This function tries to find an episode name which matches the showname, episode and season number by looking at previously downloaded relations
        /// </summary>
        public void SetupRelation(RelationCollection rc) {
            int seasonmatch = -1;
            int epmatch = -1;
            //Clear old name
            Name = "";
            //if there is no data available, don't do anything
            if (rc == null) return;
            for (int i = 0; i < rc.Count; i++) {
                string name = rc[i].Name;
                if (rc[i].Season == season && rc[i].Episode == episode) {
                    Name = name;
                    seasonmatch = i;
                    epmatch = i;
                    break;
                }
                //if there is none or invalid episode info, try to match by season
                if (rc[i].Season == season && episode == -1) seasonmatch = i;
                //if there is none or invalid season info, try to match by episode
                if (rc[i].Episode == episode && season == -1) epmatch = i;
            }
            //episode match, but no season match
            if (seasonmatch == -1 && epmatch != -1) {
                Name = rc[epmatch].Name;

            }
            else if (seasonmatch != -1 && epmatch == -1) {
                Name = rc[seasonmatch].Name;
            }
        }

        public string AdjustSpelling(string input, bool extension) {
            //treat umlaute and case
            UmlautAction ua = umlautUsage;
            if (ua == UmlautAction.Unset) {
                ua = (UmlautAction)Enum.Parse(typeof(UmlautAction), Helper.ReadProperty(Config.Umlaute), true);
                //Fallback default value
                if (ua == UmlautAction.Unset) ua = UmlautAction.Ignore;
            }
            Case c = casing;
            if (c == Case.Unset) {
                c = (Case)Enum.Parse(typeof(Case), Helper.ReadProperty(Config.Case), true);
                //Fallback default value
                if (c == Case.Unset) c = Case.Ignore;
            }

            if (ua == UmlautAction.Use && language == Helper.Languages.German) {
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
            if (name == "") {
                NewFileName = "";
            }
            else if (name != "") {
                //Target Filename format
                string tmpname;

                //Those 3 strings need case/Umlaut processing
                string epname = this.name;
                string seriesname = showname;
                string extension = Extension;

                if (!isMovie) {
                    tmpname = Helper.ReadProperty(Config.TargetPattern);
                    tmpname = tmpname.Replace("%e", episode.ToString());
                    tmpname = tmpname.Replace("%E", episode.ToString("00"));
                    tmpname = tmpname.Replace("%s", season.ToString());
                    tmpname = tmpname.Replace("%S", season.ToString("00"));
                }
                else {
                    tmpname = "%N";
                }

                AdjustSpelling(epname, false);
                AdjustSpelling(seriesname, false);
                AdjustSpelling(extension, true);

                //Now that series title, episode title and extension are properly processed, add them to the filename

                //Remove extension from target filename (if existant) and add properly cased one
                tmpname = Regex.Replace(tmpname, "\\." + extension, "", RegexOptions.IgnoreCase);
                tmpname += "." + extension;

                tmpname = tmpname.Replace("%T", seriesname);
                tmpname = tmpname.Replace("%N", epname);

                //string replace function
                List<string> replace = new List<string>(Helper.ReadProperties(Config.Replace));
                List<string> from = new List<string>();
                List<string> to = new List<string>();
                foreach (string s in replace) {
                    if (!s.StartsWith(Settings.Instance.Comment)) {
                        string[] replacement = s.Split(new string[]{"->"}, StringSplitOptions.RemoveEmptyEntries);
                        if (replacement != null && replacement.Length == 2) {
                            tmpname = Regex.Replace(tmpname, replacement[0], replacement[1], RegexOptions.IgnoreCase);
                        }
                    }
                }

                //Invalid character replace
                if (Helper.ReadProperty(Config.InvalidCharReplace) != null && (Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction), Helper.ReadProperty(Config.InvalidFilenameAction)) == Helper.InvalidFilenameAction.Replace) {
                    string pattern = "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
                    tmpname = Regex.Replace(tmpname, pattern, Helper.ReadProperty(Config.InvalidCharReplace));
                }

                //set new filename if renaming process is required
                if (Filename == tmpname) {
                    NewFileName = "";
                }
                else {
                    NewFileName = tmpname;
                }
            }
        }

        public void RemoveVideoTags(string[] regexes) {
            this.ProcessingRequested = false;
            //Go through all selected files and remove tags and clean them up
            this.Name = "";
            this.Destination = "";
            this.Movie = true;
            string temp = Path.GetFileNameWithoutExtension(this.Filename);
            //figure out if this is a multi file video
            string end = "CD";
            //Check for single number
            if (char.IsNumber(temp[temp.Length - 1])) {
                end += temp[temp.Length - 1].ToString();
            }
            //Check for IofN format                    
            else if (Regex.Match(temp, "\\dof\\d", RegexOptions.IgnoreCase).Success) {
                end = temp.Substring(temp.Length - 4, 1);
            }
            //Check for a/b
            else if (char.ToLower(temp[temp.Length - 1]) == 'a') {
                end += "1";
            }
            else if (char.ToLower(temp[temp.Length - 1]) == 'b') {
                end += "2";
            }
            else {
                end = "";
            }

            //try to match tags                    
            bool removed = false;
            foreach (string s in regexes) {
                Match m = Regex.Match(temp, s, RegexOptions.IgnoreCase);
                if (m.Success) {
                    temp = temp.Substring(0, m.Index);
                    removed = true;
                }
            }

            //add possible existant file index back
            if (removed) {
                temp = temp + " " + end;
            }

            //Get rid of dots and _
            int i = -1;
            int pos = 0;
            while ((i = temp.IndexOf(".", pos + 1)) != -1) {
                if ((Convert.ToInt32(char.IsNumber(temp[i - 1])) + Convert.ToInt32(char.IsNumber(temp[Math.Min(i + 1, temp.Length - 1)]))) < 2) {
                    temp = temp.Substring(0, i) + " " + temp.Substring(i + 1);
                }
                pos = i;
            }
            temp = temp.Replace("_", " ");
            temp = temp.Trim();



            this.Name = temp;
            if (this.NewFileName != "" || this.Destination != "") {
                this.ProcessingRequested = true;
            }

        }

        public void Rename(ref Helper.InvalidFilenameAction invalidAction, ref string replace) {
            string pattern = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";
            if (this.ProcessingRequested
                && ((this.Filename != this.NewFileName && this.NewFileName != "")
                    || (this.Destination != this.Filepath && this.Destination != ""))) {
                try {
                    while (this.NewFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 && invalidAction == Helper.InvalidFilenameAction.Skip) {
                        if (invalidAction == Helper.InvalidFilenameAction.Replace) {
                            this.NewFileName = Regex.Replace(this.NewFileName, pattern, replace);
                        }
                        else if (invalidAction == Helper.InvalidFilenameAction.Ask) {
                            InvalidFilename td = new InvalidFilename(this.NewFileName);
                            td.ShowDialog();
                            if (td.action == InvalidFilename.Action.SkipAll) {
                                invalidAction = Helper.InvalidFilenameAction.Skip;
                                if (td.remember) {
                                    Helper.WriteProperty(Config.InvalidFilenameAction, Helper.InvalidFilenameAction.Skip.ToString());
                                }
                                break;
                            }
                            else if (td.action == InvalidFilename.Action.Skip) {
                                return;
                            }
                            else if (td.action == InvalidFilename.Action.Filename) {
                                this.NewFileName = td.FileName;
                            }
                            else if (td.action == InvalidFilename.Action.Replace) {
                                if (td.remember) {
                                    Helper.WriteProperty(Config.InvalidFilenameAction, Helper.InvalidFilenameAction.Replace.ToString());
                                    Helper.WriteProperty(Config.InvalidCharReplace, td.Replace);
                                }
                                replace = td.Replace;
                                this.NewFileName = Regex.Replace(this.NewFileName, pattern, replace);
                            }
                        }
                    }

                    //check for empty extension
                    if (this.NewFileName != "" && Path.GetExtension(this.NewFileName) == "") {
                        if (MessageBox.Show(this.Filepath + Path.DirectorySeparatorChar + this.Filename + "->" + this.Destination + Path.DirectorySeparatorChar + this.NewFileName + " has no extension. Rename anyway?", "No extension", MessageBoxButtons.YesNo) == DialogResult.No) {
                            return;
                        }
                    }

                    if (this.NewFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 && (invalidAction == Helper.InvalidFilenameAction.Skip)) {
                        Logger.Instance.LogMessage("Skipped " + this.Filepath + Path.DirectorySeparatorChar + this.Filename + "->" + this.Destination + Path.DirectorySeparatorChar + this.NewFileName + " because of illegal characters in new name.", LogLevel.WARNING);
                    }
                    if (invalidAction == Helper.InvalidFilenameAction.Skip) {
                        return;
                    }

                    //create directory if needed
                    if (this.Destination != "" && !Directory.Exists(this.Destination)) {
                        Directory.CreateDirectory(this.Destination);
                    }
                    //Move to desired destination      
                    if (this.Destination != "") {
                        if (this.NewFileName != "") {
                            File.Move(this.Filepath + Path.DirectorySeparatorChar + this.Filename, this.Destination + Path.DirectorySeparatorChar + this.NewFileName);
                        }
                        else {
                            File.Move(this.Filepath + Path.DirectorySeparatorChar + this.Filename, this.Destination + Path.DirectorySeparatorChar + this.Filename);
                        }
                    }
                    else {
                        if (this.NewFileName != "") {
                            File.Move(this.Filepath + Path.DirectorySeparatorChar + this.Filename, this.Filepath + Path.DirectorySeparatorChar + this.NewFileName);
                        }
                        else {
                            File.Move(this.Filepath + Path.DirectorySeparatorChar + this.Filename, this.Filepath + Path.DirectorySeparatorChar + this.Filename);
                        }
                    }
                    //Delete empty folders code
                    if (Helper.ReadBool(Config.DeleteEmptyFolders)) {
                        //DeleteAllEmptyFolders(this.Filepath, Helper.ReadProperties(Config.IgnoreFiles));
                    }
                    if (this.NewFileName != "") {
                        this.Filename = this.NewFileName;
                    }
                    if (this.Destination != "") {
                        this.Filepath = this.Destination;
                    }
                    this.Destination = "";
                    this.NewFileName = "";
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage(this.Filepath + Path.DirectorySeparatorChar + this.Filename + " -> " + this.Destination + Path.DirectorySeparatorChar + this.NewFileName + ": " + ex.Message, LogLevel.ERROR);
                    if (invalidAction == Helper.InvalidFilenameAction.Skip) {
                        Logger.Instance.LogMessage("Skipping " + this.Filename + " beacause " + ex.Message, LogLevel.WARNING);
                        return;
                    }
                    
                }
            }
        }

        public string ToString()
        {
            return Filename;
        }
        #endregion
    }
}
