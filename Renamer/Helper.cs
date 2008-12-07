using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
namespace Renamer
{
    /// <summary>
    /// Helper class offering all kinds of functions, config file caching, logging, helper functions ;)
    /// </summary>
    class Helper
    {
        /// <summary>
        /// Type of log message
        /// </summary>
        public enum LogType:int { Error, Info, Warning, Status, Plain, Debug };

        /// <summary>
        /// where log message of specific type is to show up
        /// </summary>
        public enum LogLevel:int { None, LogFile, MessageBox, Log_and_Message };

        /// <summary>
        /// action to take when encountering invalid filename
        /// </summary>
        public enum InvalidFilenameAction : int { Ask, Skip, Replace };

        /// <summary>
        /// what to do with Umlauts
        /// </summary>
        public enum UmlautAction:int{Ignore, Use, Dont_Use};

        /// <summary>
        /// what to do with casing of filenames
        /// </summary>
        public enum Case : int { Ignore, small, Large, CAPSLOCK };
        /// <summary>
        /// Control to show log in
        /// </summary>
        public static Control LogDisplay;

        /// <summary>
        /// logs to a file and/or message box
        /// </summary>
        /// <param name="line">message to log</param>
        /// <param name="logtype">type of the message</param>
        public static void Log(string line,LogType logtype){
            string message = "";
            //get current loglevel filter and add logtype to message
            LogLevel ll = LogLevel.None;
            Color type=Color.Black; 
            if (logtype == LogType.Error)
            {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelError)); ;
                if (ll == LogLevel.None) return;
                message = "ERROR: ";
                type=Color.Red;
            }
            if (logtype == LogType.Info)
            {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelInfo)); ;
                if (ll == LogLevel.None) return;
                message = "INFO: ";
                type=Color.Green;
            }
            if (logtype == LogType.Warning)
            {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelWarning)); ;
                if (ll == LogLevel.None) return;
                message = "WARNING: ";
                type=Color.Yellow;
            }
            if (logtype == LogType.Status)
            {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelStatus)); ;
                if (ll == LogLevel.None) return;
                message = "STATUS: ";
            }
            if(logtype == LogType.Plain)
            {
                ll=LogLevel.LogFile;
                message = "LOG: ";
            }
            if (logtype == LogType.Debug)
            {
                ll = (Helper.LogLevel)Enum.Parse(typeof(Helper.LogLevel), Helper.ReadProperty(Config.LogLevelDebug)); ;
                if (ll == LogLevel.None) return;
                message = "DEBUG: ";
            }

            //add actual message to message
            message+=line;

            //and some output
            if (ll != LogLevel.MessageBox)
            {
                File.AppendAllText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Renamer.log", message + Environment.NewLine);
                if (LogDisplay != null)
                {
                    LogDisplay.Text = message + Environment.NewLine + LogDisplay.Text;
                }
                //richtextbox, additional colouring
                if (LogDisplay is RichTextBox)
                {
                    RichTextBox rtbLog = ((RichTextBox)LogDisplay);
                    int start=0;
                    int end=0;
                    while (true)
                    {
                        end=rtbLog.Text.IndexOf("\n",start);
                        if (end < 0)
                        {
                            end = rtbLog.Text.Length - 1;
                        }
                        bool found = false;
                        if (!found && rtbLog.Find("Error:", start, end, RichTextBoxFinds.None) == start)
                        {
                            rtbLog.SelectionColor = Color.Red;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if(!found && rtbLog.Find("Warning:", start, end,RichTextBoxFinds.None)==start)
                        {
                            rtbLog.SelectionColor = Color.Orange;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if (!found && rtbLog.Find("Info:", start, end, RichTextBoxFinds.None) == start)
                        {
                            rtbLog.SelectionColor = Color.DarkGreen;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if (!found && rtbLog.Find("Status:", start, end, RichTextBoxFinds.None) == start)
                        {
                            rtbLog.SelectionColor = Color.Black;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }

                        if (!found && rtbLog.Find("LOG:", start, end, RichTextBoxFinds.None) == start)
                        {
                            rtbLog.SelectionColor = Color.Black;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Regular);
                            found = true;
                        }

                        if (!found && rtbLog.Find("DEBUG:", start, end, RichTextBoxFinds.None) == start)
                        {
                            rtbLog.SelectionColor = Color.DarkGray;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Bold);
                            found = true;
                        }
                        if (found)
                        {
                            rtbLog.SelectionStart = rtbLog.SelectionStart + rtbLog.SelectionLength;
                            rtbLog.SelectionLength = end - rtbLog.SelectionStart;
                            rtbLog.SelectionColor = Color.Black;
                            rtbLog.SelectionFont = new Font(rtbLog.SelectionFont, FontStyle.Regular);
                        }
                        start=end+1;
                        if (end==rtbLog.Text.Length-1) break;
                    }
                }
            }
            if (ll == LogLevel.Log_and_Message || ll== LogLevel.MessageBox)
            {
                MessageBox.Show(message);
            }
        }
        /// <summary>
        /// Deletes log file, only called at program start
        /// </summary>
        public static void ClearLog(){
            File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Helper.ReadProperty(Config.LogName));
        }

        /// <summary>
        /// returns true if str=="1" and catches exception
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StringToBool(string str)
        {
            return str == "1" || str == "true";
        }

        /// <summary>
        /// Capitalizes The String As In This Description
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string VISSpeak(string str)
        {
            str = str.ToLower();
            string[] arr = str.Split(new char[] { ' ' });
            string result = "";
            for (int i = 0; i < arr.Length; i++)
            {
                string s = arr[i];
                int firstAlphaIndex = 0;
                for (int j = 0; j < s.Length; j++)
                {
                    if (char.IsLetter(s[j]))
                    {
                        firstAlphaIndex = j;
                        break;
                    }
                }
                if (s.Length > 0 && char.IsLower(s[firstAlphaIndex]))
                {
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
        public static bool IsNumeric(string str)
        {
            double x;
            return Double.TryParse(str, out x);
        }

        public static int ReadInt(string Identifier)
        {
            string result = ReadProperty(Identifier);
            int value = -1;
            try
            {
                Int32.TryParse(result, out value);
            }
            catch (Exception)
            {
                Helper.Log("Couldn't parse property " + Identifier + " = " + result, Helper.LogType.Error);
            }
            return value;
        }


        /// <summary>
        /// reads a property from cache or from a file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="FilePath">Path of the config file</param>
        /// <returns>value of the property, or null</returns>
        public static string ReadProperty(string Identifier, string FilePath)
        {
            foreach(ConfigFile f in Settings.files){
                if(f.FilePath==FilePath){                    
                    if(f.variables.ContainsKey(Identifier)){
                        //note: if this is an array really but this function is called, return it in one string form
                        if (f.variables[Identifier] is List<string>)
                        {
                            string value = "";
                            foreach (string s in ((List<string>)f.variables[Identifier]))
                            {
                                value += s + Helper.ReadProperty(Config.Delimiter);
                            }
                            return value.Substring(0, Math.Max(value.Length - 1,0));
                        }
                        else if (f.variables[Identifier] is string[])
                        {
                            string value = "";
                            foreach (string s in ((string[])f.variables[Identifier]))
                            {
                                value += s + Helper.ReadProperty(Config.Delimiter);
                            }
                            return value.Substring(0, value.Length - 1);
                        }
                        return (string)f.variables[Identifier];
                    }else{
                        if (Settings.Defaults.variables.ContainsKey(Identifier))
                        {
                            f.variables.Add(Identifier, Settings.Defaults.variables[Identifier]);
                            return (string)Settings.Defaults.variables[Identifier];
                        }
                        else
                        {
                            Helper.Log("Couldn't find property " + Identifier + " in " + FilePath, LogType.Error);
                            return null;
                        }
                    }
                }
            }
            if(Settings.MonoCompatibilityMode)
                Helper.Log(FilePath + " not loaded yet", Helper.LogType.Plain);
            //not found yet, lets load it
            ConfigFile file=new ConfigFile(FilePath);
            Settings.files.Add(file);
            if (file.variables.ContainsKey(Identifier))
            {
                return file.variables[Identifier].ToString();
            }else{
                if (Settings.Defaults.variables.ContainsKey(Identifier))
                {
                    file.variables.Add(Identifier, Settings.Defaults.variables[Identifier]);
                    Helper.Log("Couldn't find property " + Identifier + " in " + FilePath + ", using default one", LogType.Info);
                    //note: if this is an array really but this function is called, return it in one string form
                    if (Settings.Defaults.variables[Identifier] is List<string>)
                    {
                        string value = "";
                        foreach (string s in ((List<string>)Settings.Defaults.variables[Identifier]))
                        {
                            value += s + Helper.ReadProperty(Config.Delimiter);
                        }
                        return value.Substring(0, value.Length - 1);
                    }
                    else if (Settings.Defaults.variables[Identifier] is string[])
                    {
                        string value = "";
                        foreach (string s in ((string[])Settings.Defaults.variables[Identifier]))
                        {
                            value += s + Helper.ReadProperty(Config.Delimiter);
                        }
                        return value.Substring(0, value.Length - 1);
                    }
                    return (string)Settings.Defaults.variables[Identifier];
                }
                else
                {
                    Helper.Log("Couldn't find property " + Identifier + " in " + FilePath, LogType.Error);
                    return null;
                }
            }
        }

        /// <summary>
        /// reads a property from main config cache/file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <returns>value of the property, or null</returns>
        public static string ReadProperty(string Identifier)
        {
            return ReadProperty(Identifier, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName);
        }
            
        /// <summary>
        /// reads a property that consists of more than one value from a file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="FilePath">Path of the config file</param>
        /// <returns>string[] Array containing values, or null</returns>
        public static string[] ReadProperties(string Identifier, string FilePath)
        {
            foreach(ConfigFile f in Settings.files){
                if(f.FilePath==FilePath){
                    if(f.variables.ContainsKey(Identifier)){
                        if(f.variables[Identifier] is List<string>){
                            List<string> a = (List<string>)(f.variables[Identifier]);
                            return a.ToArray();
                        }else{
                            return new string[] { (string)f.variables[Identifier] };
                        }
                    }else{
                        if (Settings.Defaults.variables.ContainsKey(Identifier))
                        {
                            f.variables.Add(Identifier, Settings.Defaults.variables[Identifier]);
                            Helper.Log("Couldn't find property " + Identifier + " in " + FilePath + ", using default one", LogType.Info);
                            if (Settings.Defaults.variables[Identifier] is List<string>)
                            {
                                List<string> a = (List<string>)(Settings.Defaults.variables[Identifier]);
                                return a.ToArray();
                            }
                            else
                            {
                                return new string[] { (string)Settings.Defaults.variables[Identifier] };
                            }
                        }
                        else
                        {
                            Helper.Log("Couldn't find property " + Identifier + " in " + FilePath, LogType.Error);
                            return null;
                        }
                    }
                }
            }
            //not found yet, lets load it
            ConfigFile file=new ConfigFile(FilePath);
            Settings.files.Add(file);
            if (file.variables.ContainsKey(Identifier))
            {
                if (file.variables[Identifier] is List<string>)
                {
                    List<string> a = (List<string>)(file.variables[Identifier]);
                    return a.ToArray();
                }else{
                    return new string[] { (string)file.variables[Identifier] };
                }
            }else{
                if (Settings.Defaults.variables.ContainsKey(Identifier))
                {
                    file.variables.Add(Identifier, Settings.Defaults.variables[Identifier]);
                    Helper.Log("Couldn't find property " + Identifier + " in " + FilePath + ", using default one", LogType.Info);
                    if (Settings.Defaults.variables[Identifier] is List<string>)
                    {
                        List<string> a = (List<string>)(Settings.Defaults.variables[Identifier]);
                        return a.ToArray();
                    }
                    else
                    {
                        return new string[] { (string)Settings.Defaults.variables[Identifier] };
                    }
                }
                else
                {
                    Helper.Log("Couldn't find property " + Identifier + " in " + FilePath, LogType.Error);
                    return null;
                }
            }
        }
        
        /// <summary>
        /// reads a property that consists of more than one value from default config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <returns>string[] Array containing values, or null</returns>
        public static string[] ReadProperties(string Identifier)
        {
            return ReadProperties(Identifier, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName);
        }

        /// <summary>
        /// writes a property to the cache
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">Value to write</param>
        /// <param name="FilePath">Path of the config file</param>
        public static void WriteProperty(string Identifier, string Value, string FilePath)
        {
            foreach(ConfigFile f in Settings.files){
                if(f.FilePath==FilePath){
                    if (f.variables.ContainsKey(Identifier))
                    {
                        f.variables[Identifier] = Value;
                    }
                    else
                    {
                        f.variables.Add(Identifier, Value);
                    }
                    f.NeedsFlush = true;
                    return;
                }
            }

            //not found yet, lets load it
            ConfigFile file=new ConfigFile(FilePath);
            Settings.files.Add(file);
            if(file.variables.ContainsKey(Identifier)){
                file.variables[Identifier]=Value;
            }else{
                file.variables.Add(Identifier,Value);
            }            
            file.NeedsFlush=true;
        }

        /// <summary>
        /// writes a property to the main config cache
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">Value to write</param>
        public static void WriteProperty(string Identifier, string Value)
        {
            WriteProperty(Identifier, Value, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName);
        }

        /// <summary>
        /// writes a property with more than one Value to a file,
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">string[] containing values to write</param>
        /// <param name="FilePath">Path of the config file</param>
        public static void WriteProperties(string Identifier, string[] Value, string FilePath)
        {
            foreach(ConfigFile f in Settings.files){
                if(f.FilePath==FilePath){
                    if(f.variables.ContainsKey(Identifier)){
                        f.variables[Identifier]=new List<string>(Value);
                    }else{
                        f.variables.Add(Identifier,new List<string>(Value));
                    }
                    f.NeedsFlush = true;
                    return;
                }
            }
            //not found yet, lets load it
            ConfigFile file=new ConfigFile(FilePath);
            Settings.files.Add(file);
            if(file.variables.ContainsKey(Identifier)){
                file.variables[Identifier]=new List<string>(Value);
            }else{
                file.variables.Add(Identifier,new List<string>(Value));
            }
            file.NeedsFlush=true;
        }

        /// <summary>
        /// writes a property with more than one Value to default config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">string[] containing values to write</param>
        public static void WriteProperties(string Identifier, string[] Value)
        {
            WriteProperties(Identifier, Value, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName);
            
        }
        
        /// <summary>
        /// writes a property with more than one Value to default config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">delimiter separated string of values</param>
        public static void WriteProperties(string Identifier, string Value)
        {
            WriteProperties(Identifier, Value, Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName);
        }

        /// <summary>
        /// writes a property with more than one Value to config file
        /// </summary>
        /// <param name="Identifier">Name of the property</param>
        /// <param name="Value">delimiter separated string of values</param>
        /// /// <param name="FilePath">Path of the config file</param>
        public static void WriteProperties(string Identifier, string Value, string FilePath)
        {
            WriteProperties(Identifier, Value.Split(new string[] { Helper.ReadProperty(Config.Delimiter) }, StringSplitOptions.RemoveEmptyEntries), FilePath);
        }

        /// <summary>
        /// gets all files recursively from subdirectories using MaxDepth property
        /// </summary>
        /// <param name="directory">root folder</param>
        /// <param name="pattern">Pattern for file matching, "*" for all</param>
        /// <returns>List of FileSystemInfo classes from all matched files</returns>
        public static List<FileSystemInfo> GetAllFilesRecursively(string directory, string pattern)
        {
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
        private static List<FileSystemInfo> GetAllFilesRecursively(DirectoryInfo dir, string pattern, int depth)
        {
            List<FileSystemInfo> files;
            try
            {
                files = new List<FileSystemInfo>(dir.GetFileSystemInfos(pattern));
            }
            catch (Exception)
            {
                return null;
            }
            //remove directories? Why are they even there :(
            for (int i = 0; i < files.Count; i++)
            {
                if (Directory.Exists(files[i].FullName))
                {
                    files.RemoveAt(i);
                }
            }
            List<FileSystemInfo> all = new List<FileSystemInfo>(dir.GetFileSystemInfos());
            if (depth >= Convert.ToInt32(Helper.ReadProperty(Config.MaxDepth))) return files;
            foreach (FileSystemInfo f in all)
            {
                if (f is DirectoryInfo)
                {
                    List<FileSystemInfo> deeperfiles = GetAllFilesRecursively((DirectoryInfo)f, pattern, depth + 1);
                    if(deeperfiles!=null){
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
        public static byte[] ComputeMovieHash(string filename)
        {
            byte[] result;
            using (Stream input = File.OpenRead(filename))
            {
                result = ComputeMovieHash(input);
            }
            return result;
        }

        /// <summary>
        /// Obsolete function for computing a hash of a movie file for osdb project
        /// </summary>
        /// <param name="input">stream to compute hash from</param>
        /// <returns>byte[] containing hash</returns>
        private static byte[] ComputeMovieHash(Stream input)
        {
            long lhash, streamsize;
            streamsize = input.Length;
            lhash = streamsize;

            long i = 0;
            byte[] buffer = new byte[sizeof(long)];
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }

            input.Position = Math.Max(0, streamsize - 65536);
            i = 0;
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }
            input.Close();
            byte[] result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }
    }

    /// <summary>
    /// Config file used as cache
    /// </summary>
    class ConfigFile
    {
        /// <summary>
        /// Path of the file, is empty for internal defaults
        /// </summary>
        public string FilePath = "";

        /// <summary>
        /// Hashtable containing variables with Identifiers
        /// </summary>
        public Hashtable variables = new Hashtable();

        /// <summary>
        /// If set, cache has changed and file needs to be saved
        /// </summary>
        public bool NeedsFlush = false;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filepath">Path of the file, for manual file creation, leave empty here</param>
        public ConfigFile(string filepath){
            FilePath=filepath;
            if (filepath != null && filepath != "")
            {
                bool loop = true;
                while (loop)
                {
                    loop = false;
                    try
                    {
                        FileStream s = File.Open(filepath, FileMode.OpenOrCreate);
                        StreamReader r = new StreamReader(s);
                        string line = null;
                        string file = r.ReadToEnd();
                        List<string> lines = new List<string>(file.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                        for (int i = 0; i < lines.Count; i++)
                        {
                            line = lines[i];
                            if(Settings.MonoCompatibilityMode)
                                Helper.Log(line, Helper.LogType.Plain);
                            
                            //if delimiter and comment characters aren't known yet, try to find those first
                            if (Settings.Comment != null && Settings.Comment != "" && Settings.Delimiter != Convert.ToChar(0))
                            {
                                if(Settings.MonoCompatibilityMode)
                                    Helper.Log("comment and delimiter string known", Helper.LogType.Plain);
                                
                                if (line.IndexOf("=") > 0)
                                {
                                    int index = line.IndexOf(Settings.Comment);
                                    if (index == 0)
                                    {
                                        index = Settings.Comment.Length;
                                    }
                                    else
                                    {
                                        index = 0;
                                    }
                                    string key = line.Substring(index, line.IndexOf("=") - index);
                                    if (Settings.MonoCompatibilityMode)
                                        Helper.Log("Found identifier " + key, Helper.LogType.Plain);
                                   
                                    line = line.Replace(key + "=", "").Trim();
                                    List<string> split = new List<string>(line.Split(new char[] { Settings.Delimiter }, StringSplitOptions.RemoveEmptyEntries));
                                    //check so we don't add delimiter twice
                                    if (!variables.ContainsKey(key))
                                    {
                                        if (split.Count > 0 && split[0].StartsWith(Settings.Comment.ToString()))
                                        {
                                            variables.Add(key, null);
                                        }
                                        else
                                        {
                                            if (split.Count == 0)
                                            {
                                                variables.Add(key, "");
                                            }
                                            else if (split.Count == 1)
                                            {
                                                variables.Add(key, split[0]);
                                            }
                                            else
                                            {
                                                variables.Add(key, split);
                                            }
                                        }
                                    }
                                }
                                //if no comment character set yet, look for that first
                            }
                            else
                            {
                                if (Settings.MonoCompatibilityMode)
                                    Helper.Log("comment/delimiter not known yet", Helper.LogType.Plain);
                                if ((Settings.Comment == null || Settings.Comment == "") && line.StartsWith("Comment="))
                                {
                                    if (Settings.MonoCompatibilityMode)
                                        Helper.Log("Found comment identifier", Helper.LogType.Plain);
                                    Settings.Comment = line.Replace("Comment=", "").Trim();
                                    if (Settings.Comment != null && Settings.Comment != "")
                                    {
                                        variables.Add("Comment", Settings.Comment);
                                        //restart!
                                        i = -1;
                                        continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (Settings.Delimiter == Convert.ToChar(0) && line.StartsWith("Delimiter="))
                                {
                                    if (Settings.MonoCompatibilityMode)
                                        Helper.Log("Found delimiter identifier", Helper.LogType.Plain);
                                    string delim = line.Replace("Delimiter=", "").Trim();
                                    if (delim != null && delim != "")
                                    {
                                        Settings.Delimiter = delim[0];
                                        variables.Add("Delimiter", delim[0].ToString());
                                        //restart!
                                        i = -1;
                                        continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        r.Close();
                        //if no comment character is set by now, we couldn't have read anything from this file
                        if (Settings.Comment == null || Settings.Comment == "")
                        {
                            variables.Clear();
                            variables.Add(Config.Delimiter, Settings.Defaults.variables[Config.Delimiter]);
                            variables.Add(Config.Comment, Settings.Defaults.variables[Config.Comment]);
                            Settings.Comment = ((string)Settings.Defaults.variables[Config.Comment]);
                            Settings.Delimiter = ((string)Settings.Defaults.variables[Config.Delimiter])[0];
                            loop = true;
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        //possible recursion, fixed by creating config manually from defaults if not existant
                        variables = ((Hashtable)Settings.Defaults.variables.Clone());
                        Flush();
                        Helper.Log("Couldn't process config file " + filepath + ":" + ex.Message + " ...Using default values", Helper.LogType.Error);
                    }
                }
            }
        }
                
        /// <summary>
        /// Saves the file
        /// </summary>
        public void Flush()
        {
            if (NeedsFlush)
            {
                FileStream s = File.Open(FilePath, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader r = new StreamReader(s);
                List<string> lines = new List<string>(r.ReadToEnd().Split(new string[] { System.Environment.NewLine },StringSplitOptions.None));
                string file = "";
                bool NewVar = false;
                foreach (object key in variables.Keys)
                {
                    bool found = false;
                    string Identifier = key.ToString();
                    for (int i = 0; i < lines.Count; i++)
                    {
                        string line = lines[i];
                        if (line.StartsWith(Identifier + "=") || line.StartsWith(Settings.Comment + Identifier + "="))
                        {
                            found = true;
                            //no Value set, comment the set Value out if it is not already
                            if (variables[key] == null)
                            {
                                //stupid trim function not accepting strings :(
                                foreach (char c in Settings.Comment)
                                {
                                    line = line.TrimStart(new char[] { c });
                                }
                                lines[i] = Settings.Comment + line;
                                break;
                            }
                            line = Identifier + "=";
                            if (variables[key] is string)
                            {
                                line += (string)variables[key];
                            }
                            else if (variables[key] is List<string>)
                            {
                                foreach (string str in (List<string>)variables[key])
                                {
                                    line += str + Settings.Delimiter;
                                }
                                line = line.TrimEnd(new char[] { Settings.Delimiter });
                            }
                            lines[i] = line;
                            break;
                        }
                    }
                    //if property is not found at all, create a new entry at the end of the file
                    if (!found)
                    {
                        NewVar = true;
                    }

                    
                }
                //add new variables to end of file (extra loop so they don't get added in between)
                if (NewVar)
                {
                    foreach (object key in variables.Keys)
                    {
                        bool found = false;
                        string Identifier = key.ToString();
                        for (int i = 0; i < lines.Count; i++)
                        {
                            string line = lines[i];
                            if (line.StartsWith(Identifier + "=") || line.StartsWith(Settings.Comment + Identifier + "="))
                            {
                                found = true;
                                break;
                            }
                        }
                        //if property is not found at all, create a new entry at the end of the file
                        if (!found)
                        {
                            file += Identifier + "=";
                            if (variables[key] is string)
                            {
                                file += (string)variables[key] + Environment.NewLine;
                            }
                            else if (variables[key] is List<string>)
                            {
                                foreach (string str in (List<string>)variables[key])
                                {
                                    file += str + Settings.Delimiter;
                                }
                                file = file.TrimEnd(new char[] { Settings.Delimiter });
                                file += Environment.NewLine;
                            }
                        }
                    }
                }
                //create file from lines
                foreach (string l in lines)
                {
                    file += l + Environment.NewLine;
                }
                for (int i = Environment.NewLine.Length - 1; i >= 0; i--)
                {
                    file = file.TrimEnd(new char[] { Environment.NewLine[i] });
                }
                
                r.Close();
                File.Delete(FilePath);
                s = File.Open(FilePath, FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(s);
                w.Write(file);
                w.Close();
            }
            NeedsFlush = false;
        }
    }
    
}