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
        public string Season = "";

        /// <summary>
        /// Collective episode value
        /// </summary>
        public string Episode = "";
    }
}
