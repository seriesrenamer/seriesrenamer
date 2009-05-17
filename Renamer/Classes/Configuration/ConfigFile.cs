using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using Renamer.Classes.Configuration.Keywords;

namespace Renamer.Classes.Configuration
{
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
        public ConfigFile(string filepath)
        {
            FilePath = filepath;
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
                            if (Settings.MonoCompatibilityMode)
                                Helper.Log(line, Helper.LogType.Plain);

                            //if delimiter and comment characters aren't known yet, try to find those first
                            if (Settings.Comment != null && Settings.Comment != "" && Settings.Delimiter != Convert.ToChar(0))
                            {
                                if (Settings.MonoCompatibilityMode)
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
                                        //wtf? this forbids using comment character in values
                                        /*if (split.Count > 0 && split[0].StartsWith(Settings.Comment.ToString()))
                                        {
                                            variables.Add(key, null);
                                        }
                                        else
                                        {*/
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
                                        //} wtf
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
                List<string> lines = new List<string>(r.ReadToEnd().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None));
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
