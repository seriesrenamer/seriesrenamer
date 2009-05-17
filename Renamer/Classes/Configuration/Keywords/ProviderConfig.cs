using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Classes.Configuration.Keywords
{
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

        /// <summary>
        /// Language may be set in config file, mostly used for treating umlauts right now
        /// </summary>
        public static string Language = "Language";
    }
}
