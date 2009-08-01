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

namespace Renamer.Classes.Configuration.Keywords
{
    /// <summary>
    /// Helper class containing property names used in config file
    /// </summary>
    static class Config
    {
        /// <summary>
        /// delimiter character used in config file
        /// </summary>
        public const string Delimiter = "Delimiter";

        /// <summary>
        /// video file extensions
        /// </summary>
        public const string Extensions = "Extensions";

        /// <summary>
        /// subtitle file extensions
        /// </summary>
        public const string SubtitleExtensions = "SubtitleExtensions";

        /// <summary>
        /// Loglevel for logfile
        /// </summary>
        public const string LogFileLevel = "LogFileLevel";
        /// <summary>
        /// Loglevel for messageboxes
        /// </summary>
        public const string LogMessageBoxLevel = "LogMessageBoxLevel";

        /// <summary>
        /// Loglevel for TextBox
        /// </summary>
        public const string LogTextBoxLevel = "LogTextBoxLevel";

        /// <summary>
        /// Logfile name
        /// </summary>
        public const string LogName = "LogName";

        /// <summary>
        /// Maximal subdirectory search depth
        /// </summary>
        public const string MaxDepth = "MaxDepth";

        /// <summary>
        /// Replace string for invalid filename characters
        /// </summary>
        public const string InvalidCharReplace = "InvalidCharReplace";

        /// <summary>
        /// Patterns used to extract season/episode information
        /// </summary>
        public const string EpIdentifier = "EpIdentifier";

        /// <summary>
        /// Target filename pattern
        /// </summary>
        public const string TargetPattern = "TargetPattern";

        /// <summary>
        /// Last entered series titles
        /// </summary>
        public const string LastTitles = "LastTitles";

        /// <summary>
        /// Last selected provider
        /// </summary>
        public const string LastProvider = "LastProvider";

        /// <summary>
        /// Last selected directory
        /// </summary>
        public const string LastDirectory = "LastDirectory";

        /// <summary>
        /// Last selected subtitle provider
        /// </summary>
        public const string LastSubProvider = "LastSubProvider";

        /// <summary>
        /// Timeout for network connections
        /// </summary>
        public const string Timeout = "Timeout";

        /// <summary>
        /// How Umlauts are treated in target filenames
        /// </summary>
        public const string Umlaute = "Umlaute";

        /// <summary>
        /// How Umlauts are treated in target filenames
        /// </summary>
        public const string Case = "Case";

        /// <summary>
        /// Directory name for seasons, i.e. "Season %S"
        /// </summary>
        public const string Extract = "Extract";

        /// <summary>
        /// If files are to be moved in a sorted directory structure
        /// </summary>
        public const string CreateDirectoryStructure = "CreateDirectoryStructure";

        /// <summary>
        /// If empty folders resulting from file moving are to be deleted
        /// </summary>
        public const string DeleteEmptyFolders = "DeleteEmptyFolders";

        /// <summary>
        /// If other empty folders should be deleted too
        /// </summary>
        public const string DeleteAllEmptyFolders = "DeleteAllEmptyFolders";

        /// <summary>
        /// file types which should be ignored (=deleted) for above option
        /// </summary>
        public const string IgnoreFiles = "IgnoreFiles";

        /// <summary>
        /// Order in which the columns are displayed, positions in this array equal to absolute ordering of the columns, values are display order
        /// </summary>
        public const string ColumnOrder = "ColumnOrder";

        /// <summary>
        /// Widths of the columns
        /// </summary>
        public const string ColumnWidths = "ColumnWidths";

        /// <summary>
        /// Size of the main window
        /// </summary>
        public const string WindowSize = "WindowSize";

        /// <summary>
        /// Indicates if season subdirectory should be used, or if all files will be placed in show dir
        /// </summary>
        public const string UseSeasonSubDir = "UseSeasonSubDir";

        /// <summary>
        /// Size of the title history array
        /// </summary>
        public const string TitleHistorySize = "TitleHistorySize";

        /// <summary>
        /// Regex used to extract showname from filename/foldername
        /// </summary>
        public const string ShownameExtractionRegex = "ShownameExtractionRegex";

        /// <summary>
        /// strings used to determine if a recognized showname may be invalid (i.e. "Season")
        /// </summary>
        public const string PathBlacklist = "PathBlacklist";

        /// <summary>
        /// strings used to determine if a filename may not be used for shownames (i.e. "AVSEQ01")
        /// </summary>
        public const string FilenameBlacklist = "FilenameBlacklist";

        /// <summary>
        /// List used as a test to see if a file could be a movie
        /// </summary>
        public const string MovieIndicator = "MovieIndicator";

        /// <summary>
        /// Regex used to remove non-characters and other crap from names
        /// </summary>
        public const string CleanupRegex = "CleanupRegex";

        /// <summary>
        /// Replace strings in filenames
        /// </summary>
        public const string Replace = "Replace";

        /// <summary>
        /// Tags to remove from movies
        /// </summary>
        public const string Tags = "Tags";

        /// <summary>
        /// Automatically resize columns to fit window size
        /// </summary>
        public const string ResizeColumns = "ResizeColumns";

        /// <summary>
        /// If Log messages about missing episodes should be shown
        /// </summary>
        public const string FindMissingEpisodes = "FindMissingEpisodes";

        /// <summary>
        /// If Sample files should be deleted on processing
        /// </summary>
        public const string DeleteSampleFiles = "DeleteSampleFiles";

        /// <summary>
        /// Directory in which files are moved (as basedir)
        /// </summary>
        public const string DestinationDirectory = "DestinationDirectory";

        /// <summary>
        /// Languages used for preselecting search results
        /// </summary>
        public const string Languages = "Languages";

        public static class RegexMarker
        {

            public const string Title = "%T";
            public const string Season = "%S";
            public const string Episode = "%E";

        }
    }
}
