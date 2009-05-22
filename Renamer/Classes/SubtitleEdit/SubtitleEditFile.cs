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
    /// Abstract subtitle file
    /// </summary>
    abstract class SubtitleEditFile
    {
        /// <summary>
        /// Path of the file
        /// </summary>
        public string path;
        /// <summary>
        /// List of the subtitle lines
        /// </summary>
        public List<string> subs = new List<string>();
        /// <summary>
        /// Subtitle file constructor
        /// </summary>
        /// <param name="path">Path of the file</param>
        public SubtitleEditFile(string path)
        {
            this.path = path;
            ReadFile();
        }

        /// <summary>
        /// Reads the subtitle file
        /// </summary>
        public abstract void ReadFile();

        /// <summary>
        /// Offsets the times in a specific timeframe
        /// </summary>
        /// <param name="delay">Offset time, used if subtitle format is time based</param>
        /// <param name="frames">Ammount of frames to offset, if subtitle format is frame based</param>
        /// <param name="start">start of the range(only time based for now)</param>
        /// <param name="to">end of the range(only time based for now)</param>
        public abstract void Offset(TimeSpan delay, int frames, DateTime start, DateTime to);
        
        /// <summary>
        /// Scales the subtitle times to correct framerate issues(only time based for now), 0 is used as fixpoint
        /// </summary>
        /// <param name="x2">Index of second subtitle to use for scaling</param>
        /// <param name="dt2destination">Desired time of second subtitle to calculate scale factor(only time based for now)</param>
        /// <param name="start">start of the range(only time based for now)</param>
        /// <param name="to">end of the range(only time based for now)</param>
        public abstract void Scale(int x2, DateTime dt2destination, DateTime start, DateTime to);

        /// <summary>
        /// Saves the file
        /// </summary>
        public abstract void SaveFile();
    }
}

