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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Renamer.Classes;
using System.Security;
using System.Security.Permissions;

namespace Renamer
{
    
        

        
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern Boolean AttachConsole(int dwProcessId);
        const int ATTACH_PARENT_PROCESS = -1;
        public static string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Series Renamer");

        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length>0){
                if (File.Exists(args[0]))
                {
                    args[0] = Filepath.goUpwards(args[0], 1);
                }
                if (!Directory.Exists(args[0]))
                {
                    AttachConsole(ATTACH_PARENT_PROCESS);
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("Series Renamer command line argument(s):");
                    Console.Out.WriteLine("\"Series Renamer.exe [Path]\": Opens the program in the folder [Path].");
                    Console.Out.WriteLine("\"Series Renamer.exe /help\": Displays this help message.");
                    return;
                }
            }

            //Use current directory for config and log files if possible, otherwise revert to %appdata%\Series Renamer
            string localPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "tempFile.tmp");
            try
            {
                using (FileStream fs = new FileStream(localPath, FileMode.CreateNew, FileAccess.Write))
                {
                    fs.WriteByte(0xff);
                }

                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                    Program.configPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
            }
            catch(Exception e) { }

            try
            {
                localPath = Path.Combine(Program.configPath, "tempFile.tmp");
                if (!Directory.Exists(Program.configPath))
                    Directory.CreateDirectory(Program.configPath);
                using (FileStream fs = new FileStream(localPath, FileMode.CreateNew, FileAccess.Write))
                {
                    fs.WriteByte(0xff);
                }

                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Failed to write to " + Program.configPath + ". Settings won't be saved.");
            }
            Console.Out.WriteLine("");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1.Instance = new Form1(args);
            Application.Run(Form1.Instance);
        }
    }
}
