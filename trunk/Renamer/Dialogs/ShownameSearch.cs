using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Renamer.Classes.Provider;

namespace Renamer.Dialogs
{
    public partial class ShownameSearch : Form
    {
        public List<DataGenerator.ParsedSearch> Results;
        public ShownameSearch(List<DataGenerator.ParsedSearch> results)
        {
            InitializeComponent();
            Results = results;
            for (int i=0;i<Results.Count;i++)
            {
                DataGenerator.ParsedSearch ps = Results[i];
                tableLayoutPanel1.RowCount++;
                Label lbl=new Label();
                lbl.Text=ps.Showname;
                lbl.Name="Label "+i;
                lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right|AnchorStyles.Top;
                tableLayoutPanel1.Controls.Add(lbl, 0 , i+1);
                TextBox tb = new TextBox();
                tb.Text = ps.Showname;
                tb.Name = "TextBox " + i;
                tb.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                tableLayoutPanel1.Controls.Add(tb, 1, i + 1);                
                ComboBox cbProviders = new ComboBox();
                cbProviders.DropDownStyle = ComboBoxStyle.DropDownList;
                cbProviders.Name = "ComboBox Providers " + i;
                cbProviders.Items.AddRange(RelationProvider.ProviderNames);
                cbProviders.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                RelationProvider provider = RelationProvider.GetCurrentProvider();
                cbProviders.SelectedItem = provider.Name;
                tableLayoutPanel1.Controls.Add(cbProviders, 2, i + 1);
                Button btn = new Button();
                btn.Text = "Search";
                btn.Name = "Button " + i;
                btn.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                btn.Click += new EventHandler(SearchButtonClicked);
                tableLayoutPanel1.Controls.Add(btn, 3, i + 1);
                ComboBox cb = new ComboBox();
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Name = "ComboBox " + i;
                if (ps.Results != null)
                {
                    foreach (string key in ps.Results.Keys)
                    {
                        cb.Items.Add(key);
                    }
                }
                else
                {
                    cb.Items.Add("No results found");
                }
                cb.SelectedIndex = 0;
                cb.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                tableLayoutPanel1.Controls.Add(cb, 4, i + 1);
            }
        }
        public void SearchButtonClicked(object sender, EventArgs ea){
            //note: this starts at 0, even though the gui placement starts at 1
            int row = Int32.Parse(((Control)sender).Name.Substring(((Control)sender).Name.Length - 2));
            TextBox SearchBox=(TextBox)tableLayoutPanel1.Controls["TextBox "+row];
            ComboBox ProviderBox=(ComboBox)tableLayoutPanel1.Controls["ComboBox Providers "+row];
            Label ShownameLabel=(Label)tableLayoutPanel1.Controls["Label "+row];
            DataGenerator.ParsedSearch Search=DataGenerator.Search(RelationProvider.GetProviderByName(ProviderBox.SelectedItem.ToString()), SearchBox.Text, ShownameLabel.Text);
            ComboBox cbResults = (ComboBox)tableLayoutPanel1.Controls["ComboBox " + row];
            cbResults.Items.Clear();
            if (Search.Results != null)
            {
                foreach (string s in Search.Results.Keys)
                {
                    cbResults.Items.Add(s);
                }
            }
            else
            {
                cbResults.Items.Add("No results found");
            }
            cbResults.SelectedIndex=0;
            Results[row] = Search;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Results.Count; i++)
            {
                ComboBox cbResults = (ComboBox)tableLayoutPanel1.Controls["ComboBox " + i];
                Results[i].SelectedResult = cbResults.SelectedItem.ToString() ;
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;            
            Close();
        }
    }
}

