using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Classes.Configuration.Keywords;
using System.Text.RegularExpressions;
using System.IO;

namespace Renamer.Classes
{
    class MovieNameExtractor
    {
        //Splitted folder names
        private List<string> folders;
        //private string[] extractPatterns;
        //private string[] shownamePatterns;

        private string[] MovieTagPatterns;
        //Was movie name extracted from a directory name?
        private bool MovieNameFromDirectory;
        //List with path names that may not be used for name extraction
        private string pathBlacklist;
        //List with filenames that may not be used for name extraction
        private string filenameBlacklist;
        //extracted name
        private string name;
        //if filename is blacklisted
        private bool filenameBlacklisted;
        //if path is blacklisted
        private bool pathBlacklisted;
        //the involved InfoEntry
        private InfoEntry ie;
        //the part of a multifilevideo
        private static int part;
        //is this file a multifile video?
        private static bool multifile;
        //Number of this movie in a series of movies
        private static int sequelNumber;
        //Singleton vars
        private static MovieNameExtractor instance = null;
        private static object m_lock = new object();

        public static MovieNameExtractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (m_lock) { if (instance == null) instance = new MovieNameExtractor(); }
                }
                return instance;
            }
        }
        private MovieNameExtractor()
        {
            string[] tags = Helper.ReadProperties(Config.Tags);  
            List<string> MovieRegexes = new List<string>();
            foreach (string s in tags)
            {
                MovieRegexes.Add("[^A-Za-z0-9]+" + s);
            }
            MovieTagPatterns = MovieRegexes.ToArray();
            string[] blacklist = Helper.ReadProperties(Config.PathBlacklist);
            pathBlacklist = String.Join("|", blacklist);
            blacklist = Helper.ReadProperties(Config.FilenameBlacklist);
            filenameBlacklist = String.Join("|", blacklist);
            //extractPatterns = (string[])Helper.ReadProperties(Config.Extract);
            //for (int index = 0; index < extractPatterns.Length; index++) {
               // extractPatterns[index] = transformPlaceholderToRegex(extractPatterns[index]);
            //}
            filenameBlacklisted = false;
            //shownamePatterns = Helper.ReadProperties(Config.ShownameExtractionRegex);
            part=-1;
            multifile=false;
            sequelNumber=-1;
            reset();
        }

        private void reset() {
            MovieNameFromDirectory = false;
            name = null;
            ie = null;
            filenameBlacklisted = false;
        }

        public string ExtractMovieName(InfoEntry ie)
        {
            reset();
            this.ie = ie;
            name = ie.FilePath.Name;
            folders=new List<string>(Filepath.extractFoldernamesFromPath(ie.FilePath.Path));
            if (Regex.IsMatch(name, filenameBlacklist, RegexOptions.IgnoreCase))
            {
                filenameBlacklisted = true;
                //must be atleast 1 then
                ie.ExtractedNameLevel = 1;
            }
            //Remove all illegal paths
            for(int i=0; i<folders.Count;i++){
                if(Regex.IsMatch(folders[i], pathBlacklist, RegexOptions.IgnoreCase)){
                    folders.RemoveAt(i);
                    i--;
                }
            }
            if(filenameBlacklisted&&folders.Count==0){
                return "Not Recognized";
            }
            if (!filenameBlacklisted)
            {
                folders.Add(ie.FilePath.Name);
            }

            //NOTE: Everything below is WIP or outdated
            for (int i = folders.Count - 1; i >= 0; i--)
            {
                string test = folders[i];
                ExtractMultifilePart(ref test);
                test = NameCleanup.RemoveReleaseGroupTag(test);
                //if part couldn't be extracted yet, maybe it was because of a tag at the end before the part identifier
                if (part == -1)
                {
                    part = ExtractMultifilePart(ref test);
                }
                if (part != -1)
                {
                    ie.IsMultiFileMovie = true;
                }
                //try to match tags    
                foreach (string s in MovieTagPatterns)
                {
                    Match m = Regex.Match(name, s, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        name = name.Substring(0, m.Index)+name.Substring(m.Index+m.Length,name.Length-(m.Index+m.Length));
                    }
                }
                name = NameCleanup.Postprocessing(name);
                if (folders.Count != 0)
                {
                    folders[folders.Count - 1] = NameCleanup.RemoveReleaseGroupTag(folders[folders.Count - 1]);
                    if (Helper.InitialsMatch(folders[folders.Count - 1], name))
                    {
                        name = folders[folders.Count - 1];
                        //try to match tags    
                        foreach (string s in MovieTagPatterns)
                        {
                            Match m = Regex.Match(name, s, RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                name = name.Substring(0, m.Index);
                            }
                        }
                        if (part == -1)
                        {
                            part = ExtractMultifilePart(ref test);
                        }
                        name = NameCleanup.Postprocessing(name);
                    }
                }


                if (part != -1)
                {
                    name += " CD" + part;

                }
            }
            return name;
        }


        public int ExtractMultifilePart(ref string name)
        {
            //figure out if this is a multi file video
            string pattern = "(?<pos>(CD|Cd|cd))\\s?(?<number>(\\d|I|II|II|IV|V))|((?<pos>\\d\\s?of\\s?)(?<number>\\d)|(?<pos> )(?<number>(a|b|c|d|e)))$";
            Match m = Regex.Match(name, pattern);
            int part = -1;
            if (m.Success)
            {
                string number = m.Groups["number"].Value;
                if (!int.TryParse(number, out part))
                {
                    if (number == "a" || number == "I")
                    {
                        part = 1;
                    }
                    else if (number == "b" || number == "II")
                    {
                        part = 2;
                    }
                    else if (number == "c" || number == "III")
                    {
                        part = 3;
                    }
                    else if (number == "d" || number == "IV")
                    {
                        part = 4;
                    }
                    else if (number == "e" || number == "V")
                    {
                        part = 5;
                    }
                }
                name = name.Substring(0, m.Groups["pos"].Index);
                return part;
            }
            return -1;
        }
    }
}
