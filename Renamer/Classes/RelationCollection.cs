using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Renamer.Classes
{
    class RelationCollection
    {
        public List<Relation> Relations = new List<Relation>();
        public string Showname = "";

        public RelationCollection(string Showname)
        {
            this.Showname = Showname;
        }
        /// <summary>
        /// Finds lowest season in relations
        /// </summary>
        /// <returns>index of the season, or 10000 ;)</returns>
        public int FindMinSeason()
        {
            int min = 10000;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) < min) min = Convert.ToInt32(rel.Season);
            }
            return min;
        }

        /// <summary>
        /// Finds highest season in relations
        /// </summary>
        /// <returns>index of the season, or -1 ;)</returns>
        public int FindMaxSeason()
        {
            int max = -1;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) > max) max = Convert.ToInt32(rel.Season);
            }
            return max;
        }

        /// <summary>
        /// Finds lowest episode of a season in relations
        /// </summary>
        /// <param name="season">season to find lowest episode relation in</param>
        /// <returns>index of the episode, or 10000 ;)</returns>
        public int FindMinEpisode(int season)
        {
            int min = 10000;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) == season)
                {
                    if (Convert.ToInt32(rel.Episode) < min) min = Convert.ToInt32(rel.Episode);
                }
            }
            return min;
        }

        /// <summary>
        /// Finds highest episode of a season in relations
        /// </summary>
        /// <param name="season">season to find highest episode relation in</param>
        /// <returns>index of the episode, or -1 ;)</returns>
        public int FindMaxEpisode(int season)
        {
            int max = -1;
            foreach (Relation rel in Relations)
            {
                if (Convert.ToInt32(rel.Season) == season)
                {
                    if (Convert.ToInt32(rel.Episode) > max) max = Convert.ToInt32(rel.Episode);
                }
            }
            return max;
        }
    }

}
