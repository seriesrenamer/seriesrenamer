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
using Renamer.Classes.Provider;
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

