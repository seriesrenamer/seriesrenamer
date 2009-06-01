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
using Renamer.Classes.Configuration.Keywords;
using System.IO;
using Renamer.Classes.Configuration;

namespace Renamer.Classes.Provider
{
    /// <summary>
    /// An abstract provider
    /// </summary>
    public abstract class Provider
    {
        /// <summary>
        /// <see cref="Name"/>
        /// </summary>
        private string name = "";
        /// <summary>
        /// <see cref="SearchUrl"/>
        /// </summary>
        private string searchUrl = "";
        private string searchResultsUrl = "";
        private string seriesUrl = "";
        private string searchRegExp = "";
        private string[] searchRemove;
        private bool searchRightToLeft = false;
        private string encoding = "";
        private string searchStart = "";
        private string searchEnd = "";
        private string notFoundUrl = "";
        private Helper.Languages language; //Helper.Languages.None
        private string searchResultsBlacklist = "";
        public Provider() {
        }
        public Provider(string filename) {

            this.Name = Helper.ReadProperty(ProviderConfig.Name, filename);
            this.SearchRegExp = Helper.ReadProperty(ProviderConfig.SearchRegExp, filename);
            this.SearchResultsUrl = Helper.ReadProperty(ProviderConfig.SearchResultsURL, filename);
            this.SearchUrl = Helper.ReadProperty(ProviderConfig.SearchURL, filename);
            this.SeriesUrl = Helper.ReadProperty(ProviderConfig.SeriesURL, filename);
            this.SearchRemove = Helper.ReadProperties(ProviderConfig.SearchRemove, filename);
            this.SearchStart = Helper.ReadProperty(ProviderConfig.SearchStart, filename);
            this.SearchEnd = Helper.ReadProperty(ProviderConfig.SearchEnd, filename);
            this.NotFoundUrl = Helper.ReadProperty(ProviderConfig.NotFoundURL, filename);
            this.Encoding = Helper.ReadProperty(ProviderConfig.Encoding, filename);
            this.Language = Helper.ReadEnum<Helper.Languages>(ProviderConfig.Language, filename);
            this.SearchRightToLeft = Helper.ReadBool(ProviderConfig.SearchRightToLeft, filename);
            this.SearchResultsBlacklist = Helper.ReadProperty(ProviderConfig.SearchResultsBlacklist, filename);
        }


        protected static string[] getFiles(string location){
            return Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + location, ConfigFile.filePattern);
        }


        

        /// <summary>
        /// Name of the provider
        /// </summary>
        public string Name {
            get { return name; }
            set { name = value; }
        }

        //Blacklist used to filter out unwanted search results
        public string SearchResultsBlacklist
        {
            get { return searchResultsBlacklist; }
            set { searchResultsBlacklist = value; }
        }

        /// <summary>
        /// Search URL, %T is a placeholder for the search title
        /// </summary>
        public string SearchUrl {
            get { return searchUrl; }
            set { searchUrl = value; }
        }

        /// <summary>
        /// substring of the search results page URL
        /// </summary>
        public string SearchResultsUrl {
            get { return searchResultsUrl; }
            set { searchResultsUrl = value; }
        }

        /// <summary>
        /// substring of the series page URL
        /// </summary>
        public string SeriesUrl {
            get { return seriesUrl; }
            set { seriesUrl = value; }
        }

        /// <summary>
        /// Regular expression for parsing search results
        /// </summary>
        public string SearchRegExp {
            get { return searchRegExp; }
            set { searchRegExp = value; }
        }

        /// <summary>
        /// some regular expressions to remove from search results name
        /// </summary>
        public string[] SearchRemove {
            get { return searchRemove; }
            set { searchRemove = value; }
        }


        /// <summary>
        /// start regex for search pages from end of file
        /// </summary>
        public bool SearchRightToLeft {
            get { return searchRightToLeft; }
            set { searchRightToLeft = value; }
        }

        /// <summary>
        /// Page encoding, leave empty for automatic
        /// </summary>
        public string Encoding {
            get { return encoding; }
            set { encoding = value; }
        }

        public string SearchStart {
            get { return searchStart; }
            set { searchStart = value; }
        }

        public string SearchEnd {
            get { return searchEnd; }
            set { searchEnd = value; }
        }

        /// <summary>
        /// If some page forwards to this URL, it is assumed the link is invalid
        /// </summary>
        public string NotFoundUrl {
            get { return notFoundUrl; }
            set { notFoundUrl = value; }
        }

        /// <summary>
        /// Language can be specified, but doesn't have to be
        /// </summary>
        public Helper.Languages Language {
            get { return language; }
            set { language = value; }
        }
    }
}
