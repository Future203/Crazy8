using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace Crazy8WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CardCollection Deck = new();
        CardCollection PlayStack = new();
        CardCollection DrawPile = new();
        CardCollection TurnStack = new();
        List<CardCollection> Player = new();
        Random rnd = new Random();
        List<bool> ValidCardInHand = new() { false, false };
        string DeclaredSuit = "";
        bool IsPlayerTurn = true;


        public MainWindow()
        {
            InitializeComponent();
            Reset();
        }

        void Reset()
        {
            Deck.Card.Clear();
            Player.Clear();
            PlayStack.Card.Clear();
            DrawPile.Card.Clear();
            HideSuitButtons();
            HideSuitElement();

            Deck.Populate();
            Deck.Shuffle();
            Deal();

            UpdateUI();
        }

        void UpdateUI()
        {
            UpdatePlayArea();
            UpdateComputerHandPanel();
            CheckCardValidity();
            UpdatePlayerHandPanel();
            EvaluateWinCondition();
        }

        void EvaluateWinCondition()
        {
            if(Player[0].Card.Count == 0 && TurnStack.Card.Count == 0)
            {
                MessageBox.Show("You win!");
                Reset();
            }
            else if (Player[1].Card.Count == 0 && TurnStack.Card.Count == 0)
            {
                MessageBox.Show("You Lose!");
                Reset();
            }

            if(DrawPile.Card.Count == 1)
            {
                DrawPile.Card.AddRange(PlayStack.Card.GetRange(1, PlayStack.Card.Count - 1));
                PlayStack.Card.RemoveRange(1, PlayStack.Card.Count - 1);
                DrawPile.Shuffle();
            }
        }

        void UpdateComputerHandPanel()
        {
            ComputerHandPanel.Children.Clear();
            for (int i = 0; i < Player[1].Card.Count; i++)
            {
                Image image = new()
                {
                    Source = Player[1].Card[i].BackImage,
                    Height = 160,
                    Width = 112,
                    Margin = new Thickness(10, 10, -85, 0),
                    Name = $"Card{i}",
                };
                ComputerHandPanel.Children.Add(image);
            }
        }

        void UpdatePlayerHandPanel()
        {

            PlayerHandPanel.Children.Clear();
            Player[0].Card.Sort();
            for (int i = 0; i < Player[0].Card.Count; i++)
            {
                WrapPanel cardWrap = new()
                {
                    Height = 160,
                    Width = 112,
                    Margin = new Thickness(10, 10, -85, 0),
                };
                cardWrap.Name = $"CardWrap{i}";
                Image blank = new()
                {
                    Source = new BitmapImage(new Uri($"pack://application:,,,/CardImages/blank.png")),
                    Height = 160,
                    Width = 112,
                    Margin = new Thickness(0,0,-112,0),
                    Name = $"Blank{i}",
                };
                cardWrap.Children.Add(blank);
                Brush opac = new SolidColorBrush(Colors.White)
                {
                    Opacity = 0.3
                };
                Image image = new()
                {
                    Source = Player[0].Card[i].Image,
                    Height = 160,
                    Width = 112,
                    Margin = new Thickness(0, 0, -112, 0),
                    Name = $"Card{i}",
                    Tag = i,
                };
                if (!Player[0].Card[i].Valid)
                {
                    if(chkHints.IsChecked.Value) image.OpacityMask = opac;
                }
                else
                {
                    image.MouseDown += new MouseButtonEventHandler(card_Click);
                }
                cardWrap.Children.Add(image);
                PlayerHandPanel.Children.Add(cardWrap);
            }
        }

        void UpdatePlayArea()
        {
            PlayStackElement.Source = PlayStack.Card[0].Image;
            DrawPileElement.Source = DrawPile.Card[0].BackImage;
            if (PlayArea.Children.Count > 2) PlayArea.Children.RemoveRange(2, PlayArea.Children.Count - 2);
            for (int i = 0; i < TurnStack.Card.Count; i++)
            {
                Image image = new()
                {
                    Source = TurnStack.Card[i].Image,
                    Height = 160,
                    Width = 112,
                    Margin = new Thickness(0, 0, -85, 0),
                    Name = $"TurnStackCard{i}",
                };
                PlayArea.Children.Add(image);
            }
        }

        void AddToTurnStack(int index, int player)
        {
            TurnStack.Card.Add(Player[player].Card[index]);
            Player[player].Card.RemoveAt(index);
            UpdateUI();
        }

        void PlayStart()
        {
            if (TurnStack.Card.Count == 0) return;
            if (TurnStack.Card[0].Value == 8)
            {
                btnPlay.IsEnabled = false;
                ShowSuitButtons();
            }
            else
            {
                SuitElementPanel.Visibility = Visibility.Hidden;
                Play();
            }
        }

        void Play()
        {
                TurnStack.Card.Reverse();
                PlayStack.Card.InsertRange(0, TurnStack.Card);
                TurnStack.Card.Clear();
                UpdateUI();
            if (IsPlayerTurn)
            {
                IsPlayerTurn = false;
                btnPlay.IsEnabled = false;
                PlayAI();
            }
            else
            {
                IsPlayerTurn = true;
                btnPlay.IsEnabled=true;
            }
        }

        void PlayAI()
        {
            Delay();
            if (TurnStack.Card.Count == 0 && !ValidCardInHand[1])
            {
                Draw(1);
                PlayAI();
                return;
            }
            Player[1].Shuffle();
            for(int i = 0; i < Player[1].Card.Count; i++)
            {
                if (Player[1].Card[i].Valid)
                {
                    AddToTurnStack(i, 1);
                    PlayAI();
                    return;
                }
            }
            Delay();
            if (TurnStack.Card[0].Value == 8)
            {
                if (Player[1].Card.Count > 0)
                {
                    DeclaredSuit = Player[1].Card[0].Suit;
                }
                else
                {
                    DeclareSuit(rnd.Next(0, 4));
                }
                Play();
                ShowSuitElement();
                return;
            }
            else
            {
                Delay();
                SuitElementPanel.Visibility = Visibility.Hidden;
                Play();
            }
        }

        void Delay()
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();
            Thread.Sleep(300);
        }

        void CheckCardValidity()
        {
            ValidCardInHand[0] = false;
            ValidCardInHand[1] = false;

            for (int j = 0; j < Player.Count; j++)
            {
                for (int i = 0; i < Player[j].Card.Count; i++)
                {
                    if (TurnStack.Card.Count == 0)
                    {
                        if (Player[j].Card[i].Value == PlayStack.Card[0].Value ||
                            (Player[j].Card[i].Suit == PlayStack.Card[0].Suit && PlayStack.Card[0].Value != 8) ||
                            Player[j].Card[i].Value == 11 ||
                            Player[j].Card[i].Value == 8 ||
                            (Player[j].Card[i].Suit == DeclaredSuit && PlayStack.Card[0].Value == 8))
                        {
                            Player[j].Card[i].Valid = true;
                            ValidCardInHand[j] = true;
                        }
                        else
                        {
                            Player[j].Card[i].Valid = false;
                        }
                    }
                    else
                    {
                        if (Player[j].Card[i].Value == TurnStack.Card[0].Value)
                        {
                            Player[j].Card[i].Valid = true;
                        }
                        else
                        {
                            Player[j].Card[i].Valid = false;
                        }
                    }
                }
            }
        }

        void Deal()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    while (Player.Count < 2) Player.Add(new CardCollection());
                    Player[j].Card.Add(Deck.Card[0]);
                    Deck.Card.RemoveAt(0);
                }
            }
            PlayStack.Card.Add(Deck.Card[0]);
            Deck.Card.RemoveAt(0);
            DrawPile.Card.AddRange(Deck.Card);
            Deck.Card.Clear();
            if (PlayStack.Card[0].Value == 8)
            {
                DeclaredSuit = PlayStack.Card[0].Suit;
                SuitElement.Source = PlayStack.Card[0].SuitImage;
                ShowSuitElement();
            }
        }

        void Draw(int player)
        {
            if (TurnStack.Card.Count > 0) return;
            if (ValidCardInHand[player])
            {
                MessageBox.Show("You still have valid cards in your hand, please play them before attempting to draw more");
            }
            else
            {
                Player[player].Card.Add(DrawPile.Card[0]);
                DrawPile.Card.RemoveAt(0);
            }
            UpdateUI();
        }

        void DeclareSuit(int suit)
        {
            DeclaredSuit = Card.suitsStrings[suit];
        }

        void HideSuitButtons()
        {
            SuitButtons.Visibility = Visibility.Hidden;
        }

        void ShowSuitButtons()
        {
            SuitButtons.Visibility = Visibility.Visible;
        }

        void HideSuitElement()
        {
            SuitElementPanel.Visibility = Visibility.Hidden;
        }

        void ShowSuitElement()
        {
            if (DeclaredSuit != "")
            {
                SuitElement.Source = new BitmapImage(new Uri($"pack://application:,,,/Suits/{DeclaredSuit}.png"));
                SuitElementPanel.Visibility = Visibility.Visible;
            }
            HideSuitButtons();
            btnPlay.IsEnabled = true;
        }

        void card_Click(object sender, MouseEventArgs e)
        {
            if (IsPlayerTurn)
            {
                Image? image = sender as Image;
                AddToTurnStack((int)image.Tag, 0);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayStart();
        }


        private void DrawPileElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Draw(0);
        }

        private void ClubsElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeclareSuit(Convert.ToInt32(ClubsElement.Tag));
            ShowSuitElement();
            Play();
        }

        private void DiamondsElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeclareSuit(Convert.ToInt32(DiamondsElement.Tag));
            ShowSuitElement();
            Play();
        }

        private void SpadesElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeclareSuit(Convert.ToInt32(SpadesElement.Tag));
            ShowSuitElement();
            Play();
        }

        private void HeartsElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeclareSuit(Convert.ToInt32(HeartsElement.Tag));
            ShowSuitElement();
            Play();
        }

        private void chkHints_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }
    }
}
