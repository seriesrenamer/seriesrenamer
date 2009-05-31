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
        List<DataGenerator.ParsedSearch> Results;
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
                cbProviders.Name = "ComboBox Providers " + i;
                cbProviders.Items.AddRange(RelationProvider.ProviderNames);
                cbProviders.SelectedIndex = 0;
                cbProviders.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                tableLayoutPanel1.Controls.Add(cbProviders, 2, i + 1);
                Button btn = new Button();
                btn.Text = "Search";
                btn.Name = "Button " + i;
                btn.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                tableLayoutPanel1.Controls.Add(btn, 3, i + 1);
                ComboBox cb = new ComboBox();
                cb.Name = "ComboBox " + i;
                foreach (string key in ps.Results.Keys)
                {
                    cb.Items.Add(ps.Results[key]);
                }
                cb.SelectedIndex = 0;
                cb.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                tableLayoutPanel1.Controls.Add(cb, 4, i + 1);
            }
        }
    }
}
