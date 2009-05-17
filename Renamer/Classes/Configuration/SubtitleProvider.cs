using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Classes.Configuration
{
    /// <summary>
    /// A subtitle file provider
    /// </summary>
    class SubtitleProvider : Provider
    {
        /// <summary>
        /// Is the download link directly on the search results page? (Instead of a page related to that show in between)
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


        /// <summary>
        /// If the download link(s) can be constructed directly from the search results page, use this variable.
        /// %L gets replaced with the value aquired from Search results page "link" property, 
        /// %P will allow to iterate over pages/seasons etc
        /// </summary>
        public string ConstructLink = "";

    }
}
