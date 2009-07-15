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
            reset();
        }

        //we should do the other initializations here too, because they won't update when the user changes the configuration if they are initialized in the constructor
        private void reset() {
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
            MovieNameFromDirectory = false;
            name = null;
            ie = null;
            part = -1;
            sequelNumber = -1;
            multifile = false;
            filenameBlacklisted = false;
        }

        //NOTE: Need a feature to detect if the sequel name is already contained within the filename but was found somewhere else(eg directory) and added to the end again, e.g. "Hackers 2 Operation Takedown 2"
        public string ExtractMovieName(InfoEntry ie)
        {
            reset();
            this.ie = ie;
            name = ie.FilePath.Name;
            folders=new List<string>(Filepath.extractFoldernamesFromPath(ie.FilePath.Path));

            //folderlevels are used to see which directory contains the video name, so we can set ie.extractednamelevel for path creation
            List<int> folderlevels = new List<int>();            
            int origcount=folders.Count;
            if (Regex.IsMatch(name, filenameBlacklist, RegexOptions.IgnoreCase))
            {
                filenameBlacklisted = true;
                //must be atleast 1 then
                ie.ExtractedNameLevel = 1;
            }

            //Remove all illegal paths
            int j = 0;
            for(int i=0; i<folders.Count;i++){
                if (Regex.IsMatch(folders[i], pathBlacklist, RegexOptions.IgnoreCase))
                {
                    folders.RemoveAt(i);
                    i--;
                }
                else
                {
                    folderlevels.Add(origcount-j);
                }
                j++;
            }
            if(filenameBlacklisted&&folders.Count==0){
                return "Not Recognized";
            }
            if (!filenameBlacklisted)
            {
                folders.Add(ie.FilePath.Name);
                folderlevels.Add(0);
            }


            //The idea here is to test if the current name might be a better name instead of the previous one
            //We go upwards and try to find part and sequel numbers, as well as movie name. Those might be from different folders, so we need to check all legit ones

            //Clean first name (sequel and part should still be -1
            name = "";
            for (int i = folders.Count - 1; i >= 0; i--)
            {
                string testname = folders[i];                
                int testpart = -1;
                int testsequel = -1;

                //Test for sample
                if (testname.ToLower().Contains("sample"))
                {
                    name = "Sample";
                    return name;
                }
                testname = NameCleanup.RemoveReleaseGroupTag(testname);
                
                int firsttag = testname.Length;
                //remove tags and store the first occurence of a tag 
                //since we may miss a few tags, the string after the first occurence of a tag is removed later (not now since it may
                //contain additional information). Tags need to be removed before part and sequel detection, to avoid detecting things like 720p
                foreach (string s in MovieTagPatterns)
                {
                    Match m = Regex.Match(testname, s, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        if (m.Index < firsttag)
                        {
                            firsttag = m.Index;
                        }
                        testname = testname.Substring(0, m.Index)+testname.Substring(m.Index+m.Length,testname.Length-(m.Index+m.Length));
                    }
                }
                
                testpart = ExtractPartNumber(ref testname, ref firsttag);
                if (testpart != -1)
                {
                    ie.IsMultiFileMovie = true;
                }

                testsequel = ExtractSequelNumber(ref testname,ref firsttag);

                //now after recognition of part and sequel numbers, remove the rest too
                testname = testname.Substring(0, firsttag);
                testname = NameCleanup.Postprocessing(testname);

                //Some counterchecks against previous result here
                if (testpart != -1 && part == -1)
                {
                    part = testpart;
                    ie.ExtractedNameLevel = folderlevels[i];
                }
                if (testsequel != -1 && sequelNumber == -1) sequelNumber = testsequel;
                if (name==""||Helper.InitialsMatch(testname, name)) name = testname;
            }

            if (sequelNumber != -1)
            {
                name += " " + sequelNumber;
            }
            if (part != -1)
            {
                name += " CD" + part;
            }
            return name;
        }

        public int ExtractSequelNumber(ref string name, ref int tagpos)
        {
            int sequel = -1;
            //allow only one-digit numbers and [iI]+ without characters around
            MatchCollection mc = Regex.Matches(name, "[^\\d](?<sequel>\\d)([^\\d$]|$)|\\P{L}(?<sequel>(i|ii|iii|iv|v|vi|vii|viii|ix|x|xi|xii))(\\P{L}|$)",RegexOptions.IgnoreCase);
            foreach (Match m in mc)
            {
                //skip all matches which are somewhere inside the string at the beginning (i.e. skip "2 Fast 2 Furious.SOMETAG" numbers
                if (m.Groups["sequel"].Index + m.Groups["sequel"].Length < tagpos) continue;
                string number = m.Groups["sequel"].Value.ToLower();
                if (!int.TryParse(number, out sequel))
                {
                    if (number == "i" )
                    {
                        sequel = 1;
                    }
                    else if (number == "ii" )
                    {
                        sequel = 2;
                    }
                    else if (number == "iii" )
                    {
                        sequel = 3;
                    }
                    else if (number == "iv" )
                    {
                        sequel = 4;
                    }
                    else if (number == "v" )
                    {
                        sequel = 5;
                    }
                    else if (number == "vi")
                    {
                        sequel = 6;
                    }
                    else if (number == "vii")
                    {
                        sequel = 7;
                    }
                    else if (number == "viii")
                    {
                        sequel = 8;
                    }
                    else if (number == "ix")
                    {
                        sequel = 9;
                    }
                    else if (number == "x")
                    {
                        sequel = 10;
                    }
                    else if (number == "xi")
                    {
                        sequel = 11;
                    }
                    else if (number == "xii")
                    {
                        sequel = 12;
                    }
                }
                //remove the number from the name
                name = name.Substring(0, m.Groups["sequel"].Index)+name.Substring(m.Groups["sequel"].Index+m.Groups["sequel"].Length,name.Length-(m.Groups["sequel"].Index+m.Groups["sequel"].Length));

                //adjust tagpos if needed

                //if number was removed before tagpos, offset it to compensate
                if (tagpos > m.Groups["sequel"].Index)
                {
                    tagpos -= m.Groups["sequel"].Length;
                }
                if (tagpos >= name.Length)
                {
                    tagpos = name.Length;
                }
                return sequel;
            }
            return -1;
        }

        //figure out if this is a multi file video
        public int ExtractPartNumber(ref string name, ref int tagpos)
        {
            string pattern = "(CD|Cd|cd)\\s?(?<number>(\\d|I|II|II|IV|V))|(\\d\\s?of\\s?(?<number>\\d)|\\s(?<number>(a|b|c|d|e)))$";
            Match m = Regex.Match(name, pattern);
            //test for number behind tags first, and interpret it as a part number
            
            int pos;
            if (!m.Success)
            {
                pattern = "(?<number>\\d)$";
                m = Regex.Match(name.Substring(tagpos, name.Length - tagpos), pattern);
                pos = m.Index + name.Substring(0, tagpos).Length;
            }
            else
            {
                pos = m.Index;                
            }
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
                name = name.Substring(0, pos)+name.Substring(pos+m.Length,name.Length-(pos+m.Length));
                //adjust tagpos if needed
                if (tagpos >= name.Length)
                {
                    tagpos = name.Length;
                }
                return part;
            }
            return -1;
        }
    }
}
