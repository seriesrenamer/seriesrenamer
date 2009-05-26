using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Classes;
using Renamer.Logging;
using System.Windows.Forms;
using Renamer.Classes.Configuration;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Renamer.Dialogs;
using System.Runtime.InteropServices;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes.Provider;

namespace Renamer
{
    class DataGenerator
    {

        /// <summary>
        /// Info class contains all data about files, titles etc fetched from webpages
        /// </summary>
        private Info info;

        public Info Info {
            get {
                return info;
            }
        }

        public void GetAllTitles() {
            DateTime dt = DateTime.Now;
            Info.timecreatenewname = 0;
            Info.timeextractname = 0;
            Info.timesetpath = 0;
            Info.timesetuprelation = 0;
            //make a list of shownames
            List<string> shownames = new List<string>();
            foreach (InfoEntry ie in Info.Episodes) {
                if (ie.Process && !shownames.Contains(ie.Showname)) {
                    shownames.Add(ie.Showname);
                }
            }
            foreach (string showname in shownames) {
                GetTitles(showname);
            }
            Info.timegettitles = (DateTime.Now - dt).TotalSeconds;
            Logger.Instance.LogMessage("Time for getting titles: " + Info.timegettitles + " Seconds", LogLevel.INFO);
            Logger.Instance.LogMessage("Time for extracting names: " + Info.timeextractname + " Seconds", LogLevel.INFO);
            Logger.Instance.LogMessage("Time for creating paths: " + Info.timesetpath + " Seconds", LogLevel.INFO);
            Logger.Instance.LogMessage("Time for creating filenames: " + Info.timecreatenewname + " Seconds", LogLevel.INFO);
            Logger.Instance.LogMessage("Time for assigning relations: " + Info.timesetuprelation + " Seconds", LogLevel.INFO);
        }
        /// <summary>
        /// gets titles, by using database search feature and parsing results, after that, shows them in gui
        /// </summary>
        private void GetTitles(string Showname) {
            //once
            for (int a = 0; a < 1; a++) {
                // request
                RelationProvider provider = RelationProvider.GetCurrentProvider();
                if (provider == null) {
                    Logger.Instance.LogMessage("No relation provider found/selected", LogLevel.ERROR);
                    return;
                }
                //get rid of old relations
                Info.Relations.Remove(Info.GetRelationCollectionByName(Showname));
                foreach (InfoEntry ie in Info.Episodes) {
                    if (ie.Showname == Showname && ie.Process) {
                        ie.Name = "";
                        ie.NewFileName = "";
                        ie.Language = provider.Language;
                    }
                }
                string url = provider.SearchUrl;
                Logger.Instance.LogMessage("Search URL: " + url, LogLevel.DEBUG);
                if (url == null || url == "") {
                    Logger.Instance.LogMessage("Can't search because no search URL is specified for this provider", LogLevel.ERROR);
                    break;
                }
                string[] LastTitles = Helper.ReadProperties(Config.LastTitles);
                url = url.Replace("%T", Showname);
                url = System.Web.HttpUtility.UrlPathEncode(url);
                Logger.Instance.LogMessage("Encoded Search URL: " + url, LogLevel.DEBUG);
                HttpWebRequest requestHtml = null;
                try {
                    requestHtml = (HttpWebRequest)(HttpWebRequest.Create(url));
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage(ex.Message, LogLevel.ERROR);
                    if (requestHtml != null)
                        requestHtml.Abort();
                    break;
                }
                //SetProxy(requestHtml, url);
                Logger.Instance.LogMessage("Searching at " + url.Replace(" ", "%20"), LogLevel.INFO);
                requestHtml.Timeout = Convert.ToInt32(Helper.ReadProperty(Config.Timeout));
                // get response
                HttpWebResponse responseHtml = null;
                try {
                    responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage(ex.Message, LogLevel.ERROR);
                    if (responseHtml != null)
                        responseHtml.Close();
                    if (requestHtml != null)
                        requestHtml.Abort();
                    break;
                }
                Logger.Instance.LogMessage("Search Results URL: " + responseHtml.ResponseUri.AbsoluteUri, LogLevel.DEBUG);
                //if search engine directs us straight to the result page, skip parsing search results
                string seriesURL = provider.SeriesUrl;
                if (responseHtml.ResponseUri.AbsoluteUri.Contains(seriesURL)) {
                    Logger.Instance.LogMessage("Search Results URL contains Series URL: " + seriesURL, LogLevel.DEBUG);
                    Logger.Instance.LogMessage("Search engine forwarded directly to single result: " + responseHtml.ResponseUri.AbsoluteUri.Replace(" ", "%20") + provider.EpisodesUrl.Replace(" ", "%20"), LogLevel.INFO);
                    GetRelations(responseHtml.ResponseUri.AbsoluteUri + provider.EpisodesUrl, Showname);
                }
                else {
                    Logger.Instance.LogMessage("Search Results URL doesn't contain Series URL: " + seriesURL + ", this is a proper search results page", LogLevel.DEBUG);
                    // and download
                    StreamReader r = null;
                    try {
                        r = new StreamReader(responseHtml.GetResponseStream());
                    }
                    catch (Exception ex) {
                        if (r != null)
                            r.Close();
                        if (responseHtml != null) {
                            responseHtml.Close();
                        }
                        if (requestHtml != null) {
                            requestHtml.Abort();
                        }
                        Logger.Instance.LogMessage(ex.Message, LogLevel.ERROR);
                        break;
                    }
                    string source = r.ReadToEnd();
                    r.Close();

                    //Source cropping
                    source = source.Substring(Math.Max(source.IndexOf(provider.SearchStart), 0));
                    source = source.Substring(0, Math.Max(source.LastIndexOf(provider.SearchEnd), source.Length - 1));
                    ParseSearch(ref source, responseHtml.ResponseUri.AbsoluteUri, Showname);
                }

                responseHtml.Close();
                //FillListView();
            }
        }

        private void SetProxy(HttpWebRequest client, string url) {
            // Comment out foreach statement to use normal System.Net proxy detection 
            foreach (
                    Uri address
                    in WinHttpSafeNativeMethods.GetProxiesForUrl(new Uri(url))) {
                client.Proxy = new WebProxy(address);
                break;
            }
        }

        /// <summary>
        /// Parses search results from a series search
        /// </summary>
        /// <param name="source">Source code of the search results page</param>
        /// <param name="Showname">Showname</param>
        /// <param name="SourceURL">URL of the page source</param>
        private void ParseSearch(ref string source, string SourceURL, string Showname) {
            if (source == "")
                return;
            RelationProvider provider = RelationProvider.GetCurrentProvider();
            if (provider == null) {
                Logger.Instance.LogMessage("No relation provider found/selected", LogLevel.ERROR);
                return;
            }
            string pattern = provider.SearchRegExp;

            Logger.Instance.LogMessage("Trying to match source at " + SourceURL + " with " + pattern, LogLevel.DEBUG);

            RegexOptions ro = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            if (provider.SearchRightToLeft)
                ro |= RegexOptions.RightToLeft;
            MatchCollection mc = Regex.Matches(source, pattern, ro);

            if (mc.Count == 0) {
                Logger.Instance.LogMessage("No results found", LogLevel.INFO);
            }
            else if (mc.Count == 1) {
                string url = provider.RelationsPage;
                Logger.Instance.LogMessage("One result found on search page, going to " + url.Replace(" ", "%20") + " with %L=" + mc[0].Groups["link"].Value, LogLevel.DEBUG);
                url = url.Replace("%L", mc[0].Groups["link"].Value);
                url = System.Web.HttpUtility.HtmlDecode(url);
                Logger.Instance.LogMessage("Search engine found one result: " + url.Replace(" ", "%20"), LogLevel.INFO);
                GetRelations(url, Showname);
            }
            else {
                Logger.Instance.LogMessage("Search engine found multiple results at " + SourceURL.Replace(" ", "%20"), LogLevel.INFO);
                SelectResult sr = new SelectResult(mc, provider, false);
                if (sr.ShowDialog() == DialogResult.Cancel || sr.url == "")
                    return;

                //Apply language of selected result to matching episodes
                if (provider.Language == Helper.Languages.None) {
                    foreach (InfoEntry ie in Info.Episodes) {
                        if (ie.Showname == Showname && ie.Process) {
                            ie.Language = sr.Language;
                        }
                    }
                }
                string url = provider.RelationsPage;
                Logger.Instance.LogMessage("User selected " + provider.RelationsPage + "with %L=" + sr.url, LogLevel.DEBUG);
                url = url.Replace("%L", sr.url);
                url = System.Web.HttpUtility.HtmlDecode(url);
                GetRelations(url, Showname);
            }
        }

        //parse page(s) containing relations
        /// <summary>
        /// Parses page containing the relation data
        /// </summary>
        /// <param name="url">URL of the page to parse</param>
        /// <param name="Showname">Showname</param>
        private void GetRelations(string url, string Showname) {
            RelationProvider provider = RelationProvider.GetCurrentProvider();
            if (provider == null) {
                Logger.Instance.LogMessage("No relation provider found/selected", LogLevel.ERROR);
                return;
            }
            Logger.Instance.LogMessage("Trying to get relations from " + url, LogLevel.DEBUG);
            //if episode infos are stored on a new page for each season, this should be marked with %S in url, so we can iterate through all those pages
            int season = 1;
            string url2 = url;
            //Create new RelationCollection
            RelationCollection rc = new RelationCollection(Showname);
            while (true) {
                if (url2.Contains("%S")) {
                    url = url2.Replace("%S", season.ToString());
                }

                if (url == null || url == "")
                    return;
                // request
                url = System.Web.HttpUtility.UrlPathEncode(url);
                Logger.Instance.LogMessage("Trying to get relations for season " + season + " from " + url, LogLevel.DEBUG);
                HttpWebRequest requestHtml = null;
                try {
                    requestHtml = (HttpWebRequest)(HttpWebRequest.Create(url));
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage(ex.Message, LogLevel.ERROR);
                    requestHtml.Abort();
                    return;
                }
                requestHtml.Timeout = Helper.ReadInt(Config.Timeout);
                // get response
                HttpWebResponse responseHtml = null;
                try {
                    responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
                }
                catch (WebException e) {
                    Logger.Instance.LogMessage(e.Message, LogLevel.ERROR);
                    if (responseHtml != null) {
                        responseHtml.Close();
                    }
                    return;
                }

                Logger.Instance.LogMessage("Response URL: " + responseHtml.ResponseUri.AbsoluteUri, LogLevel.DEBUG);
                //if we get redirected, lets assume this page does not exist
                if (responseHtml.ResponseUri.AbsoluteUri != url) {
                    Logger.Instance.LogMessage("Response URL doesn't match request URL, page doesn't seem to exist", LogLevel.DEBUG);
                    responseHtml.Close();
                    requestHtml.Abort();
                    break;
                }
                // and download
                //Logger.Instance.LogMessage("charset=" + responseHtml.CharacterSet, LogLevel.INFO);
                Encoding enc;
                if (provider.Encoding != null && provider.Encoding != "") {
                    try {
                        enc = Encoding.GetEncoding(provider.Encoding);
                    }
                    catch (Exception ex) {
                        Logger.Instance.LogMessage("Invalid encoding in config file: " + ex.Message, LogLevel.ERROR);
                        enc = Encoding.GetEncoding(responseHtml.CharacterSet);
                    }
                }
                else {
                    enc = Encoding.GetEncoding(responseHtml.CharacterSet);
                }
                StreamReader r = new StreamReader(responseHtml.GetResponseStream(), enc);
                string source = r.ReadToEnd();
                r.Close();
                responseHtml.Close();



                //Source cropping
                source = source.Substring(Math.Max(source.IndexOf(provider.RelationsStart), 0));
                source = source.Substring(0, Math.Max(source.LastIndexOf(provider.RelationsEnd), 0));

                string pattern = provider.RelationsRegExp;
                Logger.Instance.LogMessage("Trying to match source from " + responseHtml.ResponseUri.AbsoluteUri + " with " + pattern, LogLevel.DEBUG);
                RegexOptions ro = RegexOptions.IgnoreCase | RegexOptions.Singleline;
                if (provider.RelationsRightToLeft)
                    ro |= RegexOptions.RightToLeft;
                MatchCollection mc = Regex.Matches(source, pattern, ro);

                for (int i = 0; i < mc.Count; i++) {
                    Match m = mc[i];
                    //if we are iterating through season pages, take season from page url directly
                    //parse season and episode numbers
                    int s, e;
                    Int32.TryParse(m.Groups["Season"].Value, out s);
                    Int32.TryParse(m.Groups["Episode"].Value, out e);
                    if (url != url2) {
                        rc.Relations.Add(new Relation(season.ToString(), e.ToString(), System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        Logger.Instance.LogMessage("Found Relation: " + "S" + s.ToString() + "E" + e.ToString() + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), LogLevel.DEBUG);
                    }
                    else {
                        rc.Relations.Add(new Relation(s.ToString(), e.ToString(), System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        Logger.Instance.LogMessage("Found Relation: " + "S" + s.ToString() + "E" + e.ToString() + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), LogLevel.DEBUG);
                    }
                }
                Info.AddRelationCollection(rc);

                // THOU SHALL NOT FORGET THE BREAK
                if (!url2.Contains("%S"))
                    break;
                season++;
            }
            Logger.Instance.LogMessage("" + (season - 1) + " Seasons, " + rc.Relations.Count + " relations found", LogLevel.DEBUG);
        }
        /// <summary>
        /// Creates subtitle destination and names subs when no show information is fetched yet, so they have the same name as their video files for better playback
        /// </summary>
        void RenameSubsToMatchVideos() {
            foreach (InfoEntry ie in Info.Episodes) {
                if (info.IsSubtitle(ie)) {
                    int season = -1;
                    int episode = -1;
                    try {
                        Int32.TryParse(ie.Season, out season);
                        Int32.TryParse(ie.Episode, out episode);
                    }
                    catch (Exception) {
                        Logger.Instance.LogMessage("Couldn't Convert season or episode to int because string was garbled, too bad :P", LogLevel.ERROR);
                    }
                    List<InfoEntry> lie = info.GetMatchingVideos(season, episode);
                    if (lie != null && lie.Count == 1) {
                        if (ie.NewFileName == "") {
                            if (lie[0].NewFileName == "") {
                                ie.NewFileName = Path.GetFileNameWithoutExtension(lie[0].Filename) + "." + ie.Extension;
                            }
                            else {
                                ie.NewFileName = Path.GetFileNameWithoutExtension(lie[0].NewFileName) + "." + ie.Extension;
                            }

                            //Move to Video file
                            ie.Destination = lie[0].Destination;

                            //Don't do this again if name fits already
                            if (ie.NewFileName == ie.Filename) {
                                ie.NewFileName = "";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// creates names for all entries using season, episode and name and the target pattern
        /// <param name="movie">If used on movie files, target pattern will be ignored and only name property is used</param>
        /// </summary>
        private void CreateNewNames() {
            for (int i = 0; i < Info.Episodes.Count; i++) {
                Info.Episodes[i].CreateNewName();
            }
        }

        /// <summary>
        /// Updatess list view and do lots of other connected stuff with it
        /// </summary>
        /// <param name="clear">if true, list is cleared first and unconnected subtitle files are scheduled to be renamed</param>
        /// <param name="KeepShowName">if set, show name isn't altered</param>
        public void UpdateList(bool clear) {
            if (clear) {
                Info.Episodes.Clear();
            }

            //scan for files which got deleted so we can remove them
            for (int i = Info.Episodes.Count - 1; i >= 0; i--) {
                InfoEntry ie = Info.Episodes[i];
                if (!File.Exists(ie.Path + Path.DirectorySeparatorChar + ie.Name)) {
                    Info.Episodes.Remove(ie);
                    i--;
                }
            }

            string path = Helper.ReadProperty(Config.LastDirectory);
            path = path.TrimEnd(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            bool CreateDirectoryStructure = Helper.ReadProperty(Config.CreateDirectoryStructure) == "1";
            bool UseSeasonSubdirs = Helper.ReadProperty(Config.UseSeasonSubDir) == "1";
            if (Directory.Exists(path)) {
                //scan for new files
                List<string> extensions = new List<string>(Helper.ReadProperties(Config.Extensions));
                extensions.AddRange(Helper.ReadProperties(Config.SubtitleExtensions));
                if (extensions == null) {
                    Logger.Instance.LogMessage("No File Extensions found!", LogLevel.WARNING);
                    return;
                }
                for (int i = 0; i < extensions.Count; i++) {
                    extensions[i] = extensions[i].ToLower();
                }
                List<FileSystemInfo> Files = new List<FileSystemInfo>();
                foreach (string ex in extensions) {
                    List<FileSystemInfo> fsi = Helper.GetAllFilesRecursively(path, "*." + ex);
                    Files.AddRange(fsi);
                }

                //Loop through all files and recognize things, YAY!
                string[] patterns = Helper.ReadProperties(Config.EpIdentifier);
                for (int i = 0; i < patterns.Length; i++) {
                    //replace %S and %E by proper regexps
                    //if a pattern containing %S%E is used, only use the first number for season
                    if (patterns[i].Contains("%S%E")) {
                        patterns[i] = patterns[i].Replace("%S", "(?<Season>\\d)");
                        patterns[i] = patterns[i].Replace("%E", "(?<Episode>\\d+)");
                    }
                    else {
                        patterns[i] = patterns[i].Replace("%S", "(?<Season>\\d+)");
                        patterns[i] = patterns[i].Replace("%E", "(?<Episode>\\d+)");
                    }
                }

                //some declarations already for speed
                string strSeason = "";
                string strEpisode = "";
                Match m = null;
                int DirectorySeason = -1;
                InfoEntry ie = null;
                bool contains = false;
                DateTime dt;
                string currentpath = "";
                foreach (FileSystemInfo file in Files) {
                    //showname and season recognized from path
                    DirectorySeason = -1;

                    //Check if there is already an entry on this file, and if not, create one
                    ie = null;
                    currentpath = Path.GetDirectoryName(file.FullName);
                    foreach (InfoEntry i in Info.Episodes) {
                        if (i.Filename == file.Name && i.Path == currentpath) {
                            ie = i;
                            break;
                        }
                    }

                    if (ie == null) {
                        ie = new InfoEntry();
                    }

                    //Set basic values, by setting those values destination directory and filename will be generated automagically
                    ie.Filename = file.Name;
                    ie.Path = currentpath;
                    ie.Extension = Path.GetExtension(file.FullName).ToLower().Replace(".", "");
                    //Get season number and showname from directory
                    DirectorySeason = ExtractSeasonFromDirectory(Path.GetDirectoryName(file.FullName));
                    dt = DateTime.Now;
                    //try to recognize season and episode from filename
                    foreach (string pattern in patterns) {
                        //Try to match. If it works, get the season and the episode from the match
                        m = Regex.Match(file.Name, pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                        if (m.Success) {
                            strSeason = "";
                            strEpisode = "";
                            try {
                                strSeason = Int32.Parse(m.Groups["Season"].Value).ToString();
                            }
                            catch (FormatException) {
                            }
                            try {
                                strEpisode = Int32.Parse(m.Groups["Episode"].Value).ToString();
                            }
                            catch (FormatException) {
                            }
                            //Fix for .0216. notation for example, 4 numbers should always be recognized as %S%S%E%E
                            if (strEpisode.Length == 3 && strSeason.Length == 1) {
                                strSeason += strEpisode[0];
                                strEpisode = strEpisode.Substring(1);
                                if (strSeason[0] == '0') {
                                    strSeason = strSeason.Substring(1);
                                }
                            }
                            ie.Episode = strEpisode;
                            ie.Season = strSeason;

                            //if season recognized from directory name doesn't match season recognized from filename, the file might be located in a wrong directory
                            if (DirectorySeason != -1 && ie.Season != DirectorySeason.ToString()) {
                                Logger.Instance.LogMessage("File seems to be located inconsistently: " + ie.Filename + " was recognized as season " + ie.Season + ", but folder name indicates that it should be season " + DirectorySeason.ToString(), LogLevel.WARNING);
                            }
                            break;
                        }
                    }
                    Info.timeextractnumbers += (DateTime.Now - dt).TotalSeconds;
                    //if season number couldn't be extracted, try to get it from folder
                    //(this should never happen if a pattern like %S%E is set)
                    if (ie.Season == "" && DirectorySeason != -1) {
                        ie.Season = DirectorySeason.ToString();
                    }
                    //if nothing could be recognized, assume that this is a movie
                    if (ie.Season == "" && ie.Episode == "") {
                        ie.Movie = true;
                    }
                    //if not added yet, add it
                    if (!contains) {
                        Info.Episodes.Add(ie);
                    }
                }
                //SelectSimilarFilesForProcessing(path,Helper.ReadProperties(Config.LastTitles)[0]);
                SelectRecognizedFilesForProcessing();
            }

            if (clear) {
                RenameSubsToMatchVideos();
            }

            Logger.Instance.LogMessage("Found " + Info.Episodes.Count + " Files", LogLevel.INFO);
            /*
            FillListView();

            //also update some gui elements for the sake of it
            txtTarget.Text = Helper.ReadProperty(Config.TargetPattern);
            txtPath.Text = Helper.ReadProperty(Config.LastDirectory);
            string LastProvider = Helper.ReadProperty(Config.LastProvider);
            if (LastProvider == null)
                LastProvider = "";
            cbProviders.SelectedIndex = Math.Max(0, cbProviders.Items.IndexOf(LastProvider));
            string LastSubProvider = Helper.ReadProperty(Config.LastSubProvider);
            if (LastSubProvider == null)
                LastSubProvider = "";
            cbSubs.SelectedIndex = Math.Max(0, cbSubs.Items.IndexOf(LastSubProvider));
            */
        }
        private void SelectRecognizedFilesForProcessing() {
            foreach (InfoEntry ie in Info.Episodes) {
                if (ie.Season != "" && ie.Episode != "") {
                    ie.Process = true;
                    ie.Movie = false;
                } else if(ie.Showname.ToLower()=="sample") {
                    ie.Process = false;
                    ie.Movie = false;
                } else {
                    ie.Process = false;
                    ie.Movie = true;
                }
            }
        }

        /// <summary>
        /// Extracts season from directory name
        /// </summary>
        /// <param name="path">path from which to extract the data (NO FILEPATH, JUST FOLDER)</param>
        /// <returns>recognized season, -1 if not recognized</returns>
        public int ExtractSeasonFromDirectory(string path) {
            string[] patterns = Helper.ReadProperties(Config.Extract);
            string[] folders = path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = patterns.Length - 1; i >= 0; i--) {
                string pattern = patterns[i];
                pattern = pattern.Replace("%T", "*.?");
                pattern = pattern.Replace("%S", "(?<Season>\\d)");
                Match m = Regex.Match(folders[folders.Length - 1], pattern, RegexOptions.IgnoreCase);

                if (m.Success) {
                    try {
                        return Int32.Parse(m.Groups["Season"].Value);
                    }
                    catch (Exception) {
                        return -1;
                    }
                }
            }
            return -1;
        }
    }
}
