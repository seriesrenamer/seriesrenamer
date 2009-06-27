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
        public enum Case : int { Unset, Ignore, small, UpperFirst, CAPSLOCK };

        /// <summary>
        /// if file should be moved
        /// </summary>
        public enum DirectoryStructure : int { Unset, CreateDirectoryStructure, NoDirectoryStructure };
        #endregion


        #region members
        private Filepath source;
        private Filepath destination;

        private string nameOfSeries;
        private string nameOfEpisode;

        private int season;
        private int episode;

        private bool isVideofile;
        private bool isSubtitle;
        private bool processingRequested;
        private bool isMovie;
        private bool isSample;
        private bool markedForDeletion;

        private UmlautAction umlautUsage;
        private Case casing;
        private DirectoryStructure createDirectoryStructure;
        private Helper.Languages language;
        #endregion

        #region Static Members
        private static string[] toBeReplaced = { "ä", "Ä", "ö", "Ö", "ü", "Ü", "ß", "É", "È", "Ê", "Ë", "Á", "À", "Â", "Ã", "Å", "Í", "Ì", "Î", "Ï", "Ú", "Ù", "Û", "Ó", "Ò", "Ô", "Ý", "Ç", "é", "è", "ê", "ë", "á", "à", "â", "ã", "å", "í", "ì", "î", "ï", "ú", "ù", "û", "ó", "ò", "ô", "ý", "ÿ", "ç" };
        private static string[] toBeReplacedWith = { "ae", "Ae", "oe", "Oe", "ue", "Ue", "ss", "E", "E", "E", "E", "A", "A", "A", "A", "A", "I", "I", "I", "I", "U", "U", "U", "O", "O", "O", "Y", "C", "e", "e", "e", "e", "a", "a", "a", "a", "a", "i", "i", "i", "i", "u", "u", "u", "o", "o", "o", "y", "y", "c" };
        public static string NotRecognized = "Not recognized, please enter manually";
        #endregion

        #region Properties

        /// <summary>
        /// destination directory
        /// </summary>
        public string Destination {
            get {
                if (MarkedForDeletion)
                {
                    return "To be deleted";
                }
                else
                {
                    if (String.IsNullOrEmpty(Showname))
                    {
                        return "";
                    }
                    else
                    {
                        return destination.Path;
                    }
                }
            }
            set { destination.Path = value; }
        }
        /// <summary>
        /// new filename with extension
        /// </summary>
        public string NewFileName {
            get {
                if (MarkedForDeletion)
                {
                    return "To be deleted";
                }
                else
                {
                    if (String.IsNullOrEmpty(Showname))
                    {
                        return "";
                    }
                    else
                    {
                        return this.destination.Filename;
                    }
                }
            }
            set { this.destination.Filename = value; }
        }
        /// <summary>
        /// Old filename with extension
        /// </summary>
        public string Filename {
            get { return source.Filename; }
            set {
                if (source.Filename != value) {
                    source.Filename = value;
                    if(source.Filename!=""){
                        CheckExtension();
                        if (source.Path != "")
                        {
                            ExtractName();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Extension of the file without dot, i.e. "avi" or "srt"
        /// </summary>
        public string Extension {
            get { return source.Extension; }
            set {
                if (source.Extension != value) {
                    source.Extension = value;
                    destination.Extension = value;
                    CheckExtension();
                    ExtractName();
                }
            }
        }
        /// <summary>
        /// Path of the file
        /// </summary>
        public string Filepath {
            get { return source.Path; }
            set {
                if (source.Path != value) {
                    source.Path = value;
                    if (source.Filename != "" && source.Path != "")
                    {
                        ExtractName();
                    }
                }
            }
        }
        /// <summary>
        /// Path of the file
        /// </summary>
        public Filepath Filepath1 {
            get { return source; }
            set {
                source = value;
            }
        }
        /// <summary>
        /// Name of the show this file belongs to.
        /// </summary>
        public string Showname {
            get { return nameOfSeries; }
            set {
                if (value == NotRecognized) value = "";
                if (nameOfSeries != value) {
                    if (value == null) value = "";
                    if (value == "Sample" && Helper.ReadBool(Config.DeleteSampleFiles))
                    {
                        MarkedForDeletion = true;
                    }
                    nameOfSeries = value;
                    CreateNewName();
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
                    CreateNewName();
                }
            }
        }
        /// <summary>
        /// name of the episode
        /// </summary>
        public string Name {
            get { return nameOfEpisode; }
            set {
                if (nameOfEpisode != value) {
                    nameOfEpisode = value;
                    SetPath();
                    CreateNewName();
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
                    SetPath();
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

        public bool Sample
        {
            get { return isSample; }
            set { isSample = value; }
        }
        /// <summary>
        /// If file is to be deleted
        /// </summary>
        public bool MarkedForDeletion
        {
            get { return markedForDeletion; }
            set { markedForDeletion = value; }
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
                    if (source.Name != "") SetPath();
                    if (NewFileName != "") CreateNewName();
                }
            }
        }
        #endregion

        #region Private properties

        private bool isMultiFileMovie {
            get {
                return Regex.Match(nameOfEpisode, "CD\\d", RegexOptions.IgnoreCase).Success;
            }
        }

        #endregion

        #region private Methods
        private void initMembers() {
            source = new Filepath();
            destination = new Filepath();

            nameOfSeries = "";
            nameOfEpisode = "";

            season = -1;
            episode = -1;

            isVideofile = false;
            isSubtitle = false;
            processingRequested = true;
            isMovie = false;

            umlautUsage = UmlautAction.Unset;
            casing = Case.Unset;
            createDirectoryStructure = DirectoryStructure.Unset;
            language = Helper.Languages.None;
        }


        /// <summary>
        /// Extracts the showname of the filename
        /// </summary>
        private void ExtractName() {
            if (!source.isEmpty) {
                Showname = SeriesNameExtractor.Instance.ExtractSeriesName(source);
                if (Showname == "Sample")
                {
                    Sample = true;
                }
            }
        }

        /// <summary>
        /// set Properties depending on the Extension of the file
        /// </summary>
        private void CheckExtension() {
            this.isVideofile = (new List<string>(Helper.ReadProperties(Config.Extensions, true))).Contains(this.source.Extension);
            this.isSubtitle = (new List<string>(Helper.ReadProperties(Config.SubtitleExtensions, true))).Contains(this.source.Extension);
        }


        private void SetMoviesPath() {
            if (Helper.ReadBool(Config.CreateDirectoryStructure)) {
                Destination = "";
                if (isMultiFileMovie) {
                    Destination = getMoviesDestinationPath();
                }
            }
        }

        private string getMoviesDestinationPath() {
            return this.destination.Path + nameOfEpisodeWithoutPart(); ;
        }
        private string nameOfEpisodeWithoutPart() {
            return nameOfEpisode.Substring(0, nameOfEpisode.Length - 3);
        }


        // TODO: function is still tooooooo large
        private void SetSeriesPath() {
            if (Season == -1)
            {
                Destination = "";
                return;
            }
            string basepath = Helper.ReadProperty(Config.LastDirectory);
            string DestinationPath = Helper.ReadProperty(Config.DestinationDirectory);
           
            if (!Directory.Exists(DestinationPath))
            {
                DestinationPath = basepath;
            }
            bool DifferentDestinationPath = basepath!=DestinationPath;
            //for placing files in directory structure, figure out if selected directory is show name, otherwise create one
            string[] dirs = this.source.Folders;
            bool InSeriesDir = false;
            bool InSeasonDir = false;
            bool UseSeasonSubDirs = Helper.ReadBool(Config.UseSeasonSubDir);
            //figure out if we are in a season dir
            string[] seasondirs = Helper.ReadProperties(Config.Extract);

            string seasondir = "";
            //loop backwards so first entry is used if nothing is recognized and folder has to be created
            for (int i = seasondirs.Length - 1; i >= 0; i--) {
                seasondir = RegexConverter.replaceSeriesnameAndSeason(seasondirs[i], nameOfSeries, season.ToString());

                if (dirs.Length > 0 && dirs[dirs.Length - 1].StartsWith(nameOfSeries)) {
                    InSeriesDir = true;
                    break;
                }
                else if (dirs.Length > 1 && Regex.Match(dirs[dirs.Length - 1], seasondir).Success) {
                    InSeasonDir = true;
                    break;
                }
            }
            getCreateDirectory();
            if (createDirectoryStructure != DirectoryStructure.CreateDirectoryStructure || !isSeasonValid()) {
                Destination = "";
                return;
            }
            if (InSeasonDir&&!DifferentDestinationPath) {
                DestinationPath = addSeriesDir(getParentsParentDir(Filepath));
            }
            //Some Problem here if series dir is deeper nested than in basepath dir!
            else if(!InSeriesDir&&!DifferentDestinationPath) {
                DestinationPath = addSeriesDir(Filepath);
            }
            else if (DifferentDestinationPath)
            {
                DestinationPath = addSeriesDir(DestinationPath);
            }
            Destination = addSeasonsDirIfDesired(DestinationPath);
        }
        private void getCreateDirectory() {
            if (createDirectoryStructure != DirectoryStructure.Unset)
                return;
            loadSettingCreateDirectory();
        }

        private void loadSettingCreateDirectory() {
            createDirectoryStructure = (Helper.ReadBool(Config.CreateDirectoryStructure)) ? DirectoryStructure.CreateDirectoryStructure : DirectoryStructure.NoDirectoryStructure;
        }
     
        private bool isSeasonValid() {
            return this.season >= 0;
        }

        private string getParentsParentDir(string directory){
            return Directory.GetParent(Directory.GetParent(directory).FullName).FullName;
        }

        private string addSeriesDir(string path){
            return path + System.IO.Path.DirectorySeparatorChar + nameOfSeries;
        }
        
        private string addSeasonsDirIfDesired(string path) {
            return path + ((useSeasonSubDirs())?seasonsSubDir():"");
        }
 
        private bool useSeasonSubDirs() {
            return Helper.ReadBool(Config.UseSeasonSubDir);
        }

        private string seasonsSubDir() {
            string seasondir = RegexConverter.replaceSeriesnameAndSeason(Helper.ReadProperties(Config.Extract)[0], nameOfSeries, season.ToString());
            return System.IO.Path.DirectorySeparatorChar + seasondir;
        }


        #endregion
        #region public Methods
        public InfoEntry() {
            initMembers();
        }

        /// <summary>
        /// Generates the file and directory name the file should be stored
        /// </summary>
        public void SetPath() {
            if (String.IsNullOrEmpty(nameOfSeries)) {
                Destination = "";
            }
            if (!isMovie) {
                SetSeriesPath();
            }
            else {
                SetMoviesPath();
            }
        }

        /// <summary>
        /// This function tries to find an episode name which matches the showname, episode and season number by looking at previously downloaded relations
        /// </summary>
        public void SetupRelation() {
            findEpisodeName(RelationManager.Instance.GetRelationCollection(this.Showname));
        }

        public void findEpisodeName(RelationCollection rc) {
            resetName();
            if (rc == null)
                return;

            for (int i = 0; i < rc.Count; i++) {
                if (isValidRelation(rc[i])) {
                    Name = rc[i].Name;
                    break;
                }
                if (isInValidSeason(rc[i]) || isInValidEpisode(rc[i]))
                    Name = rc[i].Name;
            }
        }

        private bool isValidRelation(Relation relation) {
            return relation.Season == season && relation.Episode == episode;
        }

        private bool isInValidEpisode(Relation relation) {
            return relation.Season == season && episode == -1;
        }

        private bool isInValidSeason(Relation relation) {
            return relation.Episode == episode && season == -1;
        }

        private void resetName() {
            this.Name = "";
        }

        public string adjustSpelling(string input, bool extension) {
            input = adjustUmlauts(input);
            input = adjustCasing(input, extension);
            return input;
        }

        private string adjustUmlauts(string input) {
            UmlautAction ua = readUmlautUsage();
            if (ua == UmlautAction.Use && language == Helper.Languages.German) {
                input = transformDoubleLetterToUmalauts(input);
            }
            else if (ua == UmlautAction.Dont_Use) {
                input = replaceUmlautsAndSpecialChars(input);
            }
            return input;
        }

        private UmlautAction readUmlautUsage() {
            UmlautAction ua = umlautUsage;
            if (ua == UmlautAction.Unset) {
                ua = Helper.ReadEnum<UmlautAction>(Config.Umlaute);
                if (ua == UmlautAction.Unset)
                    ua = UmlautAction.Ignore;
            }
            return ua;
        }

        private string adjustCasing(string input, bool extension) {
            if (casing == Case.Unset) {
                casing = Helper.ReadEnum<Case>(Config.Case);
                if (casing == Case.Unset)
                    casing = Case.Ignore;
            }
            if (casing == Case.small) {
                input = input.ToLower();
            }
            else if (casing == Case.UpperFirst) {
                if (!extension) {
                    Regex r = new Regex(@"\b(\w)(\w+)?\b", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    input = Helper.UpperEveryFirst(input);
                }
                else {
                    input = input.ToLower();
                }
            }
            else if (casing == Case.CAPSLOCK) {
                input = input.ToUpper();
            }
            return input;
        }

        private string replaceUmlautsAndSpecialChars(string input) {
            for (int index = 0; index < toBeReplaced.Length; index++) {
                input = input.Replace(toBeReplaced[index], toBeReplacedWith[index]);
            }
            return input;
        }
        private string transformDoubleLetterToUmalauts(string input) {
            input = input.Replace("ae", "ä");
            input = input.Replace("Ae", "Ä");
            input = input.Replace("oe", "ö");
            input = input.Replace("Oe", "Ö");
            input = input.Replace("ue", "ü");
            input = input.Replace("Ue", "Ü");
            input = input.Replace("eü", "eue");
            return input;
        }
     
        
        /// <summary>
        /// This function generates a new filename from the Target Pattern, episode, season, title, showname,... values
        /// </summary>
        public void CreateNewName() {
            if (nameOfEpisode == "") {
                NewFileName = "";
                return;
            }
            else if (nameOfEpisode != "") {
                //Target Filename format
                string tmpname;

                //Those 3 strings need case/Umlaut processing
                string epname = this.nameOfEpisode;
                string seriesname = nameOfSeries;
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

                adjustSpelling(epname, false);
                adjustSpelling(seriesname, false);
                adjustSpelling(extension, true);

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
                        string[] replacement = s.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
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
        public string ToString() {
            return Filename;
        }
        #endregion
    }
}
