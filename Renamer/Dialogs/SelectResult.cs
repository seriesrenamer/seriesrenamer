using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;
using Renamer.Classes;
using Renamer.Classes.Configuration;

namespace Renamer.Dialogs
{
    /// <summary>
    /// Choose results dialog class for search results
    /// </summary>
    public partial class SelectResult : Form
    {
        /// <summary>
        /// Selected urls
        /// </summary>
        public List<string> urls = new List<string>();

        /// <summary>
        /// Selected url
        /// </summary>
        public string url="";

        /// <summary>
        /// Match collection of search results
        /// </summary>
        MatchCollection mcResults = null;
        
        /// <summary>
        /// Provider to acquire regexps from
        /// </summary>
        Provider P = null;

        //Language which might be extracted from the show title
        public Helper.Languages Language = Helper.Languages.None;

        /// <summary>
        /// Multi selection allowed
        /// </summary>
        bool Multi = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mc">matched search results</param>
        /// <param name="prov">Provider to take regexps from</param>
        /// <param name="multi">multi selection</param>
        public SelectResult(MatchCollection mc,Provider prov,bool multi)
        {
            InitializeComponent();
            mcResults = mc;
            P = prov;
            Multi = multi;
            if (multi)
            {
                Height = 336;
                cbResults.Hide();
                lvResults.Show();
            }
            else
            {
                Height = 114;
                cbResults.Show();
                lvResults.Hide();
            }
            if (mc != null && mc.Count > 0)
            {
                for(int i =mc.Count-1;i>=0;i--){
                    Match m=mc[i];
                    string name = System.Web.HttpUtility.HtmlDecode(m.Groups["name"].Value + " " + m.Groups["year"].Value);
                    if (P.SearchRemove != null)
                    {
                        foreach (string str in P.SearchRemove)
                        {
                            if (str != "")
                            {
                                name = Regex.Replace(name, str, "");
                            }
                        }
                    }
                    if (multi)
                    {
                        lvResults.Items.Add(new ListViewItem(name));
                        lblResults.Text = mc.Count.ToString() + " Results found. Select the fitting ones:";
                        lvResults.Sort();
                        lvResults.Refresh();
                    }
                    else
                    {
                        cbResults.Items.Add(name);
                        cbResults.SelectedIndex = 0;
                        lblResults.Text = mc.Count.ToString() + " Results found. Select the fitting one:";
                    }
                }
                
                
            }
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Multi)
            {
                foreach (ListViewItem lvi in lvResults.Items)
                {
                    if (lvi.Checked)
                    {
                        urls.Add(mcResults[mcResults.Count - 1 - lvi.Index].Groups["link"].Value);
                    }
                }
            }
            else
            {
                url = mcResults[mcResults.Count - 1 - cbResults.SelectedIndex].Groups["link"].Value;
                List<string> Languages = new List<string>();
                foreach (string s in Enum.GetNames(typeof(Helper.Languages)))
                {
                    Languages.Add(s);
                }
                foreach (string s in Languages)
                {
                    if (cbResults.SelectedItem.ToString().Contains(s))
                    {
                        Language = (Helper.Languages)Enum.Parse(typeof(Helper.Languages), s);
                    }
                }
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            urls.Clear();
            url = "";
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
