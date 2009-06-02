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
using System.IO;
using Renamer.Classes.Configuration.Keywords;
using Renamer.Logging;
using Renamer.Classes.Configuration;

namespace Renamer.Classes.Provider
{
    /// <summary>
    /// A season/episode&lt;-&gt;name relations provider
    /// This class keeps track of all providers available
    /// </summary>
    public class RelationProvider : Provider
    {
        private const string location = "/Databases/Titles";

        private static List<RelationProvider> list;

        private static List<RelationProvider> List{
            get {
                if (list == null) {
                    list = new List<RelationProvider>();
                    LoadAll();
                }
                return list;
            }
        }

        /*
         * is there a way to move this to the superclass ...
         * */
        public static void LoadAll() {
            string[] providers = getFiles(location);
            string status = "Providers found:";
            foreach (string file in providers) {
                Logger.Instance.LogMessage("Provider: " + file, LogLevel.DEBUG);
                try {
                    RelationProvider rel = new RelationProvider(file);
                    
                    if (String.IsNullOrEmpty(rel.Name)) {
                        Logger.Instance.LogMessage("Invalid provider file: " + file, LogLevel.ERROR);
                        rel.delete();
                        continue;
                    }
                    status += " " + rel.Name + ",";
                }
                catch (Exception ex) {
                    Logger.Instance.LogMessage("Invalid provider file: " + file + " Error: " + ex.Message, LogLevel.ERROR);
                }
            }
            status = status.TrimEnd(new char[] { ',' });
            Logger.Instance.LogMessage(status, LogLevel.INFO);
        }

        public RelationProvider() {
            List.Add(this);
        }
        public RelationProvider(string filename)
            : base(filename) {
            this.RelationsPage = Helper.ReadProperty(ProviderConfig.Relations.RelationsPage, filename);
            this.RelationsRegExp = Helper.ReadProperty(ProviderConfig.Relations.RelationsRegExp, filename);
            this.EpisodesUrl = Helper.ReadProperty(ProviderConfig.Relations.EpisodesURL, filename);
            this.RelationsStart = Helper.ReadProperty(ProviderConfig.Relations.RelationsStart, filename);
            this.RelationsEnd = Helper.ReadProperty(ProviderConfig.Relations.RelationsEnd, filename);
            this.RelationsRightToLeft = Helper.ReadBool(ProviderConfig.Relations.RelationsRightToLeft, filename);
            List.Add(this);
        }

        /// <summary>
        /// Gets currently selected provider
        /// </summary>
        /// <returns>Currently selected provider, or null if error</returns>
        public static RelationProvider GetCurrentProvider() {
            return GetProviderByName(Helper.ReadProperty(Config.LastProvider));
        }

        /// <summary>
        /// Gets a provider by its name
        /// </summary>
        /// <param name="name">name of the provider</param>
        /// <returns>provider matching the name, or null if not found</returns>
        public static RelationProvider GetProviderByName(string name) {
            foreach (RelationProvider rp in List) {
                if (rp.Name == name) {
                    return rp;
                }
            }
            return null;
        }

        public static string[] ProviderNames {
            get {
                string[] ret = new string[List.Count];
                int i = 0;
                foreach (Provider rel in List) {
                    ret[i++] = rel.Name;
                }
                return (string[])ret.Clone();
            }
        }

        public string ToString()
        {
            return Name;
        }
        private string relationsPage = "";
        private string relationsRegExp = "";
        private bool relationsRightToLeft = false;
        private string episodesUrl = "";
        private string relationsStart = "";
        private string relationsEnd = "";


        public void delete() {
            List.Remove(this);
        }


        /// <summary>
        /// Link to the page containing episode infos. %L is used as placeholder for the link corresponding to the show the user selected
        /// </summary>
        public string RelationsPage {
            get { return relationsPage; }
            set { relationsPage = value; }
        }

        /// <summary>
        /// Regular expression to extract season/number/episode name relationship from the page containing this info
        /// This needs to contain:
        /// (?&ltSeason&gtRegExpToExtractSeason) - to get the season number
        /// (?&ltEpisode&gtRegExpToExtractEpisode) - to get the episode number
        /// (?&ltTitle&gtRegExpToExtractTitle) - to get the title belonging to that season/episode
        ///If Relationspage uses %S placeholder, there is no need to include (?<Season>RegExpToExtractSeason) here
        /// </summary>
        public string RelationsRegExp {
            get { return relationsRegExp; }
            set { relationsRegExp = value; }
        }

        /// <summary>
        /// start regex for relations pages from end of file
        /// </summary>
        public bool RelationsRightToLeft {
            get { return relationsRightToLeft; }
            set { relationsRightToLeft = value; }
        }

        /// <summary>
        /// Additionally, if the search engine redirects to the single result directly, we might need a string to attach to the results page to get to the episodes page
        /// </summary>

        public string EpisodesUrl {
            get { return episodesUrl; }
            set { episodesUrl = value; }
        }


        public string RelationsStart {
            get { return relationsStart; }
            set { relationsStart = value; }
        }

        public string RelationsEnd {
            get { return relationsEnd; }
            set { relationsEnd = value; }
        }
    }
}
