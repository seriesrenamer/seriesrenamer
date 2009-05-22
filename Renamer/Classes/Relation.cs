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
    /// A season/episode<->name relation
    /// </summary>
    class Relation
    {
        /// <summary>
        /// Season
        /// </summary>
        public string Season = "";

        /// <summary>
        /// Episode
        /// </summary>
        public string Episode = "";

        /// <summary>
        /// Name
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="season">Season</param>
        /// <param name="episode">Episode</param>
        /// <param name="name">Name</param>
        public Relation(string season, string episode, string name)
        {
            Season = season;
            Episode = episode;
            Name = name;
        }

        public override string ToString()
        {
            return "S" + Season + "E" + Episode + " - " + Name;
        }
    }
    
}
