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
using Renamer.Classes;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Classes.Configuration;
using Renamer.Logging;
namespace Renamer
{
    /// <summary>
    /// Database class containing all relevant data used in this program
    /// </summary>
    class Info
    {
        /// <summary>
        /// List of files, their target destinations etc
        /// </summary>
        public static List<InfoEntry> Episodes = new List<InfoEntry>();

        /// <summary>
        /// List of season/episode<->name relations
        /// </summary>
        public static List<RelationCollection> Relations = new List<RelationCollection>();

        /// <summary>
        /// List of season/episode<->name relation providers
        /// </summary>
        public List<RelationProvider> Providers = new List<RelationProvider>();

        /// <summary>
        /// List of subtitle file providers
        /// </summary>
        public List<SubtitleProvider> SubProviders = new List<SubtitleProvider>();

        /// <summary>
        /// List of subtitle links which are to be downloaded
        /// </summary>
        public List<string> SubtitleLinks = new List<string>();

        /// <summary>
        /// List of downloaded subtitle files in temp directory
        /// </summary>
        public List<SubtitleFile> SubtitleFiles = new List<SubtitleFile>();

        public static double timeloadfolder = 0;
        public static double timegettitles = 0;
        public static double timeextractname = 0;
        public static double timesetpath = 0;
        public static double timecreatenewname = 0;
        public static double timesetuprelation = 0;
        public static double timeextractnumbers = 0;
        /// <summary>
        /// Constructor, loads all providers
        /// </summary>
        public Info() {
            //Get all series name providers
            List<string> providers = new List<string>(Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Databases/Titles"));
            string status = "Providers found:";
            foreach (string file in providers) {
                if (Settings.getInstance().IsMonoCompatibilityMode) {
                    Logger.Instance.LogMessage("Provider: " + file, LogLevel.INFO);
                }
                RelationProvider rel = new RelationProvider();
                rel.Name = Helper.ReadProperty(ProviderConfig.Name, file);
                rel.RelationsPage = Helper.ReadProperty(ProviderConfig.RelationsPage, file);
                rel.RelationsRegExp = Helper.ReadProperty(ProviderConfig.RelationsRegExp, file);
                rel.SearchRegExp = Helper.ReadProperty(ProviderConfig.SearchRegExp, file);
                rel.SearchResultsURL = Helper.ReadProperty(ProviderConfig.SearchResultsURL, file);
                rel.SearchURL = Helper.ReadProperty(ProviderConfig.SearchURL, file);
                rel.SeriesURL = Helper.ReadProperty(ProviderConfig.SeriesURL, file);
                rel.EpisodesURL = Helper.ReadProperty(ProviderConfig.EpisodesURL, file);
                rel.SearchRemove = Helper.ReadProperties(ProviderConfig.SearchRemove, file);
                rel.SearchStart = Helper.ReadProperty(ProviderConfig.SearchStart, file);
                rel.SearchEnd = Helper.ReadProperty(ProviderConfig.SearchEnd, file);
                rel.RelationsStart = Helper.ReadProperty(ProviderConfig.RelationsStart, file);
                rel.RelationsEnd = Helper.ReadProperty(ProviderConfig.RelationsEnd, file);
                rel.NotFoundURL = Helper.ReadProperty(ProviderConfig.NotFoundURL, file);
                rel.Encoding = Helper.ReadProperty(ProviderConfig.Encoding, file);
                rel.Language = (Helper.Languages)Enum.Parse(typeof(Helper.Languages), Helper.ReadProperty(ProviderConfig.Language, file));
                string rrtl = Helper.ReadProperty(ProviderConfig.RelationsRightToLeft, file);
                if (rrtl == "1") {
                    rel.RelationsRightToLeft = true;
                }
                else {
                    rel.RelationsRightToLeft = false;
                }
                string srtl = Helper.ReadProperty(ProviderConfig.SearchRightToLeft, file);
                if (srtl == "1") {
                    rel.SearchRightToLeft = true;
                }
                else {
                    rel.SearchRightToLeft = false;
                }
                if (rel.Name == null || rel.Name == "") {
                    Logger.Instance.LogMessage("Invalid provider file: " + file, LogLevel.ERROR);
                    continue;
                }
                Providers.Add(rel);
                status += " " + rel.Name + ",";
            }
            status = status.TrimEnd(new char[] { ',' });
            Logger.Instance.LogMessage(status, LogLevel.INFO);
            //Get all subtitle providers
            List<string> subproviders = new List<string>(Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Databases/Subtitles"));
            status = "Subtitle Providers found:";
            foreach (string str in subproviders) {
                SubtitleProvider sub = new SubtitleProvider();
                sub.Name = Helper.ReadProperty(SubProviderConfig.Name, str);
                sub.SubtitlesPage = Helper.ReadProperty(SubProviderConfig.SubtitlesPage, str);
                sub.SubtitleRegExp = Helper.ReadProperty(SubProviderConfig.SubtitleRegExp, str);
                sub.SearchRegExp = Helper.ReadProperty(SubProviderConfig.SearchRegExp, str);
                sub.SearchResultsURL = Helper.ReadProperty(SubProviderConfig.SearchResultsURL, str);
                sub.SearchURL = Helper.ReadProperty(SubProviderConfig.SearchURL, str);
                sub.SeriesURL = Helper.ReadProperty(SubProviderConfig.SeriesURL, str);
                sub.SubtitlesURL = Helper.ReadProperty(SubProviderConfig.SubtitlesURL, str);
                sub.SearchRemove = Helper.ReadProperties(SubProviderConfig.SearchRemove, str);
                sub.SearchStart = Helper.ReadProperty(SubProviderConfig.SearchStart, str);
                sub.SearchEnd = Helper.ReadProperty(SubProviderConfig.SearchEnd, str);
                sub.SubtitlesStart = Helper.ReadProperty(SubProviderConfig.SubtitlesStart, str);
                sub.SubtitlesEnd = Helper.ReadProperty(SubProviderConfig.SubtitlesEnd, str);
                sub.ConstructLink = Helper.ReadProperty(SubProviderConfig.ConstructLink, str);
                sub.NotFoundURL = Helper.ReadProperty(SubProviderConfig.NotFoundURL, str);
                sub.Encoding = Helper.ReadProperty(SubProviderConfig.Encoding, str);
                sub.Language = (Helper.Languages)Enum.Parse(typeof(Helper.Languages), Helper.ReadProperty(SubProviderConfig.Language, str));
                string srtl = Helper.ReadProperty(SubProviderConfig.SearchRightToLeft, str);
                if (srtl == "1") {
                    sub.SearchRightToLeft = true;
                }
                else {
                    sub.SearchRightToLeft = false;
                }
                string directlink = Helper.ReadProperty(SubProviderConfig.DirectLink, str);
                if (directlink == "1") {
                    sub.DirectLink = true;
                }
                else {
                    sub.DirectLink = false;
                }
                SubProviders.Add(sub);
                status += " " + sub.Name + ",";
            }
            status = status.TrimEnd(new char[] { ',' });
            Logger.Instance.LogMessage(status, LogLevel.INFO);
        }

        /// <summary>
        /// Gets currently selected provider
        /// </summary>
        /// <returns>Currently selected provider, or null if error</returns>
        public RelationProvider GetCurrentProvider() {
            return GetProviderByName(Helper.ReadProperty(Config.LastProvider));
        }

        /// <summary>
        /// Gets a provider by its name
        /// </summary>
        /// <param name="name">name of the provider</param>
        /// <returns>provider matching the name, or null if not found</returns>
        public RelationProvider GetProviderByName(string name) {
            foreach (RelationProvider rp in Providers) {
                if (rp.Name == name) {
                    return rp;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets currently selected subtitle provider
        /// </summary>
        /// <returns>Currently selected subtitle provider, or null if error</returns>
        public SubtitleProvider GetCurrentSubtitleProvider() {
            return GetSubtitleProviderByName(Helper.ReadProperty(Config.LastSubProvider));
        }

        /// <summary>
        /// Gets a subtitle provider by its name
        /// </summary>
        /// <param name="name">name of the subtitle provider</param>
        /// <returns>subtitle provider matching the name, or null if not found</returns>
        public SubtitleProvider GetSubtitleProviderByName(string name) {
            foreach (SubtitleProvider sp in SubProviders) {
                if (sp.Name == name) {
                    return sp;
                }
            }
            return null;
        }



        /// <summary>
        /// Gets video files matching season and episode number
        /// </summary>
        /// <param name="season">season to search for</param>
        /// <param name="episode">episode to search for</param>
        /// <returns>List of all matching InfoEntries, never null, but may be empty</returns>
        public List<InfoEntry> GetMatchingVideos(int season, int episode) {
            List<InfoEntry> lie = new List<InfoEntry>();
            foreach (InfoEntry ie in Episodes) {
                if (ie.Season == season.ToString() && ie.Episode == episode.ToString() && IsVideo(ie)) {
                    lie.Add(ie);
                }
            }
            return lie;
        }

        /// <summary>
        /// Gets subtitle files matching season and episode number
        /// </summary>
        /// <param name="season">season to search for</param>
        /// <param name="episode">episode to search for</param>
        /// <returns>List of all matching InfoEntries, never null, but may be empty</returns>
        public List<InfoEntry> GetMatchingSubtitles(int season, int episode) {
            List<InfoEntry> lie = new List<InfoEntry>();
            foreach (InfoEntry ie in Episodes) {
                if (ie.Season == season.ToString() && ie.Episode == episode.ToString() && IsSubtitle(ie)) {
                    lie.Add(ie);
                }
            }
            return lie;
        }

        /// <summary>
        /// Gets video file matching to subtitle
        /// </summary>
        /// <param name="ieSubtitle">InfoEntry of a subtitle to find matching video file for</param>
        /// <returns>Matching video file</returns>
        public InfoEntry GetVideo(InfoEntry ieSubtitle) {
            List<string> vidext = new List<string>(Helper.ReadProperties(Config.Extensions));
            foreach (InfoEntry ie in Episodes) {
                if (Path.GetFileNameWithoutExtension(ieSubtitle.Filename) == Path.GetFileNameWithoutExtension(ie.Filename)) {
                    if (vidext.Contains(ie.Extension)) {
                        return ie;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Check if InfoEntry is a video
        /// </summary>
        /// <param name="ie">InfoEntry to check</param>
        /// <returns>true if video file, false otherwise</returns>
        public bool IsVideo(InfoEntry ie) {
            List<string> extensions = new List<string>(Helper.ReadProperties(Config.Extensions));
            for (int a = 0; a < extensions.Count; a++) {
                extensions[a] = extensions[a].ToLower();
            }
            if (extensions == null) {
                Logger.Instance.LogMessage("No Extensions found!", LogLevel.WARNING);
                return false;
            }
            if (extensions.Contains(ie.Extension)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if InfoEntry is a subtitle
        /// </summary>
        /// <param name="ie">InfoEntry to check</param>
        /// <returns>true if subtitle file, false otherwise</returns>
        public bool IsSubtitle(InfoEntry ie) {
            List<string> extensions = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
            for (int a = 0; a < extensions.Count; a++) {
                extensions[a] = extensions[a].ToLower();
            }
            if (extensions == null) {
                Logger.Instance.LogMessage("No Subtitle Extensions found!", LogLevel.WARNING);
                return false;
            }
            if (extensions.Contains(ie.Extension)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets subtitle file matching to video
        /// </summary>
        /// <param name="ieVideo">InfoEntry of a video to find matching subtitle file for</param>
        /// <returns>Matching subtitle file</returns>
        public InfoEntry GetSubtitle(InfoEntry ieVideo) {
            List<string> subext = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
            foreach (InfoEntry ie in Episodes) {
                if (Path.GetFileNameWithoutExtension(ieVideo.Filename) == Path.GetFileNameWithoutExtension(ie.Filename)) {
                    if (subext.Contains(ie.Extension)) {
                        return ie;
                    }
                }
            }
            return null;
        }
        public int GetNumberOfVideoFilesInFolder(string path) {
            List<string> vidext = new List<string>(Helper.ReadProperties(Config.Extensions));
            int count = 0;
            foreach (string file in Directory.GetFiles(path)) {
                if (vidext.Contains(Path.GetFileNameWithoutExtension(file))) {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Returns a RelationCollection with a given Showname
        /// </summary>
        /// <param name="Showname">The Showname</param>
        /// <returns>The RelationCollection, or null if not found</returns>
        public static RelationCollection GetRelationCollectionByName(string Showname) {
            foreach (RelationCollection rc in Relations) {
                if (rc.Showname == Showname) {
                    return rc;
                }
            }
            return null;
        }

        public static void AddRelationCollection(RelationCollection rc) {
            Relations.Add(rc);
            foreach (InfoEntry ie in Info.Episodes) {
                if (ie.Showname == rc.Showname) {
                    ie.SetupRelation();
                }
            }
        }
    }












}

