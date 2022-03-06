using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy8WPF
{
    internal class CardCollection
    {
        public List<Card> Card { get; set; } = new();


        public CardCollection()
        {

        }


        public void Populate()
        {
            for (int suit = 0; suit < Crazy8WPF.Card.suitsStrings.Length; suit++)
            {
                foreach (int face_value in Enum.GetValues(typeof(Card.Values)))
                {
                    Card.Add(new Card(face_value, suit));
                }
            }
        }

        public void Shuffle()
        {
            List<int> count = new();
            Random rnd = new();

            for (int i = 0; i < Card.Count; i++)
            {
                count.Add(i);
            }
            Card[] shuffle = new Card[count.Count];

            while (count.Count > 0)
            {
                int j = rnd.Next(0, count.Count);
                shuffle[count[j]] = Card.Last();
                Card.RemoveAt(Card.Count - 1);
                count.RemoveAt(j);
            }
            Card.AddRange(shuffle);

        }

    }
}
