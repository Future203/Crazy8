using System;
using System.Windows.Media.Imaging;

namespace Crazy8WPF
{
    class Card : IComparable<Card>
    {
        public static string[] suitsStrings = { "clubs", "spades", "hearts", "diamonds" };
        public static string[] valueStrings = { "", "", "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king", "ace" };

        public enum Values
        {
            Two = 2,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace
        }

        public int Value { get; set; }
        public string Suit { get; set; }
        public bool Valid { get; set; } = false;
        public BitmapImage Image { get; set; }

        public BitmapImage BackImage = new BitmapImage(new Uri("pack://application:,,,/CardImages/blue2.png"));

        public BitmapImage SuitImage { get; set; } 

        public override string ToString()
        {
            return $"{Suit}_{valueStrings[Value]}";
        }

        public int CompareTo(Card compareCard)
        {
            return Value.CompareTo(compareCard.Value);
        }

        public Card(int value, int suit)
        {
            Value = value;
            Suit = suitsStrings[suit];
            Image = new BitmapImage(new Uri($"pack://application:,,,/CardImages/{suitsStrings[suit]}_{valueStrings[value]}.png"));
            SuitImage = new BitmapImage(new Uri($"pack://application:,,,/Suits/{suitsStrings[suit]}.png"));
        }





    }
}
