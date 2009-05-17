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
using Renamer.Classes;
using Renamer.Dialogs;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes.Configuration;
using System.Runtime.InteropServices;
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

        private void GetAllTitles()
        {
            DateTime dt = DateTime.Now;
            Info.timecreatenewname = 0;
            Info.timeextractname = 0;
            Info.timesetpath = 0;
            Info.timesetuprelation = 0;
            //make a list of shownames
            List<string> shownames = new List<string>();
            foreach (InfoEntry ie in Info.Episodes)
            {
                if (ie.Process && !shownames.Contains(ie.Showname))
                {
                    shownames.Add(ie.Showname);
                }
            }
            foreach (string showname in shownames)
            {
                GetTitles(showname);
            }
            Info.timegettitles = (DateTime.Now - dt).TotalSeconds;
            Helper.Log("Time for getting titles: " + Info.timegettitles + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for extracting names: " + Info.timeextractname + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for creating paths: " + Info.timesetpath + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for creating filenames: " + Info.timecreatenewname + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for assigning relations: " + Info.timesetuprelation + " Seconds", Helper.LogType.Info);
        }

        /// <summary>
        /// gets titles, by using database search feature and parsing results, after that, shows them in gui
        /// </summary>
        private void GetTitles(string Showname)
        {
            //once
            for (int a = 0; a < 1; a++)
            {
                this.Cursor = Cursors.WaitCursor;
                
                // request
                RelationProvider provider = info.GetCurrentProvider();
                if (provider == null)
                {
                    Helper.Log("No relation provider found/selected", Helper.LogType.Error);
                    return;
                }           
                //get rid of old relations
                Info.Relations.Remove(Info.GetRelationCollectionByName(Showname));
                foreach (InfoEntry ie in Info.Episodes)
                {
                    if (ie.Showname == Showname && ie.Process)
                    {
                        ie.Name = "";
                        ie.NewFileName = "";
                        ie.Language = provider.Language;
                    }
                }
                string url = provider.SearchURL;
                Helper.Log("Search URL: " + url, Helper.LogType.Debug);
                if (url == null || url == "")
                {
                    Helper.Log("Can't search because no search URL is specified for this provider", Helper.LogType.Error);
                    break;
                }
                string[] LastTitles = Helper.ReadProperties(Config.LastTitles);
                url = url.Replace("%T", Showname);
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
                //SetProxy(requestHtml, url);
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
                    GetRelations(responseHtml.ResponseUri.AbsoluteUri + provider.EpisodesURL, Showname);
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
                    ParseSearch(ref source, responseHtml.ResponseUri.AbsoluteUri,Showname);
                }

                responseHtml.Close();                
                FillListView();
            }
            Cursor = Cursors.Default;
        }

        private void SetProxy(HttpWebRequest client, string url)
        {
            // Comment out foreach statement to use normal System.Net proxy detection 
            foreach (
                Uri address
                in WinHttpSafeNativeMethods.GetProxiesForUrl(new Uri(url)))
            {
                client.Proxy = new WebProxy(address);
                break;
            }
        }

        /// <summary>
        /// Some network function for faster automatic proxy detection
        /// </summary>
        internal static class WinHttpSafeNativeMethods
        {

            internal static IEnumerable<Uri> GetProxiesForUrl(
                Uri requestUrl)
            {
                return GetProxiesForUrl(requestUrl, string.Empty);
            }

            internal static IEnumerable<Uri> GetProxiesForUrl(
                Uri requestUrl, string userAgent)
            {

                IntPtr hHttpSession = IntPtr.Zero;
                string[] proxyList = null; ;

                try
                {
                    hHttpSession = WinHttpOpen(userAgent,
                        AccessType.NoProxy, null, null, 0);

                    if (hHttpSession != IntPtr.Zero)
                    {

                        AutoProxyOptions autoProxyOptions = new AutoProxyOptions();
                        autoProxyOptions.Flags = AccessType.AutoDetect;
                        autoProxyOptions.AutoLogonIfChallenged = true;
                        autoProxyOptions.AutoDetectFlags =
                            AutoDetectType.Dhcp | AutoDetectType.DnsA;

                        ProxyInfo proxyInfo = new ProxyInfo();

                        if (WinHttpGetProxyForUrl(hHttpSession,
                            requestUrl.ToString(), ref autoProxyOptions, ref proxyInfo))
                        {
                            if (!string.IsNullOrEmpty(proxyInfo.Proxy))
                            {
                                proxyList = proxyInfo.Proxy.Split(';', ' ');
                            }
                        }
                    }
                }
                catch (System.DllNotFoundException)
                {
                    // winhttp.dll is not found. 
                }
                catch (System.EntryPointNotFoundException)
                {
                    // A method within winhttp.dll is not found. 
                }
                finally
                {
                    if (hHttpSession != IntPtr.Zero)
                    {
                        WinHttpCloseHandle(hHttpSession);
                        hHttpSession = IntPtr.Zero;
                    }
                }

                if (proxyList != null && proxyList.Length > 0)
                {
                    Uri proxyUrl;
                    foreach (string address in proxyList)
                    {
                        if (TryCreateUrlFromPartialAddress(address, out proxyUrl))
                        {
                            yield return proxyUrl;
                        }
                    }
                }
            }

            /// <summary>
            /// Some network function for faster automatic proxy detection
            /// </summary>
            private static bool TryCreateUrlFromPartialAddress(string address, out Uri url)
            {
                address = address.Trim();

                if (string.IsNullOrEmpty(address))
                {
                    url = null;
                    return false;
                }

                try
                {
                    if (address.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        address.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        url = new Uri(address);
                    }
                    else if (address.StartsWith("//", StringComparison.Ordinal))
                    {
                        url = new Uri("http:" + address);
                    }
                    else
                    {
                        url = new Uri("http://" + address);
                    }
                    return true;
                }
                catch (UriFormatException)
                {
                    url = null;
                }
                return false;
            }

            [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern IntPtr WinHttpOpen(
                string userAgent,
                AccessType accessType,
                string proxyName,
                string proxyBypass,
                int flags);

            [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool WinHttpGetProxyForUrl(
                IntPtr hSession,
                string url,
                [In] ref AutoProxyOptions autoProxyOptions,
                [In, Out] ref ProxyInfo proxyInfo);

            [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool WinHttpCloseHandle(IntPtr httpSession);

            private enum AccessType
            {
                NoProxy = 1,
                AutoDetect = 1,
                AutoProxyConfigUrl = 2
            }

            [Flags]
            private enum AutoDetectType
            {
                Dhcp = 1,
                DnsA = 2,
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct AutoProxyOptions
            {

                internal AccessType Flags;

                internal AutoDetectType AutoDetectFlags;

                [MarshalAs(UnmanagedType.LPTStr)]
                internal string AutoConfigUrl;

                private IntPtr lpvReserved;

                private int dwReserved;

                internal bool AutoLogonIfChallenged;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct ProxyInfo
            {

                internal AccessType dwAccessType;

                [MarshalAs(UnmanagedType.LPTStr)]
                internal string Proxy;

                [MarshalAs(UnmanagedType.LPTStr)]
                internal string ProxyBypass;
            }
        }

        /// <summary>
        /// Parses search results from a series search
        /// </summary>
        /// <param name="source">Source code of the search results page</param>
        /// <param name="Showname">Showname</param>
        /// <param name="SourceURL">URL of the page source</param>
        private void ParseSearch(ref string source, string SourceURL, string Showname)
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
                url = System.Web.HttpUtility.HtmlDecode(url);
                Helper.Log("Search engine found one result: " + url.Replace(" ", "%20"), Helper.LogType.Status);
                GetRelations(url, Showname);
            }
            else
            {
                Helper.Log("Search engine found multiple results at " + SourceURL.Replace(" ", "%20"), Helper.LogType.Info);
                SelectResult sr = new SelectResult(mc, provider, false);
                if (sr.ShowDialog() == DialogResult.Cancel || sr.url == "") return;

                //Apply language of selected result to matching episodes
                if (provider.Language == Helper.Languages.None)
                {
                    foreach (InfoEntry ie in Info.Episodes)
                    {
                        if (ie.Showname == Showname && ie.Process)
                        {
                            ie.Language = sr.Language;
                        }
                    }
                }
                string url = provider.RelationsPage;
                Helper.Log("User selected " + provider.RelationsPage + "with %L=" + sr.url, Helper.LogType.Debug);
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
        private void GetRelations(string url, string Showname)
        {
             RelationProvider provider = info.GetCurrentProvider();
            if (provider == null)
            {
                Helper.Log("No relation provider found/selected", Helper.LogType.Error);
                return;
            }
            Helper.Log("Trying to get relations from " + url, Helper.LogType.Debug);
            //if episode infos are stored on a new page for each season, this should be marked with %S in url, so we can iterate through all those pages
            int season = 1;
            string url2 = url;
            //Create new RelationCollection
            RelationCollection rc = new RelationCollection(Showname);
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
                Encoding enc;
                if (provider.Encoding != null && provider.Encoding != "")
                {
                    try
                    {
                        enc = Encoding.GetEncoding(provider.Encoding);
                    }
                    catch (Exception ex)
                    {
                        Helper.Log("Invalid encoding in config file: " + ex.Message, Helper.LogType.Error);
                        enc = Encoding.GetEncoding(responseHtml.CharacterSet);
                    }                    
                }
                else
                {
                    enc = Encoding.GetEncoding(responseHtml.CharacterSet);
                }
                StreamReader r = new StreamReader(responseHtml.GetResponseStream(), enc);
                string source = r.ReadToEnd();
                r.Close();
                responseHtml.Close();
               


                //Source cropping
                source = source.Substring(Math.Max(source.IndexOf(provider.RelationsStart),0));
                source = source.Substring(0, Math.Max(source.LastIndexOf(provider.RelationsEnd),0));

                string pattern = provider.RelationsRegExp;
                Helper.Log("Trying to match source from " + responseHtml.ResponseUri.AbsoluteUri + " with " + pattern, Helper.LogType.Debug);
                RegexOptions ro = RegexOptions.IgnoreCase | RegexOptions.Singleline;
                if (provider.RelationsRightToLeft) ro |= RegexOptions.RightToLeft;
                MatchCollection mc = Regex.Matches(source, pattern, ro);
                
                for(int i=0;i<mc.Count;i++)
                {
                    Match m=mc[i];
                    //if we are iterating through season pages, take season from page url directly
                    //parse season and episode numbers
                    int s,e;
                    Int32.TryParse(m.Groups["Season"].Value, out s);
                    Int32.TryParse(m.Groups["Episode"].Value, out e);
                    if (url != url2)
                    {
                        rc.Relations.Add(new Relation(season.ToString(), e.ToString(), System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        Helper.Log("Found Relation: " + "S" + s.ToString() + "E" + e.ToString() + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), Helper.LogType.Debug);
                    }
                    else
                    {
                        rc.Relations.Add(new Relation(s.ToString(), e.ToString(), System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        Helper.Log("Found Relation: " + "S" + s.ToString() + "E" + e.ToString() + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), Helper.LogType.Debug);
                    }
                }
                Info.AddRelationCollection(rc);
                
                // THOU SHALL NOT FORGET THE BREAK
                if (!url2.Contains("%S")) break;
                season++;
            }
            Helper.Log("" + (season-1) + " Seasons, " + rc.Relations.Count + " relations found", Helper.LogType.Debug);
        }
        #endregion
        #region Name Creation

        /// <summary>
        /// creates names for all entries using season, episode and name and the target pattern
        /// <param name="movie">If used on movie files, target pattern will be ignored and only name property is used</param>
        /// </summary>
        private void CreateNewNames()
        {
            for(int i=0;i<Info.Episodes.Count;i++)
            {
                Info.Episodes[i].CreateNewName();
            }
        }
        
        /// <summary>
        /// Creates subtitle destination and names subs when no show information is fetched yet, so they have the same name as their video files for better playback
        /// </summary>
        void RenameSubsToMatchVideos()
        {
            foreach (InfoEntry ie in Info.Episodes)
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

        /// <summary>
        /// Removes unneeded tags from videos
        /// </summary>
        private void RemoveVideoTags()
        {

            //Tags should be preceeded by . or _
            string[] tags=Helper.ReadProperties(Config.Tags);
            List<string> regexes=new List<string>();
            foreach(string s in tags){
                regexes.Add("[\\._\\(\\[-]+"+s);
            }
            foreach (ListViewItem lvi in lstFiles.Items)
            {
                InfoEntry ie=Info.Episodes[((int)lvi.Tag)];
                ie.Process = false;
                //Go through all selected files and remove tags and clean them up
                if (lvi.Selected)
                {
                    ie.Name = "";
                    ie.Destination = "";
                    ie.Movie = true;
                    string temp = Path.GetFileNameWithoutExtension(ie.Filename);
                    //figure out if this is a multi file video
                    string end="CD";
                    //Check for single number
                    if(char.IsNumber(temp[temp.Length-1])){
                        end+=temp[temp.Length-1].ToString();
                    }
                    //Check for IofN format                    
                    else if(Regex.Match(temp,"\\dof\\d",RegexOptions.IgnoreCase).Success){
                        end=temp.Substring(temp.Length-4,1);
                    }
                    //Check for a/b
                    else if (char.ToLower(temp[temp.Length - 1]) == 'a')
                    {
                        end+="1";
                    }
                    else if (char.ToLower(temp[temp.Length - 1]) == 'b')
                    {
                        end += "2";
                    }
                    else
                    {
                        end = "";
                    }
                    
                    //try to match tags                    
                    bool removed = false;
                    foreach (string s in regexes)
                    {
                        Match m = Regex.Match(temp, s, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            temp = temp.Substring(0, m.Index);
                            removed = true;
                        }
                    }

                    //add possible existant file index back
                    if (removed)
                    {
                        temp = temp + " " + end;
                    }

                    //Get rid of dots and _
                    int i=-1;
                    int pos = 0;
                    while((i=temp.IndexOf(".",pos+1))!=-1){
                        if ((Convert.ToInt32(char.IsNumber(temp[i - 1])) + Convert.ToInt32(char.IsNumber(temp[Math.Min(i + 1, temp.Length - 1)]))) < 2)
                        {
                            temp=temp.Substring(0,i)+" "+temp.Substring(i+1);                            
                        }
                        pos = i;
                    }
                    temp = temp.Replace("_", " ");
                    //temp = Regex.Replace(temp, "([^0-9]\\.[^0-9]|[0-9]\\.[^0-9]|[^0-9]\\.[0-9]|_)", " ");
                    temp = temp.Trim();

                    
                    lvi.Selected = false;

                    ie.Name = temp;
                    if (ie.NewFileName != "" || ie.Destination!="")
                    {
                        ie.Process = true;
                        //string seasondir = Helper.ReadProperty(Config.Extract).Replace("%S", "\\d*");
                        //seasondir = seasondir.Replace("%E", "\\d*");
                    }

                    //Move and process multiple part movie files (i.e. "CD1")
                    //SetDestinationPath(ie, ie.Path, (Helper.ReadInt(Config.CreateDirectoryStructure) > 0), (Helper.ReadInt(Config.UseSeasonSubDir) > 0));
                    
                }
                SyncItem(((int)lvi.Tag), false);
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
            for(int i=0;i<Info.Episodes.Count;i++){
                InfoEntry ie=Info.Episodes[i];
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
        public void UpdateList(bool clear)
        {
            if (clear)
            {
                Info.Episodes.Clear();                
            }
            
            //scan for files which got deleted so we can remove them
            for (int i = Info.Episodes.Count-1; i >=0; i--)
            {
                InfoEntry ie = Info.Episodes[i];
                if (!File.Exists(ie.Path + Path.DirectorySeparatorChar + ie.Name))
                {
                    Info.Episodes.Remove(ie);
                    i--;
                }
            }

            string path = Helper.ReadProperty(Config.LastDirectory);
            path = path.TrimEnd(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
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

                //Loop through all files and recognize things, YAY!
                string[] patterns = Helper.ReadProperties(Config.EpIdentifier);                
                for(int i=0;i<patterns.Length;i++){
                    //replace %S and %E by proper regexps
                    //if a pattern containing %S%E is used, only use the first number for season
                    if(patterns[i].Contains("%S%E")){
                        patterns[i]=patterns[i].Replace("%S", "(?<Season>\\d)");
                        patterns[i]=patterns[i].Replace("%E", "(?<Episode>\\d+)");
                    }else{
                        patterns[i]=patterns[i].Replace("%S", "(?<Season>\\d+)");
                        patterns[i]=patterns[i].Replace("%E", "(?<Episode>\\d+)");
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
                string currentpath="";
                foreach (FileSystemInfo file in Files)
                {
                    //showname and season recognized from path
                    DirectorySeason = -1;

                    //Check if there is already an entry on this file, and if not, create one
                    ie=null;
                    currentpath = Path.GetDirectoryName(file.FullName);
                    foreach (InfoEntry i in Info.Episodes)
                    {
                        if (i.Filename == file.Name && i.Path == currentpath)
                        {
                            ie = i;
                            break;
                        }
                    }
                    
                    if (ie == null)
                    {
                        ie = new InfoEntry();
                    }

                    //Set basic values, by setting those values destination directory and filename will be generated automagically
                    ie.Filename = file.Name;
                    ie.Path = currentpath;
                    ie.Extension = Path.GetExtension(file.FullName).ToLower().Replace(".","");
                    //Get season number and showname from directory
                    DirectorySeason = ExtractSeasonFromDirectory(Path.GetDirectoryName(file.FullName));
                    dt = DateTime.Now;
                    //try to recognize season and episode from filename
                    foreach (string pattern in patterns)
                    {
                        //Try to match. If it works, get the season and the episode from the match
                        m = Regex.Match(file.Name, pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                        if (m.Success)
                        {
                            strSeason = "";
                            strEpisode = "";
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
                            //Fix for .0216. notation for example, 4 numbers should always be recognized as %S%S%E%E
                            if (strEpisode.Length == 3 && strSeason.Length == 1)
                            {
                                strSeason += strEpisode[0];
                                strEpisode = strEpisode.Substring(1);
                                if (strSeason[0] == '0')
                                {
                                    strSeason = strSeason.Substring(1);
                                }
                            }
                            ie.Episode = strEpisode;
                            ie.Season = strSeason;

                            //if season recognized from directory name doesn't match season recognized from filename, the file might be located in a wrong directory
                            if (DirectorySeason != -1 && ie.Season != DirectorySeason.ToString())
                            {
                                Helper.Log("File seems to be located inconsistently: " + ie.Filename + " was recognized as season " + ie.Season + ", but folder name indicates that it should be season " + DirectorySeason.ToString(), Helper.LogType.Warning);
                            }                                                                                 
                            break;
                        }
                    }
                    Info.timeextractnumbers += (DateTime.Now - dt).TotalSeconds;
                    //if season number couldn't be extracted, try to get it from folder
                    //(this should never happen if a pattern like %S%E is set)
                    if (ie.Season == "" && DirectorySeason != -1)
                    {
                        ie.Season = DirectorySeason.ToString();
                    }
                    //if nothing could be recognized, assume that this is a movie
                    if (ie.Season == "" && ie.Episode == "")
                    {
                        ie.Movie = true;
                    }
                    //if not added yet, add it
                    if (!contains)
                    {
                        Info.Episodes.Add(ie);
                    }                                 
                }
                //SelectSimilarFilesForProcessing(path,Helper.ReadProperties(Config.LastTitles)[0]);
                SelectRecognizedFilesForProcessing();
            }

            if (clear)
            {
                RenameSubsToMatchVideos();
            }

            Helper.Log("Found " + Info.Episodes.Count + " Files", Helper.LogType.Info);
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

        private void SelectRecognizedFilesForProcessing()
        {
            foreach (InfoEntry ie in Info.Episodes)
            {
                if (ie.Season != "" && ie.Episode != "")
                {
                    ie.Process = true;
                    ie.Movie = false;
                }
                else
                {
                    ie.Process = false;
                    ie.Movie = true;
                }
            }            
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
                        Info.AddRelationCollection(new Relation(season.ToString(), m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
                        //Helper.Log("Found Relation: " + "S" + season.ToString() + "E" + m.Groups["Episode"].Value + " - " + System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value), Helper.LogType.Status);
                    }
                    else
                    {
                        Info.AddRelationCollection(new Relation(m.Groups["Season"].Value, m.Groups["Episode"].Value, System.Web.HttpUtility.HtmlDecode(m.Groups["Title"].Value)));
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

                    //now that season and episode are known, assign the filename to a SubtitleFile object
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
                foreach (InfoEntry ie in Info.Episodes)
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
                UpdateList(true);
            }
        }
        #endregion
        #region LstFilesEvents
        //Update Coloring when file is checked/unchecked and set process flag
        private void lstFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            Info.Episodes[(int)e.Item.Tag].Process = e.Item.Checked;
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
                RelationCollection rc = Info.GetRelationCollectionByName(Info.Episodes[(int)e.Item.Tag].Showname);
                if (e.SubItem == 4)
                {
                    //if season is valid and there are relations at all, show combobox. Otherwise, just show edit box
                    if (rc!=null && Info.Relations.Count > 0 && Convert.ToInt32(e.Item.SubItems[2].Text) >= rc.FindMinSeason() && Convert.ToInt32(e.Item.SubItems[2].Text) <= rc.FindMaxSeason())
                    {
                        comEdit.Items.Clear();
                        foreach (Relation rel in rc.Relations)
                        {
                            if (rel.Season == Info.Episodes[(int)e.Item.Tag].Season)
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
                else if (e.SubItem == 5 || e.SubItem == 6 || e.SubItem == 7 )
                {
                    lstFiles.StartEditing(txtEdit, e.Item, e.SubItem);
                }                 
                else
                {
                    //clamp season and episode values to allowed values
                    if (rc!=null && rc.Relations.Count > 0)
                    {
                        if (e.SubItem == 2)
                        {
                            numEdit.Minimum = rc.FindMinSeason();
                            numEdit.Maximum = rc.FindMaxSeason();
                        }
                        else if (e.SubItem == 3)
                        {
                            numEdit.Minimum = rc.FindMinEpisode(Convert.ToInt32(Info.Episodes[(int)e.Item.Tag].Season));
                            numEdit.Maximum = rc.FindMaxEpisode(Convert.ToInt32(Info.Episodes[(int)e.Item.Tag].Season));
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
            string dir = Helper.ReadProperty(Config.LastDirectory);
            bool CreateDirectoryStructure = Helper.ReadInt(Config.CreateDirectoryStructure) == 1;
            bool UseSeasonSubdirs = Helper.ReadInt(Config.UseSeasonSubDir) == 1;
            //add lots of stuff here
            switch (e.SubItem)
            {
                //season
                case 2:
                    Info.Episodes[(int)e.Item.Tag].Season = e.DisplayText;
                    if (e.DisplayText == "")
                    {
                        Info.Episodes[(int)e.Item.Tag].Movie = true;
                    }
                    else
                    {
                        Info.Episodes[(int)e.Item.Tag].Movie = false;
                    }
                    //SetupRelation((int)e.Item.Tag);
                    //foreach (InfoEntry ie in Info.Episodes)
                    //{
                    //    SetDestinationPath(ie, dir, CreateDirectoryStructure, UseSeasonSubdirs);
                    //}
                    break;
                //Episode
                case 3:
                    Info.Episodes[(int)e.Item.Tag].Episode = e.DisplayText;
                    if (e.DisplayText == "")
                    {
                        Info.Episodes[(int)e.Item.Tag].Movie = true;
                    }
                    else
                    {
                        Info.Episodes[(int)e.Item.Tag].Movie = false;
                    }
                    //SetupRelation((int)e.Item.Tag);                    
                    //SetDestinationPath(Info.Episodes[(int)e.Item.Tag], dir, CreateDirectoryStructure, UseSeasonSubdirs);
                    break;
                //name
                case 4:
                    //backtrack to see if entered text matches a season/episode
                    RelationCollection rc = Info.GetRelationCollectionByName(Info.Episodes[(int)e.Item.Tag].Showname);
                    if (rc != null)
                    {
                        foreach (Relation rel in rc.Relations)
                        {
                            //if found, set season and episode in gui and sync back to data
                            if (e.DisplayText == rel.Name)
                            {
                                e.Item.SubItems[2].Text = rel.Season;
                                e.Item.SubItems[3].Text = rel.Episode;
                            }
                        }
                    }
                    Info.Episodes[(int)e.Item.Tag].Name = e.DisplayText;
                    break;
                //Filename
                case 5:
                    Info.Episodes[(int)e.Item.Tag].NewFileName = e.DisplayText;
                    break;
                //Destination
                case 6:
                    try
                    {
                        Path.GetDirectoryName(e.DisplayText);
                        Info.Episodes[(int)e.Item.Tag].Destination = e.DisplayText;
                    }
                    catch (Exception) {
                        e.Cancel = true;
                    }
                    break;
                case 7:
                    Info.Episodes[(int)e.Item.Tag].Showname = e.DisplayText;
                    break;
                default:
                    throw new Exception("Unreachable code");
                }
            SyncItem((int)e.Item.Tag, false);
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
                    Process myProc = Process.Start(Info.Episodes[(int)lvi.Tag].Path + Path.DirectorySeparatorChar + Info.Episodes[(int)lvi.Tag].Filename);
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
                Settings.files.Add(f);
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
                UpdateList(true);
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
                    Helper.Log("Invalid Value for ColumnWidths["+i+"]",Helper.LogType.Error);
                }
                try
                {
                    int order = lstFiles.Columns[i].DisplayIndex;
                    Int32.TryParse(ColumnOrder[i], out order);
                    lstFiles.Columns[i].DisplayIndex = order;
                }
                catch (Exception)
                {
                    Helper.Log("Invalid Value for ColumnOrder[" + i + "]", Helper.LogType.Error);
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
                txtPath.Focus();
                txtPath.Select(txtPath.Text.Length, 0);
            }
        }
        
        //Auto column resize by storing column width ratios at resize start
        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            if (Helper.ReadInt(Config.ResizeColumns) == 1)
            {
                columnsizes = new float[]{
                (float)(lstFiles.Columns[0].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[1].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[2].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[3].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[4].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[5].Width)/(float)(lstFiles.ClientRectangle.Width),
                (float)(lstFiles.Columns[6].Width)/(float)(lstFiles.ClientRectangle.Width),};
                float sum = 0;
                for (int i = 0; i < 7; i++)
                {
                    sum += columnsizes[i];
                }
                //some numeric correction to make ratios:
                for (int i = 0; i < 7; i++)
                {
                    columnsizes[i] *= (float)1 / sum;
                }
            }
        }
        
        //Auto column resize, restore Column width ratios at resize end (to make sure!)
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (Helper.ReadInt(Config.ResizeColumns) == 1)
            {
                if (lstFiles != null && lstFiles.Columns.Count == 7 && columnsizes != null)
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
        }

        //Auto column resize, restore Column width ratios during resize
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (Helper.ReadInt(Config.ResizeColumns) == 1)
                {
                    if (lstFiles != null && lstFiles.Columns.Count == 6 && columnsizes != null)
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
            AboutBox ab = new AboutBox();
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
        
        /*//Update Destination paths when title of show is changed
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
        }*/

        //Clear episode informations fetched from providers
        private void btnClear_Click(object sender, EventArgs e)
        {
            Info.Relations.Clear();
            UpdateList(true);
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
                UpdateList(true);
            }
        }

        //Fetch all title information etc yada yada yada blalblabla
        private void btnTitles_Click(object sender, EventArgs e)
        {            
            GetAllTitles();
        }

        

        /*//Enter = Click "Get Titles" button
        private void cbTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetNewTitle(cbTitle.Text);
                btnTitles.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                cbTitle.Text = Helper.ReadProperties(Config.LastTitles)[0];
                cbTitle.SelectionStart = cbTitle.Text.Length;
            }
        }*/

        //Enter = Change current directory
        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetPath(txtPath.Text);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtPath.Text = Helper.ReadProperty(Config.LastDirectory);
                txtPath.SelectionStart = txtPath.Text.Length;
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
            else if (e.KeyCode == Keys.Escape)
            {
                txtTarget.Text = Helper.ReadProperty(Config.TargetPattern);
                txtTarget.SelectionStart = txtTarget.Text.Length;
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
            if (this.WindowState == FormWindowState.Normal)
            {
                WindowSize[0] = this.Size.Width.ToString();
                WindowSize[1] = this.Size.Height.ToString();
            }
            else
            {
                WindowSize[0] = this.RestoreBounds.Width.ToString();
                WindowSize[1] = this.RestoreBounds.Height.ToString();
            }            
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
            renamingToolStripMenuItem.Visible = false;
            if (lstFiles.SelectedItems.Count == 1)
            {
                //if selected file is a subtitle
                List<string> subext = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
                if (subext.Contains(Info.Episodes[((int)lstFiles.SelectedItems[0].Tag)].Extension.ToLower()))
                {
                    editSubtitleToolStripMenuItem.Visible = true;
                }

                //if selected file is a video and there is a matching subtitle
                if (info.GetSubtitle(Info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]) != null)
                {
                    editSubtitleToolStripMenuItem.Visible = true;
                }

                //if there is a matching video
                if (info.GetVideo(Info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]) != null)
                {
                    viewToolStripMenuItem.Visible = true;
                }                
            }
            if (lstFiles.SelectedItems.Count > 0)
            {
                renamingToolStripMenuItem.Visible = true;
                createDirectoryStructureToolStripMenuItem1.Checked=false;
                dontCreateDirectoryStructureToolStripMenuItem.Checked=false;
                largeToolStripMenuItem.Checked=false;
                smallToolStripMenuItem.Checked=false;
                igNorEToolStripMenuItem.Checked=false;
                cAPSLOCKToolStripMenuItem.Checked=false;
                useUmlautsToolStripMenuItem.Checked=false;
                dontUseUmlautsToolStripMenuItem.Checked=false;
                useProvidedNamesToolStripMenuItem.Checked=false;
                copyToolStripMenuItem.Visible = true;
                bool OldPath=false;
                bool OldFilename=false;
                bool Name=false;
                bool Destination=false;
                bool NewFilename=false;
                InfoEntry.DirectoryStructure CreateDirectoryStructure = InfoEntry.DirectoryStructure.Unset;
                InfoEntry.Case Case = InfoEntry.Case.Unset;
                InfoEntry.UmlautAction Umlaute = InfoEntry.UmlautAction.Unset;
                for(int i=0;i<lstFiles.SelectedItems.Count;i++)
                {
                    ListViewItem lvi=lstFiles.SelectedItems[i];
                    if(i==0){
                        CreateDirectoryStructure = Info.Episodes[((int)lvi.Tag)].CreateDirectoryStructure;
                        Case = Info.Episodes[((int)lvi.Tag)].Casing;                        
                        Umlaute = Info.Episodes[((int)lvi.Tag)].UmlautUsage;
                    }else{
                        if(CreateDirectoryStructure!=Info.Episodes[((int)lvi.Tag)].CreateDirectoryStructure){
                            CreateDirectoryStructure = InfoEntry.DirectoryStructure.Unset;
                        }
                        if(Case!=Info.Episodes[((int)lvi.Tag)].Casing){
                            Case = InfoEntry.Case.Unset;
                        }
                        if(Umlaute!=Info.Episodes[((int)lvi.Tag)].UmlautUsage){
                            Umlaute = InfoEntry.UmlautAction.Unset;
                        }
                    }
                }
                if(CreateDirectoryStructure==InfoEntry.DirectoryStructure.CreateDirectoryStructure){
                    createDirectoryStructureToolStripMenuItem1.Checked=true;
                }else if(CreateDirectoryStructure==InfoEntry.DirectoryStructure.NoDirectoryStructure){
                    dontCreateDirectoryStructureToolStripMenuItem.Checked=true;
                }
                if(Case==InfoEntry.Case.Large){
                    largeToolStripMenuItem.Checked=true;
                }else if(Case==InfoEntry.Case.small){
                    smallToolStripMenuItem.Checked=true;
                }else if(Case==InfoEntry.Case.Ignore){
                    igNorEToolStripMenuItem.Checked=true;
                }else if(Case==InfoEntry.Case.CAPSLOCK){
                    cAPSLOCKToolStripMenuItem.Checked=true;
                }
                if(Umlaute==InfoEntry.UmlautAction.Use){
                    useUmlautsToolStripMenuItem.Checked=true;
                }else if(Umlaute==InfoEntry.UmlautAction.Dont_Use){
                    dontUseUmlautsToolStripMenuItem.Checked=true;
                }else if(Umlaute==InfoEntry.UmlautAction.Ignore){
                    useProvidedNamesToolStripMenuItem.Checked=true;
                }
                for(int i=0;i<lstFiles.SelectedItems.Count;i++)
                {
                    ListViewItem lvi=lstFiles.SelectedItems[i];
                    if (Info.Episodes[((int)lvi.Tag)].Filename      != "") OldFilename = true;
                    if (Info.Episodes[((int)lvi.Tag)].Path          != "") OldPath     = true;
                    if (Info.Episodes[((int)lvi.Tag)].Name          != "") Name        = true;
                    if (Info.Episodes[((int)lvi.Tag)].Destination   != "") Destination = true;
                    if (Info.Episodes[((int)lvi.Tag)].NewFileName   != "") NewFilename = true;
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
            foreach (InfoEntry ie in Info.Episodes)
            {
                ie.Process = true;
            }
            SyncAllItems(false);
        }

        //Uncheck all list boxes
        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (InfoEntry ie in Info.Episodes)
            {
                ie.Process = false;
            }
            SyncAllItems(false);
        }

        //Invert check status of Selected list boxes
        private void invertCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (InfoEntry ie in Info.Episodes)
            {
                ie.Process = !ie.Process;
            }
            SyncAllItems(false);
        }

        //Filter function to select files by keyword
        private void selectByKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter f = new Filter("");
            if (f.ShowDialog() == DialogResult.OK)
            {
                lstFiles.SelectedIndices.Clear();
                foreach (ListViewItem lvi in lstFiles.Items)
                {
                    if (lvi.Text.ToLower().Contains(f.result.ToLower()))
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
                    Int32.TryParse(Info.Episodes[(int)lvi.Tag].Season, out s);
                    sum += s;
                    count++;
                }
                catch (Exception) { }
            }
            int EstimatedSeason = (int) Math.Round(((float)sum / (float)count));
            EnterSeason es = new EnterSeason(EstimatedSeason);
            if (es.ShowDialog() == DialogResult.OK)
            {
                string basepath=Helper.ReadProperty(Config.LastDirectory);
                bool createdirectorystructure=(Helper.ReadInt(Config.CreateDirectoryStructure)>0);
                bool UseSeasonDir=(Helper.ReadInt(Config.UseSeasonSubDir)>0);
                foreach (ListViewItem lvi in lstFiles.SelectedItems)
                {
                    int season = es.season;
                    Info.Episodes[(int)lvi.Tag].Season = season.ToString();
                    //SetupRelation((int)lvi.Tag);
                    //SetDestinationPath(Info.Episodes[(int)lvi.Tag], basepath, createdirectorystructure, UseSeasonDir);
                    if (Info.Episodes[(int)lvi.Tag].Destination != "")
                    {
                        Info.Episodes[(int)lvi.Tag].Process = true;
                    }
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
                    Info.Episodes[((int)lstFiles.SelectedItems[i].Tag)].Episode = (i + se.From).ToString();
                    lstFiles.SelectedItems[i].SubItems[3].Text = (i + se.From).ToString();
                }
            }



        }

        //Refresh file list
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateList(true);
        }

        //Delete file
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete selected files?", "Delete selected files?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            List<InfoEntry> lie = new List<InfoEntry>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                lie.Add(Info.Episodes[(int)lvi.Tag]);
            }
            foreach (InfoEntry ie in lie)
            {
                try
                {
                    File.Delete(ie.Path + Path.DirectorySeparatorChar + ie.Filename);
                    Info.Episodes.Remove(ie);
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
            InfoEntry ie = info.GetVideo(Info.Episodes[(int)lstFiles.SelectedItems[0].Tag]);
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
            InfoEntry sub = info.GetSubtitle(Info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]);
            InfoEntry video = info.GetVideo(Info.Episodes[((int)lstFiles.SelectedItems[0].Tag)]);
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
                    Info.Episodes[(int)lvi.Tag].Destination = destination;
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
        public void SelectSimilarFilesForProcessing(string Basepath, string Showname)
        {
            List<InfoEntry> matches = Helper.FindSimilarByName(Info.Episodes, Showname);
            foreach (InfoEntry ie in Info.Episodes)
            {
                if (matches.Contains(ie))
                {
                    ie.Process = true;
                    ie.Movie = false;
                }
                else
                {
                    ie.Process = false;
                }
            }
            return;
        }

        /// <summary>
        /// Sets new title to some files and takes care of storing it properly (last [TitleHistorySize] Titles are stored)
        /// </summary>
        /// <param name="files">files to which this title should be set to</param>
        /// <param name="title">name to be set</param>
        public void SetNewTitle(List<InfoEntry> files, string title)
        {
            string[] LastTitlesOld = Helper.ReadProperties(Config.LastTitles);
            foreach (InfoEntry ie in files)
            {
                if (ie.Showname != title)
                {
                    ie.Showname = title;
                }
            }
            
            //check if list of titles contains new title
            int Index = -1;
            for (int i = 0; i < LastTitlesOld.Length; i++)
            {
                string str = LastTitlesOld[i];
                if (str == title)
                {
                    Index = i;
                    break;
                }
            }

            //if the title is new
            if (Index == -1)
            {
                List<string> LastTitlesNew = new List<string>();
                LastTitlesNew.Add(title);
                foreach (string s in LastTitlesOld)
                {
                    LastTitlesNew.Add(s);
                }
                int size = Helper.ReadInt(Config.TitleHistorySize);
                Helper.WriteProperties(Config.LastTitles, LastTitlesNew.GetRange(0, Math.Min(LastTitlesNew.Count, size)).ToArray());
            }
            //if the title is in the list already, bring it to the front
            else
            {
                List<string> items = new List<string>(LastTitlesOld);
                items.RemoveAt(Index);
                items.Insert(0, title);
                Helper.WriteProperties(Config.LastTitles, items.ToArray());
            }
        }
        

        /// <summary>
        /// Sets a new path for the list view
        /// </summary>
        /// <param name="path">Path to be set</param>
        public void SetPath(string path)
        {
            DateTime dt = DateTime.Now;
            Info.timecreatenewname = 0;
            Info.timeextractname = 0;
            Info.timesetpath = 0;
            Info.timesetuprelation = 0;
            Info.timeextractnumbers = 0;
            if (path == null || path == "" || !Directory.Exists(path)) return;

            if (path.Length == 2)
            {
                if (char.IsLetter(path[0]) && path[1] == ':')
                {
                    path = path + Path.DirectorySeparatorChar;
                }
            }
            DirectoryInfo currentpath=new DirectoryInfo(path);
            
            //fix casing of the path if user entered it
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
                Helper.WriteProperty(Config.LastDirectory, path);
                txtPath.Text = path;
                Environment.CurrentDirectory = path;
                UpdateList(true);
            }
            Info.timeloadfolder = (DateTime.Now - dt).TotalSeconds;
            Helper.Log("Time for loading folder: " + Info.timeloadfolder + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for extracting names: " + Info.timeextractname + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for creating paths: " + Info.timesetpath + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for creating filenames: " + Info.timecreatenewname + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for assigning relations: " + Info.timesetuprelation + " Seconds", Helper.LogType.Info);
            Helper.Log("Time for extracting numbers: " + Info.timeextractnumbers + " Seconds", Helper.LogType.Info);
        }
        /// <summary>
        /// Extracts season from directory name
        /// </summary>
        /// <param name="path">path from which to extract the data (NO FILEPATH, JUST FOLDER)</param>
        /// <returns>recognized season, -1 if not recognized</returns>
        public int ExtractSeasonFromDirectory(string path)
        {
            string[] patterns = Helper.ReadProperties(Config.Extract);
            string[] folders = path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for(int i=patterns.Length-1;i>=0;i--){
                string pattern=patterns[i];
                pattern = pattern.Replace("%T", "*.?");
                pattern = pattern.Replace("%S", "(?<Season>\\d)");
                Match m = Regex.Match(folders[folders.Length - 1], pattern, RegexOptions.IgnoreCase);
                
                if (m.Success)
                {
                    try
                    {
                        return Int32.Parse(m.Groups["Season"].Value);
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }
            return -1;
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
            InfoEntry ie = Info.Episodes[item];
            if (direction == false)
            {
                lvi.SubItems[0].Text = ie.Filename;
                lvi.SubItems[1].Text = ie.Path;
                lvi.SubItems[2].Text = ie.Season;
                lvi.SubItems[3].Text = ie.Episode;
                lvi.SubItems[4].Text = ie.Name;
                lvi.SubItems[5].Text = ie.NewFileName;
                lvi.SubItems[6].Text = ie.Destination;
                lvi.SubItems[7].Text = ie.Showname;
                lvi.Checked = ie.Process;
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
                ie.Showname = lvi.SubItems[7].Text;
                ie.Process = lvi.Checked;
            }

            Colorize(lvi);
        }
        
        /// <summary>
        /// Fills list view control with info data
        /// </summary>
        private void FillListView()
        {
            lstFiles.Items.Clear();
            for (int i = 0; i < Info.Episodes.Count; i++)
            {
                InfoEntry ie = Info.Episodes[i];
                ListViewItem lvi = new ListViewItem(ie.Filename);
                lvi.Tag = i;
                lvi.SubItems.Add(ie.Path);
                lvi.SubItems.Add(ie.Season);
                lvi.SubItems.Add(ie.Episode);
                lvi.SubItems.Add(ie.Name);
                lvi.SubItems.Add(ie.NewFileName);
                lvi.SubItems.Add(ie.Destination);
                lvi.SubItems.Add(ie.Showname);
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


        /*bool UsedDropDown = false;
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
        }*/       

        private void originalNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach(ListViewItem lvi in lstFiles.SelectedItems){
                clipboard += Info.Episodes[((int)lvi.Tag)].Filename + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void pathOrigNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                clipboard += Info.Episodes[((int)lvi.Tag)].Path + Path.DirectorySeparatorChar + Info.Episodes[((int)lvi.Tag)].Filename + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                clipboard += Info.Episodes[((int)lvi.Tag)].Name + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void newFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                clipboard += Info.Episodes[((int)lvi.Tag)].NewFileName + Environment.NewLine;
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void destinationNewFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                if (Info.Episodes[((int)lvi.Tag)].Destination != "" && Info.Episodes[((int)lvi.Tag)].NewFileName != "")
                {
                    clipboard += Info.Episodes[((int)lvi.Tag)].Destination + Path.DirectorySeparatorChar + Info.Episodes[((int)lvi.Tag)].NewFileName + Environment.NewLine;
                }
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
            clipboard = clipboard.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Clipboard.SetText(clipboard);
        }

        private void operationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                InfoEntry ie = Info.Episodes[((int)lvi.Tag)];
                if (ie.Destination != "")
                {
                    clipboard += ie.Filename + " --> " + ie.Destination + Path.DirectorySeparatorChar + ie.NewFileName + Environment.NewLine;
                }
                else
                {
                    clipboard += ie.Filename + " --> " + ie.NewFileName + Environment.NewLine;
                }
            }
            clipboard = clipboard.Substring(0, Math.Max(clipboard.Length - Environment.NewLine.Length, 0));
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

        

        private void removeTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveVideoTags();           
        }        

        /// <summary>
        /// Syncs all items
        /// </summary>
        /// <param name="direction">Direction in which synching should occur. false = data->GUI, true = GUI->data</param>
        private void SyncAllItems(bool direction)
        {
            foreach (ListViewItem lvi in lstFiles.Items)
            {
                SyncItem((int)lvi.Tag, direction);
            }
        }
        
        private void replaceInPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceWindow rw = new ReplaceWindow(this);
            rw.Show();
            rw.TopMost = true;
        }




        /// <summary>
        /// Replaces strings from various fields in selected files
        /// </summary>
        /// <param name="SearchString">String to look for, may contain parameters</param>
        /// <param name="ReplaceString">Replace string, may contain parameters</param>
        /// <param name="Source">Field from which the source string is taken</param>
        public void Replace(string SearchString, string ReplaceString, string Source)
        {
            int count = 0;            
            string title = Helper.ReadProperties(Config.LastTitles)[0];
            string basedir=Helper.ReadProperty(Config.LastDirectory);
            string destination = "Filename";
            if (Source.Contains("Path")) destination = "Path";
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                InfoEntry ie = Info.Episodes[(int)lvi.Tag];
                string source = "";
                
                string LocalSearchString = SearchString;
                string LocalReplaceString = ReplaceString;
                //aquire source string
                switch(Source){
                    case "Original Filename":
                        source=ie.Filename;
                        break;
                    case "Original Path":
                        source=ie.Path;
                        break;
                    case "Destination Filename":
                        source=ie.NewFileName;
                        break;
                    case "Destination Path":
                        source=ie.Destination;
                        break;
                }
                //Insert parameter values
                LocalSearchString = LocalSearchString.Replace("%OF", ie.Filename);
                LocalSearchString = LocalSearchString.Replace("%DF", ie.NewFileName);
                LocalSearchString = LocalSearchString.Replace("%OP", ie.Path);
                LocalSearchString = LocalSearchString.Replace("%DP", ie.Destination);
                LocalSearchString = LocalSearchString.Replace("%T", title);
                LocalSearchString = LocalSearchString.Replace("%N", ie.Name);
                LocalSearchString = LocalSearchString.Replace("%E", ie.Episode);
                LocalSearchString = LocalSearchString.Replace("%s", ie.Season);
                LocalSearchString = LocalSearchString.Replace("%BD", basedir);
                string LongSeason = ie.Season;
                if (LongSeason.Length == 1)
                {
                    LongSeason = "0" + LongSeason;
                }
                LocalSearchString = LocalSearchString.Replace("%S", LongSeason);
                LocalReplaceString = LocalReplaceString.Replace("%OF", ie.Filename);
                LocalReplaceString = LocalReplaceString.Replace("%DF", ie.NewFileName);
                LocalReplaceString = LocalReplaceString.Replace("%OP", ie.Path);
                LocalReplaceString = LocalReplaceString.Replace("%DP", ie.Destination);
                LocalReplaceString = LocalReplaceString.Replace("%T", title);
                LocalReplaceString = LocalReplaceString.Replace("%N", ie.Name);
                LocalReplaceString = LocalReplaceString.Replace("%E", ie.Episode);
                LocalReplaceString = LocalReplaceString.Replace("%s", ie.Season);
                LocalReplaceString = LocalReplaceString.Replace("%S", LongSeason);

                //see if replace will be done for count var
                if (source.Contains(SearchString)) count++;
                //do the replace
                source = source.Replace(LocalSearchString, LocalReplaceString);
                if (destination == "Filename")
                {
                    ie.NewFileName = source;
                }
                else if (destination == "Path")
                {
                    ie.Destination = source;
                }

                //mark files for processing
                ie.Process = true;
                SyncItem((int)lvi.Tag, false);
            }
            if (count > 0)
            {
                Helper.Log(SearchString + " was replaced with " + ReplaceString + " in " + count + " fields.", Helper.LogType.Status);
            }
            else
            {
                Helper.Log(SearchString + " was not found in any of the selected files.", Helper.LogType.Status);
            }
        }

        private void byNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string>names=new List<string>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                string Showname = Info.Episodes[(int)lvi.Tag].Showname;
                if (!names.Contains(Showname))
                {
                    names.Add(Showname);
                }
            }
            List<InfoEntry> similar = new List<InfoEntry>();
            foreach (string str in names)
            {
                similar.AddRange(Helper.FindSimilarByName(Info.Episodes, str));
            }
            foreach (ListViewItem lvi in lstFiles.Items)
            {
                if (similar.Contains(Info.Episodes[(int)lvi.Tag]))
                {
                    lvi.Selected = true;
                }
                else
                {
                    lvi.Selected = false;
                }
            }
        }

        private void byPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> paths = new List<string>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                string path = Info.Episodes[(int)lvi.Tag].Path;
                if (!paths.Contains(path))
                {
                    paths.Add(path);
                }
            }
            List<InfoEntry> similar=new List<InfoEntry>();
            foreach (string str in paths)
            {
                similar.AddRange(Helper.FindSimilarByPath(Info.Episodes, str));
            }
            foreach(ListViewItem lvi in lstFiles.Items){
                if(similar.Contains(Info.Episodes[(int)lvi.Tag])){
                    lvi.Selected=true;
                }else{
                    lvi.Selected=false;
                }
            }
        }

        private void createDirectoryStructureToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].CreateDirectoryStructure = InfoEntry.DirectoryStructure.CreateDirectoryStructure;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            dontCreateDirectoryStructureToolStripMenuItem.Checked = false;                
        }

        private void dontCreateDirectoryStructureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].CreateDirectoryStructure = InfoEntry.DirectoryStructure.NoDirectoryStructure;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            createDirectoryStructureToolStripMenuItem.Checked = false;
        }

        private void useUmlautsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].UmlautUsage = InfoEntry.UmlautAction.Use;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            dontUseUmlautsToolStripMenuItem.Checked = false;
            useProvidedNamesToolStripMenuItem.Checked = false;
        }

        private void dontUseUmlautsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].UmlautUsage = InfoEntry.UmlautAction.Dont_Use;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            useUmlautsToolStripMenuItem.Checked = false;
            useProvidedNamesToolStripMenuItem.Checked = false;
        }

        private void useProvidedNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].UmlautUsage = InfoEntry.UmlautAction.Ignore;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            useUmlautsToolStripMenuItem.Checked = false;
            dontUseUmlautsToolStripMenuItem.Checked = false;
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].Casing = InfoEntry.Case.Large;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            smallToolStripMenuItem.Checked = false;
            igNorEToolStripMenuItem.Checked = false;
            cAPSLOCKToolStripMenuItem.Checked = false;
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].Casing = InfoEntry.Case.small;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            largeToolStripMenuItem.Checked = false;
            igNorEToolStripMenuItem.Checked = false;
            cAPSLOCKToolStripMenuItem.Checked = false;
        }

        private void igNorEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].Casing = InfoEntry.Case.Ignore;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            smallToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = false;
            cAPSLOCKToolStripMenuItem.Checked = false;
        }

        private void cAPSLOCKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                Info.Episodes[(int)lvi.Tag].Casing = InfoEntry.Case.CAPSLOCK;
                SyncItem((int)lvi.Tag, false);
            }
            ((ToolStripMenuItem)sender).Checked = true;
            smallToolStripMenuItem.Checked = false;
            igNorEToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = false;
        }

        private void setShownameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> ht = new Dictionary<string, int>();
            foreach (ListViewItem lvi in lstFiles.SelectedItems)
            {
                if(!ht.ContainsKey(Info.Episodes[(int)lvi.Tag].Showname)){
                    ht.Add(Info.Episodes[(int)lvi.Tag].Showname,1);
                }else{
                    ht[Info.Episodes[(int)lvi.Tag].Showname]+=1;
                }
            }
            int max = 0;
            string Showname="";
            foreach (KeyValuePair<string,int> pair in ht)
            {
                if (pair.Value > max)
                {
                    Showname = pair.Key;
                }
            }
            EnterShowname es = new EnterShowname(Showname);
            if (es.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem lvi in lstFiles.SelectedItems)
                {
                    Info.Episodes[(int)lvi.Tag].Showname = es.SelectedName;
                    SyncItem((int)lvi.Tag, false);
                }
            }
        }

        private void regexTesterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegexTester rt = new RegexTester();
            rt.Show();
        }        
    }        
}

