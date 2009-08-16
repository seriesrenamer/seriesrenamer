using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Classes;

namespace Renamer
{
    class RelationManager
    {
        #region Singleton
        protected static RelationManager instance;
        private static object m_lock = new object();

        public static RelationManager Instance {
            get {
                if (instance == null) {
                    lock (m_lock) { if (instance == null) instance = new RelationManager(); }
                }
                return instance;
            }
        }
        #endregion

        #region Members

        /// <summary>
        /// List of season/episode<->name relations
        /// </summary>
        protected List<RelationCollection> relations = new List<RelationCollection>();
        #endregion

        #region Constructor(s)
        protected RelationManager() {
        }
        #endregion

        #region Properties
        public int Count {
            get { return this.relations.Count; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a RelationCollection with a given Showname
        /// </summary>
        /// <param name="Showname">The Showname</param>
        /// <returns>The RelationCollection, or null if not found</returns>
        public RelationCollection GetRelationCollection(string Showname) {
            foreach (RelationCollection rc in relations) {
                if (rc.Showname == Showname) {
                    return rc;
                }
            }
            return null;
        }
        /// <summary>
        /// Removes a RelationCollection with a given Showname
        /// </summary>
        /// <param name="Showname">The Showname</param>
        /// <returns>The RelationCollection, or null if not found</returns>
        public void RemoveRelationCollection(string showname) {
            this.relations.Remove(this.GetRelationCollection(showname));
            InfoEntryManager.Instance.ClearRelation(showname);
        }

        public void AddRelationCollection(RelationCollection rc) {
            this.relations.Add(rc);
            InfoEntryManager manager = InfoEntryManager.Instance;
            List<InfoEntry> infoEntryList = manager.GetVideos(rc.Showname);
            foreach (InfoEntry ie in infoEntryList) {
                ie.findEpisodeName(rc);
            }
        }
        public void Clear() {
            this.relations.Clear();
            InfoEntryManager.Instance.ClearRelations();
        }
        #endregion

    }
}
