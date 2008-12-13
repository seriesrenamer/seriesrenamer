using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Renamer
{
    /// <summary>
    /// Class used to store settings which can't be stored in config file
    /// </summary>
    class Settings
    {
        /// <summary>
        /// filename of the main config file
        /// </summary>
        public static string ConfigName = "Renamer.cfg";

        /// <summary>
        /// Delimiter used in config file
        /// </summary>        
        public static char Delimiter
        {
            get { return delim; }
            set
            {
                if (delim != Convert.ToChar(0))
                {
                    Helper.WriteProperty(Config.Delimiter, value.ToString());
                }
                delim = value;                
            }
        }
        private static char delim;
        
        
        /// <summary>
        /// Comment string used in config file
        /// </summary>
        public static string Comment
        {
            get { return comment; }
            set
            {
                if (comment != null)
                {
                    Helper.WriteProperty(Config.Comment, value);                    
                }
                comment = value;
            }
        }
        private static string comment;

        /// <summary>
        /// List of cached config files
        /// </summary>
        public static List<ConfigFile> files=new List<ConfigFile>();

        /// <summary>
        /// Cache config file containing default values for main config file
        /// </summary>
        public static ConfigFile Defaults = new ConfigFile("");

        /// <summary>
        /// Mono compatibility mode
        /// </summary>
        public static bool MonoCompatibilityMode = false;

        /// <summary>
        /// if main config file is loaded, used for avoiding some recursion at startup
        /// </summary>
        public static bool ConfigLoaded = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Settings()
        {
            delim = Convert.ToChar(0);
            SetupDefaults();
        }

        /// <summary>
        /// Adds default values to default config file
        /// </summary>
        public void SetupDefaults(){
            Defaults.variables.Add(Config.Comment, "//");
            Defaults.variables.Add(Config.Delimiter, "^");
            Defaults.variables.Add(Config.Extensions, new List<string>(new string[] { "avi", "mpg", "mpeg", "mp4", "divx", "mkv", "wmv" }));
            Defaults.variables.Add(Config.SubtitleExtensions, new List<string>(new string[] { "srt", "sub" }));
            Defaults.variables.Add(Config.LogLevelInfo, "LogFile");
            Defaults.variables.Add(Config.LogLevelWarning, "LogFile");
            Defaults.variables.Add(Config.LogLevelError, "Log_and_Message");
            Defaults.variables.Add(Config.LogLevelStatus, "LogFile");
            Defaults.variables.Add(Config.LogLevelDebug, "None");
            Defaults.variables.Add(Config.LogName, "Renamer.log");
            Defaults.variables.Add(Config.MaxDepth, "0");
            Defaults.variables.Add(Config.InvalidCharReplace, "-");
            Defaults.variables.Add(Config.EpIdentifier, new List<string>(new string[] { "S%SE%E", "%Sx%E", "S%S.E%E", "- %S%E -", "-E%E-", ".%S%E.", "%S.%E", "%S%E" }));
            Defaults.variables.Add(Config.TargetPattern, "S%sE%E - %N");
            Defaults.variables.Add(Config.LastTitles, new List<string>(new string[]{"The Simpsons", "Heroes", "Scrubs", "Supernatural", "Eureka", "Chuck", "Stargate", "Stargate Atlantis", "Dexter", "Doctor Who", "24", "Firefly", "Kyle XY", "Prison Break", "Moonlight", "Monk", "House M.D.", "Gilmore Girls", "Friends", "Sex and the City"}));
            Defaults.variables.Add(Config.LastProvider, "");
            Defaults.variables.Add(Config.LastDirectory, "");
            Defaults.variables.Add(Config.LastSubProvider, "");
            Defaults.variables.Add(Config.Timeout, "10000");
            Defaults.variables.Add(Config.InvalidFilenameAction, "Ask");
            Defaults.variables.Add(Config.Umlaute, "Dont_Use");
            Defaults.variables.Add(Config.Case, "Large");
            Defaults.variables.Add(Config.Extract, "Season %S");
            Defaults.variables.Add(Config.CreateDirectoryStructure, "1");
            Defaults.variables.Add(Config.DeleteEmptyFolders, "1");
            Defaults.variables.Add(Config.DeleteAllEmptyFolders, "1");
            Defaults.variables.Add(Config.IgnoreFiles, new List<string>(new string[]{"nfo","diz"}));
            Defaults.variables.Add(Config.ColumnOrder, new List<string>(new string[]{"0","1","2","3","4","6","5"}));
            Defaults.variables.Add(Config.ColumnWidths, new List<string>(new string[] {"209", "137", "55", "57", "144", "188", "188" }));
            Defaults.variables.Add(Config.WindowSize, new List<string>(new string[] { "1024", "600" }));
            Defaults.variables.Add(Config.UseSeasonSubDir, "1");
            Defaults.variables.Add(Config.TitleHistorySize, "100");
        }
    }

    /// <summary>
    /// Helper class containing property names used in config file
    /// </summary>
    static class Config
    {
        /// <summary>
        /// comment string used in config file
        /// </summary>
        public static string Comment = "Comment";

        /// <summary>
        /// delimiter character used in config file
        /// </summary>
        public static string Delimiter = "Delimiter";

        /// <summary>
        /// video file extensions
        /// </summary>
        public static string Extensions = "Extensions";

        /// <summary>
        /// subtitle file extensions
        /// </summary>
        public static string SubtitleExtensions = "SubtitleExtensions";

        /// <summary>
        /// How info messages are displayed
        /// </summary>
        public static string LogLevelInfo = "LogLevelInfo";

        /// <summary>
        /// How warning messages are displayed
        /// </summary>
        public static string LogLevelWarning = "LogLevelWarning";

        /// <summary>
        /// How error messages are displayed
        /// </summary>
        public static string LogLevelError = "LogLevelError";

        /// <summary>
        /// How status messages are displayed
        /// </summary>
        public static string LogLevelStatus = "LogLevelStatus";

        /// <summary>
        /// How debug messages are displayed
        /// </summary>
        public static string LogLevelDebug = "LogLevelDebug";

        /// <summary>
        /// Logfile name
        /// </summary>
        public static string LogName = "LogName";

        /// <summary>
        /// Maximal subdirectory search depth
        /// </summary>
        public static string MaxDepth = "MaxDepth";

        /// <summary>
        /// Replace string for invalid filename characters
        /// </summary>
        public static string InvalidCharReplace = "InvalidCharReplace";

        /// <summary>
        /// Patterns used to extract season/episode information
        /// </summary>
        public static string EpIdentifier = "EpIdentifier";

        /// <summary>
        /// Target filename pattern
        /// </summary>
        public static string TargetPattern = "TargetPattern";

        /// <summary>
        /// Last entered series titles
        /// </summary>
        public static string LastTitles = "LastTitles";

        /// <summary>
        /// Last selected provider
        /// </summary>
        public static string LastProvider = "LastProvider";

        /// <summary>
        /// Last selected directory
        /// </summary>
        public static string LastDirectory = "LastDirectory";

        /// <summary>
        /// Last selected subtitle provider
        /// </summary>
        public static string LastSubProvider = "LastSubProvider";

        /// <summary>
        /// Timeout for network connections
        /// </summary>
        public static string Timeout = "Timeout";

        /// <summary>
        /// Action to tkae when invalid filenames are encountered
        /// </summary>
        public static string InvalidFilenameAction = "InvalidFilenameAction";

        /// <summary>
        /// How Umlauts are treated in target filenames
        /// </summary>
        public static string Umlaute = "Umlaute";

        /// <summary>
        /// How Umlauts are treated in target filenames
        /// </summary>
        public static string Case = "Case";

        /// <summary>
        /// Directory name for seasons, i.e. "Season %S"
        /// </summary>
        public static string Extract = "Extract";

        /// <summary>
        /// If files are to be moved in a sorted directory structure
        /// </summary>
        public static string CreateDirectoryStructure = "CreateDirectoryStructure";

        /// <summary>
        /// If empty folders resulting from file moving are to be deleted
        /// </summary>
        public static string DeleteEmptyFolders = "DeleteEmptyFolders";

        /// <summary>
        /// If other empty folders should be deleted too
        /// </summary>
        public static string DeleteAllEmptyFolders = "DeleteAllEmptyFolders";

        /// <summary>
        /// file types which should be ignored (=deleted) for above option
        /// </summary>
        public static string IgnoreFiles = "IgnoreFiles";

        /// <summary>
        /// Order in which the columns are displayed, positions in this array equal to absolute ordering of the columns, values are display order
        /// </summary>
        public static string ColumnOrder = "ColumnOrder";

        /// <summary>
        /// Widths of the columns
        /// </summary>
        public static string ColumnWidths = "ColumnWidths";

        /// <summary>
        /// Size of the main window
        /// </summary>
        public static string WindowSize = "WindowSize";

        /// <summary>
        /// Indicates if season subdirectory should be used, or if all files will be placed in show dir
        /// </summary>
        public static string UseSeasonSubDir = "UseSeasonSubDir";

        /// <summary>
        /// Size of the title history array
        /// </summary>
        public static string TitleHistorySize = "TitleHistorySize";
    }

    /// <summary>
    /// Helper class containing property names used in season/episode&lt-&gtprovider files
    /// </summary>
    static class ProviderConfig
    {
        /// <summary>
        /// Name of the provider
        /// </summary>
        public static string Name = "Name";

        /// <summary>
        /// Search URL, %T is a placeholder for the search title
        /// </summary>
        public static string SearchURL = "SearchURL";

        /// <summary>
        /// substring of the search results page URL
        /// </summary>
        public static string SearchResultsURL = "SearchResultsURL";

        /// <summary>
        /// substring of the series page URL
        /// </summary>
        public static string SeriesURL = "SeriesURL";

        /// <summary>
        /// Additionally, if the search engine redirects to the single result directly, we might need a string to attach to the results page to get to the episodes page
        /// </summary>
        public static string EpisodesURL = "EpisodesURL";

        /// <summary>
        /// Regular expression for parsing search results
        /// </summary>
        public static string SearchRegExp = "SearchRegExp";

        /// <summary>
        /// string to search for for cropping off some source from the search page start
        /// </summary>
        public static string SearchStart = "SearchStart";

        /// <summary>
        /// string to search for for cropping off some source from the search page end
        /// </summary>
        public static string SearchEnd = "SearchEnd";

        /// <summary>
        /// start regex for search pages from end of file
        /// </summary>
        public static string SearchRightToLeft = "SearchRightToLeft";

        /// <summary>
        /// some regular expressions to remove from search results name
        /// </summary>
        public static string SearchRemove = "SearchRemove";

        /// <summary>
        /// Link to the page containing episode infos. %L is used as placeholder for the link corresponding to the show the user selected
        /// </summary>
        public static string RelationsPage = "RelationsPage";

        /// <summary>
        /// Regular expression to extract season/number/episode name relationship from the page containing this info
        /// This needs to contain:
        /// (?&ltSeason&gtRegExpToExtractSeason) - to get the season number
        /// (?&ltEpisode&gtRegExpToExtractEpisode) - to get the episode number
        /// (?&ltTitle&gtRegExpToExtractTitle) - to get the title belonging to that season/episode
        ///If Relationspage uses %S placeholder, there is no need to include (?<Season>RegExpToExtractSeason) here
        /// </summary>
        public static string RelationsRegExp = "RelationsRegExp";

        /// <summary>
        /// string to search for for cropping off some source from the relations page start
        /// </summary>
        public static string RelationsStart = "RelationsStart";

        /// <summary>
        /// string to search for for cropping off some source from the relations page end
        /// </summary>
        public static string RelationsEnd = "RelationsEnd";

        /// <summary>
        /// start regex for relations pages from end of file
        /// </summary>
        public static string RelationsRightToLeft = "RelationsRightToLeft";

        /// <summary>
        /// If some page forwards to this URL, it is assumed the link is invalid
        /// </summary>
        public static string NotFoundURL = "NotFoundURL";

        /// <summary>
        /// Encoding of the page, leave empty for automatic
        /// </summary>
        public static string Encoding = "Encoding";

    }
    /// <summary>
    /// Helper class containing property names used in subtitle provider files
    /// </summary>
    static class SubProviderConfig
    {
        /// <summary>
        /// Name of the provider
        /// </summary>
        public static string Name = "Name";

        /// <summary>
        /// Search URL, %T is a placeholder for the search title
        /// </summary>
        public static string SearchURL = "SearchURL";

        /// <summary>
        /// Is the download link directly on the search results page?
        /// </summary>
        public static string DirectLink = "DirectLink";

        /// <summary>
        /// substring of the search results page URL
        /// </summary>
        public static string SearchResultsURL = "SearchResultsURL";

        /// <summary>
        /// substring of the series page URL
        /// </summary>
        public static string SeriesURL = "SeriesURL";

        /// <summary>
        /// Additionally, if the search engine redirects to the single result directly, we might need a string to attach to the results page to get to the episodes page
        /// </summary>
        public static string SubtitlesURL = "SubtitlesURL";

        /// <summary>
        /// Regular expression for parsing search results
        /// </summary>
        public static string SearchRegExp = "SearchRegExp";

        /// <summary>
        /// string to search for for cropping off some source from the search page start
        /// </summary>
        public static string SearchStart = "SearchStart";

        /// <summary>
        /// string to search for for cropping off some source from the search page end
        /// </summary>
        public static string SearchEnd = "SearchEnd";

        /// <summary>
        /// some regular expressions to remove from search results name
        /// </summary>
        public static string SearchRemove = "SearchRemove";

        /// <summary>
        /// start regex for search pages from end of file
        /// </summary>
        public static string SearchRightToLeft = "SearchRightToLeft";

        /// <summary>
        /// Link to the page containing subtitle links. %L is used as placeholder for the link corresponding to the show the user selected
        /// For multiple pages of subtitle downloads, use %P
        /// </summary>
        public static string SubtitlesPage = "SubtitlesPage";

        /// <summary>
        /// Regular expression to extract subtitle links (along with names) from downloads page
        /// This needs to contain: 
        /// (?&ltSeason&gtRegExpToExtractSeason) - to get the season number
        /// (?&ltEpisode&gtRegExpToExtractEpisode) - to get the episode number
        /// (?&ltLink&gtRegExpToExtractLink) - to get the download link for one episode
        /// If Package is set to 1, only download link is required
        /// </summary>
        public static string SubtitleRegExp = "SubtitleRegExp";

        /// <summary>
        /// string to search for for cropping off some source from the search page start
        /// </summary>
        public static string SubtitlesStart = "SubtitlesStart";

        /// <summary>
        /// string to search for for cropping off some source from the search page end
        /// </summary>
        public static string SubtitlesEnd = "SubtitlesEnd";

        /// <summary>
        /// If the download link(s) can be constructed directly from the search results page, use this variable.
        /// %L gets replaced with the value aquired from Search results page "link" property, 
        /// %P will allow to iterate over pages/seasons etc
        /// </summary>
        public static string ConstructLink = "ConstructLink";

        /// <summary>
        /// If some page forwards to this URL, it is assumed the link is invalid
        /// </summary>
        public static string NotFoundURL = "NotFoundURL";

        /// <summary>
        /// Encoding of the page, leave empty for automatic
        /// </summary>
        public static string Encoding = "Encoding";
    }
}