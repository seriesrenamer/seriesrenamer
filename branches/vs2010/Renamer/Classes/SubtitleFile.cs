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

namespace Renamer.Classes
{
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
        public int Season = -1;

        /// <summary>
        /// Collective episode value
        /// </summary>
        public int Episode = -1;
    }
}
