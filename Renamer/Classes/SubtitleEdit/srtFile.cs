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

namespace Renamer.Classes.SubtitleEdit
{
    /// <summary>
    /// srt Subtitle file
    /// </summary>
    class srtEditFile : SubtitleEditFile
    {
        /// <summary>
        /// Starting times of the subtitles
        /// </summary>
        public List<DateTime> timesFrom=new List<DateTime>();

        /// <summary>
        /// End times of the subtitles
        /// </summary>
        public List<DateTime> timesTo = new List<DateTime>();

        /// <summary>
        /// srt file constructor, simply calls base constructor
        /// </summary>
        /// <param name="path"></param>
        public srtEditFile(string path)
            : base(path)
        {            
        }
        /// <summary>
        /// Reads the subtitle file
        /// </summary>
        public override void ReadFile()
        {
            FileStream s = File.Open(path, FileMode.Open);
            StreamReader r = new StreamReader(s,Encoding.Default);
            string line = null;
            List<string> lines = new List<string>(r.ReadToEnd().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None));
            r.Close();
            int status = 1; //0=waiting for empty line, 1=waiting for sequencial number, 2=expecting timestamps, 3=expecting subs
            string sub="";
            for (int i = 0; i < lines.Count; i++)
            {
                line = lines[i];
                //skip lines until empty line
                if(status==0){
                    if(line==null||line==""){
                        status=1;
                    }
                    continue;
                }
                //skip empty lines until sequential number found
                if(status==1){
                    if (line == null || line == "")
                    {
                        continue;
                    }
                    try{
                        int number=Int32.Parse(line);
                        status=2;
                        continue;
                    }catch(FormatException){
                        Helper.Log("Error parsing "+path+" at line "+i+":"+line,Helper.LogType.Warning);
                        status=0;
                        continue;
                    }
                }
                //try to extract times
                if (status == 2)
                {
                    try
                    {
                        List<string> split = new List<string>(line.Split(new string[] { ":", ",", " --> " }, StringSplitOptions.RemoveEmptyEntries));
                        DateTime dtFrom = new DateTime(), dtTo = new DateTime();
                        if (split.Count == 8)
                        {
                            dtFrom = new DateTime(2000, 1, 1, Int32.Parse(split[0]), Int32.Parse(split[1]), Int32.Parse(split[2]), Int32.Parse(split[3]));
                            dtTo = new DateTime(2000, 1, 1, Int32.Parse(split[4]), Int32.Parse(split[5]), Int32.Parse(split[6]),Int32.Parse(split[7]));
                        }
                        timesFrom.Add(dtFrom);
                        timesTo.Add(dtTo);
                        status = 3;
                        continue;
                    }
                    catch (Exception)
                    {
                        Helper.Log("Error parsing " + path + " at line " + i + ":" + line, Helper.LogType.Warning);
                        status = 0;
                    }
                }
                //try to extract subtitles
                if (status == 3)
                {
                    //empty line found, end of subtitle
                    if (line == null || line == "")
                    {
                        status = 1;
                        subs.Add(sub);
                        sub = "";
                        continue;
                    }
                    //Add line to subtitle string
                    if (sub == "")
                    {
                        sub = line;
                    }
                    else
                    {
                        sub += System.Environment.NewLine + line;
                    }
                    continue;
                }
            }
            //if there's no empty line in the end we have to add the last subtitle
            if (status == 3 && sub != "")
            {
                subs.Add(sub);
            }

        }

        /// <summary>
        /// Offsets the times in a specific timeframe
        /// </summary>
        /// <param name="delay">Offset time</param>
        /// <param name="frames">not used</param>
        /// <param name="start">start of the range</param>
        /// <param name="to">end of the range</param>
        public override void Offset(TimeSpan delay, int frames, DateTime start, DateTime to)
        {
            //timesto first since we don't want to stretch a subtitle if ranged offset
            for (int i = 0; i < timesTo.Count; i++)
            {
                if (timesFrom[i].Subtract(start).TotalMilliseconds >= 0 && to.Subtract(timesFrom[i]).TotalMilliseconds >= 0)
                {
                    timesTo[i] = timesTo[i].Add(delay);
                }
            }
            for(int i=0;i<timesFrom.Count;i++)
            {
                if (timesFrom[i].Subtract(start).TotalMilliseconds >= 0 && to.Subtract(timesFrom[i]).TotalMilliseconds >= 0)
                {
                    timesFrom[i] = timesFrom[i].Add(delay);
                }
            }
            
        }

        /// <summary>
        /// Scales the subtitle times to correct framerate issues, 0 is used as fixpoint
        /// </summary>
        /// <param name="x2">Index of second subtitle to use for scaling</param>
        /// <param name="dt2destination">Desired time of second subtitle to calculate scale factor</param>
        /// <param name="start">start of the range</param>
        /// <param name="to">end of the range</param>
        public override void Scale(int x2, DateTime dt2destination, DateTime start, DateTime to)
        {
            DateTime dt1 = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            DateTime dt2current = timesFrom[x2];
            TimeSpan ts2current = dt2current.Subtract(dt1);
            TimeSpan ts2destination = dt2destination.Subtract(dt1);
            double scale = ts2destination.TotalMilliseconds / ts2current.TotalMilliseconds;
            //timesto first since we don't want to stretch a subtitle if ranged scale
            for (int i = 0; i < timesTo.Count; i++)
            {
                if (timesFrom[i].Subtract(start).TotalMilliseconds >= 0 && to.Subtract(timesFrom[i]).TotalMilliseconds >= 0)
                {
                    TimeSpan tsicurrent = timesTo[i].Subtract(dt1);
                    TimeSpan tsi = new TimeSpan(0, 0, 0, 0, (int)(tsicurrent.TotalMilliseconds * scale));
                    timesTo[i] = dt1.Add(tsi);
                }
            }
            for (int i = 0; i < timesFrom.Count; i++)
            {
                if (timesFrom[i].Subtract(start).TotalMilliseconds >= 0 && to.Subtract(timesFrom[i]).TotalMilliseconds >= 0)
                {
                    TimeSpan tsicurrent = timesFrom[i].Subtract(dt1);
                    TimeSpan tsi = new TimeSpan(0, 0, 0, 0, (int)(tsicurrent.TotalMilliseconds * scale));
                    timesFrom[i] = dt1.Add(tsi);
                }
            }
        }

        /// <summary>
        /// Saves the file
        /// </summary>
        public override void SaveFile()
        {
            string file="";
            for (int i = 0; i < subs.Count; i++)
            {
                file += (i+1).ToString() + System.Environment.NewLine;
                file += String.Format("{0:00}:{1:00}:{2:00},{3:000} --> {4:00}:{5:00}:{6:00},{7:000}", new object[] { timesFrom[i].Hour, timesFrom[i].Minute, timesFrom[i].Second, timesFrom[i].Millisecond, timesTo[i].Hour, timesTo[i].Minute, timesTo[i].Second, timesTo[i].Millisecond }) + System.Environment.NewLine;
                file += subs[i] + System.Environment.NewLine;
                //new line except at the end
                if (i != subs.Count - 1)
                {
                    file += System.Environment.NewLine;
                }
            }
            try
            {
                File.Delete(path);
                FileStream s = File.Open(path, FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(s);
                w.Write(file);
                w.Close();
            }
            catch (Exception ex)
            {
                Helper.Log("Couldn't write subtitle file " + path + ":" + ex.Message, Helper.LogType.Error);
            }
        }
        

    }
}
