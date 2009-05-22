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
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using Renamer.Classes;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes.Configuration;
namespace Renamer
{
    /// <summary>
    /// Helper class offering all kinds of functions, config file caching, logging, helper functions ;)
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Type of log message
        /// </summary>
        public enum LogType : int { Error, Info, Warning, Status, Plain, Debug };

        /// <summary>
        /// where log message of specific type is to show up
        /// </summary>
        public enum LogLevel : int { None, LogFile, MessageBox, Log_and_Message };

        /// <summary>
        /// action to take when encountering invalid filename
        /// </summary>
        public enum InvalidFilenameAction : int { Ask, Skip, Replace };

        public enum Languages : int { None, German, English, French, Italian };

        /// <summary>
        /// Control to show log in
        /// </summary>
        public static Control LogDisplay;

        /// <summary>
        /// logs to a file and/or message box
        /// </summary>
        /// <param name="line">message to log</param>
        /// <param name="logtype">type of the message</param>
        public static void Log(string line, LogType logtype) {
            string message = "";
            //get current loglevel filter and add logtype to message
            LogLevel ll = LogLevel.None;
            Color type = Color.Black;
            if (logtype == LogType.Error) {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelError)); ;
                if (ll == LogLevel.None) return;
                message = "ERROR: ";
                type = Color.Red;
            }
            if (logtype == LogType.Info) {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelInfo)); ;
                if (ll == LogLevel.None) return;
                message = "INFO: ";
                type = Color.Green;
            }
            if (logtype == LogType.Warning) {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelWarning)); ;
                if (ll == LogLevel.None) return;
                message = "WARNING: ";
                type = Color.Yellow;
            }
            if (logtype == LogType.Status) {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelStatus)); ;
                if (ll == LogLevel.None) return;
                message = "STATUS: ";
            }
            if (logtype == LogType.Plain) {
                ll = LogLevel.LogFile;
                message = "LOG: ";
            }
            if (logtype == LogType.Debug) {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelDebug)); ;
                if (ll == LogLevel.None) return;
                message = "DEBUG: ";
            }

            //add actual message to message
            message += line;

            //and some output
            if (ll != LogLevel.MessageBox) {
                File.AppendAllText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Renamer.log", message + Environment.NewLine);
                if (LogDisplay != null) {
                    LogDisplay.Text = message + Environment.NewLine + LogDisplay.Text;
                }
                //richtextbox, additional colouring
                if (LogDisplay is RichTextBox) {
                    RichTextBox rtbLog = ((RichTextBox)LogDisplay);
                    int start = 0;
                    int end = 0;
                    while (true) {
                        end = rtbLog.Text.IndexOf("\n", start);
                        if (end < 0) {
                            end = rtbLog.Text.Length - 1;
                        }
                        bool found = false;
                        if (!found && rtbLog.Find("Error:", start, end, RichTextBoxFinds.None) == start) {
                            rtbLog.SelectionColor = Color.Red;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if (!found && rtbLog.Find("Warning:", start, end, RichTextBoxFinds.None) == start) {
                            rtbLog.SelectionColor = Color.Orange;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if (!found && rtbLog.Find("Info:", start, end, RichTextBoxFinds.None) == start) {
                            rtbLog.SelectionColor = Color.DarkGreen;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if (!found && rtbLog.Find("Status:", start, end, RichTextBoxFinds.None) == start) {
                            rtbLog.SelectionColor = Color.Black;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if (!found && rtbLog.Find("LOG:", start, end, RichTextBoxFinds.None) == start) {
                            rtbLog.SelectionColor = Color.Black;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Regular);
                            found = true;
                        }

                        if (!found && rtbLog.Find("DEBUG:", start, end, RichTextBoxFinds.None) == start) {
                            rtbLog.SelectionColor = Color.DarkGray;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }
                        if (found) {
                            rtbLog.SelectionStart = rtbLog.SelectionStart + rtbLog.SelectionLength;
                            rtbLog.SelectionLength = end - rtbLog.SelectionStart;
                            rtbLog.SelectionColor = Color.Black;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Regular);
                        }
                        start = end + 1;
                        if (end == rtbLog.Text.Length - 1) break;
                    }
                }
            }
            if (ll == LogLevel.Log_and_Message || ll == LogLevel.MessageBox) {
                MessageBox.Show(message);
            }
        }
        /// <summary>
        /// Deletes log file, only called at program start
        /// </summary>
        public static void ClearLog() {
            File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Helper.ReadProperty(Config.LogName));
        }



        /// <summary>
        /// returns true if str=="1" and catches exception
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StringToBool(string str) {
            return str == "1" || str == "true";
        }

        /// <summary>
        /// Checks if a string contains a series of letters, like hlo in hello
        /// </summary>
        /// <param name="letters">string which contains the letters which will be checked for in the other string</param>
        /// <param name="container">string in which the letters should be contained</param>
        /// <returns>true if container contains those letters, false otherwise</returns>
        public static bool ContainsLetters(string letters, string container) {
            int pos = 0;
            foreach (char c in letters) {
                bool found = false;
                for (int i = pos; i < container.Length; i++) {
                    if (char.ToLower(c) == char.ToLower(container[i])) {
                        pos = i;
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Capitalizes The String As In This Description
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string VISSpeak(string str) {
            str = str.ToLower();
            string[] arr = str.Split(new char[] { ' ' });
            string result = "";
            for (int i = 0; i < arr.Length; i++) {
                string s = arr[i];
                int firstAlphaIndex = 0;
                for (int j = 0; j < s.Length; j++) {
                    if (char.IsLetter(s[j])) {
                        firstAlphaIndex = j;
                        break;
                    }
                }
                if (s.Length > 0 && char.IsLower(s[firstAlphaIndex])) {
                    s = s.Substring(0, firstAlphaIndex) + char.ToUpper(s[firstAlphaIndex]) + s.Substring(firstAlphaIndex + 1);
                }
                result += s + " ";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }

        /// <summary>
        /// Is string a number?
        /// </summary>
        /// <param name="str">string to check</param>
        /// <returns>true if strig is numeric</returns>
        public static bool IsNumeric(string str) {
            double x;
            return Double.TryParse(str, out x);
        }

        /// <summary>
        /// Figures out if this is a movie file by looking at the destination path. Right now this only works
        /// if season subdirectories are used, as this check looks for the season directory folder in the destination
        /// path. Future versions might also check for similarity between the name of the file and the destination folder,
        /// since movie files are to be put in the same folder as their name (minus part identifiers, i.e. "CD1").
        /// </summary>
        /// <param name="ie">the file which is checked</param>
        /// <returns>true if ie is a movie file, false otherwise</returns>
        public static bool IsMovie(InfoEntry ie) {
            if (ie.Destination == "") return false;
            string[] patterns = Helper.ReadProperties(Config.Extract);
            for (int i = patterns.Length - 1; i >= 0; i--) {
                string seasondir = patterns[i].Replace("%E", "\\d*");
                seasondir = seasondir.Replace("%S", "\\d*");
                if (Regex.Match(ie.Destination, seasondir).Success) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Find similar files from subfolders
        /// </summary>
        /// <param name="source">source files</param>
        /// <param name="Basepath">base path to look for</param>
        /// <returns>a list of matches</returns>
        public static List<InfoEntry> FindSimilarByPath(List<InfoEntry> source, string Basepath) {
            List<InfoEntry> matches = new List<InfoEntry>();
            foreach (InfoEntry ie in source) {
                if (ie.Path.StartsWith(Basepath)) {
                    matches.Add(ie);
                }
            }
            return matches;
        }
        /// <summary>
        /// Finds similar files by looking at the filename and comparing it to a showname
        /// </summary>
        /// <param name="Basepath">basepath of the show</param>
        /// <param name="Showname">name of the show to filter</param>
        /// <param name="source">source files</param>
        /// <returns>a list of matches</returns>
        public static List<InfoEntry> FindSimilarByName(List<InfoEntry> source, string Showname) {
            List<InfoEntry> matches = new List<InfoEntry>();
            Showname = Showname.ToLower();
            //whatever, just check path and filename if it contains the showname
            foreach (InfoEntry ie in source) {
                string[] folders = ie.Path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                string processed = ie.Filename.ToLower();

                //try to extract the name from a shortcut, i.e. sga for Stargate Atlantis
                string pattern = "[^\\w]";
                Match m = Regex.Match(processed, pattern, RegexOptions.IgnoreCase);
                if (m != null && m.Success) {
                    string abbreviation = processed.Substring(0, m.Index);
                    if (abbreviation.Length > 0 && Helper.ContainsLetters(abbreviation, Showname)) {
                        matches.Add(ie);
                        continue;
                    }
                }

                //now check if whole showname is in the filename
                string CleanupRegex = Helper.ReadProperty(Config.CleanupRegex);
                processed = Regex.Replace(processed, CleanupRegex, " ");
                if (processed.Contains(Showname)) {
                    matches.Add(ie);
                    continue;
                }

                //or in some top folder
                foreach (string str in folders) {
                    processed = str.ToLower();
                    processed = Regex.Replace(processed, CleanupRegex, " ");
                    if (processed.Contains(Showname)) {
                        matches.Add(ie);
                        break;
                    }
                }
            }
            return matches;
        }
        public static int ReadInt(string Identifier) {
            string result = ReadProperty(Identifier);
            int value = -1;
            try {
                Int32.TryParse(result, out value);
            }
            catch (Exception) {
                Helper.Log("Couldn't parse property " + Identifier + " = " + result, Helper.LogType.Error);
            }
            return value;
        }

        /// <summary>
        /// Returns a variable as a string, adds a delemiter between fields of an array
        /// </summary>
        /// <param name="variable">variable to turn into a string</param>
        /// <returns></returns>
        private static string MakeConfigString(object variable) {
            if (variable == null) {
                return null;
            }
            //note: if this is an array really but this function is called, return it in one string form
            if (variable is string[]) {
                string value = "";
                foreach (string s in ((string[])variable)) {
                    value += s + Helper.ReadProperty(Config.Delimiter);
                }
                return value.Substring(0, value.Length - 1);
            }
            return (string)variable;
        }

        /// <summary>
        /// Returns a variable as a string array
        /// </summary>
        /// <param name="variable">variable to turn into a string array</param>
        /// <returns></returns>
        private static string[] MakeConfigStringArray(object variable) {
            if (variable == null) {
                return null;
            }
            //note: if this is an array really but this function is called, return it in one string form
            if (variable is string[]) {
                return (string[])variable;
            }
            return new string[] { (string)variable };
        }

        /// <summary>
        /// reads a property from cache or from a file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="FilePath">Path of the config file</param>
        /// <returns>value of the property, or null</returns>
        public static string ReadProperty(string Identifier, string FilePath) {
            Settings settings = Settings.getInstance();

            ConfigFile config = settings[FilePath];
            if (config == null) {
                return null;
            }
            return MakeConfigString(config[Identifier]);
        }

        /// <summary>
        /// reads a property from main config cache/file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <returns>value of the property, or null</returns>
        public static string ReadProperty(string Identifier) {
            return ReadProperty(Identifier, DefaultConfigFile());
        }

        /// <summary>
        /// generates the default filepath for the configuration file
        /// </summary>
        /// <returns>path to the configuration file</returns>
        public static string DefaultConfigFile() {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.MainConfigFileName;
        }

        /// <summary>
        /// reads a property that consists of more than one value from a file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="FilePath">Path of the config file</param>
        /// <returns>string[] Array containing values, or null</returns>
        public static string[] ReadProperties(string Identifier, string FilePath) {
            Settings settings = Settings.getInstance();

            ConfigFile config = settings[FilePath];
            if (config == null) {
                return null;
            }
            return MakeConfigStringArray(config[Identifier]);
        }

        /// <summary>
        /// reads a property that consists of more than one value from default config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <returns>string[] Array containing values, or null</returns>
        public static string[] ReadProperties(string Identifier) {
            return ReadProperties(Identifier, DefaultConfigFile());
        }

        /// <summary>
        /// writes a property to the cache
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">Value to write</param>
        /// <param name="FilePath">Path of the config file</param>
        public static void WriteProperty(string Identifier, string Value, string FilePath) {
            Settings settings = Settings.getInstance();

            ConfigFile config = settings[FilePath];
            if (config == null) {
                return;
            }
            config[Identifier] = Value;
        }

        /// <summary>
        /// writes a property to the main config cache
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">Value to write</param>
        public static void WriteProperty(string Identifier, string Value) {
            WriteProperty(Identifier, Value, DefaultConfigFile());
        }

        /// <summary>
        /// writes a property with more than one Value to a file,
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">string[] containing values to write</param>
        /// <param name="FilePath">Path of the config file</param>
        public static void WriteProperties(string Identifier, string[] Value, string FilePath) {
            Settings settings = Settings.getInstance();

            ConfigFile config = settings[FilePath];
            if (config == null) {
                return;
            }
            config[Identifier] = new List<string>(Value);
        }

        /// <summary>
        /// writes a property with more than one Value to default config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">string[] containing values to write</param>
        public static void WriteProperties(string Identifier, string[] Value) {
            WriteProperties(Identifier, Value, DefaultConfigFile());

        }

        /// <summary>
        /// writes a property with more than one Value to default config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">delimiter separated string of values</param>
        public static void WriteProperties(string Identifier, string Value) {
            WriteProperties(Identifier, Value, DefaultConfigFile());
        }

        /// <summary>
        /// writes a property with more than one Value to config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">delimiter separated string of values</param>
        /// /// <param name="FilePath">Path of the config file</param>
        public static void WriteProperties(string Identifier, string Value, string FilePath) {
            WriteProperties(Identifier, Value.Split(new string[] { Helper.ReadProperty(Config.Delimiter) }, StringSplitOptions.RemoveEmptyEntries), FilePath);
        }

        /// <summary>
        /// gets all files recursively from subdirectories using MaxDepth property
        /// </summary>
        /// <param name="directory">root folder</param>
        /// <param name="pattern">Pattern for file matching, "*" for all</param>
        /// <returns>List of FileSystemInfo classes from all matched files</returns>
        public static List<FileSystemInfo> GetAllFilesRecursively(string directory, string pattern) {
            DirectoryInfo dir = new DirectoryInfo(directory);
            return GetAllFilesRecursively(dir, pattern, 0);
        }

        /// <summary>
        /// internal recursive function for getting subdirectories
        /// </summary>
        /// <param name="dir">current recursive root folder</param>
        /// <param name="pattern">Pattern for file matching, "*" for all</param>
        /// <param name="depth">Current recursive depth for cancelling recursion</param>
        /// <returns>List of FileSystemInfo classes from all matched files</returns>
        private static List<FileSystemInfo> GetAllFilesRecursively(DirectoryInfo dir, string pattern, int depth) {
            List<FileSystemInfo> files;
            try {
                files = new List<FileSystemInfo>(dir.GetFileSystemInfos(pattern));
            }
            catch (Exception) {
                return null;
            }
            //remove directories? Why are they even there :(
            for (int i = 0; i < files.Count; i++) {
                if (Directory.Exists(files[i].FullName)) {
                    files.RemoveAt(i);
                }
            }
            List<FileSystemInfo> all = new List<FileSystemInfo>(dir.GetFileSystemInfos());
            if (depth >= Convert.ToInt32(Helper.ReadProperty(Config.MaxDepth))) return files;
            foreach (FileSystemInfo f in all) {
                if (f is DirectoryInfo) {
                    List<FileSystemInfo> deeperfiles = GetAllFilesRecursively((DirectoryInfo)f, pattern, depth + 1);
                    if (deeperfiles != null) {
                        files.AddRange(deeperfiles);
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// Obsolete function for computing a hash of a movie file for osdb project
        /// </summary>
        /// <param name="filename">File to compute hash from</param>
        /// <returns>byte[] containing hash</returns>
        public static byte[] ComputeMovieHash(string filename) {
            byte[] result;
            using (Stream input = File.OpenRead(filename)) {
                result = ComputeMovieHash(input);
            }
            return result;
        }

        /// <summary>
        /// Obsolete function for computing a hash of a movie file for osdb project
        /// </summary>
        /// <param name="input">stream to compute hash from</param>
        /// <returns>byte[] containing hash</returns>
        private static byte[] ComputeMovieHash(Stream input) {
            long lhash, streamsize;
            streamsize = input.Length;
            lhash = streamsize;

            long i = 0;
            byte[] buffer = new byte[sizeof(long)];
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0)) {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }

            input.Position = Math.Max(0, streamsize - 65536);
            i = 0;
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0)) {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }
            input.Close();
            byte[] result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }
    }
}