using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace MainWindow
{
    internal class HoverCards
    {
        public List<Card> Cards { get; set; }
        public Canvas Canvas { get; set; }

        public HoverCards(HoverCards other)
        {
            Cards = new List<Card>(other.Cards);
            Canvas = other.Canvas; 
        }
        public HoverCards() 
        {
            Cards = new List<Card>();
        }

        public void addCard(Card card)
        {
            Cards.Add(card);
        }

        public void removeCard(Card card)
        {
            Cards.Remove(card);
        }

        public void ToggleHideCanvas(HoverCards hoverCards, bool isEnabled)
        {
            hoverCards.Canvas.IsEnabled = isEnabled;
        }

    }
}
