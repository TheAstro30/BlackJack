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
using BlackJack.Classes.Helpers.Management;
using BlackJack.Classes.Player;
using BlackJack.Classes.UI;
using BlackJack.Properties;

namespace BlackJack.Controls
{
    public class Game : Form
    {
        private readonly UiSynchronize _sync;
        private readonly Font _font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        private Size _tableSize;
        private Point _startPoint;
        private readonly Regions _regions = new Regions();
        private IPlayer _currentPlayer;

        public event Action<IPlayer> PlayerResigned;

        #region Properties
        public int MinimumBet { get; set; }

        public bool GameInProgress { get; set; }

        /* Decks */
        public Deck GameDeck { get; private set; }

        public Deck DiscardDeck { get; private set; }

        /* Players list */
        public List<IPlayer> Players { get; private set; }
        #endregion

        public Game()
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            /* Settings - this class is called first, before FrmGame, so load settings here */
            SettingsManager.Load();
            AudioManager.Init();
            /* Double buffering */
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            /* UI sync object */
            _sync = new UiSynchronize(this);
            MinimumBet = 100;
        }

        #region Overridable methods
        protected virtual void OnPlayerActionRequired(IPlayer player, PlayerAction action)
        {
            /* Do nothing on this class - handled in derived class */
        }
        #endregion

        #region Public methods
        public void NewGame()
        {
            /* Set up the players - with current table, we can have up to 6 computer players/1 human, I'm using a total of 3 to keep it cleaner */
            GameInProgress = true;
            Players = new List<IPlayer>();
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
                /* Add callback handles */
                player.PlayerActionRequired += OnPlayerActionRequired;
                player.EndBet += OnPlayerEndBet;
                player.EndTurn += OnPlayerEndTurn;
            }
            /* Build new deck - four decks */
            DiscardDeck = new Deck();
            GameDeck = new Deck();
            for (var x = 0; x <= 3; x++)
            {
                foreach (var c in GraphicsBuilder.MasterDeck)
                {
                    GameDeck.Add(new Card(c));
                }
            }
            /* Shuffle it */
            GameDeck.Shuffle();
            NewRound();
        }

        public void NewRound()
        {
            if (InvokeRequired)
            {
                System.Diagnostics.Debug.Print("Need to invoke");
                _sync.Execute(NewRound);
                return;
            }
            System.Diagnostics.Debug.Print("Begin new round");
            foreach (var p in Players)
            {
                foreach (var c in p.Hand)
                {
                    DiscardDeck.Add(c);
                }
                p.Hand = new List<Card>();
                p.Total = 0;
                p.Bet = 0;
                p.State = PlayerState.None;
                if (p.GetType() != typeof(Dealer) && p.Money < MinimumBet)
                {
                    /* Player can no longer play */
                    PlayerResigned?.Invoke(p);
                    if (p.GetType() == typeof(HumanPlayer))
                    {
                        /* It's me */
                        GameInProgress = false;
                        Invalidate();
                        return;
                    }
                }
            }
            Invalidate();
            /* Check deck isn't in need of shuffling */
            if (GameDeck.Count < 25)
            {
                System.Diagnostics.Debug.Print("RESHUFFLE");
                foreach (var c in DiscardDeck)
                {
                    GameDeck.Add(c);
                }
                DiscardDeck.Clear();
                GameDeck.Shuffle();
            }
            /* Begin a new round - place bets */
            var t = new Thread(BeginBetting) { IsBackground = true};
            t.Start();
        }
        #endregion

        #region Overrides
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            /* Dump settings */
            SettingsManager.Save();
            base.OnFormClosing(e);
        }

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
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime && !Visible || !GameInProgress)
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
                        g.DrawString(p.ToString(), _font,
                            _currentPlayer != null && _currentPlayer.Index == i ? Brushes.Red : Brushes.Gold,
                            new Point(_regions.Text[i].X, _regions.Text[i].Y), format);
                        /* Draw card count */
                        if (p.Total > 0 && p.Hand.Count > 1 && !p.Hand[1].IsHidden)
                        {
                            /* If total count of cards is 2 and adds up to 21, check for black jack */
                            var total = p.Total.ToString();
                            if (p.Hand.Count == 2 && p.Total == 21)
                            {
                                var bj = p.Hand.FirstOrDefault(o => o.Value >= 10);
                                if (bj != null)
                                {
                                    total = "BlackJack!";
                                }
                            }
                            if (p.Total > 21)
                            {
                                total = "Bust!";
                            }
                            g.DrawString(total, _font, total.Equals("Bust!") ? Brushes.Red : Brushes.Gold, _regions.Count[i], format);
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
                        var xStart = _regions.Cards[i].X - 8 * p.Hand.Count; /* Center the card hand within the region */
                        foreach (var c in p.Hand)
                        {
                            bmp = c.IsHidden
                                ? Resources.card_back
                                : GraphicsBuilder.Cards[new KeyValuePair<Suit, int>(c.Suit, c.Value)];
                            g.DrawImage(bmp, xStart + xOffset, _regions.Cards[i].Y, bmp.Width, bmp.Height);
                            xOffset += 16;
                        }
                    }
                }
                /* Output bitmap */
                e.Graphics.DrawImage(table, new Rectangle(_startPoint.X, _startPoint.Y, _tableSize.Width, _tableSize.Height));
                /* Draw minimum bet text */
                e.Graphics.DrawString($"Minimum bet: ${MinimumBet.FormatNumber()}", _font, Brushes.Gold, new Point(10, ClientSize.Height - 40));
            }
            base.OnPaint(e);
        }
        #endregion

        #region Playing thread callbacks
        private void BeginBetting()
        {
            System.Diagnostics.Debug.Print("begin betting");
            Thread.Sleep(1000);
            AudioManager.PlayVoice(SoundVoiceType.PlaceBets, true);
            Thread.Sleep(1000);
            /* Get the first player */
            _currentPlayer = Players[0];
            Redraw();
            Thread.Sleep(200);
            _currentPlayer.BeginBet();
        }

        private void BeginDeal()
        {
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
                        c.IsHidden = true;
                    }
                    p.AddCard(c);
                    AudioManager.Play(SoundEffectType.Deal, true);
                    Redraw();
                    Thread.Sleep(300);
                }
            }
            /* Check players for BlackJack */
            CheckPlayersForBlackJack();
            /* Does dealer have an ace? Check dealer has BlackJack */
            var dealer = Players[Players.Count - 1];
            if (dealer.Hand[0].Value == 1)
            {
                /* Possible dealer has BlackJack. Ask for insurance? */

                /* Dealer checks hidden card, if not a BlackJack, game play moves on as normal */
                if (dealer.Hand[1].Value >= 10)
                {
                    /* Dealer has BlackJack! Calculate values, end round */
                    dealer.Hand[1].IsHidden = false;
                    Redraw();
                    AudioManager.Play(SoundEffectType.Deal, true);
                    Thread.Sleep(500);
                    AudioManager.PlayVoice(SoundVoiceType.DealerBlackJack, true);
                    Thread.Sleep(1000);
                    CalculatePlayerTotals();
                    return;
                }
            }
            /* Dealing ended */
            EndDeal();
        }

        private void EndDeal()
        {
            if (InvokeRequired)
            {
                _sync.Execute(EndDeal);
                return;
            }
            /* Pass on to next stage */
            var t = new Thread(() => BeginPlayerTurn(0)) {IsBackground = true};
            t.Start();
        }

        private void BeginPlayerTurn(int index)
        {
            /* Get the first player */
            _currentPlayer = Players[index];
            System.Diagnostics.Debug.Print("Beginning turn: " + _currentPlayer.Name);
            Redraw();
            if (_currentPlayer.GetType() == typeof(Dealer))
            {
                System.Diagnostics.Debug.Print("We are now exiting the beginplayerturn thread");
                EndPlayerTurn();
                return;
            }
            if (_currentPlayer.State == PlayerState.BlackJack)
            {
                /* Skip this player, move to next */
                System.Diagnostics.Debug.Print("Recursive call - blackjack");
                BeginPlayerTurn(index + 1);
                return;
            }
            /* Announce value */
            AudioManager.PlayVoiceNumeric(_currentPlayer.Total);
            Thread.Sleep(1000);
            _currentPlayer.BeginTurn();
        }

        private void EndPlayerTurn()
        {
            if (InvokeRequired)
            {
                _sync.Execute(EndPlayerTurn);
                return;
            }
            /* It's the dealer's turn */
            System.Diagnostics.Debug.Print("Dealer's turn!!");
            var t = new Thread(BeginDealerStandOff) {IsBackground = true};
            t.Start();
        }

        private void BeginDealerStandOff()
        {
            System.Diagnostics.Debug.Print("Beginning dealer standoff...");
            _currentPlayer.Hand[1].IsHidden = false;
            Redraw();
            /* Announce number */
            if (_currentPlayer.Total == 21)
            {
                /* BlackJack! */
                _currentPlayer.State = PlayerState.BlackJack;
                AudioManager.PlayVoice(SoundVoiceType.DealerBlackJack, true);
                Thread.Sleep(1000);
            }
            else
            {
                AudioManager.PlayVoiceNumeric(_currentPlayer.Total);
                Thread.Sleep(500);
                var b = true;
                while (b)
                {
                    if (_currentPlayer.Total == 17 && _currentPlayer.Hand.Count == 2 && _currentPlayer.Hand.FirstOrDefault(o => o.Value == 1) != null)
                    {
                        /* MUST be soft 17 (aces worth 1) */
                        _currentPlayer.Total -= 10;
                    }
                    else if (_currentPlayer.Total < 17)
                    {
                        /* Deal another card unless soft 17 or greater */
                        var c = GameDeck[0];
                        GameDeck.RemoveAt(0);
                        /* Check current hand doesn't have an ace in it (2 cards), if so, change total -10 */
                        if (_currentPlayer.Hand.Count == 2 && _currentPlayer.Total + c.Value > 21 && _currentPlayer.Hand.FirstOrDefault(o => o.Value == 1) != null)
                        {
                            System.Diagnostics.Debug.Print("Dealer removing 10");
                            _currentPlayer.Total -= 10;
                        }
                        _currentPlayer.Hand.Add(c);
                        _currentPlayer.Total += c.Value > 10 ? 10 : c.Value;
                        Redraw();
                        AudioManager.Play(SoundEffectType.Deal, true);
                        Thread.Sleep(500);
                        if (_currentPlayer.Total <= 21)
                        {
                            AudioManager.PlayVoiceNumeric(_currentPlayer.Total);
                            Thread.Sleep(500);
                            if (_currentPlayer.Total < 17)
                            {
                                continue;
                            }
                            /* At soft 17 or greater - end of round */
                        }
                        else
                        {
                            /* Dealer busted - end of round - everyone wins */
                            _currentPlayer.State = PlayerState.Bust;
                            AudioManager.PlayVoice(SoundVoiceType.DealerBust, true);
                        }
                        b = false;
                    }
                    else
                    {
                        b = false;
                    }
                }
            }
            Thread.Sleep(1000);
            CalculatePlayerTotals();
        }

        private void CheckPlayersForBlackJack()
        {
            foreach (var p in Players)
            {
                _currentPlayer = p;
                Redraw();
                if (_currentPlayer.Total == 21 && _currentPlayer.GetType() != typeof(Dealer))
                {
                    /* BlackJack! - end of any turns for this player */
                    _currentPlayer.State = PlayerState.BlackJack;
                    AudioManager.PlayVoice(SoundVoiceType.PlayerBlackJack, true);
                    Thread.Sleep(2000);
                    Redraw();
                }
            }
            _currentPlayer = null;
        }

        private void CalculatePlayerTotals()
        {
            var dealer = Players[Players.Count - 1];
            /* Evaluate player totals */
            foreach (var p in Players.TakeWhile(p => p.GetType() != typeof(Dealer)))
            {
                _currentPlayer = p;
                if (dealer.State == PlayerState.Bust)
                {
                    switch (_currentPlayer.State)
                    {
                        case PlayerState.Bust:
                            continue;

                        case PlayerState.BlackJack:
                            /* They win 2.5 times their bet */
                            _currentPlayer.Money += (int)(_currentPlayer.Bet * 2.5);
                            break;

                        default:
                            /* They win double their bet */
                            _currentPlayer.Money += _currentPlayer.Bet * 2;
                            break;
                    }
                    AudioManager.PlayVoice(SoundVoiceType.PlayerWins);
                    Redraw();
                    Thread.Sleep(1000);
                }
                else if (_currentPlayer.State == PlayerState.BlackJack && dealer.State == PlayerState.BlackJack)
                {
                    /* They win original bet back */
                    AudioManager.PlayVoice(SoundVoiceType.Push);
                    Redraw();
                    Thread.Sleep(1000);
                }
                else if (_currentPlayer.State != PlayerState.Bust)
                {
                    if (p.Total > dealer.Total)
                    {
                        switch (_currentPlayer.State)
                        {
                            case PlayerState.BlackJack:
                                /* They win 2.5 times their bet */
                                _currentPlayer.Money += (int)(_currentPlayer.Bet * 2.5);
                                break;

                            default:
                                /* They win double their bet */
                                _currentPlayer.Money += _currentPlayer.Bet * 2;
                                break;
                        }
                        AudioManager.PlayVoice(SoundVoiceType.PlayerWins);
                        Redraw();
                        Thread.Sleep(1000);
                    }
                    else if (p.Total == dealer.Total)
                    {
                        if (p.State == PlayerState.BlackJack && dealer.State != PlayerState.BlackJack)
                        {
                            /* They win 2.5 times their bet */
                            _currentPlayer.Money += (int)(_currentPlayer.Bet * 2.5);
                            AudioManager.PlayVoice(SoundVoiceType.PlayerWins);
                        }
                        else
                        {
                            /* They win original bet back */
                            _currentPlayer.Money += _currentPlayer.Bet;
                            AudioManager.PlayVoice(SoundVoiceType.Push);
                        }
                        Redraw();
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        AudioManager.PlayVoice(SoundVoiceType.PlayerLoses);
                        Redraw();
                        Thread.Sleep(1000);
                    }
                }
            }
            _currentPlayer = null;
            Redraw();
            Thread.Sleep(1000);
            /* Exit thread and begin a new round */
            NewRound();
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

        #region Game play callbacks
        private void OnPlayerEndBet(IPlayer player)
        {
            if (InvokeRequired)
            {
                AudioManager.Play(SoundEffectType.ChipDrop, true);
                Redraw();
                Thread.Sleep(500);
                /* Get next player */
                _currentPlayer = Players[player.Index + 1];
                if (_currentPlayer.GetType() == typeof(Dealer))
                {
                    /* End of betting */
                    _currentPlayer = null;
                    AudioManager.PlayVoice(SoundVoiceType.NoMoreBets, true);
                    Thread.Sleep(1500);
                }
                _sync.Execute(() => OnPlayerEndBet(player));
                return;
            }
            System.Diagnostics.Debug.Print(">> PLAYER " + player.Name + " ended bet");
            /* Move on to next player. Player is dealer? */
            if (_currentPlayer == null)
            {
                /* Betting ends, moving on to dealing */
                var t = new Thread(BeginDeal) { IsBackground = true };
                t.Start();
            }
            else
            {
                /* Move to next player (still in same background thread) */
                var t = new Thread(_currentPlayer.BeginBet) { IsBackground = true };
                t.Start();
            }
        }

        private void OnPlayerEndTurn(IPlayer player)
        {
            //invoke back to UI
            System.Diagnostics.Debug.Print(">> PLAYER " + player.Name + " ended turn");
            Redraw();
            switch (_currentPlayer.State)
            {
                case PlayerState.Hit:
                    /* Deal another card to player */
                    var c = GameDeck[0];
                    GameDeck.RemoveAt(0);
                    /* Check current hand doesn't have an ace in it (2 cards), if so, change total -10 */
                    if (_currentPlayer.Hand.Count == 2 && _currentPlayer.Total + c.Value > 21 &&
                        _currentPlayer.Hand.FirstOrDefault(o => o.Value == 1) != null)
                    {
                        _currentPlayer.Total -= 10;
                    }
                    /* Otherwise, above code is ignored, add card to hand - they bust, they bust! */
                    _currentPlayer.AddCard(c);
                    AudioManager.Play(SoundEffectType.Deal, true);
                    Redraw();
                    Thread.Sleep(300);
                    if (_currentPlayer.Total <= 21)
                    {
                        /* End of deal, turn play back to player */
                        AudioManager.PlayVoiceNumeric(_currentPlayer.Total);
                        Thread.Sleep(500);
                        if (_currentPlayer.Total < 21)
                        {
                            /* Current player plays again */
                            _currentPlayer.State = PlayerState.None;
                            _currentPlayer.BeginTurn();
                            return;
                        }
                    }
                    else
                    {
                        /* End of deal - player bust */
                        _currentPlayer.State = PlayerState.Bust;
                        AudioManager.PlayVoice(SoundVoiceType.PlayerBust, true);
                        Thread.Sleep(1000);
                    }
                    break;
            }
            /* Move to next player */
            Thread.Sleep(1000);
            BeginPlayerTurn(player.Index + 1);
        }
        #endregion
    }
}
