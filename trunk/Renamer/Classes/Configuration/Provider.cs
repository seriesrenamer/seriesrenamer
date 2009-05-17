using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Classes.Configuration
{
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

        /// <summary>
        /// Page encoding, leave empty for automatic
        /// </summary>
        public string Encoding = "";

        public string SearchStart = "";
        public string SearchEnd = "";

        /// <summary>
        /// If some page forwards to this URL, it is assumed the link is invalid
        /// </summary>
        public string NotFoundURL = "";

        /// <summary>
        /// Language can be specified, but doesn't have to be
        /// </summary>
        public Helper.Languages Language = Helper.Languages.None;
    }    
}
