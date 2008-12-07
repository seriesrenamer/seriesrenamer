using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.Diagnostics;
using Schematrix;
using ICSharpCode.SharpZipLib.Zip;

namespace Renamer
{
    /// <summary>
    /// Main Form
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Settings class contains some stuff which can't be stored in config file for one reason or another
        /// </summary>
        private Settings settings;
        
        /// <summary>
        /// Info class contains all data about files, titles etc fetched from webpages
        /// </summary>
        private Info info;
        
        /// <summary>
        /// Custom sorting class to sort list view for 2 columns
        /// </summary>
        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        
        /// <summary>
        /// Column width ratios needed to keep them during resize
        /// </summary>
        private float[] columnsizes;

        /// <summary>
        /// Temp variable to store a previously focused control
        /// </summary>
        private Control focused = null;

        /// <summary>
        /// Program arguments
        /// </summary>
        private List<string> args;
        
        /// <summary>
        /// GUI constructor
        /// </summary>
        /// <param name="args">program arguments</param>
        public Form1(string[] args)
        {
            this.args = new List<string>(args);
            InitializeComponent();
        }
        
        #region processing
        #region Data Acquisition
        /// <summary>
        /// gets titles, by using database search feature and parsing results, after that, shows them in gui
        /// </summary>
        private void GetTitles()
        {
            //once
            for (int a = 0; a < 1; a++)
            {
                this.Cursor = Cursors.WaitCursor;
                //get rid of old relations
                info.Relations.Clear();
                foreach (InfoEntry ie in info.Episodes)
                {
                    ie.Name = "";
                    ie.NewFileName = "";
                }
                // request
                RelationProvider provider = info.GetCurrentProvider();
                if (provider == null)
                {
                    Helper.Log("No relation provider found/selected", Helper.LogType.Error);
                    return;
                }
                string url = provider.SearchURL;
                Helper.Log("Search URL: " + url, Helper.LogType.Debug);
                if (url == null || url == "")
                {
                    Helper.Log("Can't search because no search URL is specified for this provider", Helper.LogType.Error);
                    break;
                }
                string[] LastTitles = Helper.ReadProperties(Config.LastTitles);
                url = url.Replace("%T", LastTitles[0]);
                url = System.Web.HttpUtility.UrlPathEncode(url);
                Helper.Log("Encoded Search URL: " + url, Helper.LogType.Debug);
                HttpWebRequest requestHtml = null;
                try
                {
                    requestHtml = (HttpWebRequest)(HttpWebRequest.Create(url));
                }
                catch (Exception ex)
                {
                    Helper.Log(ex.Message, Helper.LogType.Error);
                    if (requestHtml != null)
                        requestHtml.Abort();
                    break;
                }
                Helper.Log("Searching at " + url.Replace(" ", "%20"), Helper.LogType.Status);
                requestHtml.Timeout = Convert.ToInt32(Helper.ReadProperty(Config.Timeout));
                // get response
                HttpWebResponse responseHtml = null;
                try
                {
                    responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
                }
                catch (Exception ex)
                {
                    Helper.Log(ex.Message, Helper.LogType.Error);
                    if (responseHtml != null)
                        responseHtml.Close();
                    if (requestHtml != null)
                        requestHtml.Abort();
                    break;
                }
                Helper.Log("Search Results URL: " + responseHtml.ResponseUri.AbsoluteUri, Helper.LogType.Debug);
                //if search engine directs us straight to the result page, skip parsing search results
                string seriesURL = provider.SeriesURL;
                if (responseHtml.ResponseUri.AbsoluteUri.Contains(seriesURL))
                {
                    Helper.Log("Search Results URL contains Series URL: " + seriesURL, Helper.LogType.Debug);
                    Helper.Log("Search engine forwarded directly to single result: " + responseHtml.ResponseUri.AbsoluteUri.Replace(" ", "%20") + provider.EpisodesURL.Replace(" ", "%20"), Helper.LogType.Status);
                    GetRelations(responseHtml.ResponseUri.AbsoluteUri + provider.EpisodesURL);
                }
                else
                {
                    Helper.Log("Search Results URL doesn't contain Series URL: " + seriesURL+", this is a proper search results page", Helper.LogType.Debug);
                    // and download
                    StreamReader r = null;
                    try
                    {
                        r = new StreamReader(responseHtml.GetResponseStream());
                    }
                    catch (Exception ex)
                    {
                        if (r != null)
                            r.Close();
                        if (responseHtml != null)
                        {
                            responseHtml.Close();
                        }
                        if (requestHtml != null)
                        {
                            requestHtml.Abort();
                        }
                        Helper.Log(ex.Message, Helper.LogType.Error);
                        break;
                    }
                    string source = r.ReadToEnd();
                    r.Close();

                    //Source cropping
                    source = source.Substring(Math.Max(source.IndexOf(provider.SearchStart),0));
                    source = source.Substring(0, Math.Max(source.LastIndexOf(provider.SearchEnd),source.Length-1));
                    ParseSearch(ref source, responseHtml.ResponseUri.AbsoluteUri);
                }

                responseHtml.Close();

                if (info.Relations.Count > 0)
                {
                    SetupRelations();
                }
                FillListView();
            }
            Cursor = Cursors.Default;
        }
        
        /// <summary>
        /// finds titles for the files on hdd by scanning through the imdb data and generates filename afterwards
        /// </summary>
        private void SetupRelations()
        {
            Helper.Log("Setting up relations", Helper.LogType.Debug);
            for (int i = 0; i < info.Episodes.Count; i++)
            {
                SetupRelation(i);
            }
        }

        /// <summary>
        /// finds title for a single file on hdd by scanning through the search engine data and generates filename afterwards
        /// </summary>
        /// <param name="item">Index from info.Episodes list for which to create the relation</param>
        private void SetupRelation(int item)
        {
            InfoEntry ie = info.Episodes[item];
            int seasonmatch = -1;
            int epmatch = -1;
            for (int i = 0; i < info.Relations.Count; i++)
            {
                string season = info.Relations[i].Season;
                string episode = info.Relations[i].Episode;
                string name = info.Relations[i].Name;
                if (season == ie.Season && episode == ie.Episode)
                {
                    ie.Name = name;
                    seasonmatch = i;
                    epmatch = i;
                    break;
                }
                //if there is none or invalid episode info, try to match by season
                if (season == ie.Season && !Helper.IsNumeric(ie.Episode)) seasonmatch = i;
                //if there is none or invalid season info, try to match by episode
                if (episode == ie.Episode && !Helper.IsNumeric(ie.Season)) epmatch = i;
            }
            //episode match, but no season match
            if (seasonmatch == -1 && epmatch != -1)
            {
                ie.Name = info.Relations[epmatch].Name;

            }
            if (seasonmatch != -1 && epmatch == -1)
            {
                ie.Name = info.Relations[seasonmatch].Name;
            }            
            CreateNewName(item);
        }

        /// <summary>
        /// Parses search results from a series search
        /// </summary>
        /// <param name="source">Source code of the search results page</param>
        private void ParseSearch(ref string source, string SourceURL)
        {
            if (source == "") return;
            RelationProvider provider = info.GetCurrentProvider();
            if (provider == null)
            {
                Helper.Log("No relation provider found/selected", Helper.LogType.Error);
                return;
            }
            string pattern = provider.SearchRegExp;
            
            Helper.Log("Trying to match source at "+SourceURL+" with "+pattern, Helper.LogType.Debug);

            RegexOptions ro = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            if (provider.SearchRightToLeft) ro |= RegexOptions.RightToLeft;
            MatchCollection mc = Regex.Matches(source, pattern, ro);
            
            if (mc.Count == 0)
            {
                Helper.Log("No results found", Helper.LogType.Info);
            }
            else if (mc.Count == 1)
            {
                string url = provider.RelationsPage;
                Helper.Log("One result found on search page, going to " + url.Replace(" ", "%20") + " with %L=" + mc[0].Groups["link"].Value, Helper.LogType.Debug);
                url = url.Replace("%L", mc[0].Groups["link"].Value);
                Helper.Log("Search engine found one result: " + url.Replace(" ", "%20"), Helper.LogType.Status);
                GetRelations(url);
            }
            else
            {
                Helper.Log("Search engine found multiple results at " + SourceURL.Replace(" ", "%20"), Helper.LogType.Info);
                SelectResult sr = new SelectResult(mc, provider, false);
                if (sr.ShowDialog() == DialogResult.Cancel || sr.url == "") return;
                string url = provider.RelationsPage;
                Helper.Log("User selected " + provider.RelationsPage + "with %L=" + sr.url, Helper.LogType.Debug);
                url = url.Replace("%L", sr.url);
                GetRelations(url);
            }
        }

        //parse page(s) containing relations
        /// <summary>
        /// Parses page containing the relation data
        /// </summary>
        /// <param name="url">URL of the page to parse</param>
        private void GetRelations(string url)
        {
            Helper.Log("Trying to get relations from " + url, Helper.LogType.Debug);
            //if episode infos are stored on a new page for each season, this should be marked with %S in url, so we can iterate through all those pages
            int season = 1;
            string url2 = url;
            while (true)
            {
                if (url2.Contains("%S"))
                {
                    url = url2.Replace("%S", season.ToString());
                }
                
                if (url == null || url == "") return;
                // request
                url = System.Web.HttpUtility.UrlPathEncode(url);
                Helper.Log("Trying to get relations for season " + season + " from " + url, Helper.LogType.Debug);
                HttpWebRequest requestHtml = null;
                try
                {
                    requestHtml = (HttpWebRequest)(HttpWebRequest.Create(url));
                }
                catch (Exception ex)
                {
                    Helper.Log(ex.Message, Helper.LogType.Error);
                    requestHtml.Abort();
                    return;
                }
                requestHtml.Timeout = Helper.ReadInt(Config.Timeout);
                // get response
                HttpWebResponse responseHtml=null;
                try
                {
                    responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
                }
                catch (WebException e)
                {
                    Helper.Log(e.Message, Helper.LogType.Error);
                    if(responseHtml!=null){
                        responseHtml.Close();
                    }
                    return;
                }

                Helper.Log("Response URL: "+responseHtml.ResponseUri.AbsoluteUri, Helper.LogType.Debug);
                //if we get redirected, lets assume this page does not exist
                if (responseHtml.ResponseUri.AbsoluteUri != url)
                {
                    Helper.Log("Response URL doesn't match request URL, page doesn't seem to exist", Helper.LogType.Debug);
                    responseHtml.Close();
                    requestHtml.Abort();
                    break;
                }
                // and download
                //Helper.Log("charset=" + responseHtml.CharacterSet, Helper.LogType.Status);

                StreamReader r = new StreamReader(responseHtml.GetResponseStream(), Encoding.GetEncoding(responseHtml.CharacterSet));
                string source = r.ReadToEnd();
                r.Close();
                responseHtml.Close();
                RelationProvider provider = info.GetCurrentProvider();
                if (provider == null)
                {
                    Helper.Log("No relation provider found/selected", Helper.LogType.Error);
                    return;
                }


                //Source cropping
                source = source.Substring(Math.Max(source.IndexOf(provider.RelationsStart),0));
                source = source.Substring(0, Math.Max(source.LastIndexOf(provider.RelationsEnd),0));

                string pattern = provider.RelationsRegExp;
                Helper.Log("Trying to match source from " + responseHtml.ResponseUri.AbsoluteUri + " with " + pattern, Helper.LogType.Debug);
                RegexOptions ro = RegexOptions.IgnoreCase | RegexOptions.Singleline;
                if (provider.RelationsRightToLeft) ro |= RegexOptions.RightToLeft;
                MatchCollection mc = Regex.Matches(source, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                for(int i=0;i<mc.Count;i++)
                {
                    Match m=mc[i];
                    //if we are iterating through season pages, take season from page url directly
                    if (url != url2)
                    {
                        info.Relations.Add(new Relation(season.ToString(), m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        Helper.Log("Found Relation: " + "S" + season.ToString() + "E" + m.Groups["Episode"].Value + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), Helper.LogType.Debug);
                    }
                    else
                    {
                        info.Relations.Add(new Relation(m.Groups["Season"].Value, m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        Helper.Log("Found Relation: " + "S" + m.Groups["Season"].Value + "E" + m.Groups["Episode"].Value + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), Helper.LogType.Debug);
                    }
                }
                
                // THOU SHALL NOT FORGET THE BREAK
                if (!url2.Contains("%S")) break;
                season++;
            }
            Helper.Log("" + (season-1) + " Seasons, " + info.Relations.Count + " relations found", Helper.LogType.Debug);
        }
        #endregion
        #region Name Creation

        /// <summary>
        /// creates names for all entries using season, episode and name and the target pattern
        /// </summary>
        private void CreateNewNames()
        {
            for(int i=0;i<info.Episodes.Count;i++)
            {
                CreateNewName(i);
            }
        }

        /// <summary>
        /// creates single filename from season, episode and name from imdb
        /// </summary>
        /// <param name="item">Index of an InfoEntry from info.Episodes list to create a name from</param>
        private void CreateNewName(int item)
        {
            InfoEntry ie=info.Episodes[item];
            if (ie.Name != "")
            {
                //Target Filename format
                string name = txtTarget.Text;

                //Those 3 strings need case/Umlaut processing
                string epname = ie.Name;
                string seriesname = Helper.ReadProperties(Config.LastTitles)[0];
                string extension = Path.GetExtension(ie.Filename);

                name = name.Replace("%e", ie.Episode);
                string episode = ie.Episode;
                if (episode.Length == 1) episode = "0" + episode;
                name = name.Replace("%E", episode);
                name = name.Replace("%s", ie.Season);
                string season = ie.Season;
                if (season.Length == 1) season = "0" + season;
                name = name.Replace("%S", season);
                
                

                //treat umlaute and case
                if (Helper.ReadProperty(Config.Umlaute) == "Use")
                {
                    epname = epname.Replace("ae", "ä");
                    epname = epname.Replace("Ae", "Ä");
                    epname = epname.Replace("oe", "ö");
                    epname = epname.Replace("Oe", "Ö");
                    epname = epname.Replace("ue", "ü");
                    epname = epname.Replace("Ue", "Ü");
                    seriesname = seriesname.Replace("ae", "ä");
                    seriesname = seriesname.Replace("Ae", "Ä");
                    seriesname = seriesname.Replace("oe", "ö");
                    seriesname = seriesname.Replace("Oe", "Ö");
                    seriesname = seriesname.Replace("ue", "ü");
                    seriesname = seriesname.Replace("Ue", "Ü");
                    //extension too since we're generous ;)
                    extension = extension.Replace("ae", "ä");
                    extension = extension.Replace("Ae", "Ä");
                    extension = extension.Replace("oe", "ö");
                    extension = extension.Replace("Oe", "Ö");
                    extension = extension.Replace("ue", "ü");
                    extension = extension.Replace("Ue", "Ü");
                }
                else if (Helper.ReadProperty(Config.Umlaute) == "Dont_Use")
                {
                    epname = epname.Replace("ä", "ae");
                    epname = epname.Replace("Ä", "Ae");
                    epname = epname.Replace("ö", "oe");
                    epname = epname.Replace("Ö", "Oe");
                    epname = epname.Replace("ü", "ue");
                    epname = epname.Replace("Ü", "Ue");
                    epname = epname.Replace("ß", "ss");

                    epname = epname.Replace("É", "E");
                    epname = epname.Replace("È", "E");
                    epname = epname.Replace("Ê", "E");
                    epname = epname.Replace("Ë", "E");
                    epname = epname.Replace("Á", "A");
                    epname = epname.Replace("À", "A");
                    epname = epname.Replace("Â", "A");
                    epname = epname.Replace("Ã", "A");
                    epname = epname.Replace("Å", "A");
                    epname = epname.Replace("Í", "I");
                    epname = epname.Replace("Ì", "I");
                    epname = epname.Replace("Î", "I");
                    epname = epname.Replace("Ï", "I");
                    epname = epname.Replace("Ú", "U");
                    epname = epname.Replace("Ù", "U");
                    epname = epname.Replace("Û", "U");
                    epname = epname.Replace("Ó", "O");
                    epname = epname.Replace("Ò", "O");
                    epname = epname.Replace("Ô", "O");
                    epname = epname.Replace("Ý", "Y");
                    epname = epname.Replace("Ç", "C");

                    epname = epname.Replace("é", "e");
                    epname = epname.Replace("è", "e");
                    epname = epname.Replace("ê", "e");
                    epname = epname.Replace("ë", "e");
                    epname = epname.Replace("á", "a");
                    epname = epname.Replace("à", "a");
                    epname = epname.Replace("â", "a");
                    epname = epname.Replace("ã", "a");
                    epname = epname.Replace("å", "a");                    
                    epname = epname.Replace("í", "i");
                    epname = epname.Replace("ì", "i");
                    epname = epname.Replace("î", "i");
                    epname = epname.Replace("ï", "i");
                    epname = epname.Replace("ú", "u");
                    epname = epname.Replace("ù", "u");
                    epname = epname.Replace("û", "u");
                    epname = epname.Replace("ó", "o");
                    epname = epname.Replace("ò", "o");
                    epname = epname.Replace("ô", "o");
                    epname = epname.Replace("ý", "y");
                    epname = epname.Replace("ÿ", "y");
                    epname = epname.Replace("ç", "c");

                    seriesname = seriesname.Replace("ä", "ae");
                    seriesname = seriesname.Replace("Ä", "Ae");
                    seriesname = seriesname.Replace("ö", "oe");
                    seriesname = seriesname.Replace("Ö", "Oe");
                    seriesname = seriesname.Replace("ü", "ue");
                    seriesname = seriesname.Replace("Ü", "Ue");
                    seriesname = seriesname.Replace("ß", "ss");

                    seriesname = seriesname.Replace("É", "E");
                    seriesname = seriesname.Replace("È", "E");
                    seriesname = seriesname.Replace("Ê", "E");
                    seriesname = seriesname.Replace("Ë", "E");
                    seriesname = seriesname.Replace("Á", "A");
                    seriesname = seriesname.Replace("À", "A");
                    seriesname = seriesname.Replace("Â", "A");
                    seriesname = seriesname.Replace("Ã", "A");
                    seriesname = seriesname.Replace("Å", "A");
                    seriesname = seriesname.Replace("Í", "I");
                    seriesname = seriesname.Replace("Ì", "I");
                    seriesname = seriesname.Replace("Î", "I");
                    seriesname = seriesname.Replace("Ï", "I");
                    seriesname = seriesname.Replace("Ú", "U");
                    seriesname = seriesname.Replace("Ù", "U");
                    seriesname = seriesname.Replace("Û", "U");
                    seriesname = seriesname.Replace("Ó", "O");
                    seriesname = seriesname.Replace("Ò", "O");
                    seriesname = seriesname.Replace("Ô", "O");
                    seriesname = seriesname.Replace("Ý", "Y");
                    seriesname = seriesname.Replace("Ç", "C");

                    seriesname = seriesname.Replace("é", "e");
                    seriesname = seriesname.Replace("è", "e");
                    seriesname = seriesname.Replace("ê", "e");
                    seriesname = seriesname.Replace("ë", "e");
                    seriesname = seriesname.Replace("á", "a");
                    seriesname = seriesname.Replace("à", "a");
                    seriesname = seriesname.Replace("â", "a");
                    seriesname = seriesname.Replace("ã", "a");
                    seriesname = seriesname.Replace("å", "a");
                    seriesname = seriesname.Replace("í", "i");
                    seriesname = seriesname.Replace("ì", "i");
                    seriesname = seriesname.Replace("î", "i");
                    seriesname = seriesname.Replace("ï", "i");
                    seriesname = seriesname.Replace("ú", "u");
                    seriesname = seriesname.Replace("ù", "u");
                    seriesname = seriesname.Replace("û", "u");
                    seriesname = seriesname.Replace("ó", "o");
                    seriesname = seriesname.Replace("ò", "o");
                    seriesname = seriesname.Replace("ô", "o");
                    seriesname = seriesname.Replace("ý", "y");
                    seriesname = seriesname.Replace("ÿ", "y");
                    seriesname = seriesname.Replace("ç", "c");

                    extension = extension.Replace("ä", "ae");
                    extension = extension.Replace("Ä", "Ae");
                    extension = extension.Replace("ö", "oe");
                    extension = extension.Replace("Ö", "Oe");
                    extension = extension.Replace("ü", "ue");
                    extension = extension.Replace("Ü", "Ue");
                    extension = extension.Replace("ß", "ss");

                    extension = extension.Replace("É", "E");
                    extension = extension.Replace("È", "E");
                    extension = extension.Replace("Ê", "E");
                    extension = extension.Replace("Ë", "E");
                    extension = extension.Replace("Á", "A");
                    extension = extension.Replace("À", "A");
                    extension = extension.Replace("Â", "A");
                    extension = extension.Replace("Ã", "A");
                    extension = extension.Replace("Å", "A");
                    extension = extension.Replace("Í", "I");
                    extension = extension.Replace("Ì", "I");
                    extension = extension.Replace("Î", "I");
                    extension = extension.Replace("Ï", "I");
                    extension = extension.Replace("Ú", "U");
                    extension = extension.Replace("Ù", "U");
                    extension = extension.Replace("Û", "U");
                    extension = extension.Replace("Ó", "O");
                    extension = extension.Replace("Ò", "O");
                    extension = extension.Replace("Ô", "O");
                    extension = extension.Replace("Ý", "Y");
                    extension = extension.Replace("Ç", "C");

                    extension = extension.Replace("é", "e");
                    extension = extension.Replace("è", "e");
                    extension = extension.Replace("ê", "e");
                    extension = extension.Replace("ë", "e");
                    extension = extension.Replace("á", "a");
                    extension = extension.Replace("à", "a");
                    extension = extension.Replace("â", "a");
                    extension = extension.Replace("ã", "a");
                    extension = extension.Replace("å", "a");
                    extension = extension.Replace("í", "i");
                    extension = extension.Replace("ì", "i");
                    extension = extension.Replace("î", "i");
                    extension = extension.Replace("ï", "i");
                    extension = extension.Replace("ú", "u");
                    extension = extension.Replace("ù", "u");
                    extension = extension.Replace("û", "u");
                    extension = extension.Replace("ó", "o");
                    extension = extension.Replace("ò", "o");
                    extension = extension.Replace("ô", "o");
                    extension = extension.Replace("ý", "y");
                    extension = extension.Replace("ÿ", "y");
                    extension = extension.Replace("ç", "c");
                }
                if (Helper.ReadProperty(Config.Case) == "small")
                {
                    epname = epname.ToLower();
                    seriesname = seriesname.ToLower();
                    extension = extension.ToLower();
                }
                else if (Helper.ReadProperty(Config.Case) == "Large")
                {
                    Regex r = new Regex(@"\b(\w)(\w+)?\b",RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    epname = Helper.VISSpeak(epname);
                    seriesname = Helper.VISSpeak(seriesname);
                    extension = extension.ToLower();
                }
                else if (Helper.ReadProperty(Config.Case) == "CAPSLOCK")
                {
                    epname = epname.ToUpper();
                    extension = extension.ToUpper();
                    seriesname = seriesname.ToUpper();
                }

                //Now that series title, episode title and extension are properly processed, add them to the filename

                //Remove extension from target filename (if existant) and add properly cased one
                name = Regex.Replace(name, extension, "", RegexOptions.IgnoreCase);
                name += extension;

                name = name.Replace("%T", seriesname);
                name = name.Replace("%N", epname);

                //Invalid character replace
                if (Helper.ReadProperty(Config.InvalidCharReplace) != null && (Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction), Helper.ReadProperty(Config.InvalidFilenameAction)) == Helper.InvalidFilenameAction.Replace)
                {
                    string pattern = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";
                    ie.NewFileName = Regex.Replace(ie.NewFileName, pattern, Helper.ReadProperty(Config.InvalidCharReplace));
                }

                //set new filename if renaming process is required
                if (ie.Filename == name)
                {
                    ie.NewFileName = "";
                }
                else
                {
                    ie.NewFileName = name;
                }
            }
        }
        
        /// <summary>
        /// Creates subtitle destination and names subs when no show information is fetched yet, so they have the same name as their video files for better playback
        /// </summary>
        void RenameSubsToMatchMovies()
        {
            foreach (InfoEntry ie in info.Episodes)
            {
                if (info.IsSubtitle(ie))
                {
                    int season = -1;
                    int episode = -1;
                    try
                    {
                        Int32.TryParse(ie.Season, out season);
                        Int32.TryParse(ie.Episode, out episode);
                    }
                    catch (Exception)
                    {
                        Helper.Log("Couldn't Convert season or episode to int because string was garbled, too bad :P", Helper.LogType.Error);
                    }
                    List<InfoEntry> lie = info.GetMatchingVideos(season, episode);
                    if (lie != null && lie.Count == 1)
                    {
                        if (ie.NewFileName == "")
                        {
                            if (lie[0].NewFileName == "")
                            {
                                ie.NewFileName = Path.GetFileNameWithoutExtension(lie[0].Filename) + "." + ie.Extension;
                            }
                            else
                            {
                                ie.NewFileName = Path.GetFileNameWithoutExtension(lie[0].NewFileName) + "." + ie.Extension;
                            }

                            //Move to Video file
                            ie.Destination = lie[0].Destination;

                            //Don't do this again if name fits already
                            if (ie.NewFileName == ie.Filename)
                            {
                                ie.NewFileName = "";
                            }
                        }
                    }
                }
            }
        }
        #endregion
        
        /// <summary>
        /// Main Rename function
        /// </summary>
        private void Rename()
        {
            bool skip=false;
            bool skipall = (Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction),Helper.ReadProperty(Config.InvalidFilenameAction)) == Helper.InvalidFilenameAction.Skip;
            string replace = Helper.ReadProperty(Config.InvalidCharReplace);

            //Go through all files and do stuff
            for(int i=0;i<info.Episodes.Count;i++){
                InfoEntry ie=info.Episodes[i];
                if (ie.Process /*&& ie.Name != "" */&&((ie.Filename!=ie.NewFileName && ie.NewFileName!="")||(ie.Destination!=ie.Path && ie.Destination!="")))
                {
                    try
                    {
                        while(ie.NewFileName.IndexOfAny(Path.GetInvalidFileNameChars())>=0 && !skipall){
                            if((Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction),Helper.ReadProperty(Config.InvalidFilenameAction))==Helper.InvalidFilenameAction.Replace){
                                string pattern="["+Regex.Escape(new string(Path.GetInvalidFileNameChars()))+"]";
                                ie.NewFileName=Regex.Replace(ie.NewFileName,pattern, replace);
                            }else if((Helper.InvalidFilenameAction)Enum.Parse(typeof(Helper.InvalidFilenameAction),Helper.ReadProperty(Config.InvalidFilenameAction))==Helper.InvalidFilenameAction.Ask){
                                InvalidFilename td=new InvalidFilename(ie.NewFileName);
                                td.ShowDialog();
                                if(td.action==InvalidFilename.Action.SkipAll){
                                    skipall=true;
                                    if (td.remember)
                                    {
                                        Helper.WriteProperty(Config.InvalidFilenameAction, Helper.InvalidFilenameAction.Skip.ToString());
                                    }
                                    break;
                                }else if(td.action==InvalidFilename.Action.Skip){
                                    skip=true;
                                    break;
                                }else if(td.action==InvalidFilename.Action.Filename){
                                    ie.NewFileName=td.FileName;
                                }else if(td.action==InvalidFilename.Action.Replace){
                                    if (td.remember)
                                    {
                                        Helper.WriteProperty(Config.InvalidFilenameAction, Helper.InvalidFilenameAction.Replace.ToString());
                                        Helper.WriteProperty(Config.InvalidCharReplace, td.Replace);
                                    }
                                    replace=td.Replace;
                                    string pattern = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";
                                    ie.NewFileName = Regex.Replace(ie.NewFileName, pattern, replace);
                                }
                            }
                        }

                        //check for empty extension
                        if (ie.NewFileName!="" &&  Path.GetExtension(ie.NewFileName) == "")
                        {
                            if (MessageBox.Show(ie.Path + Path.DirectorySeparatorChar + ie.Filename + "->" + ie.Destination + Path.DirectorySeparatorChar + ie.NewFileName + " has no extension. Rename anyway?", "No extension", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                skip = true;
                            }
                        }

                        if (ie.NewFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 && (skip || skipall))
                        {
                            Helper.Log("Skipped "+ie.Path+Path.DirectorySeparatorChar+ie.Filename+"->"+ie.Destination+Path.DirectorySeparatorChar+ie.NewFileName+" because of illegal characters in new name.",Helper.LogType.Warning);
                            skip=false;
                            continue;
                        }
                        if (skipall || skip)
                        {
                            continue;
                        }

                        //create directory if needed
                        if (ie.Destination!="" && !Directory.Exists(ie.Destination))
                        {
                            Directory.CreateDirectory(ie.Destination);
                        }
                        //Move to desired destination      
                        if (ie.Destination != "")
                        {
                            if (ie.NewFileName != "")
                            {
                                File.Move(ie.Path + Path.DirectorySeparatorChar + ie.Filename, ie.Destination + Path.DirectorySeparatorChar + ie.NewFileName);
                            }
                            else
                            {
                                File.Move(ie.Path + Path.DirectorySeparatorChar + ie.Filename, ie.Destination + Path.DirectorySeparatorChar + ie.Filename);
                            }
                        }
                        else
                        {
                            if (ie.NewFileName != "")
                            {
                                File.Move(ie.Path + Path.DirectorySeparatorChar + ie.Filename, ie.Path + Path.DirectorySeparatorChar + ie.NewFileName);
                            }
                            else
                            {
                                File.Move(ie.Path + Path.DirectorySeparatorChar + ie.Filename, ie.Path + Path.DirectorySeparatorChar + ie.Filename);
                            }
                        }
                        //Delete empty folders code
                        if (Helper.ReadProperty(Config.DeleteEmptyFolders) == "1")
                        {
                            DeleteAllEmptyFolders(ie.Path, new List<string>(Helper.ReadProperties(Config.IgnoreFiles)));
                        }
                        if (ie.NewFileName != "")
                        {
                            ie.Filename = ie.NewFileName;
                        }
                        if (ie.Destination != "")
                        {
                            ie.Path = ie.Destination;
                        }
                        ie.Destination = "";
                        ie.NewFileName = "";
                    }
                    catch (Exception ex)
                    {
                        if(skipall){
                            Helper.Log("Skipping "+ie.Filename+":\r\n"+ex.Message,Helper.LogType.Warning);
                            continue;
                        }
                        Helper.Log(ie.Path + Path.DirectorySeparatorChar + ie.Filename + " -> " + ie.Destination + Path.DirectorySeparatorChar + ie.NewFileName + ": " + ex.Message, Helper.LogType.Error);
                    }
                }
            }
            if (Helper.ReadProperty(Config.DeleteAllEmptyFolders) == "1")
            {
                //Delete all empty folders code
                DeleteAllEmptyFolders(Helper.ReadProperty(Config.LastDirectory), new List<string>(Helper.ReadProperties(Config.IgnoreFiles)));
            }
            //Get a list of all involved folders
            FillListView();
        }
       
        /// <summary>
        /// Updatess list view and do lots of other connected stuff with it
        /// </summary>
        /// <param name="clear">if true, list is cleared first and unconnected subtitle files are scheduled to be renamed</param>
        /// <param name="KeepShowName">if set, show name isn't altered</param>
        public void UpdateList(bool clear, bool KeepShowName)
        {
            if (clear)
            {
                info.Episodes.Clear();                
            }
            
            //scan for files which got deleted so we can remove them
            for (int i = info.Episodes.Count-1; i >=0; i--)
            {
                InfoEntry ie = info.Episodes[i];
                if (!File.Exists(ie.Path + Path.DirectorySeparatorChar + ie.Name))
                {
                    info.Episodes.Remove(ie);
                    i--;
                }
            }

            string path = Helper.ReadProperty(Config.LastDirectory);
            bool CreateDirectoryStructure = Helper.ReadProperty(Config.CreateDirectoryStructure) == "1";
            bool UseSeasonSubdirs = Helper.ReadProperty(Config.UseSeasonSubDir) == "1";
            if (Directory.Exists(path))
            {
                //scan for new files
                List<string> extensions = new List<string>(Helper.ReadProperties(Config.Extensions));
                extensions.AddRange(Helper.ReadProperties(Config.SubtitleExtensions));
                if (extensions == null)
                {
                    Helper.Log("No File Extensions found!", Helper.LogType.Warning);
                    return;
                }
                for (int i = 0; i < extensions.Count; i++)
                {
                    extensions[i] = extensions[i].ToLower();
                }
                List<FileSystemInfo> Files = new List<FileSystemInfo>();
                foreach (string ex in extensions)
                {
                    List<FileSystemInfo> fsi = Helper.GetAllFilesRecursively(path, "*." + ex);
                    Files.AddRange(fsi);
                }
                string[] patterns = Helper.ReadProperties(Config.EpIdentifier);
                string name = "";

                //Season read from directory for counterchecking
                int DirectorySeason = -1;

                //Get name and season if possible
                ExtractFromDirectoryNames(path, ref name, ref DirectorySeason);
                if (name != "" && !KeepShowName)
                {
                    SetNewTitle(name);
                }                

                foreach (FileSystemInfo file in Files)
                {
                    //showname and season recognized from path
                    int SpecificSeason = -1;
                    string SpecificShowName= "";

                    //Check if there is already an entry on this file, and if not, create one
                    InfoEntry i=null;
                    bool contains = false;
                    foreach (InfoEntry ie in info.Episodes)
                    {
                        if (ie.Filename == file.Name)
                        {
                            i = ie;
                            contains = true;
                            break;
                        }
                    }
                    
                    if (i == null)
                    {
                        i = new InfoEntry();
                    }

                    //Set basic values
                    i.Filename = file.Name;
                    i.Path = Path.GetDirectoryName(file.FullName);
                    i.Extension = Path.GetExtension(file.FullName).ToLower().Replace(".","");

                    //Get season number and showname from directory
                    ExtractFromDirectoryNames(Path.GetDirectoryName(file.FullName), ref SpecificShowName, ref SpecificSeason);

                    //try to recognize season and episode from filename
                    foreach (string str in patterns)
                    {
                        //replace %S and %E by proper regexps
                        string pattern = str;                            
                       
                        //if a pattern containing %S%E is used, only use the first number for season
                        if (str.Contains("%S%E"))
                        {
                            pattern = pattern.Replace("%S", "(?<Season>\\d)");
                            pattern = pattern.Replace("%E", "(?<Episode>\\d+)");
                        }
                        else
                        {
                            pattern = pattern.Replace("%S", "(?<Season>\\d+)");
                            pattern = pattern.Replace("%E", "(?<Episode>\\d+)");
                        }

                        //Try to match. If it works, get the season and the episode from the match
                        Match m = Regex.Match(file.Name, pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                        if (m.Success)
                        {
                            string strSeason = "";
                            string strEpisode = "";
                            try
                            {
                                strSeason = Int32.Parse(m.Groups["Season"].Value).ToString();
                            }
                            catch (FormatException) { }
                            try
                            {
                                strEpisode = Int32.Parse(m.Groups["Episode"].Value).ToString();
                            }
                            catch (FormatException) { }

                            i.Episode = strEpisode;
                            i.Season = strSeason;

                            //if season recognized from directory name doesn't match season recognized from filename, the file might be located in a wrong directory
                            if (i.Season != SpecificSeason.ToString()&&SpecificSeason!=-1)
                            {
                                Helper.Log("File seems to be located inconsistently: " + i.Filename + " was recognized as season " + i.Season + ", but folder name indicates that it should be season " + SpecificSeason.ToString(), Helper.LogType.Warning);
                            }
                            
                            //Figure out where this file should be located
                            SetDestinationPath(i, path, CreateDirectoryStructure, UseSeasonSubdirs);
                                                            
                            break;
                        }
                    }

                    //if season number couldn't be extracted, try to get it from folder
                    //(this should never happen if a pattern like %S%E is set)
                    if (i.Season == "" && DirectorySeason != -1)
                    {
                        i.Season = DirectorySeason.ToString();
                    }

                    //if not added yet, add it
                    if (!contains)
                    {
                        info.Episodes.Add(i);
                    }

                    //if names should be created and there is some info avaiable, go ahead
                    if (info.Relations.Count > 0)
                    {
                        SetupRelations();
                    }                    
                }
                DecideWhatShouldBeProcessed(path,Helper.ReadProperties(Config.LastTitles)[0]);
            }

            if (clear)
            {
                RenameSubsToMatchMovies();
            }

            
            FillListView();

            //also update some gui elements for the sake of it
            txtTarget.Text = Helper.ReadProperty(Config.TargetPattern);
            txtPath.Text = Helper.ReadProperty(Config.LastDirectory);
            string LastProvider = Helper.ReadProperty(Config.LastProvider);
            if (LastProvider == null) LastProvider = "";
            cbProviders.SelectedIndex = Math.Max(0, cbProviders.Items.IndexOf(LastProvider));
            string LastSubProvider = Helper.ReadProperty(Config.LastSubProvider);
            if (LastSubProvider == null) LastSubProvider = "";
            cbSubs.SelectedIndex = Math.Max(0, cbSubs.Items.IndexOf(LastSubProvider));
        }
        #endregion
        #region Subtitles
        #region Parsing
        /// <summary>
        /// Main subtitle acquisition function
        /// </summary>
        private void GetSubtitles()
        {
            if (Settings.MonoCompatibilityMode)
            {
                Helper.Log("Subtitle downloading is not supported in Mono, since additional dlls for unpacking are required which won't work here :(", Helper.LogType.Warning);
                return;
            }
            info.SubtitleLinks.Clear();
            // request
            SubtitleProvider subprovider = info.GetCurrentSubtitleProvider();
            if (subprovider == null)
            {
                Helper.Log("No subtitle provider found/selected", Helper.LogType.Error);
                return;
            }
            string url = subprovider.SearchURL;
            if (url == null || url == "")
            {
                Helper.Log("Can't search because no search URL is specified for this subtitle provider", Helper.LogType.Error);
                return;
            }
            url = url.Replace("%T", Helper.ReadProperties(Config.LastTitles)[0]);
            url = System.Web.HttpUtility.UrlPathEncode(url);
            HttpWebRequest requestHtml;
            try
            {
                requestHtml = (HttpWebRequest)(HttpWebRequest.Create(url));
            }
            catch (Exception ex)
            {
                Helper.Log(ex.Message, Helper.LogType.Error);
                return;
            }
            Helper.Log("Searching at " + url.Replace(" ", "%20"), Helper.LogType.Status);
            requestHtml.Timeout = Convert.ToInt32(Helper.ReadProperty(Config.Timeout));
            // get response
            HttpWebResponse responseHtml = null;
            try
            {
                responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
            }
            catch (Exception ex)
            {
                Helper.Log(ex.Message, Helper.LogType.Error);
                if (responseHtml != null)
                    responseHtml.Close();
                return;
            }
            //if search engine directs us straight to the result page, skip parsing search results
            string seriesURL = subprovider.SeriesURL;
            if (responseHtml.ResponseUri.AbsoluteUri.Contains(seriesURL))
            {
                Helper.Log("Search engine forwarded directly to single result: " + responseHtml.ResponseUri.AbsoluteUri.Replace(" ", "%20") + subprovider.SubtitlesURL.Replace(" ", "%20"), Helper.LogType.Status);
                GetSubtitleFromSeriesPage(responseHtml.ResponseUri.AbsoluteUri + subprovider.SubtitlesURL);
            }
            else
            {
                
                // and download
                StreamReader r = null;
                try
                {
                    r = new StreamReader(responseHtml.GetResponseStream());
                }
                catch (Exception ex)
                {
                    if (r != null)
                        r.Close();
                    Helper.Log(ex.Message, Helper.LogType.Error);
                    return;
                }
                string source = r.ReadToEnd();
                r.Close();


                //Source cropping
                source = source.Substring(Math.Max(source.IndexOf(subprovider.SearchStart),0));
                source = source.Substring(0, Math.Max(source.LastIndexOf(subprovider.SearchEnd),0));
                
                ParseSubtitleSearch(ref source, responseHtml.ResponseUri.AbsoluteUri);
            }
            int i;
            if (info.SubtitleLinks.Count > 0)
            {
                i=DownloadSubtitles();
                ProcessSubtitles(i);
                FillListView();
                string folder = Helper.ReadProperty(Config.LastDirectory)+"TEMP" + i.ToString();
            }
            
            responseHtml.Close();
        }

        /// <summary>
        /// Subtitle Search Result Parsing function.
        /// Extracts search results (i.e. Show names) and gets links to them.
        /// If more than one show is found, user gets to select one, otherwise he will be directly forwarded
        /// For now only pages where search links directly to subtitles work
        /// </summary>
        /// <param name="source">HTML Source of the search results page</param>
        /// <param name="SourceURL">URL of the source</param>
        private void ParseSubtitleSearch(ref string source, string SourceURL)
        {
            if (source == "") return;
            SubtitleProvider subprovider = info.GetCurrentSubtitleProvider();
            string pattern = subprovider.SearchRegExp;
            RegexOptions ro = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            if (subprovider.SearchRightToLeft) ro |= RegexOptions.RightToLeft;
            MatchCollection mc = Regex.Matches(source, pattern, ro);
            /*foreach(Match m in mc){
                MessageBox.Show("Match: "+m.Value+"\r\nName: "+m.Groups["name"].Value+"\r\nyear: "+m.Groups["year"].Value+"\r\nlink: "+m.Groups["link"].Value);
            }*/
            if (mc.Count == 0)
            {
                Helper.Log("No results found", Helper.LogType.Info);
            }
            else if (mc.Count == 1)
            {
                string url = subprovider.SubtitlesPage;
                url = url.Replace("%L", mc[0].Groups["link"].Value);
                if (subprovider.ConstructLink != "")
                {
                    ConstructLinks(mc[0].Groups["link"].Value);
                }
                else
                {
                    GetSubtitleFromSeriesPage(url);
                }
            }
            else
            {
                Helper.Log("Search engine found multiple results at " + SourceURL.Replace(" ", "%20"), Helper.LogType.Info);
                SelectResult sr = new SelectResult(mc,subprovider, true);
                if(sr.ShowDialog()==DialogResult.Cancel) return;
                if (sr.urls.Count == 0) return;
                foreach (string str in sr.urls)
                {
                    string url = subprovider.SubtitlesPage;
                    url = url.Replace("%L", str);
                    if (subprovider.ConstructLink != "")
                    {
                        ConstructLinks(str);
                    }
                    else
                    {
                        GetSubtitleFromSeriesPage(url);
                    }
                }
            }
        }
        
        /// <summary>
        /// Used if the download link(s) can be constructed directly from the search results page
        /// %L gets replaced with the value aquired from Search results page "link" property, 
        /// %P will allow to iterate over pages/seasons etc
        /// </summary>
        /// <param name="extracted">Extracted value from search results which is inserted into "ConstructLink" url</param>
        private void ConstructLinks(string extracted)
        {
            SubtitleProvider subprovider = info.GetCurrentSubtitleProvider();
            string link = subprovider.ConstructLink;
            link=link.Replace("%L", extracted);
            int loop = 1;
            if (link.Contains("%P"))
            {
                loop = 20;
            }
            //TODO: Make 20 setable somewhere or find better cancel condition
            for (int i = 1; i < loop+1; i++)
            {
                string anotherlink = link.Replace("%P", i.ToString());
                anotherlink = System.Web.HttpUtility.UrlPathEncode(anotherlink);
                HttpWebRequest requestHtml;
                try
                {
                    requestHtml = (HttpWebRequest)(HttpWebRequest.Create(anotherlink));
                }
                catch (Exception ex)
                {
                    Helper.Log(ex.Message, Helper.LogType.Error);
                    return;
                }
                requestHtml.Timeout = Convert.ToInt32(Helper.ReadProperty(Config.Timeout));
                // get response
                HttpWebResponse responseHtml = null;
                try
                {
                    responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
                }
                catch (Exception ex)
                {
                    Helper.Log(ex.Message, Helper.LogType.Error);
                    if (responseHtml != null)
                        responseHtml.Close();
                    return;
                }

                responseHtml.Close();
                if (subprovider.NotFoundURL == "" || responseHtml.ResponseUri.ToString() != subprovider.NotFoundURL)
                {
                    info.SubtitleLinks.Add(responseHtml.ResponseUri.ToString());
                }
            }
            
        }
        /// <summary>
        /// This function is needed if Subtitle links are located on a series page, not implemented yet
        /// </summary>
        /// <param name="url">URL of the page to get subtitle links from</param>
        private void GetSubtitleFromSeriesPage(string url)
        {
            //Don't forget the cropping
            //
            //Source cropping
            //source = source.Substring(Math.Max(source.IndexOf(provider.SearchStart),0));
            //source = source.Substring(0, Math.Max(source.LastIndexOf(provider.SearchEnd),0));
            /*
            //if episode infos are stored on a new page for each season, this should be marked with %S in url, so we can iterate through all those pages
            int season = 1;
            string url2 = url;
            while (true)
            {
                if (url2.Contains("%S"))
                {
                    url = url2.Replace("%S", season.ToString());
                }
                if (url == null || url == "") return;
                // request
                url = System.Web.HttpUtility.UrlPathEncode(url);
                HttpWebRequest requestHtml = null;
                try
                {
                    requestHtml = (HttpWebRequest)(HttpWebRequest.Create(url));
                }
                catch (Exception ex)
                {
                    Helper.Log(ex.Message, Helper.LogType.Error);
                    return;
                }
                requestHtml.Timeout = 5000;
                // get response
                HttpWebResponse responseHtml;
                try
                {
                    responseHtml = (HttpWebResponse)(requestHtml.GetResponse());
                }
                catch (WebException e)
                {
                    Helper.Log(e.Message, Helper.LogType.Error);
                    return;
                }
                //if we get redirected, lets assume this page does not exist
                if (responseHtml.ResponseUri.AbsoluteUri != url)
                {
                    break;
                }
                // and download
                //Helper.Log("charset=" + responseHtml.CharacterSet, Helper.LogType.Status);

                StreamReader r = new StreamReader(responseHtml.GetResponseStream(), Encoding.GetEncoding(responseHtml.CharacterSet));
                string source = r.ReadToEnd();
                r.Close();
                responseHtml.Close();
                string pattern = Settings.subprovider.RelationsRegExp;
                MatchCollection mc = Regex.Matches(source, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in mc)
                {
                    //if we are iterating through season pages, take season from page url directly
                    if (url != url2)
                    {
                        info.Relations.Add(new Relation(season.ToString(), m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        //Helper.Log("Found Relation: " + "S" + season.ToString() + "E" + m.Groups["Episode"].Value + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), Helper.LogType.Status);
                    }
                    else
                    {
                        info.Relations.Add(new Relation(m.Groups["Season"].Value, m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        //Helper.Log("Found Relation: " + "S" + m.Groups["Season"].Value + "E" + m.Groups["Episode"].Value + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), Helper.LogType.Status);
                    }
                }
                // THOU SHALL NOT FORGET THE BREAK
                if (!url2.Contains("%S")) break;
                season++;
            }*/
        }
        #endregion

        /// <summary>
        /// Downloads all collected subtitle links in a temp directory
        /// </summary>
        /// <returns></returns>
        private int DownloadSubtitles()
        {
            //find empty temp dir
            int i = 0;
            string folder = "TEMP" + i.ToString();
            while (Directory.Exists(Helper.ReadProperty(Config.LastDirectory) + folder))
            {
                i++;
                folder = "TEMP" + i.ToString();
            }
            Directory.CreateDirectory(Helper.ReadProperty(Config.LastDirectory) + folder);
            foreach (string url in info.SubtitleLinks)
            {
                WebClient Client = new WebClient();
                string target=Helper.ReadProperty(Config.LastDirectory) + folder+Path.DirectorySeparatorChar+Path.GetFileName(url);
                Client.DownloadFile(url, target);
            }
            return i;
        }
        
        /// <summary>
        /// Extracts all downloaded archives and moves subtitles to the movie files with proper naming
        /// </summary>
        /// <param name="i">Index of the temporary directory in which subtitles are stored. Temp Directory Name is "TEMP"+i</param>
        private void ProcessSubtitles(int i)
        {
            string folder = Helper.ReadProperty(Config.LastDirectory)+"TEMP" + i.ToString();
            List<string> extensions = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
            for (int a = 0; a < extensions.Count; a++)
            {
                extensions[a] = extensions[a].ToLower();
            }
            if (extensions == null)
            {
                Helper.Log("No Subtitle Extensions found!", Helper.LogType.Warning);
                return;
            }

            if (Directory.Exists(folder))
            {
                //extract downloaded archives
                List<string> archives =new List<string>();
                archives.AddRange(Directory.GetFiles(folder, "*.zip"));
                archives.AddRange(Directory.GetFiles(folder, "*.rar"));
                if (archives.Count > 0)
                {
                    Unrar unrar=null;
                    foreach (string file in archives)
                    {
                        if (Path.GetExtension(file).ToLower() == ".rar")
                        {
                            try
                            {


                                // Create new unrar class and attach event handlers for
                                // progress, missing volumes, and password
                                unrar = new Unrar();
                                //AttachHandlers(unrar);

                                // Set destination path for all files
                                unrar.DestinationPath = folder;

                                // Open archive for extraction
                                unrar.Open(file, Unrar.OpenMode.Extract);

                                // Extract each file with subtitle extension
                                while (unrar.ReadHeader())
                                {

                                    string extension = Path.GetExtension(unrar.CurrentFile.FileName).Substring(1).ToLower().Replace(".","");
                                    if (extensions.Contains(extension))
                                    {
                                        unrar.Extract();
                                    }
                                    else
                                    {
                                        unrar.Skip();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                if (unrar != null)
                                    unrar.Close();
                            }
                        }
                        else
                        {
                            using (ZipInputStream s = new ZipInputStream(File.OpenRead(file)))
                            {

                                ZipEntry theEntry;
                                while ((theEntry = s.GetNextEntry()) != null)
                                {

                                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                                    string fileName = Path.GetFileName(theEntry.Name);
                                    string extension = Path.GetExtension(theEntry.Name).Substring(1).ToLower().Replace(".","");

                                    //put it all in one dir!                				
                                    if (fileName != String.Empty && extensions.Contains(extension))
                                    {
                                        using (FileStream streamWriter = File.Create(folder+Path.DirectorySeparatorChar+fileName))
                                        {
                                            int size = 2048;
                                            byte[] data = new byte[2048];
                                            while (true)
                                            {
                                                size = s.Read(data, 0, data.Length);
                                                if (size > 0)
                                                {
                                                    streamWriter.Write(data, 0, size);
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //now that everything is extracted, try to assign subtitles to episodes                
                //first, figure out episode and season numbers from filenames

                //scan for subtitle files in temp folder             
                List<FileSystemInfo> Files = new List<FileSystemInfo>();
                foreach (string ex in extensions)
                {
                    List<FileSystemInfo> fsi = Helper.GetAllFilesRecursively(folder, "*." + ex);
                    Files.AddRange(fsi);
                }
                string[] patterns = Helper.ReadProperties(Config.EpIdentifier);
                foreach (FileSystemInfo file in Files)
                {
                    string Season = "";
                    string Episode = "";
                    foreach (string str in patterns)
                    {
                        //replace %S and %E by proper regexps
                        string pattern = null;
                        if (str.Contains("%S%E"))
                        {
                            pattern = str.Replace("%S", "(?<Season>\\d)");
                            pattern = pattern.Replace("%E", "(?<Episode>\\d+)");
                        }
                        else
                        {
                            pattern = str.Replace("%S", "(?<Season>\\d+)");
                            pattern = pattern.Replace("%E", "(?<Episode>\\d+)");
                        }
                        Match m = Regex.Match(file.Name, pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                        if (m.Success)
                        {
                            
                            try
                            {
                                Season = Int32.Parse(m.Groups["Season"].Value).ToString();
                            }
                            catch (FormatException) { }
                            try
                            {
                                Episode = Int32.Parse(m.Groups["Episode"].Value).ToString();
                            }
                            catch (FormatException) { }                                
                            break;
                        }
                    }

                    //now that season and episode are known, assign the filename to a subtitlefile object
                    bool contains = false;
                    foreach (SubtitleFile s in info.SubtitleFiles)
                    {
                        if (s.Episode == Episode &&s.Season==Season && Season != "" && Episode != "")
                        {
                            s.Filenames.Add(file.Name);
                            contains = true;
                        }
                    }
                    if(!contains){
                        SubtitleFile sf=new SubtitleFile();
                        sf.Episode=Episode;
                        sf.Season=Season;
                        sf.Filenames.Add(file.Name);
                        info.SubtitleFiles.Add(sf);
                    }
                }
                int MatchedSubtitles = 0;
                //Move subtitle files to their video files
                foreach (InfoEntry ie in info.Episodes)
                {
                    List<string> ext = new List<string>(Helper.ReadProperties(Config.Extensions));
                    for (int b = 0; b < ext.Count; b++)
                    {
                        ext[b] = ext[b].ToLower();
                    }
                    if (ext.Contains(Path.GetExtension(ie.Filename).Substring(1).ToLower()) && ie.Process && ie.Episode != "" && ie.Season != "")
                    {
                        foreach (SubtitleFile sf in info.SubtitleFiles)
                        {
                            if (sf.Season == ie.Season && sf.Episode == ie.Episode)
                            {
                                bool move = false;
                                string source = "";
                                string target = ie.Path + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(ie.Filename) + Path.GetExtension(sf.Filenames[0]);
                                if (sf.Filenames.Count == 1)
                                {
                                    move=true;
                                    source=folder + Path.DirectorySeparatorChar + sf.Filenames[0];
                                }
                                else
                                {
                                    FileSelector fs = new FileSelector(sf.Filenames);
                                    if (fs.ShowDialog() == DialogResult.OK)
                                    {
                                        move=true;
                                        source=folder + Path.DirectorySeparatorChar + sf.Filenames[fs.selection];
                                    }
                                }
                                
                                if(File.Exists(target)){
                                    if(MessageBox.Show(target+" already exists. Overwrite?", "Overwrite?",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button1)==DialogResult.Yes){
                                        File.Delete(target);
                                    }else{
                                        move=false;
                                    }
                                }
                                if(move){
                                    try
                                    {
                                        File.Copy(source, target);
                                        MatchedSubtitles++;
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.Log(source+" --> "+target+": "+ex.Message, Helper.LogType.Error);
                                    }
                                }
                            }
                        }
                    }
                }
                Helper.Log("Downloaded " + Files.Count + " subtitles and matched " + MatchedSubtitles + " of them.", Helper.LogType.Status);
                //cleanup
                info.SubtitleFiles.Clear();
                Directory.Delete(folder, true);
                UpdateList(true, true);
            }
        }
        #endregion
        #region LstFilesEvents
        //Update Coloring when file is checked/unchecked and set process flag
        private void lstFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            info.Episodes[(int)e.Item.Tag].Process = e.Item.Checked;
            Colorize(e.Item);
        }

        //Since sorting after the last two selected columns is supported, we need some event handling here
        private void lstFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lstFiles.Sort();
        }

        //End editing with combo box data types
        private void cbEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstFiles.EndEditing(true);
        }
        
        //Start editing single values
        private void lstFiles_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            if (e.SubItem != 0 && e.SubItem != 1)
            {
                if (Settings.MonoCompatibilityMode)
                {
                    Helper.Log("Editing Entries dynamically is not supported in Mono unfortunately :(", Helper.LogType.Warning);
                    return;
                }
                if (e.SubItem == 4)
                {
                    //if season is valid and there are relations at all, show combobox. Otherwise, just show edit box
                    if (info.Relations.Count > 0 && Convert.ToInt32(e.Item.SubItems[2].Text) >= info.FindMinSeason() && Convert.ToInt32(e.Item.SubItems[2].Text) <= info.FindMaxSeason())
                    {
                        comEdit.Items.Clear();
                        foreach (Relation rel in info.Relations)
                        {
                            if (rel.Season == info.Episodes[(int)e.Item.Tag].Season)
                            {
                                comEdit.Items.Add(rel.Name);
                            }
                        }
                        lstFiles.StartEditing(comEdit, e.Item, e.SubItem);
                    }
                    else
                    {
                        lstFiles.StartEditing(txtEdit, e.Item, e.SubItem);
                    }
                }
                else if (e.SubItem == 5 || e.SubItem == 6)
                {
                    lstFiles.StartEditing(txtEdit, e.Item, e.SubItem);
                }
                else
                {
                    //clamp season and episode values to allowed values
                    if (info.Relations.Count > 0)
                    {
                        if (e.SubItem == 2)
                        {
                            numEdit.Minimum = info.FindMinSeason();
                            numEdit.Maximum = info.FindMaxSeason();
                        }
                        else if (e.SubItem == 3)
                        {
                            numEdit.Minimum = info.FindMinEpisode(Convert.ToInt32(info.Episodes[(int)e.Item.Tag].Season));
                            numEdit.Maximum = info.FindMaxEpisode(Convert.ToInt32(info.Episodes[(int)e.Item.Tag].Season));
                        }
                    }
                    else
                    {
                        numEdit.Minimum = 0;
                        numEdit.Maximum = 10000;
                    }
                    lstFiles.StartEditing(numEdit, e.Item, e.SubItem);                    
                }
            }
        }
        
        //End editing a value, apply possible changes and process them
        private void lstFiles_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            //add lots of stuff here
            switch (e.SubItem)
            {
                //season
                case 2:
                    info.Episodes[(int)e.Item.Tag].Season = e.DisplayText;
                    SetupRelation((int)e.Item.Tag);
                    //lstFiles.Sort();
                    break;
                //Episode
                case 3:
                    info.Episodes[(int)e.Item.Tag].Episode = e.DisplayText;
                    SetupRelation((int)e.Item.Tag);
                    //lstFiles.Sort();
                    break;
                //name
                case 4:
                    //backtrack to see if entered text matches a season/episode
                    foreach (Relation rel in info.Relations)
                    {
                        //if found, set season and episode in gui and sync back to data
                        if (e.DisplayText == rel.Name)
                        {
                            e.Item.SubItems[2].Text = rel.Season;
                            e.Item.SubItems[3].Text = rel.Episode;
                        }
                    }
                    e.Item.SubItems[4].Text = e.DisplayText;
                    SyncItem((int)e.Item.Tag, true);
                    CreateNewName((int)e.Item.Tag);
                    break;
                //Filename
                case 5:
                    info.Episodes[(int)e.Item.Tag].NewFileName = e.DisplayText;
                    break;
                //Destination
                case 6:
                    try
                    {
                        Path.GetDirectoryName(e.DisplayText);
                        info.Episodes[(int)e.Item.Tag].Destination = e.DisplayText;
                    }
                    catch (Exception) {
                        e.Cancel = true;
                    }
                    break;
                default:
                    throw new Exception("Unreachable code");
                }
            SyncItem((int)e.Item.Tag, false);
            Colorize(e.Item);
        }

        //Double click = Invert process flag
        private void lstFiles_DoubleClick(object sender, EventArgs e)
        {
            lstFiles.Items[lstFiles.SelectedIndices[0]].Checked = !lstFiles.Items[lstFiles.SelectedIndices[0]].Checked;
        }

        //Enter = Open file
        private void lstFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                foreach (ListViewItem lvi in lstFiles.SelectedItems)
                {
                    Process myProc = Process.Start(info.Episodes[(int)lvi.Tag].Path + Path.DirectorySeparatorChar + info.Episodes[(int)lvi.Tag].Filename);
                }
            }
        }
        #endregion
        #region GUI-Events
        //Main Initialization
        private void Form1_Load(object sender, EventArgs e)
        {
            //mono compatibility fixes
            if (Type.GetType("Mono.Runtime") != null)
            {
                Helper.LogDisplay = txtLog;
                rtbLog.Visible = false;
                txtLog.Visible = true;
                Helper.Log("Running on Mono", Helper.LogType.Info);
                Settings.MonoCompatibilityMode = true;
            }
            else
            {
                Helper.LogDisplay = rtbLog;
            }
            settings = new Settings();
            //create config file if not existant from defaults
            if (!File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName))
            {
                ConfigFile f = new ConfigFile("");
                f.variables = ((Hashtable)Settings.Defaults.variables.Clone());
                f.FilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Settings.ConfigName;
                f.Flush();
            }
            //and read a value to make sure it is loaded into memory
            Helper.ReadProperty(Config.Case);
            Settings.ConfigLoaded = true;

            Helper.ClearLog();
            info = new Info();
            lstFiles.ListViewItemSorter = lvwColumnSorter;
            txtTarget.Text = Helper.ReadProperty(Config.TargetPattern);

            //relations provider combo box
            foreach (RelationProvider rel in info.Providers)
            {
                cbProviders.Items.Add(rel.Name);
            }
            string LastProvider = Helper.ReadProperty(Config.LastProvider);
            if (LastProvider == null) LastProvider = "";
            cbProviders.SelectedIndex = Math.Max(0, cbProviders.Items.IndexOf(LastProvider));
            RelationProvider provider = info.GetCurrentProvider();
            if (provider == null)
            {
                Helper.Log("No relation provider found/selected", Helper.LogType.Error);
                return;
            }
            Helper.WriteProperty(Config.LastProvider, cbProviders.Text);

            //subtitle provider combo box
            foreach (SubtitleProvider sub in info.SubProviders)
            {
                cbSubs.Items.Add(sub.Name);
            }
            string LastSubProvider = Helper.ReadProperty(Config.LastSubProvider);
            if (LastSubProvider == null) LastSubProvider = "";
            cbSubs.SelectedIndex = Math.Max(0, cbSubs.Items.IndexOf(LastSubProvider));
            Helper.WriteProperty(Config.LastSubProvider, cbSubs.Text);

            //Last title
            string[] LastTitles = Helper.ReadProperties(Config.LastTitles);
            for (int i = 0; i < Math.Min(LastTitles.Length, Helper.ReadInt(Config.TitleHistorySize)); i++)
            {
                cbTitle.Items.Add(LastTitles[i]);
            }
            cbTitle.SelectedIndex = -1;

            //Last directory
            string lastdir = Helper.ReadProperty(Config.LastDirectory);

            //First argument=folder
            if (args.Count > 0)
            {
                string dir = args[0].Replace("\"", "");
                if (Directory.Exists(args[0]))
                {
                    lastdir = dir;
                    Helper.WriteProperty(Config.LastDirectory, lastdir);
                }
            }
            if (lastdir != null && lastdir != "" && Directory.Exists(lastdir))
            {
                txtPath.Text = lastdir;
                Environment.CurrentDirectory = lastdir;
                UpdateList(true, false);
            }

            
            string[] ColumnWidths=Helper.ReadProperties(Config.ColumnWidths);
            string[] ColumnOrder=Helper.ReadProperties(Config.ColumnOrder);
            for (int i = 0; i < lstFiles.Columns.Count; i++)
            {
                try{
                    int width = lstFiles.Columns[i].Width;
                    Int32.TryParse(ColumnWidths[i], out width);
                    lstFiles.Columns[i].Width = width;
                }catch(Exception){
                    Helper.Log("Invalid Value for ColumnWidths["+i+"]: "+ColumnWidths[i],Helper.LogType.Error);
                }
                try
                {
                    int order = lstFiles.Columns[i].DisplayIndex;
                    Int32.TryParse(ColumnOrder[i], out order);
                    lstFiles.Columns[i].DisplayIndex = order;
                }
                catch (Exception)
                {
                    Helper.Log("Invalid Value for ColumnOrder[" + i + "]: " + ColumnOrder[i], Helper.LogType.Error);
                }
            }
            string[] Windowsize = Helper.ReadProperties(Config.WindowSize);
            if (Windowsize.GetLength(0) >= 2)
            {
                try
                {
                    int w, h;
                    Int32.TryParse(Windowsize[0], out w);
                    Int32.TryParse(Windowsize[1], out h);
                    this.Width = w;
                    this.Height = h;
                }
                catch (Exception)
                {
                    Helper.Log("Couldn't process WindowSize property: " + Helper.ReadProperty(Config.WindowSize), Helper.LogType.Error);
                }
                //focus fix
                cbTitle.Select(0,0);
                txtPath.Focus();
                txtPath.Select(txtPath.Text.Length, 0);
            }
        }
        
        //Auto column resize by storing column width ratios at resize start
        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            columnsizes = new float[]{
                (float)(lstFiles.Columns[0].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[1].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[2].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[3].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[4].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[5].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[6].Width)/(float)(lstFiles.ClientRectangle.Width),};
            float sum=0;
            for (int i = 0; i < 7; i++)
            {
                sum += columnsizes[i];
            }
            //some numeric correction to make ratios:
            for (int i = 0; i < 7; i++)
            {
                columnsizes[i]*=(float)1/sum;
            }                
        }
        
        //Auto column resize, restore Column width ratios at resize end (to make sure!)
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (lstFiles != null && lstFiles.Columns.Count == 7&&columnsizes!=null)
            {
                lstFiles.Columns[0].Width = (int)(columnsizes[0] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[1].Width = (int)(columnsizes[1] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[2].Width = (int)(columnsizes[2] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[3].Width = (int)(columnsizes[3] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[4].Width = (int)(columnsizes[4] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[5].Width = (int)(columnsizes[5] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[6].Width = (int)(columnsizes[5] * (float)(lstFiles.ClientRectangle.Width));
            }
        }

        //Auto column resize, restore Column width ratios during resize
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (lstFiles != null && lstFiles.Columns.Count == 6&&columnsizes!=null)
            {
                lstFiles.Columns[0].Width = (int)(columnsizes[0] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[1].Width = (int)(columnsizes[1] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[2].Width = (int)(columnsizes[2] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[3].Width = (int)(columnsizes[3] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[4].Width = (int)(columnsizes[4] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[5].Width = (int)(columnsizes[5] * (float)(lstFiles.ClientRectangle.Width));
                lstFiles.Columns[6].Width = (int)(columnsizes[6] * (float)(lstFiles.ClientRectangle.Width));
            }
        }

        //Save last focussed control so it can be restored after splitter between file and log window is moved
        private void scContainer_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the focused control before the splitter is focused
            focused = getFocused(this.Controls);
        }

        //restore last focussed control so splitter doesn't keep focus
        private void scContainer_MouseUp(object sender, MouseEventArgs e)
        {
            // If a previous control had focus
            if (focused != null)
            {
                // Return focus and clear the temp variable for
                // garbage collection (is this needed?)
                focused.Focus();
                focused = null;
            }
        }        

        //Update Current Provider setting
        private void cbProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            Helper.WriteProperty(Config.LastProvider, cbProviders.SelectedItem.ToString());
        }

        //Update Current Subtitle Provider setting
        private void cbSubs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Helper.WriteProperty(Config.LastSubProvider, cbSubs.SelectedItem.ToString());
        }

        //Show About box dialog
        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBoxDemo.AboutBox ab = new AboutBoxDemo.AboutBox();
            ab.AppMoreInfo += Environment.NewLine;
            ab.AppMoreInfo += "Using:" + Environment.NewLine;
            ab.AppMoreInfo += "ListViewEx " + "http://www.codeproject.com/KB/list/ListViewCellEditors.aspx" + Environment.NewLine;
            ab.AppMoreInfo += "About Box " + "http://www.codeproject.com/KB/vb/aboutbox.aspx" + Environment.NewLine;
            ab.AppMoreInfo += "Unrar.dll " + "http://www.rarlab.com" + Environment.NewLine;
            ab.AppMoreInfo += "SharpZipLib " + "http://www.icsharpcode.net/OpenSource/SharpZipLib/" + Environment.NewLine;
            ab.ShowDialog(this);
        }
        
        //Open link in browser when clicked in log
        private void rtbLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process myProc = Process.Start(e.LinkText);
            }
            catch (Exception ex)
            {
                Helper.Log("Couldn't open " + e.LinkText + ":" + ex.Message, Helper.LogType.Error);
            }
        }
        
        //Update Destination paths when title of show is changed
        private void cbTitle_TextChanged(object sender, EventArgs e)
        {
            //when nothing is selected, assume there is no autocompletion taking place right now
            if (cbTitle.SelectedText == "" && cbTitle.Text != "")
            {
                for (int i = 0; i < cbTitle.Items.Count; i++)
                {
                    if (((string)cbTitle.Items[i]).ToLower() == cbTitle.Text.ToLower())
                    {
                        bool found = false;
                        foreach (object o in cbTitle.Items)
                        {
                            if ((string)o == cbTitle.Text)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            cbTitle.Items.Add(cbTitle.Text);
                        }
                        break;
                    }
                }
            }
        }

        //Clear episode informations fetched from providers
        private void btnClear_Click(object sender, EventArgs e)
        {
            info.Relations.Clear();
            UpdateList(true, false);
        }
        
        //Show File Open dialog and update file list
        private void btnPath_Click(object sender, EventArgs e)
        {
            //weird mono hackfix
            if (Settings.MonoCompatibilityMode)
            {
                fbdPath.SelectedPath = Environment.CurrentDirectory;
            }
            string lastdir = Helper.ReadProperty(Config.LastDirectory);
            if (!Settings.MonoCompatibilityMode)
            {
                if (lastdir != null && lastdir != "" && Directory.Exists(lastdir))
                {
                    fbdPath.SelectedPath = lastdir;
                }
                else
                {
                    fbdPath.SelectedPath = Environment.CurrentDirectory;
                }
            }

            DialogResult dr = fbdPath.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SetPath(fbdPath.SelectedPath);
            }
        }

        //Show configuration dialog
        private void btnConfig_Click(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            if (cfg.ShowDialog() == DialogResult.OK)
            {
                UpdateList(true, true);
            }
        }

        //Fetch all title information etc yada yada yada blalblabla
        private void btnTitles_Click(object sender, EventArgs e)
        {
            if (cbTitle.Text == "")
            {
                Helper.Log("Enter a title for the show", Helper.LogType.Info);
                return;
            }
            GetTitles();
        }

        //Enter = Click "Get Titles" button
        private void cbTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetNewTitle(cbTitle.Text);
                btnTitles.PerformClick();
            }
        }

        //Enter = Change current directory
        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetPath(txtPath.Text);
            }
        }

        //Focus lost = change current directory
        private void txtPath_Leave(object sender, EventArgs e)
        {
            SetPath(txtPath.Text);
        }

        //Start renaming
        private void btnRename_Click(object sender, EventArgs e)
        {
            Rename();
        }

        //Focus lost = store desired pattern and update names
        private void txtTarget_Leave(object sender, EventArgs e)
        {
            if (txtTarget.Text != Helper.ReadProperty(Config.TargetPattern))
            {
                Helper.WriteProperty(Config.TargetPattern, txtTarget.Text);
                CreateNewNames();
                FillListView();
            }
        }

        //Enter = store desired pattern and update names
        private void txtTarget_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtTarget.Text != Helper.ReadProperty(Config.TargetPattern))
                {
                    Helper.WriteProperty(Config.TargetPattern, txtTarget.Text);
                    CreateNewNames();
                    FillListView();
                }
            }
        }

        //start fetching subtitles
        private void btnSubs_Click(object sender, EventArgs e)
        {
            GetSubtitles();
        }

        //Cleanup, save some stuff etc
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Helper.WriteProperty(Config.LastSubProvider, cbSubs.SelectedItem.ToString());

            //Save column order and sizes
            string[] ColumnWidths = new string[lstFiles.Columns.Count];
            string[] ColumnOrder = new string[lstFiles.Columns.Count];
            for (int i = 0; i < lstFiles.Columns.Count; i++)
            {
                ColumnOrder[i] = lstFiles.Columns[i].DisplayIndex.ToString();
                ColumnWidths[i] = lstFiles.Columns[i].Width.ToString();
            }
            Helper.WriteProperties(Config.ColumnOrder, ColumnOrder);
            Helper.WriteProperties(Config.ColumnWidths, ColumnWidths);

            //save window size
            string[] WindowSize = new string[2];
            WindowSize[0] = this.RestoreBounds.Width.ToString();
            WindowSize[1] = this.RestoreBounds.Height.ToString();
            Helper.WriteProperties(Config.WindowSize, WindowSize);

            foreach (ConfigFile f in Settings.files)
            {
                f.Flush();
            }
        }
        #endregion
        #region Contextmenu
        //Set which context menu options should be avaiable
        private void contextFiles_Opening(object sender, CancelEventArgs e)
        {
            editSubtitleToolStripMenuItem.Visible = false;
            viewToolStripMenuItem.Visible = false;
            if (lstFiles.SelectedItems.Count == 1)
            {
                //if selected file is a subtitle
                List<string> subext = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
                if (subext.Contains(info.Episodes[((int)lstFiles.SelectedItems[0].Tag)].Extension.ToLower()))
                {
                    editSubtitleToolStripMenuItem.Visible = true;
                }

                //if selected file is a video and there is a matching subtitle
                if (info.GetSubtitle(info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]) != null)
                {
                    editSubtitleToolStripMenuItem.Visible = true;
                }

                //if there is a matching video
                if (info.GetVideo(info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]) != null)
                {
                    viewToolStripMenuItem.Visible = true;
                }

            }
            if (lstFiles.SelectedItems.Count > 0)
            {
                copyToolStripMenuItem.Visible = true;
                bool OldPath=false;
                bool OldFilename=false;
                bool Name=false;
                bool Destination=false;
                bool NewFilename=false;
                foreach (ListViewItem lvi in lstFiles.SelectedItems)
                {
                    if (info.Episodes[((int)lvi.Tag)].Filename      != "") OldFilename = true;
                    if (info.Episodes[((int)lvi.Tag)].Path          != "") OldPath     = true;
                    if (info.Episodes[((int)lvi.Tag)].Name          != "") Name        = true;
                    if (info.Episodes[((int)lvi.Tag)].Destination   != "") Destination = true;
                    if (info.Episodes[((int)lvi.Tag)].NewFileName   != "") NewFilename = true;
                }
                originalNameToolStripMenuItem.Visible = OldFilename;
                pathOrigNameToolStripMenuItem.Visible = OldPath && OldFilename;
                titleToolStripMenuItem.Visible = Name;
                newFileNameToolStripMenuItem.Visible = NewFilename;
                destinationNewFileNameToolStripMenuItem.Visible = Destination && NewFilename;
            }
            else
            {
                copyToolStripMenuItem.Visible = false;
            }
            

        }

        //Select all list items
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstFiles.SelectedIndices.Clear();
            foreach(ListViewItem lvi in lstFiles.Items){
                lstFiles.SelectedIndices.Add(lvi.Index);
            }
        }

        //Invert file list selection
        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.Items)
            {
                if (lstFiles.SelectedIndices.Contains(lvi.Index))
                {
                    lstFiles.SelectedIndices.Remove(lvi.Index);
                }
                else
                {
                    lstFiles.SelectedIndices.Add(lvi.Index);
                }
            }
        }

        //Check all list boxes
        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.Items)
            {
                lvi.Checked = true;
            }
        }

        //Uncheck all list boxes
        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.Items)
            {
                lvi.Checked = false;
            }
        }

        //Invert check status of Selected list boxes
        private void invertCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.Items)
            {
                lvi.Checked = !lvi.Checked;
            }
        }

        //Filter function to select files by keyword
        private void selectByKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter f = new Filter(cbTitle.Text);
            if (f.ShowDialog() == DialogResult.OK)
            {
                lstFiles.SelectedIndices.Clear();
                foreach (ListViewItem lvi in lstFiles.Items)
                {
                    if (lvi.Text.Contains(f.result))
                    {
                        lstFiles.SelectedIndices.Add(lvi.Index);
                    }
                }
            }

        }
        
        //Set season property for selected items
        private void setSeasonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //yes we are smart and guess the season from existing ones
            int sum=0;
            int count=0;
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                try
                {
                    int s = 0;
                    Int32.TryParse(info.Episodes[(int)lvi.Tag].Season, out s);
                    sum += s;
                    count++;
                }
                catch (Exception) { }
            }
            int EstimatedSeason = (int) Math.Round(((float)sum / (float)count));
            EnterSeason es = new EnterSeason(EstimatedSeason);
            if (es.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem lvi in lstFiles.SelectedItems)
                {
                    int season = es.season;
                    info.Episodes[(int)lvi.Tag].Season = season.ToString();
                    SetupRelation((int)lvi.Tag);                    
                }
                FillListView();
            }
        }

        //Set episodes for selected items to a range
        private void setEpisodesFromtoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEpisodes se = new SetEpisodes(lstFiles.SelectedIndices.Count);
            if (se.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < lstFiles.SelectedIndices.Count; i++)
                {
                    info.Episodes[((int)lstFiles.SelectedItems[i].Tag)].Episode = (i + se.From).ToString();
                    lstFiles.SelectedItems[i].SubItems[3].Text = (i + se.From).ToString();
                }
            }



        }

        //Refresh file list
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateList(true, true);
        }

        //Delete file
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete selected files?", "Delete selected files?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            List<InfoEntry> lie = new List<InfoEntry>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                lie.Add(info.Episodes[(int)lvi.Tag]);
            }
            foreach (InfoEntry ie in lie)
            {
                try
                {
                    File.Delete(ie.Path + Path.DirectorySeparatorChar + ie.Filename);
                    info.Episodes.Remove(ie);
                }
                catch (Exception ex)
                {
                    Helper.Log("Error deleting file: " + ex.Message, Helper.LogType.Error);
                }
            }
            FillListView();
        }

        //Open file
        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoEntry ie = info.GetVideo(info.Episodes[(int)lstFiles.SelectedItems[0].Tag]);
            string VideoPath = ie.Path + Path.DirectorySeparatorChar + ie.Filename;
            try
            {
                Process myProc = Process.Start(VideoPath);
            }
            catch (Exception ex)
            {
                Helper.Log("Couldn't open " + VideoPath + ":" + ex.Message, Helper.LogType.Error);
            }
        }
                
        //Edit subtitle
        private void editSubtitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoEntry sub = info.GetSubtitle(info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]);
            InfoEntry video = info.GetVideo(info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]);
            if (sub != null)
            {
                string path = sub.Path + Path.DirectorySeparatorChar + sub.Filename;
                string videopath = "";
                if (video != null)
                {
                    videopath = video.Path + Path.DirectorySeparatorChar + video.Filename;
                }
                EditSubtitles es = new EditSubtitles(path, videopath);
                es.ShowDialog();
            }
        }

        //Set Destination
        private void setDestinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Set Destination", "Set Destination directory for selected files", Helper.ReadProperty(Config.LastDirectory), InputBox.BrowseType.Folder, true);
            if (ib.ShowDialog(this) == DialogResult.OK)
            {
                foreach (ListViewItem lvi in lstFiles.SelectedItems)
                {
                    string destination = ib.input;
                    info.Episodes[(int)lvi.Tag].Destination = destination;
                    lvi.SubItems[6].Text = destination;
                }
            }
        }
        #endregion
        #region misc

        /// <summary>
        /// Decides which files should be marked for processing
        /// </summary>
        /// <param name="Basepath">basepath, as always</param>
        /// <param name="Showname">user entered showname</param>
        public void DecideWhatShouldBeProcessed(string Basepath, string Showname)
        {
            string basefolder=Basepath.Substring(Math.Max(Basepath.LastIndexOfAny(new char[]{Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})+1,0));
            string pattern = Helper.ReadProperty(Config.Extract);
            bool UseSeasonDirs = Helper.ReadProperty(Config.UseSeasonSubDir) == "1";
            if (pattern.Contains("%T"))
            {
                pattern = pattern.Replace("%T", "(?<Title>*.?)");
            }
            if (pattern.Contains("%S"))
            {
                pattern = pattern.Replace("%S", "(?<Season>\\d)");
            }
            //Create lists of show dirs and no show dirs to be a bit more efficient
            List<string> ShowDirs = new List<string>();
            foreach (InfoEntry ie in info.Episodes)
            {
                //Skip paths we already have
                if (ShowDirs.Contains(ie.Path))
                {
                    continue;
                }
                if (ShowDirs.Contains(Directory.GetParent(ie.Path).FullName))
                {
                    ShowDirs.Add(ie.Path);
                    continue;
                }
                if (basefolder == Showname && ie.Path == Basepath)
                {
                    ShowDirs.Add(ie.Path);
                    continue;
                }

                //get all folder nodes starting with basepath
                List<string> folders = new List<string>(ie.Path.Substring(Math.Max(Basepath.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }),0)).Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
                int level = folders.Count;

                //if basepath is no showdir, just ignore it (don't add it to NoShowDir list)
                if (level == 1)
                {
                    continue;
                }

                if (level>1 && folders[1] == Showname)
                {
                    ShowDirs.Add(ie.Path);
                    continue;
                }
                else
                {
                    //if season dirs are used, include base dir, show dir and season dir files
                    if (UseSeasonDirs)
                    {
                        //figure out if this is a showdir by checking if there are season subdirectories
                        string DirBelowPossibleSeasonDirs = ie.Path;
                        int i = level;
                        while (i > 2)
                        {
                            DirBelowPossibleSeasonDirs = Directory.GetParent(DirBelowPossibleSeasonDirs).FullName;
                            i--;
                        }
                        string[] subdirs = Directory.GetDirectories(DirBelowPossibleSeasonDirs);
                        
                        bool ContainsSeasonDirs = false;
                        foreach (string s in subdirs)
                        {
                            Match m = Regex.Match(s, pattern, RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                ContainsSeasonDirs = true;
                                break;
                            }
                        }
                        if (ContainsSeasonDirs)
                        {
                            DirBelowPossibleSeasonDirs=ie.Path;
                            i = level;
                            //mark all parent dirs down to show dir as show dirs
                            while (i >= 2)
                            {
                                ShowDirs.Add(DirBelowPossibleSeasonDirs);
                                DirBelowPossibleSeasonDirs = Directory.GetParent(DirBelowPossibleSeasonDirs).FullName;
                                i--;
                            }
                            continue;
                        }
                    }
                }
            }

            foreach (InfoEntry ie in info.Episodes)
            {
                //If nothing was recognized, this is probably a movie and we don't want to move it
                if (ie.Season == "" && ie.Episode == "")
                {
                    ie.Process = false;
                    continue;
                }

                //if this is in basepath, include it if filename contains showname
                if (ie.Path == Basepath)
                {
                    if (!UseSeasonDirs && Showname != Path.GetFileName(Basepath))
                    {
                        if (!ie.Filename.ToLower().Replace(".", " ").Contains(Showname.ToLower()))
                        {
                            ie.Process = false;
                            continue;
                        }
                    }
                    ie.Process = true;
                    continue;
                }

                //Now try to decide by the folder this file is in.
                
                //If it is in the basepath or a folder that matches the entered showname,
                //it shall probably be renamed
                
                //get all folder nodes starting with basepath
                int index=Basepath.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) + 1;
                string[] split = ie.Path.Substring(Math.Max(index, 0)).Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                List<string> folders = new List<string>( split);
                int level = folders.Count;

                //Determine if we are in the showdir. 0=other dir, 1=desiredshowdir, 2=some other show in showdir
                int isDesiredShowDir = 0;
                foreach (string str in folders)
                {
                    if (str.ToLower().Contains(Showname.ToLower())||Showname.ToLower().Contains(str.ToLower()))
                    {
                        isDesiredShowDir = 1;
                        continue;
                    }
                    if (isDesiredShowDir == 1)
                    {
                        Match m = Regex.Match(str, pattern, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            break;
                        }
                        else
                        {
                            isDesiredShowDir = 2;
                            continue;
                        }
                    }
                    if (isDesiredShowDir == 2)
                    {
                        Match m = Regex.Match(str, pattern, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            isDesiredShowDir = 0;
                            break;
                        }
                        continue;
                    }
                }

                //if we are, process this file
                if(isDesiredShowDir == 1 || isDesiredShowDir == 2){
                    ie.Process=true;
                    continue;
                }else{
                    //if we aren't and this is another show dir, ignore it
                    if(ShowDirs.Contains(ie.Path)){
                        ie.Process=false;
                        continue;
                    //not in any showdir
                    }else{
                        if (level == 2 && Directory.GetDirectories(ie.Path).Length == 0 && info.GetNumberOfVideoFilesInFolder(ie.Path) == 1)
                        {
                            ie.Process = true;
                        }
                        else
                        {
                            ie.Process = false;
                        }
                        continue;
                    }
                }                
            }            
        }
        
        /// <summary>
        /// Sets new title and takes care of storing it properly (last [TitleHistorySize] Titles are stored)
        /// </summary>
        /// <param name="name">name to be set</param>
        public void SetNewTitle(string name)
        {
            string[] LastTitlesOld = Helper.ReadProperties(Config.LastTitles);
            //not updating title, return
            if (LastTitlesOld.Length > 0 && LastTitlesOld[0] == name)
            {
                cbTitle.SelectedIndex = 0;
                return;
            }
            //if new item is entered
            int Index = -1;
            for(int i=0;i<LastTitlesOld.Length;i++)
            {
                string str = LastTitlesOld[i];
                if (str == name)
                {
                    Index = i;
                    break;
                }
            }
            if (Index==-1)
            {                
                List<string> LastTitlesNew = new List<string>();
                LastTitlesNew.Add(name);
                foreach (string s in LastTitlesOld)
                {
                    LastTitlesNew.Add(s);
                }
                int size = Helper.ReadInt(Config.TitleHistorySize);
                Helper.WriteProperties(Config.LastTitles, LastTitlesNew.GetRange(0, Math.Min(LastTitlesNew.Count, size)).ToArray());
                cbTitle.Items.Clear();
                for (int i = 0; i < Math.Min(LastTitlesNew.Count, size); i++)
                {
                    cbTitle.Items.Add(LastTitlesNew[i]);
                }
                cbTitle.SelectedIndex = 0;
            }
            else
            {
                cbTitle.Items.RemoveAt(Index);
                cbTitle.Items.Insert(0, name);
                cbTitle.SelectedItem = name;
                List<string> items = new List<string>();
                foreach (object o in cbTitle.Items)
                {
                    items.Add((string)o);
                }
                Helper.WriteProperties(Config.LastTitles, items.ToArray());
            }
            string dir = Helper.ReadProperty(Config.LastDirectory);
            bool CreateDirectoryStructure = Helper.ReadProperty(Config.CreateDirectoryStructure) == "1";
            bool UseSeasonSubdirs = Helper.ReadProperty(Config.UseSeasonSubDir) == "1";
            foreach (InfoEntry ie in info.Episodes)
            {
                SetDestinationPath(ie, dir, CreateDirectoryStructure, UseSeasonSubdirs);
            }
            //As Spoon noted, we need to update the names too because they might contain the show name
            SetupRelations();
            DecideWhatShouldBeProcessed(Helper.ReadProperty(Config.LastDirectory),name);
            FillListView();
        }

        /// <summary>
        /// Sets a new path for the list view
        /// </summary>
        /// <param name="path">Path to be set</param>
        public void SetPath(string path)
        {
            if (path == null || path == "" || !Directory.Exists(path)) return;

            if (path.Length == 2)
            {
                if (char.IsLetter(path[0]) && path[1] == ':')
                {
                    path = path + Path.DirectorySeparatorChar;
                }
            }
            DirectoryInfo currentpath=new DirectoryInfo(path);
            
            string fixedpath="";
            while(currentpath.Parent!=null){
                fixedpath=currentpath.Parent.GetDirectories(currentpath.Name)[0].Name+Path.DirectorySeparatorChar+fixedpath;
                currentpath=currentpath.Parent;
            }
            fixedpath=currentpath.Name.ToUpper()+fixedpath;
            fixedpath = fixedpath.TrimEnd(new char[] { Path.DirectorySeparatorChar });
            if(fixedpath.Length==2){
                if (char.IsLetter(fixedpath[0]) && fixedpath[1] == ':')
                {
                    fixedpath = fixedpath + Path.DirectorySeparatorChar;
                }
            }
            path = fixedpath;
            //Same path, ignore
            if (Helper.ReadProperty(Config.LastDirectory) == path)
            {
                txtPath.Text = path;
                return;
            }
            else
            {
                string name="";
                int season=0;
                ExtractFromDirectoryNames(path, ref name, ref season);

                //If we are going to a different show, clear relations :(
                string[] LastTitles=Helper.ReadProperties(Config.LastTitles);
                if (LastTitles.Length == 0 || LastTitles[0] == "" || name != LastTitles[0])
                {
                    info.Relations.Clear();
                }
                Helper.WriteProperty(Config.LastDirectory, path);
                txtPath.Text = path;
                Environment.CurrentDirectory = path;
                UpdateList(true, false);
            }
        }
        /// <summary>
        /// Extracts season and show name from directory names
        /// </summary>
        /// <param name="path">path from which to extract the data (NO FILEPATH, JUST FOLDER)</param>
        /// <param name="name">showname or "" after function call</param>
        /// <param name="season">season number or -1 after function call</param>
        public void ExtractFromDirectoryNames(string path, ref string name, ref int season)
        {
            string pattern = Helper.ReadProperty(Config.Extract);
            string[] folders = path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            name = "";
            if (pattern.Contains("%T"))
            {
                pattern = pattern.Replace("%T", "(?<Title>*.?)");
            }
            if (pattern.Contains("%S"))
            {
                pattern = pattern.Replace("%S", "(?<Season>\\d)");
            }
            Match m = Regex.Match(folders[folders.GetLength(0) - 1], pattern, RegexOptions.IgnoreCase);
            bool foundseason = false;
            bool foundname = false;
            if (m.Success)
            {
                try
                {
                    season = Int32.Parse(m.Groups["Season"].Value);
                    foundseason = true;
                }
                catch (Exception)
                {
                    season = -1;
                }
                try
                {
                    name = m.Groups["Title"].Value;
                    if (name != null && name != "")
                    {
                        foundname = true;
                    }
                }
                catch (Exception)
                {
                    name = "";
                }
            }
            if (foundseason && !foundname && folders.GetLength(0) > 1)
            {
                name = folders[folders.GetLength(0) - 2];
            }
            else if (!foundname && folders.GetLength(0) > 0)
            {
                name = "";
            }
        }

        /// <summary>
        /// Deletes all empty folders recursively, ignoring files from IgnoredFiles list
        /// </summary>
        /// <param name="path">Path from which to delete folders</param>
        /// <param name="IgnoredFiletypes">List of extensions(without '.' at start) of filetypes which may be deleted</param>
        public void DeleteAllEmptyFolders(string path, List<string> IgnoredFiletypes)
        {
            bool delete = true;
            string[] folders = Directory.GetDirectories(path);
            if (folders.GetLength(0) > 0)
            {
                foreach (string folder in folders)
                {
                    DeleteAllEmptyFolders(folder, IgnoredFiletypes);
                }
            }
            folders = Directory.GetDirectories(path);
            if (folders.Length != 0)
            {
                return;
            }
            string[] files = Directory.GetFiles(path);
            if (files.Length != 0)
            {
                foreach (string s in files)
                {

                    if (Path.GetExtension(s) == "" || !IgnoredFiletypes.Contains(Path.GetExtension(s).Substring(1)))
                    {
                        delete = false;
                        break;
                    }
                }
            }
            if (delete)
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception ex)
                {
                    Helper.Log("Couldn't delete " + path + ": " + ex.Message, Helper.LogType.Error);
                }
            }
        }

        /// <summary>
        /// Syncs an item between data and GUI, will bitch if item doesn't exist already though. To create new items, add one to the data manually and use FillListView()
        /// </summary>
        /// <param name="item">Item to be synchronized. The index which is used here is that of the InfoEntry list in info class. ListViewItems Use Tag Property.</param>
        /// <param name="direction">Direction in which synching should occur. 0 = data->GUI, 1 = GUI->data</param>
        private void SyncItem(int item, bool direction)
        {
            ListViewItem lvi = null;
            foreach (ListViewItem lv in lstFiles.Items)
            {
                if ((int)lv.Tag == item)
                {
                    lvi = lv;
                    break;
                }
            }
            if (lvi == null)
            {
                Helper.Log("Synching between data and gui failed because item doesn't exist in GUI.", Helper.LogType.Error);
                return;
            }
            InfoEntry ie = info.Episodes[item];
            if (direction == false)
            {
                lvi.SubItems[0].Text = ie.Filename;
                lvi.SubItems[1].Text = ie.Path;
                lvi.SubItems[2].Text = ie.Season;
                lvi.SubItems[3].Text = ie.Episode;
                lvi.SubItems[4].Text = ie.Name;
                lvi.SubItems[5].Text = ie.NewFileName;
                lvi.SubItems[6].Text = ie.Destination;
            }
            else
            {
                ie.Filename = lvi.SubItems[0].Text;
                ie.Path = lvi.SubItems[1].Text;
                ie.Season = lvi.SubItems[2].Text;
                ie.Episode = lvi.SubItems[3].Text;
                ie.Name = lvi.SubItems[4].Text;
                ie.NewFileName = lvi.SubItems[5].Text;
                ie.Destination = lvi.SubItems[6].Text;
            }

            Colorize(lvi);
        }
        
        /// <summary>
        /// Fills list view control with info data
        /// </summary>
        private void FillListView()
        {
            lstFiles.Items.Clear();
            for (int i = 0; i < info.Episodes.Count; i++)
            {
                InfoEntry ie = info.Episodes[i];
                ListViewItem lvi = new ListViewItem(ie.Filename);
                lvi.Tag = i;
                lvi.SubItems.Add(ie.Path);
                lvi.SubItems.Add(ie.Season);
                lvi.SubItems.Add(ie.Episode);
                lvi.SubItems.Add(ie.Name);
                lvi.SubItems.Add(ie.NewFileName);
                lvi.SubItems.Add(ie.Destination);
                lvi.Checked = ie.Process;
                lstFiles.Items.Add(lvi);
            }
            Colorize();
            lstFiles.Sort();
            lstFiles.Refresh();
        }

        /// <summary>
        /// colorizes the file list
        /// </summary>
        private void Colorize()
        {
            foreach (ListViewItem lvi1 in lstFiles.Items)
            {
                Colorize(lvi1);
            }
        }

        /// <summary>
        /// Colorizes single list item
        /// </summary>
        /// <param name="lvi">List item to be colorized</param>
        private void Colorize(ListViewItem lvi)
        {
            if ((lvi.SubItems[5].Text == "" && lvi.SubItems[1].Text == lvi.SubItems[6].Text && lvi.SubItems[6].Text != "") || !lvi.Checked)
            {
                lvi.ForeColor = Color.Gray;
            }
            else
            {
                lvi.ForeColor = Color.Black;
            }
            if (lvi.SubItems[5].Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                lvi.BackColor = Color.Yellow;
            }
            else
            {
                lvi.BackColor = Color.White;
            }
            foreach (ListViewItem lvi2 in lstFiles.Items)
            {
                if (lvi != lvi2)
                {
                    if (lvi.SubItems[1].Text == lvi2.SubItems[1].Text && lvi.SubItems[5].Text == lvi2.SubItems[5].Text && lvi.SubItems[5].Text != "")
                    {
                        lvi.BackColor = Color.IndianRed;
                    }
                    else if (lvi.BackColor != Color.Yellow)
                    {
                        lvi.BackColor = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// Sets destination path
        /// </summary>
        /// <param name="basepath">Basepath is usually the selected directory in main view. It is the root of the directory structure which is created.</param>
        /// <param name="ie">Entry to set the destination path to</param>
        /// <param name="CreateDirectoryStructure">If true, entry is scheduled to be moved to directory structure created from basepath. If false, destination directory will be source directory</param>
        /// <param name="UseSeasonSubDirs">If true, files are put into showname\season x\ directories, if false, only showname\ directory is set</param>
        void SetDestinationPath(InfoEntry ie, string basepath, bool CreateDirectoryStructure, bool UseSeasonSubDirs)
        {
            //for placing files in directory structure, figure out if selected directory is show name, otherwise create one
            string[] dirs = basepath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            bool InShowDir = false;
            bool InSeasonDir = false;
            string showname = Helper.ReadProperties(Config.LastTitles)[0];
            if (dirs.GetLength(0) > 0 && dirs[dirs.GetLength(0) - 1] == showname)
            {
                InShowDir = true;
            }
            else if (dirs.GetLength(0) > 1 && dirs[dirs.GetLength(0) - 2] == showname)
            {
                InSeasonDir = true;
            }
            //if we want to move the files to a directory structure, set destination property to it here, otherwise use same dir
            if (CreateDirectoryStructure)
            {
                string seasondir = Helper.ReadProperty(Config.Extract);
                int season = -1;
                try
                {
                    Int32.TryParse(ie.Season, out season);
                }
                catch (Exception) { };
                if (season >= 0)
                {
                    seasondir = seasondir.Replace("%S", ie.Season);
                    seasondir = seasondir.Replace("%T", showname);
                    if (InShowDir)
                    {
                        if (UseSeasonSubDirs)
                        {
                            ie.Destination = basepath + Path.DirectorySeparatorChar + seasondir;
                        }
                        else
                        {
                            ie.Destination = basepath;
                        }
                    }
                    else if (InSeasonDir)
                    {
                        if (UseSeasonSubDirs)
                        {
                            //Go up one dir and add proper season dir
                            ie.Destination = Directory.GetParent(basepath).FullName + Path.DirectorySeparatorChar + seasondir;
                        }
                        else
                        {
                            ie.Destination = Directory.GetParent(basepath).FullName;
                        }
                    }
                    else
                    {
                        if (UseSeasonSubDirs)
                        {
                            ie.Destination = basepath + Path.DirectorySeparatorChar + showname + Path.DirectorySeparatorChar + seasondir;
                        }
                        else
                        {
                            ie.Destination = basepath + Path.DirectorySeparatorChar + showname;
                        }
                    }
                }
                //no valid season found, this could be a movie or so and probably shouldn't be moved
                else
                {
                    ie.Destination = ie.Path;
                }
            }
            //We don't want to move anything at all, so lets just leave it where it is
            else
            {
                ie.Destination = ie.Path;
            }
            if (ie.Destination == ie.Path)
            {
                ie.Destination = "";
            }
        }

        //Get focussed control
        /// <summary>
        /// Gets focussed control
        /// </summary>
        /// <param name="controls">Array of controls in which to search for</param>
        /// <returns>focussed control or null</returns>
        private Control getFocused(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c.Focused)
                {
                    // Return the focused control
                    return c;
                }
                else if (c.ContainsFocus)
                {
                    // If the focus is contained inside a control's children
                    // return the child
                    return getFocused(c.Controls);
                }
            }
            // No control on the form has focus
            return null;
        }
        #endregion

        private void cbTitle_Leave(object sender, EventArgs e)
        {
            SetNewTitle(cbTitle.Text);
        }

        bool UsedDropDown = false;
        private void cbTitle_DropDownClosed(object sender, EventArgs e)
        {
            UsedDropDown = true;
        }

        private void cbTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UsedDropDown)
            {
                UsedDropDown = !UsedDropDown;
                SetNewTitle(cbTitle.Text);
            }
        }       

        private void originalNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach(ListViewItem lvi in lstFiles.SelectedItems){
                clipboard += info.Episodes[((int)lvi.Tag)].Filename + Environment.NewLine;
            }
            clipboard=clipboard.Substring(0,Math.Max(clipboard.Length-1,0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void pathOrigNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                clipboard += info.Episodes[((int)lvi.Tag)].Path + Path.DirectorySeparatorChar + info.Episodes[((int)lvi.Tag)].Filename + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - 1, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                clipboard += info.Episodes[((int)lvi.Tag)].Name + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - 1, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void newFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                clipboard += info.Episodes[((int)lvi.Tag)].NewFileName + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - 1, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void destinationNewFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                if (info.Episodes[((int)lvi.Tag)].Destination != "" && info.Episodes[((int)lvi.Tag)].NewFileName != "")
                {
                    clipboard += info.Episodes[((int)lvi.Tag)].Destination + Path.DirectorySeparatorChar + info.Episodes[((int)lvi.Tag)].NewFileName + Environment.NewLine;
                }
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - 1, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtPath.Text))
            {
                Process myProc = Process.Start(txtPath.Text);
            }
        }

        private void lstFiles_DragDrop(object sender, DragEventArgs e)
        {            
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s.Length == 1 && Directory.Exists(s[0]))
            {
                SetPath(s[0]);
            }
        }

        private void lstFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (s.Length == 1 && Directory.Exists(s[0]))
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }
    }        
}

