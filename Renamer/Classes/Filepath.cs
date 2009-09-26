using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Renamer.Classes.Configuration.Keywords;
using System.IO;

namespace Renamer.Classes
{
    public class Filepath
    {
        #region Static factorymethods
        public static Filepath fromFileLocation(string filename) {
            return new Filepath(filename);
        }

        public static Filepath fromFileNameAndPath(string filename, string path) {
            return new Filepath(filename, path);
        }
        #endregion

        protected static char[] DIRECTORY_SEPERATORS = new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar };
        protected static string EXTENSION_SEPERATOR = ".";

        protected string path;
        protected string filename;
        protected string extension;

        #region Constructors
        public Filepath() {
            this.Filename = "";
            this.Path = "";
        }
        public Filepath(string filename) {
            this.Filename = filename;
            extractPath();
        }
        public Filepath(string filename, string path) {
            if(isValidFilename(filename)){
                throw new Exception("Invalid filename. The path must not be in the filename, use th");
            }
            this.Path = path;
            this.Filename = filename;
        }
        #endregion

        #region private methods
        private void extractPath() {
            if (hasPath()) {
                this.Path = getPath();
                removePath();
            }
        }

        private bool hasPath() {
            return (getPathEndPosition() != -1);
        }
        private string getPath() {
            return this.filename.Substring(0, getPathEndPosition());
        }
        private void removePath() {
            if (hasPath())
                this.filename = this.filename.Substring(getPathEndPosition());
        }
        private int getPathEndPosition() {
            return this.filename.LastIndexOfAny(DIRECTORY_SEPERATORS);
        }

        private void extractAndRemoveExtension() {
            if (hasExtension()) {
                this.Extension = getExtension();
                removeExtension();
            }
        }

        private bool hasExtension() {
            return (getExtensionStartPosition() != -1);
        }
        private string getExtension() {
            return filename.Substring(getExtensionStartPosition());
        }
        private void removeExtension() {
            this.filename = this.filename.Substring(0, Math.Max(0, getExtensionStartPosition()-1));
        }
        private int getExtensionStartPosition() {
            return this.filename.LastIndexOf(EXTENSION_SEPERATOR)+1;
        }


        private bool isValidFilename(string filename) {
            if (filename.IndexOfAny(DIRECTORY_SEPERATORS) != -1) {
                return false;
            }
            return true;
        }

        private string handleNullValue(string value) {
            return (String.IsNullOrEmpty(value) ? "" : value);
        }

        private void cleanup() {
            makePathUniform();
            replaceInvalidCharsInFilename();
        }

        private void makePathUniform(){
            if (String.IsNullOrEmpty(this.path))
                return;
            
            this.replaceAltDirectorySeperator();
            path = path.TrimEnd(DIRECTORY_SEPERATORS);
            if (this.path.Length == 2)
            {
                this.trailingSlashCheck();
            }
            replaceDoubleSlashes();
        }
        private void replaceDoubleSlashes()
        {
            while (this.path.IndexOf(System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString()) >0)
            {
                this.path = this.path.Replace(System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString(), System.IO.Path.DirectorySeparatorChar.ToString());
            }
        }
        private void replaceInvalidCharsInPath() {
            this.path = replaceInvalidChars(this.path);
        }
        private void replaceInvalidCharsInFilename() {
            this.filename = replaceInvalidChars(this.filename);
        }
        private void replaceInvalidCharsInExtension() {
            this.filename = replaceInvalidChars(this.filename);
        }
        private string replaceInvalidChars(string str) {
            return Regex.Replace(str, invalidFileCharsPattern(), Helper.ReadProperty(Config.InvalidCharReplace));
        }

        private string invalidFileCharsPattern() {
            return "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
        }

        private void replaceAltDirectorySeperator() {
            this.path = this.path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }
        private void trailingSlashCheck() {
            if(!pathHasTrailingSlash())
                this.addTrailingSlash();
        }

        private bool pathHasTrailingSlash() {
            return this.path.Substring(this.path.Length - 1).Equals(System.IO.Path.DirectorySeparatorChar);
        }

        private void addTrailingSlash() {
            this.path = this.path + System.IO.Path.DirectorySeparatorChar;
        }

        #endregion
        #region Properties
        public string Path {
            get {
                return this.path;
            }
            set {
                value = handleNullValue(value);
                this.path = value;
                this.makePathUniform();
            }
        }

        public string Name {
            get {
                return this.filename;
            }
            set {
                value = handleNullValue(value);
                this.filename = value;
                replaceInvalidCharsInFilename();
            }
        }

        public string Extension {
            get {
                return this.extension;
            }
            set {
                value = handleNullValue(value);
                this.extension = value.ToLower();
                replaceInvalidCharsInExtension();
            }
        }

        public string Fullfilename {
            get {
                return this.Path + System.IO.Path.DirectorySeparatorChar + this.Filename;
            }
        }

        public string Filename {
            get {
                if (this.filename != "" && this.extension != "")
                {
                    return this.filename + EXTENSION_SEPERATOR + this.extension;
                }
                else if (this.filename != "" || this.extension != "")
                {
                    return "";
                }
                else
                {
                    return "";
                }
            }
            set {
                value = handleNullValue(value);
                this.filename = value;
                extractAndRemoveExtension();
                replaceInvalidCharsInFilename();
                replaceInvalidCharsInExtension();
            }
        }

        public string[] Folders {
            get {
                return this.path.Split(DIRECTORY_SEPERATORS);
            }
        }

        public bool isEmpty {
            get {
                return this.Fullfilename == "";
            }
        }

#endregion
        #region static helper functions
        public static string goUpwards(string directory, int howOften)
        {
            while (howOften > 0)
            {
                directory = Directory.GetParent(directory).FullName;
                howOften--;
            }
            return directory;
        }
        public static string goIntoFolder(string directory, string folder)
        {
            directory = directory.Trim(DIRECTORY_SEPERATORS);
            folder=folder.Trim(DIRECTORY_SEPERATORS);
            return directory + System.IO.Path.DirectorySeparatorChar + folder;
        }

        /// <summary>
        /// Splits a path into its folder names
        /// TODO: Why is the regex needed?
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] extractFoldernamesFromPath(string path)
        {
            string[] folders = path.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < folders.Length; i++)
            {
                folders[i] = Regex.Replace(folders[i], "\\((?<letter>\\w)\\)", "${letter}");//.Replace("(", "").Replace(")", "");
            }
            return folders;
        }

        /// <summary>
        /// Figures out if the directory is a extraction directory from an archive by looking at tags in the foldername
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsExtractionDirectory(string path)
        {
            string[] tags = Helper.ReadProperties(Config.Tags);
            string tag = "";
            foreach (string t in tags)
            {
                tag += "|\\." + t;
            }
            tag = tag.Substring(1);
            string[] dirs = extractFoldernamesFromPath(path);
            if (dirs.Length > 0 && Regex.IsMatch(dirs[dirs.Length - 1], tag))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// figures out if a folder is a subdir of another folder, and how deep it is nested
        /// </summary>
        /// <param name="basepath">the base path</param>
        /// <param name="PossibleSubdir">the path which is to be checked for subfolderity</param>
        /// <returns>-1 if not a subfolder, >=0 otherwise</returns>
        public static int GetSubdirectoryLevel(string basepath, string PossibleSubdir)
        {
            string[] basefolders = basepath.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            string[] possiblesubfolders = PossibleSubdir.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            if (basefolders.Length > possiblesubfolders.Length) return -1;
            bool match=true;
            for (int i = 0; i < Math.Min(basefolders.Length, possiblesubfolders.Length); i++)
            {
                if (basefolders[i] == possiblesubfolders[i])
                {
                    match = true;
                }
                else
                {
                    match = false;
                }
            }
            if (match)
            {
                return possiblesubfolders.Length - basefolders.Length;
            }
            else return -1;
        }
        #endregion

    }
}
