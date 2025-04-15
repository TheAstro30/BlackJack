/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BlackJack.Classes.GameAssets;
using BlackJack.Classes.Helpers;
using BlackJack.Classes.Player;
using BlackJack.Classes.UI;
using BlackJack.Properties;

namespace BlackJack.Classes
{
    public class Game : Form
    {
        private readonly UiSynchronize _sync;

        private Font _font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);

        private Size _tableSize;
        private Point _startPoint;

        private readonly Regions _regions = new Regions();

        /* Decks */
        public Deck GameDeck = new Deck();
        public Deck DiscardDeck = new Deck();

        public List<IPlayer> Players = new List<IPlayer>();

        public int MinimumBet { get; set; }

        public Game()
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            /* Double buffering */
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            /* UI sync object */
            _sync = new UiSynchronize(this);

            MinimumBet = 100;
        }

        #region Public methods
        public void NewGame()
        {
            /* Set up the players - with current table, we can have up to 6 computer players/1 human */
            for (var p = 0; p <= 3; p++)
            {
                IPlayer player;
                switch (p)
                {
                    case 1:
                        /* Add human */
                        player = new HumanPlayer {Name = "Human", Money = 2000};
                        break;

                    case 3:
                        /* Add the dealer */
                        player = new Dealer();
                        break;

                    default:
                        player = new ComputerPlayer();
                        break;
                }
                player.Index = p;
                Players.Add(player);
            }
            /* Build new deck - four decks */
            DiscardDeck = new Deck();
            GameDeck = new Deck();
            for (var x = 0; x <= 3; x++)
            {
                foreach (var c in GraphicsBuilder.MasterDeck)
                {
                    GameDeck.Add(c);
                }
            }
            /* Shuffle it */
            GameDeck.Shuffle();

            NewHand(); //test code
        }

        public void NewHand()
        {
            foreach (var p in Players)
            {
                foreach (var c in p.Hand)
                {
                    DiscardDeck.Add(c);
                }
                p.Hand = new List<Card>();
                p.Total = 0;
                p.Stand = false;
            }

            Invalidate();

            /* Check deck isn't in need of shuffling */
            if (GameDeck.Count < 15)
            {
                foreach (var c in DiscardDeck)
                {
                    GameDeck.Add(c);
                }
                DiscardDeck.Clear();
                GameDeck.Shuffle();
            }

            /* Ask to place bets */

            /* Deal cards */
            var t = new Thread(DealThread) {IsBackground = true};
            t.Start();
        }
        #endregion

        //test code
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                MessageBox.Show("Load event");
                return;
            }
            NewGame();
            base.OnLoad(e);
        }
        //test code

        protected override void OnResize(EventArgs e)
        {
            /* Rescale playing table and set the scale for card sizes */
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            /* Calculate what the size of the images should be based on clientsize */
            var img = Resources.table;
            var ratioX = (double)ClientSize.Width / img.Width;
            var ratioY = (double)ClientSize.Height / img.Height;
            /* Use whichever multiplier is smaller */
            var ratio = ratioX < ratioY ? ratioX : ratioY;
            /* Now we can get the new height and width - only to the actual card size */
            var newWidth = Convert.ToInt32(img.Width * ratio / 1.2F);
            var newHeight = Convert.ToInt32(img.Height * ratio / 1.2F);
            _tableSize = new Size(newWidth > img.Width ? img.Width : newWidth, newHeight > img.Height ? img.Height : newHeight);
            _startPoint = new Point(ClientSize.Width / 2 - _tableSize.Width / 2, 40);
            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime && !Visible)
            {
                return;
            }
            /* Draw background */
            e.Graphics.DrawImageTiled(Resources.carpet, ClientRectangle.Size);
            /* Create a background bitmap (table) to draw the game play on */
            using (var table = new Bitmap(Resources.table.Width, Resources.table.Height))
            {
                using (var g = Graphics.FromImage(table))
                {
                    /* Draw playing table */
                    Image bmp = Resources.table;
                    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                    /* Draw each players chips, bets, text and dealt hand */
                    for (var i = 0; i <= 3; i++)
                    {
                        var p = Players[i];
                        /* Draw player text */
                        var format = new StringFormat {Alignment = StringAlignment.Center};
                        g.DrawString(p.ToString(), _font, Brushes.Gold, new Point(_regions.Text[i].X, _regions.Text[i].Y), format);
                        /* Draw card count */
                        if (p.Total > 0 && p.Hand.Count > 1)
                        {
                            if (p.GetType() == typeof(Dealer) && p.Hand.Count == 2 && p.Hand[1].IsHidden)
                            {
                                /* Do nothing */
                            }
                            else
                            {
                                /* If total count of cards is 2 and adds up to 21, check for black jack */
                                var total = p.Total.ToString();
                                if (p.Hand.Count == 2 && p.Total == 21)
                                {
                                    var bj = p.Hand.FirstOrDefault(o => o.Value > 10);
                                    if (bj != null)
                                    {
                                        total = "BlackJack!";
                                    }
                                }
                                g.DrawString(total, _font, Brushes.Gold, _regions.Count[i], format);
                            }
                        }

                        /* Draw placed bet chip(s) */
                        if (p.Bet > 0)
                        {
                            bmp = Resources.chip;
                            g.DrawImage(bmp, _regions.Bet[i].X - bmp.Width / 2, _regions.Bet[i].Y - bmp.Height / 2, bmp.Width, bmp.Height);
                        }
                        if (p.Hand.Count == 0)
                        {
                            continue;
                        }
                        /* Draw dealt hand */
                        var xOffset = 0;
                        var xStart = _regions.Cards[i].X - (15 * p.Hand.Count - 1); /* Center the card hand within the region */
                        foreach (var c in p.Hand)
                        {
                            bmp = c.IsHidden
                                ? Resources.card_back
                                : GraphicsBuilder.Cards[new KeyValuePair<Suit, int>(c.Suit, c.Value)];
                            g.DrawImage(bmp, xStart + xOffset, _regions.Cards[i].Y, bmp.Width, bmp.Height);
                            xOffset += 15;
                        }
                    }
                }
                /* Output bitmap */
                e.Graphics.DrawImage(table, new Rectangle(_startPoint.X, _startPoint.Y, _tableSize.Width, _tableSize.Height));
                /* Draw minimum bet text */
                e.Graphics.DrawString($"Minimum bet: ${Formatting.FormatNumber(MinimumBet)}", _font, Brushes.Gold, new Point(10, ClientSize.Height - 40));
            }
            base.OnPaint(e);
        }

        #region Deal thread callbacks
        private void DealThread()
        {
            Thread.Sleep(500);
            /* Begin new deal */
            for (var i = 0; i <= 1; i++)
            {
                foreach (var p in Players)
                {
                    /* Pull card from deck */
                    var c = GameDeck[0];
                    GameDeck.RemoveAt(0);
                    /* If the player is the dealer and this is the second card, hide it */
                    if (p.GetType() == typeof(Dealer) && i == 1)
                    {
                        System.Diagnostics.Debug.Print("Dealer");
                        c.IsHidden = true;
                    }

                    p.AddCard(c);
                    System.Diagnostics.Debug.Print("Player cards add " + p.ToString() + " " + c.Suit + " " + c.Value + " Count: " + p.Total);
                    Redraw();
                    Thread.Sleep(200);
                }
            }
        }

        private void Redraw()
        {
            if (InvokeRequired)
            {
                _sync.Execute(Redraw);
                return;
            }
            Invalidate();
        }
        #endregion
    }
}
