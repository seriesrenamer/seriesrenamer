using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
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
        public List<InfoEntry> Episodes = new List<InfoEntry>();

        /// <summary>
        /// List of season/episode<->name relations
        /// </summary>
        public List<Relation> Relations = new List<Relation>();

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

        /// <summary>
        /// Constructor, loads all providers
        /// </summary>
        public Info()
        {
            //Get all series name providers
            List<string> providers = new List<string>(Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Databases/Titles"));
            string status = "Providers found:";
            foreach (string str in providers)
            {
                if(Settings.MonoCompatibilityMode)
                Helper.Log("Provider: " + str, Helper.LogType.Status);
                RelationProvider rel = new RelationProvider();
                rel.Name = Helper.ReadProperty(ProviderConfig.Name, str);
                rel.RelationsPage = Helper.ReadProperty(ProviderConfig.RelationsPage, str);
                rel.RelationsRegExp = Helper.ReadProperty(ProviderConfig.RelationsRegExp, str);
                rel.SearchRegExp = Helper.ReadProperty(ProviderConfig.SearchRegExp, str);
                rel.SearchResultsURL = Helper.ReadProperty(ProviderConfig.SearchResultsURL, str);
                rel.SearchURL = Helper.ReadProperty(ProviderConfig.SearchURL, str);
                rel.SeriesURL = Helper.ReadProperty(ProviderConfig.SeriesURL, str);
                rel.EpisodesURL = Helper.ReadProperty(ProviderConfig.EpisodesURL, str);
                rel.SearchRemove = Helper.ReadProperties(ProviderConfig.SearchRemove, str);
                rel.SearchStart = Helper.ReadProperty(ProviderConfig.SearchStart, str);
                rel.SearchEnd = Helper.ReadProperty(ProviderConfig.SearchEnd, str);
                rel.RelationsStart = Helper.ReadProperty(ProviderConfig.RelationsStart, str);
                rel.RelationsEnd = Helper.ReadProperty(ProviderConfig.RelationsEnd, str);
                string rrtl=Helper.ReadProperty(ProviderConfig.RelationsRightToLeft, str);
                if (rrtl == "1")
                {
                    rel.RelationsRightToLeft = true;
                }
                else
                {
                    rel.RelationsRightToLeft = false;
                }
                string srtl=Helper.ReadProperty(ProviderConfig.SearchRightToLeft, str);
                if (srtl == "1")
                {
                    rel.SearchRightToLeft = true;
                }
                else
                {
                    rel.SearchRightToLeft = false;
                }
                if (rel.Name == null||rel.Name=="")
                {
                    Helper.Log("Invalid provider file: " + str, Helper.LogType.Error);
                    continue;
                }
                Providers.Add(rel);
                status += " " + rel.Name+",";
            }
            status = status.TrimEnd(new char[] { ',' });
            Helper.Log(status, Helper.LogType.Info);
            //Get all subtitle providers
            List<string> subproviders = new List<string>(Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Databases/Subtitles"));
            status = "Subtitle Providers found:";
            foreach (string str in subproviders)
            {
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
                sub.SubtitlesEnd = Helper.ReadProperty (SubProviderConfig.SubtitlesEnd, str);
                string srtl = Helper.ReadProperty(SubProviderConfig.SearchRightToLeft, str);
                if (srtl == "1")
                {
                    sub.SearchRightToLeft = true;
                }
                else
                {
                    sub.SearchRightToLeft = false;
                }
                string directlink = Helper.ReadProperty(SubProviderConfig.DirectLink, str);
                if (directlink == "1")
                {
                    sub.DirectLink = true;
                }
                else
                {
                    sub.DirectLink = false;
                }
                SubProviders.Add(sub);
                status += " " + sub.Name + ",";
            }
            status=status.TrimEnd(new char[] { ',' });
            Helper.Log(status, Helper.LogType.Info);
        }

        /// <summary>
        /// Gets currently selected provider
        /// </summary>
        /// <returns>Currently selected provider, or null if error</returns>
        public RelationProvider GetCurrentProvider(){
            return GetProviderByName(Helper.ReadProperty(Config.LastProvider));
        }

        /// <summary>
        /// Gets a provider by its name
        /// </summary>
        /// <param name="name">name of the provider</param>
        /// <returns>provider matching the name, or null if not found</returns>
        public RelationProvider GetProviderByName(string name)
        {
            foreach (RelationProvider rp in Providers)
            {
                if (rp.Name == name)
                {
                    return rp;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets currently selected subtitle provider
        /// </summary>
        /// <returns>Currently selected subtitle provider, or null if error</returns>
        public SubtitleProvider GetCurrentSubtitleProvider()
        {
            return GetSubtitleProviderByName(Helper.ReadProperty(Config.LastSubProvider));
        }

        /// <summary>
        /// Gets a subtitle provider by its name
        /// </summary>
        /// <param name="name">name of the subtitle provider</param>
        /// <returns>subtitle provider matching the name, or null if not found</returns>
        public SubtitleProvider GetSubtitleProviderByName(string name)
        {
            foreach (SubtitleProvider sp in SubProviders)
            {
                if (sp.Name == name)
                {
                    return sp;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds lowest season in relations
        /// </summary>
        /// <returns>index of the season, or 10000 ;)</returns>
        public int FindMinSeason()
        {
            int min = 10000;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) < min) min = Convert.ToInt32(rel.Season);
            }
            return min;
        }

        /// <summary>
        /// Finds highest season in relations
        /// </summary>
        /// <returns>index of the season, or -1 ;)</returns>
        public int FindMaxSeason()
        {
            int max = -1;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) > max) max = Convert.ToInt32(rel.Season);
            }
            return max;
        }

        /// <summary>
        /// Finds lowest episode of a season in relations
        /// </summary>
        /// <param name="season">season to find lowest episode relation in</param>
        /// <returns>index of the episode, or 10000 ;)</returns>
        public int FindMinEpisode(int season)
        {
            int min = 10000;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) == season)
                {
                    if (Convert.ToInt32(rel.Episode) < min) min = Convert.ToInt32(rel.Episode);
                }
            }
            return min;
        }

        /// <summary>
        /// Finds highest episode of a season in relations
        /// </summary>
        /// <param name="season">season to find highest episode relation in</param>
        /// <returns>index of the episode, or -1 ;)</returns>
        public int FindMaxEpisode(int season)
        {
            int max = -1;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) == season)
                {
                    if (Convert.ToInt32(rel.Episode) > max) max = Convert.ToInt32(rel.Episode);
                }
            }
            return max;
        }

        /// <summary>
        /// Gets video files matching season and episode number
        /// </summary>
        /// <param name="season">season to search for</param>
        /// <param name="episode">episode to search for</param>
        /// <returns>List of all matching InfoEntries, never null, but may be empty</returns>
        public List<InfoEntry> GetMatchingVideos(int season, int episode)
        {
            List<InfoEntry> lie=new List<InfoEntry>();
            foreach (InfoEntry ie in Episodes)
            {
                if (ie.Season == season.ToString() && ie.Episode == episode.ToString() && IsVideo(ie))
                {
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
        public List<InfoEntry> GetMatchingSubtitles(int season, int episode)
        {
            List<InfoEntry> lie = new List<InfoEntry>();
            foreach (InfoEntry ie in Episodes)
            {
                if (ie.Season == season.ToString() && ie.Episode == episode.ToString() && IsSubtitle(ie))
                {
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
        public InfoEntry GetVideo(InfoEntry ieSubtitle)
        {
            List<string> vidext = new List<string>(Helper.ReadProperties(Config.Extensions));
            foreach (InfoEntry ie in Episodes)
            {
                if (Path.GetFileNameWithoutExtension(ieSubtitle.Filename) == Path.GetFileNameWithoutExtension(ie.Filename))
                {
                    if (vidext.Contains(ie.Extension))
                    {
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
        public bool IsVideo(InfoEntry ie)
        {
            List<string> extensions = new List<string>(Helper.ReadProperties(Config.Extensions));
            for (int a = 0; a < extensions.Count; a++)
            {
                extensions[a] = extensions[a].ToLower();
            }
            if (extensions == null)
            {
                Helper.Log("No Extensions found!", Helper.LogType.Warning);
                return false;
            }
            if (extensions.Contains(ie.Extension))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if InfoEntry is a subtitle
        /// </summary>
        /// <param name="ie">InfoEntry to check</param>
        /// <returns>true if subtitle file, false otherwise</returns>
        public bool IsSubtitle(InfoEntry ie)
        {
            List<string> extensions = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
            for (int a = 0; a < extensions.Count; a++)
            {
                extensions[a] = extensions[a].ToLower();
            }
            if (extensions == null)
            {
                Helper.Log("No Subtitle Extensions found!", Helper.LogType.Warning);
                return false;
            }
            if (extensions.Contains(ie.Extension))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets subtitle file matching to video
        /// </summary>
        /// <param name="ieVideo">InfoEntry of a video to find matching subtitle file for</param>
        /// <returns>Matching subtitle file</returns>
        public InfoEntry GetSubtitle(InfoEntry ieVideo)
        {
            List<string> subext = new List<string>(Helper.ReadProperties(Config.SubtitleExtensions));
            foreach (InfoEntry ie in Episodes)
            {
                if (Path.GetFileNameWithoutExtension(ieVideo.Filename) == Path.GetFileNameWithoutExtension(ie.Filename))
                {
                    if (subext.Contains(ie.Extension))
                    {
                        return ie;
                    }
                }
            }
            return null;
        }
        public int GetNumberOfVideoFilesInFolder(string path)
        {
            List<string> vidext = new List<string>(Helper.ReadProperties(Config.Extensions));
            int count = 0;
            foreach (string file in Directory.GetFiles(path))
            {
                if (vidext.Contains(Path.GetFileNameWithoutExtension(file)))
                {
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Contains all information about a single video or subtitle file, including scheduled renaming info
    /// </summary>
    public class InfoEntry
    {
        /// <summary>
        /// Old filename with extension
        /// </summary>
        public string Filename = "";   
     
        /// <summary>
        /// Extension of the file without dot, i.e. "avi" or "srt"
        /// </summary>
        public string Extension = "";

        /// <summary>
        /// Path of the file
        /// </summary>
        public string Path = "";

        /// <summary>
        /// number of the season
        /// </summary>
        public string Season = "";

        /// <summary>
        /// number of the episode
        /// </summary>
        public string Episode = "";

        /// <summary>
        /// name of the episode
        /// </summary>
        public string Name = "";

        /// <summary>
        /// new filename with extension
        /// </summary>
        public string NewFileName = "";

        /// <summary>
        /// destination directory
        /// </summary>
        public string Destination = "";

        /// <summary>
        /// If file is to be processed
        /// </summary>
        public bool Process = true;
    }

    /// <summary>
    /// A season/episode<->name relation
    /// </summary>
    class Relation
    {
        /// <summary>
        /// Season
        /// </summary>
        public string Season = "";

        /// <summary>
        /// Episode
        /// </summary>
        public string Episode = "";

        /// <summary>
        /// Name
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="season">Season</param>
        /// <param name="episode">Episode</param>
        /// <param name="name">Name</param>
        public Relation(string season, string episode, string name)
        {
            Season = season;
            Episode = episode;
            Name = name;
        }

        public override string ToString()
        {
            return "S" + Season + "E" + Episode + " - " + Name;
        }
    }
    
    /// <summary>
    /// A collection of subtitle files matching to one season+episode
    /// </summary>
    public class SubtitleFile
    {
        /// <summary>
        /// List of subtitle files
        /// </summary>
        public List<string> Filenames = new List<string>();

        /// <summary>
        /// collective season value
        /// </summary>
        public string Season = "";

        /// <summary>
        /// Collective episode value
        /// </summary>
        public string Episode = "";
    }

    /// <summary>
    /// An abstract provider
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// Name of the provider
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Search URL, %T is a placeholder for the search title
        /// </summary>
        public string SearchURL = "";

        /// <summary>
        /// substring of the search results page URL
        /// </summary>
        public string SearchResultsURL = "";

        /// <summary>
        /// substring of the series page URL
        /// </summary>
        public string SeriesURL = "";

        /// <summary>
        /// Regular expression for parsing search results
        /// </summary>
        public string SearchRegExp = "";

        /// <summary>
        /// some regular expressions to remove from search results name
        /// </summary>
        public string[] SearchRemove;

        /// <summary>
        /// start regex for search pages from end of file
        /// </summary>
        public bool SearchRightToLeft = false;


        public string SearchStart = "";
        public string SearchEnd = "";
    }

    /// <summary>
    /// A season/episode<->name relations provider
    /// </summary>
    class RelationProvider:Provider
    {
        /// <summary>
        /// Link to the page containing episode infos. %L is used as placeholder for the link corresponding to the show the user selected
        /// </summary>
        public string RelationsPage = "";

        /// <summary>
        /// Regular expression to extract season/number/episode name relationship from the page containing this info
        /// This needs to contain:
        /// (?&ltSeason&gtRegExpToExtractSeason) - to get the season number
        /// (?&ltEpisode&gtRegExpToExtractEpisode) - to get the episode number
        /// (?&ltTitle&gtRegExpToExtractTitle) - to get the title belonging to that season/episode
        ///If Relationspage uses %S placeholder, there is no need to include (?<Season>RegExpToExtractSeason) here
        /// </summary>
        public string RelationsRegExp = "";

        /// <summary>
        /// start regex for relations pages from end of file
        /// </summary>
        public bool RelationsRightToLeft = false;

        /// <summary>
        /// Additionally, if the search engine redirects to the single result directly, we might need a string to attach to the results page to get to the episodes page
        /// </summary>
        public string EpisodesURL = "";

        public string RelationsStart = "";
        public string RelationsEnd = "";
    }
    
    /// <summary>
    /// A subtitle file provider
    /// </summary>
    class SubtitleProvider:Provider
    {
        /// <summary>
        /// Is the download link directly on the search results page?
        /// </summary>
        public bool DirectLink = false;

        /// <summary>
        /// Link to the page containing subtitle links. %L is used as placeholder for the link corresponding to the show the user selected
        /// For multiple pages of subtitle downloads, use %P
        /// </summary>
        public string SubtitlesPage = "";

        /// <summary>
        /// Regular expression to extract subtitle links (along with names) from downloads page
        /// This needs to contain: 
        /// (?&ltSeason&gtRegExpToExtractSeason) - to get the season number
        /// (?&ltEpisode&gtRegExpToExtractEpisode) - to get the episode number
        /// (?&ltLink&gtRegExpToExtractLink) - to get the download link for one episode
        /// If Package is set to 1, only download link is required
        /// </summary>
        public string SubtitleRegExp = "";

        /// <summary>
        /// Additionally, if the search engine redirects to the single result directly, we might need a string to attach to the results page to get to the episodes page
        /// </summary>
        public string SubtitlesURL = "";

        public string SubtitlesStart = "";
        public string SubtitlesEnd = "";
    }
}

