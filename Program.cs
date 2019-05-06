using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Traverse_Returns_Addon
{
    static class Program
    {

        internal static string strRaNum = String.Empty;
        internal static string strReportRaNum = String.Empty;
        internal static string sglbEntryNum = String.Empty;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmReturns());
           
        }
    }
}