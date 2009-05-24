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
using System.Collections;
using System.IO;
using Renamer.Classes.Configuration.Keywords;
using System.Diagnostics;
using Renamer.Logging;

namespace Renamer.Classes.Configuration
{
    /// <summary>
    /// Config file used as cache
    /// </summary>
    class ConfigFile : IEnumerable
    {
        /// <summary>
        /// Path of the file, is empty for internal defaults
        /// </summary>
        public string FilePath = "";

        /// <summary>
        /// Hashtable containing variables with Identifiers<br/>
        /// Contains either a String or a list of Strings
        /// </summary>
        private Hashtable variables = new Hashtable();
        /// <summary>
        /// Hashtable containing variables with Identifiers<br/>
        /// Contains either a String or a list of Strings
        /// </summary>
        private Hashtable originalVariables = new Hashtable();

        /// <summary>
        /// If set, cache has changed and file needs to be saved
        /// </summary>
        private bool needsFlush = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filepath">Path of the file, for manual file creation, leave empty here</param>
        public ConfigFile(string filepath) {
            FilePath = filepath;
            if (filepath == null || filepath == "") {
                return;
            }
            ConfigFileParser parser = new ConfigFileParser(this);
            parser.readConfigFile(filepath);
        }

        /// <summary>
        /// Parser for configurationfiles
        /// </summary>
        private class ConfigFileParser
        {
            private enum ParserState : int
            {
                Error,
                Comment,
                MultiValueField,
                NormalLine
            };

            private ParserState lastState;
            private Settings settings;
            private string line;
            private int lineCounter;
            private string currentKey;
            private List<string> currentValues;
            private ConfigFile config;

            /// <summary>
            /// Creates a new ConfigFileParser object for a given config
            /// </summary>
            /// <param name="config">ConfigFile the parser should store the data of the config file.</param>
            public ConfigFileParser(ConfigFile config) {
                this.settings = Settings.getInstance();
                line = null;
                lineCounter = 0;
                this.config = config;
            }

            /// <summary>
            /// Parse the given configuration file and stores the data to the config
            /// </summary>
            /// <param name="filepath"></param>
            public void readConfigFile(string filepath) {
                lastState = ParserState.NormalLine;
                FileStream s = null;
                StreamReader r = null;
                try {
                    s = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Read);
                    r = new StreamReader(s);
                    while ((line = r.ReadLine()) != null) {
                        //Remove leading and trailing whitespaces
                        line = line.Trim();
                        lineCounter++;
                        // Skip empty lines
                        if (String.IsNullOrEmpty(line)) {
                            continue;
                        }
                        //skipping comments
                        if (lastState != ParserState.MultiValueField && line.StartsWith(settings.Comment)) {
                            if (lastState != ParserState.Comment) {
                                //FIX Cannot log here, because this will cause a endless loop of calls to myself 
                                //Logger.Instance.LogMessage("Skipping comment", LogLevel.DEBUG);
                                lastState = ParserState.Comment;
                            }
                            continue;
                        }
                        // remove comments at the end of a line
                        int linePosition;
                        string valuePartOfLine;
                        if (lastState != ParserState.MultiValueField) {
                            if ((linePosition = line.IndexOf("=")) > 0) {
                                // extract key from line
                                currentKey = line.Substring(0, linePosition).Trim();
                                valuePartOfLine = line.Substring(linePosition + 1).Trim();
                                // continue if a multivalue field is found
                                if (valuePartOfLine == settings.BeginMultiValueField) {
                                    this.lastState = ParserState.MultiValueField;
                                    this.currentValues = new List<string>();
                                    continue;
                                }
                                if (String.IsNullOrEmpty(valuePartOfLine)) {
                                    //Logger.Instance.LogMessage("Option " + currentKey + " is empty", LogLevel.DEBUG);
                                }

                                config[currentKey] = valuePartOfLine;
                                this.lastState = ParserState.NormalLine;
                            }
                            else {
                                //Logger.Instance.LogMessage("The configfile seems to be corrupt (at line " + lineCounter + "), I'll try to ignore this", LogLevel.WARNING);
                            }
                        }
                        else /* Multi Value Field */ {
                            if (line == settings.EndMultiValueField) {
                                config[currentKey] = currentValues;
                                this.lastState = ParserState.NormalLine;
                                continue;
                            }
                            currentValues.Add(line);
                        }

                    }
                }
                catch (Exception ex) {
                    config.LoadDefaults();
                    config.Flush();
                    Logger.Instance.LogMessage("Couldn't process config file " + filepath + ":" + ex.Message, LogLevel.ERROR);
                    Logger.Instance.LogMessage("Using default values", LogLevel.WARNING);
                }
                finally {
                    if (s != null) {
                        s.Close();
                    }
                    if (r != null) {
                        r.Close();
                    }
                }
            }

            /// <summary>
            /// removes trailing comments on a line
            /// Some special handling is needed here, e.g. "http://"
            /// </summary>
            /// <returns>line without comments</returns>
            private string removeComment() {
                int commentStart = line.IndexOf(settings.Comment);
                if (commentStart == -1 || (line.IndexOf("http://") == commentStart - 5)) {
                    return line;
                }
                return line.Substring(0, line.IndexOf(settings.Comment));
            }
        }


        /// <summary>
        /// Stores a ConfigFile in a File
        /// </summary>
        private class ConfigFileWriter
        {

            private enum ParserState : int
            {
                Error,
                Comment,
                MultiValueField,
                NormalLine
            };

            private ParserState lastState;
            private Settings settings;
            private string line;
            private int lineCounter;
            private string currentKey;
            private ConfigFile config;
            private List<string> writtenProperties;
            StreamWriter fileWriter;

            /// <summary>
            /// Creates a new ConfigFileWriter object for a given config
            /// </summary>
            /// <param name="config">ConfigFile the writer should read the data from.</param>
            public ConfigFileWriter(ConfigFile config) {
                this.settings = Settings.getInstance();
                this.line = null;
                this.lineCounter = 0;
                this.config = config;
                this.writtenProperties = new List<string>(config.Count);
                this.fileWriter = null;
            }


            /// <summary>
            /// Writes the <see cref="config"/> to the given path.
            /// </summary>
            /// <param name="filepath">Path where config file should be stored to.</param>
            public void writeConfigFile(string filepath) {
                lastState = ParserState.NormalLine;
                FileStream writeStream = null;
                try {
                    FileStream fileStream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamReader fileReader = new StreamReader(fileStream);
                    writeStream = File.Open(filepath + ".tmp", FileMode.Create, FileAccess.ReadWrite);
                    fileWriter = new StreamWriter(writeStream);
                    fileWriter.AutoFlush = true;
                    while ((line = fileReader.ReadLine()) != null) {
                        //Remove leading and trailing whitespaces
                        line = line.Trim();
                        lineCounter++;
                        // Skip empty lines
                        if (String.IsNullOrEmpty(line)) {
                            continue;
                        }
                        // redirect comments
                        if (lastState != ParserState.MultiValueField && line.StartsWith(settings.Comment)) {
                            fileWriter.WriteLine(line);
                            lastState = ParserState.Comment;
                            continue;
                        }
                        // remove comments at the end of a line
                        line = this.removeComment();
                        int linePosition;
                        if (lastState != ParserState.MultiValueField) {
                            if ((linePosition = line.IndexOf("=")) > 0) {
                                // extract key from line
                                currentKey = line.Substring(0, linePosition).Trim();
                                writeVariable();
                            }
                        }
                        else /* Multi Value Field */ {
                            if (line == settings.EndMultiValueField) {
                                this.lastState = ParserState.NormalLine;
                            }
                        }
                    }
                    // check for unwritten properties, added to the config
                    // TODO: maybe replace with non linear search for better performance
                    if (this.writtenProperties.Count < this.config.Count) {
                        foreach (string key in this.config) {
                            if (!this.writtenProperties.Contains(key)) {
                                currentKey = key;
                                writeVariable();
                            }
                        }
                    }

                    fileStream.Close();
                    File.Delete(filepath);
                    writeStream.Close();
                    File.Move(filepath + ".tmp", filepath);
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage("Couldn't write config file " + filepath + "\nFehler:\n" + ex.Message, LogLevel.ERROR);
                }
                finally {
                    if (writeStream != null) {
                        writeStream.Close();
                        File.Delete(filepath + ".tmp");
                    }
                }
            }

            /// <summary>
            /// Writes variable with the <see cref="currentKey"/> to the <see cref="fileWriter"/>
            /// </summary>
            private void writeVariable() {
                line = currentKey + "=";
                if (config.containsVariable(currentKey)) {

                    this.writtenProperties.Add(currentKey);

                    if (config[currentKey] is string[]) {
                        this.lastState = ParserState.MultiValueField;
                        line += settings.BeginMultiValueField;
                        fileWriter.WriteLine(line);

                        //recoverEasyRegex((string[])config[currentKey]));
                        foreach (string val in ((string[])config[currentKey])) {
                            fileWriter.WriteLine("\t" + val);
                        }
                        line = settings.EndMultiValueField;
                    }
                    else {
                        line += (string)config[currentKey];
                        this.lastState = ParserState.NormalLine;
                    }
                }

                fileWriter.WriteLine(line);
                fileWriter.WriteLine();
            }

            /// <summary>
            /// removes trailing comments on a line
            /// </summary>
            /// <returns>line without comments</returns>
            private string removeComment() {
                int commentStart = line.IndexOf(settings.Comment);
                if (commentStart == -1) {
                    return line;
                }
                return line.Substring(0, line.IndexOf(settings.Comment));
            }
        }

        /// <summary>
        /// Saves the file
        /// </summary>
        public void Flush() {
            if (NeedsFlush) {
                this.cleanUpVariables();
                ConfigFileWriter configWriter = new ConfigFileWriter(this);
                configWriter.writeConfigFile(FilePath);
                needsFlush = false;
            }
        }

        /// <summary>
        /// Check wether a variable is available in the config or not.
        /// </summary>
        /// <param name="name">Name of the variable the config should be checked for.</param>
        /// <returns>True if variable is availabe, false otherwise.</returns>
        public bool containsVariable(string name) {
            return this.variables.ContainsKey(name);
        }

        /// <summary>
        /// Add a variable to the config.
        /// <b>It's prefered to use the index operator to add variables</b>
        /// </summary>
        /// <param name="key">Key for the variable.</param>
        /// <param name="value">The variable itself.</param>
        private void addVariable(string key, object value) {
            if (value is string[]) {
                this.originalVariables.Add(key, ((string[])value).Clone());
            }
            else {
                this.originalVariables.Add(key, ((string)value).Clone());
            }
            this.variables.Add(key, value);
            this.needsFlush = true;
        }

        /// <summary>
        /// Assigns a new value to a already existing variable and check if a flush will be needed after poerfoming this operation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void setVariable(string key, object value) {
            if (this.variables[key] != value) {
                this.variables[key] = value;
                this.needsFlush = true;
            }
        }

        /// <summary>
        /// IndexOperator for accessing variables directly with the key
        /// </summary>
        /// <param name="key">Key of the variable.</param>
        /// <returns>The variable</returns>
        public object this[string key] {
            get {
                Settings settings = Settings.getInstance();
                if (this.containsVariable(key)) {
                    return this.variables[key];
                }
                if (settings.Defaults.containsVariable(key)) {
                    this[key] = settings.Defaults[key];
                    return this[key];
                }
                else {
                    Logger.Instance.LogMessage("Couldn't find property " + key + " in " + FilePath, LogLevel.ERROR);
                    return null;
                }
            }
            set {

                if (value is List<string>) {
                    value = ((List<string>)value).ToArray();
                }

                if (this.containsVariable(key)) {
                    this.setVariable(key, value);
                }
                else {

                    this.addVariable(key, value);
                }
            }
        }
        /// <summary>
        /// Return true if the file should be flushed/written.
        /// </summary>
        public bool NeedsFlush {
            get {
                return this.needsFlush;
            }
        }

        /// <summary>
        /// Loads default settings to this config file
        /// </summary>
        public void LoadDefaults() {
            Settings settings = Settings.getInstance();
            this.variables = (Hashtable)settings.Defaults.variables.Clone();
        }

        /// <summary>
        /// Converts lists of strings to arrays to get consistent types
        /// </summary>
        private void cleanUpVariables() {
            object[] keys = new object[this.variables.Keys.Count];
            this.variables.Keys.CopyTo(keys, 0);
            foreach (object key in keys) {
                if (this.variables[key] is List<string>) {                   
                    Debug.WriteLine(this.variables);
                    this.variables[key] = ((List<string>)this.variables[key]).ToArray();
                }
            }
        }

        /// <summary>
        /// Number of known variables
        /// </summary>
        public int Count {
            get {
                return this.variables.Count;
            }
        }

        #region IEnumerable Member

        public IEnumerator GetEnumerator() {
            return this.variables.Keys.GetEnumerator();
        }

        #endregion
    }
}
