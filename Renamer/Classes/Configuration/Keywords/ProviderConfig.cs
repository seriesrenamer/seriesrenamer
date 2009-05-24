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
    /// Helper class containing property names used in subtitle provider files
    /// </summary>
    public class ProviderConfig
    {
        /// <summary>
        /// Name of the provider
        /// </summary>
        public const string Name = "Name";

        /// <summary>
        /// Search URL, %T is a placeholder for the search title
        /// </summary>
        public const string SearchURL = "SearchURL";

        /// <summary>
        /// substring of the search results page URL
        /// </summary>
        public const string SearchResultsURL = "SearchResultsURL";

        /// <summary>
        /// substring of the series page URL
        /// </summary>
        public const string SeriesURL = "SeriesURL";

        /// <summary>
        /// Regular expression for parsing search results
        /// </summary>
        public const string SearchRegExp = "SearchRegExp";

        /// <summary>
        /// string to search for for cropping off some source from the search page start
        /// </summary>
        public const string SearchStart = "SearchStart";

        /// <summary>
        /// string to search for for cropping off some source from the search page end
        /// </summary>
        public const string SearchEnd = "SearchEnd";

        /// <summary>
        /// some regular expressions to remove from search results name
        /// </summary>
        public const string SearchRemove = "SearchRemove";

        /// <summary>
        /// start regex for search pages from end of file
        /// </summary>
        public const string SearchRightToLeft = "SearchRightToLeft";

        /// <summary>
        /// If some page forwards to this URL, it is assumed the link is invalid
        /// </summary>
        public const string NotFoundURL = "NotFoundURL";

        /// <summary>
        /// Encoding of the page, leave empty for automatic
        /// </summary>
        public const string Encoding = "Encoding";

        /// <summary>
        /// Language may be set in config file, mostly used for treating umlauts right now
        /// </summary>
        public const string Language = "Language";


        /// <summary>
        /// Helper class containing property names used in subtitle provider files
        /// </summary>
        public static class Subtitles
        {
            /// <summary>
            /// Is the download link directly on the search results page?
            /// </summary>
            public const string DirectLink = "DirectLink";

            /// <summary>
            /// Additionally, if the search engine redirects to the single result directly, we might need a string to attach to the results page to get to the episodes page
            /// </summary>
            public const string SubtitlesURL = "SubtitlesURL";

            /// <summary>
            /// Link to the page containing subtitle links. %L is used as placeholder for the link corresponding to the show the user selected
            /// For multiple pages of subtitle downloads, use %P
            /// </summary>
            public const string SubtitlesPage = "SubtitlesPage";

            /// <summary>
            /// Regular expression to extract subtitle links (along with names) from downloads page
            /// This needs to contain: 
            /// (?&ltSeason&gtRegExpToExtractSeason) - to get the season number
            /// (?&ltEpisode&gtRegExpToExtractEpisode) - to get the episode number
            /// (?&ltLink&gtRegExpToExtractLink) - to get the download link for one episode
            /// If Package is set to 1, only download link is required
            /// </summary>
            public const string SubtitleRegExp = "SubtitleRegExp";

            /// <summary>
            /// string to search for for cropping off some source from the search page start
            /// </summary>
            public const string SubtitlesStart = "SubtitlesStart";

            /// <summary>
            /// string to search for for cropping off some source from the search page end
            /// </summary>
            public const string SubtitlesEnd = "SubtitlesEnd";

            /// <summary>
            /// If the download link(s) can be constructed directly from the search results page, use this variable.
            /// %L gets replaced with the value aquired from Search results page "link" property, 
            /// %P will allow to iterate over pages/seasons etc
            /// </summary>
            public const string ConstructLink = "ConstructLink";
        }

        /// <summary>
        /// Helper class containing property names used in season/episode&lt;-&gt;provider files
        /// </summary>
        public static class Relations
        {
            /// <summary>
            /// Additionally, if the search engine redirects to the single result directly, we might need a string to attach to the results page to get to the episodes page
            /// </summary>
            public const string EpisodesURL = "EpisodesURL";

            /// <summary>
            /// Link to the page containing episode infos. %L is used as placeholder for the link corresponding to the show the user selected
            /// </summary>
            public const string RelationsPage = "RelationsPage";

            /// <summary>
            /// Regular expression to extract season/number/episode name relationship from the page containing this info
            /// This needs to contain:
            /// (?&ltSeason&gtRegExpToExtractSeason) - to get the season number
            /// (?&ltEpisode&gtRegExpToExtractEpisode) - to get the episode number
            /// (?&ltTitle&gtRegExpToExtractTitle) - to get the title belonging to that season/episode
            ///If Relationspage uses %S placeholder, there is no need to include (?<Season>RegExpToExtractSeason) here
            /// </summary>
            public const string RelationsRegExp = "RelationsRegExp";

            /// <summary>
            /// string to search for for cropping off some source from the relations page start
            /// </summary>
            public const string RelationsStart = "RelationsStart";

            /// <summary>
            /// string to search for for cropping off some source from the relations page end
            /// </summary>
            public const string RelationsEnd = "RelationsEnd";

            /// <summary>
            /// start regex for relations pages from end of file
            /// </summary>
            public const string RelationsRightToLeft = "RelationsRightToLeft";
        }

    }
}
