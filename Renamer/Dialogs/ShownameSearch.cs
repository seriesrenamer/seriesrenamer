using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Renamer.Classes.Provider;
using Renamer.Logging;

namespace Renamer.Dialogs
{
    public partial class ShownameSearch : Form
    {
        public List<DataGenerator.ParsedSearch> Results;
        private int maxHeight = 600;
        public ShownameSearch(List<DataGenerator.ParsedSearch> results)
        {
            InitializeComponent();
            Results = results;
            KeyPreview = true;
            for (int i=0;i<Results.Count;i++)
            {
                DataGenerator.ParsedSearch ps = Results[i];
                tableLayoutPanel1.RowCount++;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                Label lbl=new Label();
                lbl.Text=ps.Showname;
                lbl.Name="Label "+i;
                lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                tableLayoutPanel1.Controls.Add(lbl, 0 , i+1);
                TextBox tb = new TextBox();
                tb.Text = ps.Showname;
                tb.Name = "TextBox " + i;
                tb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                tb.KeyDown += new KeyEventHandler(SearchBoxKeyDown);
                tableLayoutPanel1.Controls.Add(tb, 1, i + 1);                
                ComboBox cbProviders = new ComboBox();
                cbProviders.DropDownStyle = ComboBoxStyle.DropDownList;
                cbProviders.Name = "ComboBox Providers " + i;
                cbProviders.Items.AddRange(RelationProvider.ProviderNames);
                cbProviders.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                RelationProvider provider = RelationProvider.GetCurrentProvider();
                cbProviders.SelectedItem = provider.Name;
                tableLayoutPanel1.Controls.Add(cbProviders, 2, i + 1);
                Button btn = new Button();
                btn.Text = "Search";
                btn.Name = "Button " + i;
                btn.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                btn.Click += new EventHandler(SearchButtonClicked);
                tableLayoutPanel1.Controls.Add(btn, 3, i + 1);
                ComboBox cb = new ComboBox();
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Name = "ComboBox " + i;
                if (ps.Results != null && ps.Results.Count!=0)
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
                cb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                tableLayoutPanel1.Controls.Add(cb, 4, i + 1);
            }
            
            //add one more because of stretching
            tableLayoutPanel1.RowCount++;
            if (Settings.Instance.IsMonoCompatibilityMode)
            {
                AutoSize = false;
                Height = tableLayoutPanel1.Height + 107;
            }
            if (Height > maxHeight)
            {
                AutoSize = false;
                Height = maxHeight;
                //107 is the height of the dialog minus the table control
                tableLayoutPanel1.AutoSize = false;
                tableLayoutPanel1.Height = maxHeight - 107;                
                
            }
        }

        public void SearchBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int row = Int32.Parse(((Control)sender).Name.Substring(((Control)sender).Name.Length - 2));
                Button SearchButton = (Button)tableLayoutPanel1.Controls["Button " + row];
                SearchButton.PerformClick();
            }
        }

        public void SearchButtonClicked(object sender, EventArgs e){
            //note: this starts at 0, even though the gui placement starts at 1
            int row = Int32.Parse(((Control)sender).Name.Substring(((Control)sender).Name.Length - 2));
            TextBox SearchBox=(TextBox)tableLayoutPanel1.Controls["TextBox "+row];
            ComboBox ProviderBox=(ComboBox)tableLayoutPanel1.Controls["ComboBox Providers "+row];
            Label ShownameLabel=(Label)tableLayoutPanel1.Controls["Label "+row];
            DataGenerator.ParsedSearch Search=DataGenerator.Search(RelationProvider.GetProviderByName(ProviderBox.SelectedItem.ToString()), SearchBox.Text, ShownameLabel.Text);
            ComboBox cbResults = (ComboBox)tableLayoutPanel1.Controls["ComboBox " + row];
            cbResults.Items.Clear();
            if (Search.Results != null && Search.Results.Count!=0)
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

        private void ShownameSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Logger.Instance.LogMessage("keydown", LogLevel.INFO);
                foreach (Control c in tableLayoutPanel1.Controls)
                {
                    if (c.Focused && c.GetType() == typeof(TextBox))
                    {
                        return;
                    }
                }
                e.Handled = true;
                btnOK.PerformClick();
            }
        }
    }
}

