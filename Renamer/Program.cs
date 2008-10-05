using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Renamer
{
    static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(args));
        }
    }
}
