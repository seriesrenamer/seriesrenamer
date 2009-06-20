using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Logging;
using Renamer.Classes.Configuration.Keywords;
using System.IO;
using Schematrix;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;
using Renamer.Dialogs;
using System.Windows.Forms;
using System.Net;

namespace Renamer.Classes
{
    class SubtitleFileManager
    {
        #region Singleton
        protected static SubtitleFileManager instance;
        private static object m_lock = new object();

        public static SubtitleFileManager Instance {
            get {
                if (instance == null) {
                    lock (m_lock) { if (instance == null) instance = new SubtitleFileManager(); }
                }
                return instance;
            }
        }

        #endregion
        /// <summary>
        /// List of files, their target destinations etc
        /// </summary>
        protected List<SubtitleFile> subtitles = new List<SubtitleFile>();
        /// <summary>
        /// List of links to subtitles
        /// </summary>
        protected List<string> subtitleLinks = new List<string>();


        protected void unrar(string filename, string destination, List<string> extensions) {
            Unrar unrar = null;
            try {
                // Create new unrar class and attach event handlers for
                // progress, missing volumes, and password
                unrar = new Unrar();
                //AttachHandlers(unrar);

                // Set destination path for all files
                unrar.DestinationPath = destination;

                // Open archive for extraction
                unrar.Open(filename, Unrar.OpenMode.Extract);

                // Extract each file with subtitle extension
                while (unrar.ReadHeader()) {

                    string extension = Path.GetExtension(unrar.CurrentFile.FileName).Substring(1).ToLower().Replace(".", "");
                    if (extensions.Contains(extension)) {
                        unrar.Extract();
                    }
                    else {
                        unrar.Skip();
                    }
                }
            }
            catch (Exception ex) {
                Logger.Instance.LogMessage("Error during unpack of file: " + filename + " (" + ex.Message + ")", LogLevel.ERROR);
            }
            finally {
                if (unrar != null)
                    unrar.Close();
            }
        }

        protected void unzip(string filename, string folder, List<string> extensions) {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(filename))) {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null) {

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    string extension = Path.GetExtension(theEntry.Name);
                    extension = extension.Substring(Math.Min(extension.Length, 1)).ToLower().Replace(".", "");

                    //put it all in one dir!   
                    if (String.IsNullOrEmpty(fileName) || !extensions.Contains(extension)) {
                        continue;
                    }
                    using (FileStream streamWriter = File.Create(folder + Path.DirectorySeparatorChar + fileName)) {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true) {
                            size = s.Read(data, 0, data.Length);
                            if (size <= 0) {
                                break;
                            }
                            streamWriter.Write(data, 0, size);
                        }
                    }
                }
            }
        }

        protected void extractArchives(string folder, List<string> extensions) {
            //extract downloaded archives
            List<string> archives = new List<string>();
            archives.AddRange(Directory.GetFiles(folder, "*.zip"));
            archives.AddRange(Directory.GetFiles(folder, "*.rar"));
            if (archives.Count == 0) {
                return;
            }
            foreach (string file in archives) {
                if (Path.GetExtension(file).ToLower() == ".rar") {
                    unrar(file, folder, extensions);
                }
                else {
                    unzip(file, folder, extensions);
                }
            }
        }

        private int DownloadSubtitles() {
            //find empty temp dir
            int i = 0;
            string folder = Path.DirectorySeparatorChar + "TEMP" + i.ToString();
            while (Directory.Exists(Helper.ReadProperty(Config.LastDirectory) + folder)) {
                i++;
                folder = Path.DirectorySeparatorChar + "TEMP" + i.ToString();
            }
            Directory.CreateDirectory(Helper.ReadProperty(Config.LastDirectory) + folder);
            foreach (string url in this.subtitleLinks) {
                WebClient Client = new WebClient();
                string target = Helper.ReadProperty(Config.LastDirectory) + folder + Path.DirectorySeparatorChar + Path.GetFileName(url);
                Client.DownloadFile(url, target);
            }
            return i;
        }

        /// <summary>
        /// Extracts all downloaded archives and moves subtitles to the movie files with proper naming
        /// </summary>
        /// <param name="i">Index of the temporary directory in which subtitles are stored. Temp Directory Name is "TEMP"+i</param>
        public void ProcessSubtitles(int i) {
            string folder = Helper.ReadProperty(Config.LastDirectory) + "TEMP" + i.ToString();
            List<string> extensions = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions, true));
            if (extensions == null) {
                Logger.Instance.LogMessage("No Subtitle Extensions found!", LogLevel.WARNING);
                return;
            }

            if (Directory.Exists(folder)) {

                extractArchives(folder, extensions);
                //now that everything is extracted, try to assign subtitles to episodes                
                //first, figure out episode and season numbers from filenames

                //scan for subtitle files in temp folder             
                List<FileSystemInfo> Files = new List<FileSystemInfo>();
                foreach (string ex in extensions) {
                    List<FileSystemInfo> fsi = Helper.GetAllFilesRecursively(folder, "*." + ex);
                    Files.AddRange(fsi);
                }
                string[] patterns = Helper.ReadProperties(Config.EpIdentifier);
                foreach (FileSystemInfo file in Files) {
                    int Season = -1;
                    int Episode = -1;
                    foreach (string str in patterns) {
                        //replace %S and %E by proper regexps
                        string pattern = RegexConverter.toRegex(str);
                        Match m = Regex.Match(file.Name, pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                        if (m.Success) {

                            try {
                                Season = Int32.Parse(m.Groups["Season"].Value);
                            }
                            catch (FormatException) {
                                Logger.Instance.LogMessage("Cannot parse " + m.Groups["Season"].Value + " to an integer", LogLevel.WARNING);
                            }
                            try {
                                Episode = Int32.Parse(m.Groups["Episode"].Value);
                            }
                            catch (FormatException) {
                                Logger.Instance.LogMessage("Cannot parse " + m.Groups["Episode"].Value + " to an integer", LogLevel.WARNING);
                            }
                            break;
                        }
                    }

                    //now that season and episode are known, assign the filename to a SubtitleFile object
                    bool contains = false;
                    foreach (SubtitleFile s in this.subtitles) {
                        if (Season != -1 && Episode != -1 && s.Episode == Episode && s.Season == Season) {
                            s.Filenames.Add(file.Name);
                            contains = true;
                        }
                    }
                    if (!contains) {
                        SubtitleFile sf = new SubtitleFile();
                        sf.Episode = Episode;
                        sf.Season = Season;
                        sf.Filenames.Add(file.Name);
                        this.subtitles.Add(sf);
                    }
                }
                int MatchedSubtitles = 0;
                //Move subtitle files to their video files
                foreach (InfoEntry ie in InfoEntryManager.Instance) {
                    List<string> ext = new List<string>(Helper.ReadProperties(Config.Extensions));
                    for (int b = 0; b < ext.Count; b++) {
                        ext[b] = ext[b].ToLower();
                    }
                    if (ext.Contains(Path.GetExtension(ie.Filename).Substring(1).ToLower()) && ie.ProcessingRequested && ie.Episode != -1 && ie.Season != -1) {
                        foreach (SubtitleFile sf in this.subtitles) {
                            if (sf.Season == ie.Season && sf.Episode == ie.Episode) {
                                bool move = false;
                                string source = "";
                                string target = ie.Filepath + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(ie.Filename) + Path.GetExtension(sf.Filenames[0]);
                                if (sf.Filenames.Count == 1) {
                                    move = true;
                                    source = folder + Path.DirectorySeparatorChar + sf.Filenames[0];
                                }
                                else {
                                    FileSelector fs = new FileSelector(sf.Filenames);
                                    if (fs.ShowDialog() == DialogResult.OK) {
                                        move = true;
                                        source = folder + Path.DirectorySeparatorChar + sf.Filenames[fs.selection];
                                    }
                                }

                                if (File.Exists(target)) {
                                    if (MessageBox.Show(target + " already exists. Overwrite?", "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
                                        File.Delete(target);
                                    }
                                    else {
                                        move = false;
                                    }
                                }
                                if (move) {
                                    try {
                                        File.Copy(source, target);
                                        MatchedSubtitles++;
                                    }
                                    catch (Exception ex) {
                                        Logger.Instance.LogMessage(source + " --> " + target + ": " + ex.Message, LogLevel.ERROR);
                                    }
                                }
                            }
                        }
                    }
                }
                Logger.Instance.LogMessage("Downloaded " + Files.Count + " subtitles and matched " + MatchedSubtitles + " of them.", LogLevel.INFO);
                //cleanup
                this.subtitles.Clear();
                Directory.Delete(folder, true);
                //UpdateList(true);
            }
        }

        public void ClearLinks() {
            this.subtitleLinks.Clear();
        }

        internal void AddSubtitleLink(string link) {
            this.subtitleLinks.Add(link);
        }
    }
}
