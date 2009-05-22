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

namespace Renamer.Classes.Configuration
{
    /// <summary>
    /// A season/episode<->name relations provider
    /// </summary>
    class RelationProvider : Provider
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
}
