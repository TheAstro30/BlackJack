/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.Drawing;
using System.Windows.Forms;
using BlackJack.Classes;

namespace BlackJack.Forms
{
    public sealed class FrmGame : Game
    {
        public FrmGame()
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            MinimumSize = new Size(900, 600);
            ClientSize = new Size(900, 600);
            Text = @"BlackJack";

        }
    }
}
