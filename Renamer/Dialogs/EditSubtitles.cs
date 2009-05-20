using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Renamer.Classes.SubtitleEdit;
using System.Diagnostics;

namespace Renamer.Dialogs
{
    /// <summary>
    /// Subtitle edit dialog
    /// </summary>
    public partial class EditSubtitles : Form
    {
        /// <summary>
        /// Path of the subtitle which is edited
        /// </summary>
        string path;

        /// <summary>
        /// Path of the corresponding video file
        /// </summary>
        string videopath;

        /// <summary>
        /// subtitle file contents
        /// </summary>
        SubtitleEditFile subtitle=null;

        
        /// <summary>
        /// needed for getting timespan?
        /// </summary>
        DateTime backup = new DateTime();

        /// <summary>
        /// Constructor, extracts all information from subtitle file and shows it in GUI
        /// </summary>
        /// <param name="path">Path of the subtitle file</param>
        /// <param name="videopath">Path of the video file</param>
        public EditSubtitles(string path, string videopath)
        {
            this.path = path;
            this.videopath = videopath;
            InitializeComponent();
            if (videopath == null || videopath == "")
            {
                btnVideo.Enabled = false;
            }
            if (Path.GetExtension(path).ToLower() == ".srt")
            {
                subtitle = new srtEditFile(path);
                //no subs found, error and close dialog
                if(subtitle.subs.Count==0){
                    Helper.Log("Subtitle file could not be processed properly",Helper.LogType.Error);
                    Close();
                    return;
                }
            }
            else
            {
                Helper.Log("No support for this subtitle format yet, sorry.", Helper.LogType.Info);
                Close();
                return;
            }
            foreach (string sub in subtitle.subs)
            {
                string replace = Regex.Replace(sub, "<.*?>", "");
                replace = replace.Replace(System.Environment.NewLine, " ");
                cbSubs.Items.Add(replace);
                cbSecond.Items.Add(replace);
            }
            cbSubs.SelectedIndex = 0;
            cbSecond.SelectedIndex = cbSecond.Items.Count-1;
            dtpDelay.Value = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            if (subtitle is srtEditFile)
            {
                dtpSub.Value = ((srtEditFile)subtitle).timesFrom[0];
                nudMSSub.Value = ((srtEditFile)subtitle).timesFrom[0].Millisecond;
                dtpSecond.Value = ((srtEditFile)subtitle).timesFrom[cbSecond.SelectedIndex];
                nudSecond.Value = ((srtEditFile)subtitle).timesFrom[cbSecond.SelectedIndex].Millisecond;
                dtpFrom.Value = ((srtEditFile)subtitle).timesFrom[0];
                nudFrom.Value = ((srtEditFile)subtitle).timesFrom[0].Millisecond;
                dtpTo.Value = ((srtEditFile)subtitle).timesTo[((srtEditFile)subtitle).timesTo.Count-1];
                nudTo.Value = ((srtEditFile)subtitle).timesTo[((srtEditFile)subtitle).timesTo.Count-1].Millisecond;
            }
            backup = dtpDelay.Value;
        }

        /// <summary>
        /// Discards changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Applies changes and saves subtitle file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbOffset.Checked)
            {
                if (rbManually.Checked)
                {
                    DateTime dtStart, dtTo, dtOffset;
                    if (cbRange.Checked)
                    {
                        dtStart = new DateTime(2000, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                        dtTo = new DateTime(2000, 1, 1, dtpTo.Value.Hour, dtpTo.Value.Minute, dtpTo.Value.Second, ((int)nudTo.Value));
                    }
                    else
                    {
                        dtStart = new DateTime(1900, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                        dtTo = new DateTime(2100, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                    }
                    dtOffset = new DateTime(2000, 1, 1, dtpDelay.Value.Hour, dtpDelay.Value.Minute, dtpDelay.Value.Second, ((int)nudMSDelay.Value));
                    if (cbBackwards.Checked)
                    {
                        subtitle.Offset(backup.Subtract(dtOffset), 0, dtStart, dtTo);
                    }
                    else
                    {
                        subtitle.Offset(dtOffset.Subtract(backup), 0, dtStart, dtTo);
                    }
                }
                else if (rbSub.Checked)
                {
                    //Offset depending on subtitle file type
                    if (subtitle is srtEditFile)
                    {
                        srtEditFile srtfile = (srtEditFile)subtitle;
                        //figure out delay by difference between entered time and time subtitle appears in movie
                        DateTime dt = new DateTime(2000, 1, 1, dtpSub.Value.Hour, dtpSub.Value.Minute, dtpSub.Value.Second, ((int)nudMSSub.Value));
                        TimeSpan ts = dt.Subtract(srtfile.timesFrom[cbSubs.SelectedIndex]);
                        DateTime dtStart, dtTo;
                        if (cbRange.Checked)
                        {
                            dtStart = new DateTime(2000, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                            dtTo = new DateTime(2000, 1, 1, dtpTo.Value.Hour, dtpTo.Value.Minute, dtpTo.Value.Second, ((int)nudTo.Value));
                        }
                        else
                        {
                            dtStart = new DateTime(1900, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                            dtTo = new DateTime(2100, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                        }
                        srtfile.Offset(ts, 0,dtStart,dtTo);
                    }
                }
            }
            else if(rbScale.Checked)
            {
                //scale depending on subtitle file type
                if (subtitle is srtEditFile)
                {
                    srtEditFile srtfile = (srtEditFile)subtitle;
                    //sub scale
                    DateTime dtSecond = new DateTime(2000, 1, 1, dtpSecond.Value.Hour, dtpSecond.Value.Minute, dtpSecond.Value.Second, ((int)nudSecond.Value));
                    DateTime dtStart,dtTo;
                    if (cbRange.Checked)
                    {
                        dtStart = new DateTime(2000, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                        dtTo = new DateTime(2000, 1, 1, dtpTo.Value.Hour, dtpTo.Value.Minute, dtpTo.Value.Second, ((int)nudTo.Value));
                    }
                    else
                    {
                        dtStart = new DateTime(1900, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                        dtTo = new DateTime(2100, 1, 1, dtpFrom.Value.Hour, dtpFrom.Value.Minute, dtpFrom.Value.Second, ((int)nudFrom.Value));
                    }
                    srtfile.Scale(cbSecond.SelectedIndex, dtSecond,dtStart,dtTo);
                }
            }
            subtitle.SaveFile();
            Close();
        }

        #region Random GUI stuff which I can't bother commenting right now
        private void cbSubs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (subtitle is srtEditFile)
            {
                dtpSub.Value = ((srtEditFile)subtitle).timesFrom[cbSubs.SelectedIndex];
                nudMSSub.Value = ((srtEditFile)subtitle).timesFrom[cbSubs.SelectedIndex].Millisecond;
                string replace = Regex.Replace(((srtEditFile)subtitle).subs[cbSubs.SelectedIndex], "<.*?>", "");
                toolTip1.SetToolTip(cbSubs, replace);
            }
        }

        private void Subs_Click(object sender, EventArgs e)
        {
            rbSub.Checked = true;
            rbOffset.Checked = true;
        }

        private void Manually_Click(object sender, EventArgs e)
        {
            rbManually.Checked = true;
            rbOffset.Checked = true;
        }

        private void dtpDelay_MouseDown(object sender, MouseEventArgs e)
        {
            rbManually.Checked = true;
            rbOffset.Checked = true;
        }

        private void cbBackwards_CheckedChanged(object sender, EventArgs e)
        {
            rbOffset.Checked = true;
            rbManually.Checked = true;
        }

        private void dtpSub_MouseDown(object sender, MouseEventArgs e)
        {
            rbSub.Checked = true;
            rbOffset.Checked = true;
        }

        private void rbManually_CheckedChanged(object sender, EventArgs e)
        {
            rbOffset.Checked = true;
        }

        private void rbSub_CheckedChanged(object sender, EventArgs e)
        {
            rbOffset.Checked = true;
        }

        private void cbSecond_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (subtitle is srtEditFile)
            {
                dtpSecond.Value = ((srtEditFile)subtitle).timesFrom[cbSecond.SelectedIndex];
                nudSecond.Value = ((srtEditFile)subtitle).timesFrom[cbSecond.SelectedIndex].Millisecond;
                string replace = Regex.Replace(((srtEditFile)subtitle).subs[cbSecond.SelectedIndex], "<.*?>", "");
                toolTip1.SetToolTip(cbSecond, replace);
            }
        }

        private void cbSecond_Click(object sender, EventArgs e)
        {
            rbScale.Checked = true;
        }

        private void dtpSecond_MouseDown(object sender, MouseEventArgs e)
        {
            rbScale.Checked = true;
        }

        private void nudSecond_Click(object sender, EventArgs e)
        {
            rbScale.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            try
            {
                Process myProc = Process.Start(videopath);
            }
            catch (Exception ex)
            {
                Helper.Log("Couldn't open " + videopath + ":" + ex.Message, Helper.LogType.Error);
            }
        }
        #endregion
    }
}
