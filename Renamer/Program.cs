using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Renamer
{
    
        

        
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern Boolean AttachConsole(int dwProcessId);
        const int ATTACH_PARENT_PROCESS = -1;
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length>0){
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
            
            Console.Out.WriteLine("");
            Console.Out.WriteLine("Test");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(args));
        }
    }
}
