using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Classes.Configuration.Keywords
{
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

        /// <summary>
        /// Language may be set in config file, mostly used for treating umlauts right now
        /// </summary>
        public static string Language = "Language";
    }
}
