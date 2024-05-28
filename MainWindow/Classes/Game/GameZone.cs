using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MainWindow
{
    internal class GameZone
    {
        public Canvas Canvas { get; set; }
        public List<Card> Cards { get; set; }
        public int ActualColorCard { get; set; } = 0; // Змінено доступ до set аксесора

        public GameZone gameZone;
        private Random random = new Random();


        public GameZone()
        {
            Cards = new List<Card>();
            gameZone = this;
        }


        //


        public void AddCardToGameZone(Card card, Canvas gameCanvas)
        {
            gameCanvas.Children.Remove(card.Image);

            gameZone.Canvas.Children.Add(card.Image);
            RotateTransform rotateTransform = new RotateTransform(random.Next(-15, 16));
            card.Image.RenderTransform = rotateTransform;
            card.Image.RenderTransformOrigin = new Point(0.5, 0.5);

            Canvas.SetLeft(card.Image, (gameZone.Canvas.ActualWidth - card.Image.Width) / 2 - random.Next(0, 50));
            Canvas.SetTop(card.Image, (gameZone.Canvas.ActualHeight - card.Image.Height) / 2 - random.Next(0, 50));

            gameZone.Cards.Add(card);

            int maxZIndex = gameZone.Canvas.Children.OfType<UIElement>().Select(Panel.GetZIndex).DefaultIfEmpty(0).Max();
            Panel.SetZIndex(card.Image, maxZIndex + 1);
        }

        public void UpdateActualColor()
        {
            if (gameZone.Cards.Last().Color != 5)
            {
                ActualColorCard = gameZone.Cards.Last().Color;
            }
        }

        public void MoveCardsFromGameZoneToDeck(DeckCards deckCards)
        {
            foreach (var card in gameZone.Cards)
            {
                card.Image = null;
            }
            gameZone.Cards = gameZone.Cards.OrderBy(card => random.Next()).ToList();
            deckCards.Cards = new List<Card>(gameZone.Cards);
            gameZone.Cards.Clear();
            gameZone.Canvas.Children.Clear();
        }

    }
}
