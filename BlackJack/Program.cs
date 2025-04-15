/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Windows.Forms;
using BlackJack.Classes.Helpers;
using BlackJack.Forms;

namespace BlackJack
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            /* Build graphics assets */
            GraphicsBuilder.BuildDeck();
            /* Run the application */
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmGame());
        }
    }
}
