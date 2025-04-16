/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */

using System;
using System.Drawing;
using System.Windows.Forms;
using BlackJack.Classes;
using BlackJack.Classes.Helpers;
using BlackJack.Classes.Helpers.Management;
using BlackJack.Controls;
using BlackJack.Properties;

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
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Black;
            BackgroundImage = Resources.lobby;
            BackgroundImageLayout = ImageLayout.Zoom;

            ReturnToLobby += OnReturnToLobby;
        }

        protected override void OnLoad(EventArgs e)
        {
            /* Set window position and size */
            var loc = SettingsManager.Settings.Location;
            if (loc == Point.Empty)
            {
                /* Scale form to less than the screen width/height */
                var screen = Utils.GetCurrentMonitor(this);
                var x = screen.Bounds.Width - 100;
                var y = screen.Bounds.Height - 100;
                Size = new Size(x, y);
                /* Set location to center screen */
                Location = new Point((screen.Bounds.Width / 2) - (Size.Width / 2), (screen.Bounds.Height / 2) - (Size.Height / 2));
            }
            else
            {
                /* Big fucking white snow flakes... */
                Location = loc;
                Size = SettingsManager.Settings.Size;
                if (SettingsManager.Settings.Maximized)
                {
                    WindowState = FormWindowState.Maximized;
                }
            }
            OnResize(e);
            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            /* Ask the user if they really want to quit - yes, I know, kind of annoying */
            if (SettingsManager.Settings.Options.Confirm.OnExit)
            {
                if (MessageBox.Show(this, @"Are you sure you want to really quit?", @"Quit BlackJack") == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (WindowState == FormWindowState.Normal)
            {
                SettingsManager.Settings.Location = Location;
                SettingsManager.Settings.Size = Size;
            }
            SettingsManager.Settings.Maximized = WindowState == FormWindowState.Maximized;
            /* Through the cockpit window, we can now piss off :) */
            base.OnFormClosing(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (!Visible)
            {
                return;
            }
            if (WindowState == FormWindowState.Normal)
            {
                SettingsManager.Settings.Location = Location;
                SettingsManager.Settings.Size = Size;
            }
            base.OnResize(e);
        }

        private void OnReturnToLobby(Game obj)
        {
            System.Diagnostics.Debug.Print("Lobby");
        }
    }
}
