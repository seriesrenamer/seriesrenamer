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

        /// <summary>
        /// Regex used to extract showname from filename/foldername
        /// </summary>
        public static string ShownameExtractionRegex = "ShownameExtractionRegex";

        /// <summary>
        /// strings used to determine if a recognized showname may be invalid
        /// </summary>
        public static string ShownameBlacklist = "ShownameBlacklist";

        /// <summary>
        /// Regex used to remove non-characters and other crap from names
        /// </summary>
        public static string CleanupRegex = "CleanupRegex";

        /// <summary>
        /// Replace strings in filenames
        /// </summary>
        public static string Replace = "Replace";

        /// <summary>
        /// Tags to remove from movies
        /// </summary>
        public static string Tags = "Tags";

        /// <summary>
        /// Automatically resize columns to fit window size
        /// </summary>
        public static string ResizeColumns = "ResizeColumns";
    }
}
