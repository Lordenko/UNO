using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Input;

namespace MainWindow
{
    internal class DeckCards
    {
        public List<Card> Cards { get; set; }
        public Image CardDeckImage { get; set; }
        public Canvas CardDeckCanvas { get; set; }
        private Random rand = new Random();
        private static readonly int cardWidth = 64;
        private static readonly int cardHeight = 96;
        private Random random = new Random();
        private GameZone gameZone;
        private PlayerManager playerManager;
        private DeckCards deckCards;
        private UiManager uiManager;

        public DeckCards(GameZone gameZone, PlayerManager playerManager, UiManager uiManager)
        {
            Cards = new List<Card>();
            this.gameZone = gameZone;
            this.playerManager = playerManager;
            this.deckCards = this;
            this.uiManager = uiManager;
        }

        public void GenerateCards()
        {
            for (int color = 1; color <= 5; color++)
            {
                if (color >= 1 && color <= 4)
                {
                    for (int duoCard = 1; duoCard <= 2; duoCard++)
                    {
                        for (int defaultCard = 0; defaultCard <= 12; defaultCard++)
                        {
                            Cards.Add(CreateCard(color, defaultCard, rand));
                        }
                    }
                }
                else if (color == 5)
                {
                    for (int soloCard = 1; soloCard <= 4; soloCard++)
                    {
                        for (int uniqueCard = 13; uniqueCard < 15; uniqueCard++)
                        {
                            Cards.Add(CreateCard(color, uniqueCard, rand));
                        }
                    }
                }
            }

            Cards = Cards.OrderBy(card => rand.Next()).ToList();
        }

        private static Card CreateCard(int color, int suit, Random rand)
        {
            double x = 0;
            double y = 0;
            double width = 100;
            double height = 100;

            return new Card(color, suit, x, y, width, height, CropImage(GetCoordinateCard(color, suit)), 10);
        }

        // 1 - red, 2 - yellow, 3 - green, 4 - blue, 5 - black
        // 0 - 9, 10 - block, 11 - skip, 12 - add2, 13 - add4, 14 - change color
        private static Int32Rect GetCoordinateCard(int color, int suit)
        {
            int x = 0, y = 0;

            if (color >= 1 && color <= 4)
            {
                y = (color - 1) * cardHeight;
                x = suit * cardWidth;
            }
            else if (color == 5)
            {
                y = 5 * cardHeight; // Останній рядок
                if (suit == 13) // add 4
                {
                    x = cardWidth * 13;
                }
                else if (suit == 14) // Change Color
                {
                    x = 13 * cardWidth;
                    y = 3 * cardHeight;
                }
            }

            return new Int32Rect(x, y, cardWidth, cardHeight);
        }

        private static CroppedBitmap CropImage(Int32Rect rect)
        {
            string path = "pack://application:,,,/Data/Cards/cards.png";

            BitmapImage sourceImage = new BitmapImage();
            sourceImage.BeginInit();
            sourceImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            sourceImage.EndInit();

            if (rect.X + rect.Width > sourceImage.PixelWidth || rect.Y + rect.Height > sourceImage.PixelHeight)
            {
                throw new ArgumentException("Rect dimensions exceed the bounds of the source image.");
            }

            CroppedBitmap croppedBitmap = new CroppedBitmap(sourceImage, rect);
            return croppedBitmap;
        }

        //////


        public void AddCardToPlayerFromDeck(Player player, DeckCards deckCards, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Debug.WriteLine($"add card to {player.Name}");
                if (deckCards.Cards.Count == 0) return;
                player.HoverCards.addCard(deckCards.Cards.Last());
                deckCards.Cards.RemoveAt(deckCards.Cards.Count - 1);
            }
            SortCards(player);
            UpdateCardPositions(player);
        }
        public void SortCards(Player player)
        {
            player.HoverCards.Cards = player.HoverCards.Cards
                .OrderBy(card => card.Color)
                .ThenBy(card => card.Suit)
                .ToList();
        }
        public void UpdateCardPositions(Player player)
        {
            var canvas = player.HoverCards.Canvas;

            double cardWidth = 40;
            double fieldWidth = canvas.ActualWidth - 50;
            double totalWidth = cardWidth * player.HoverCards.Cards.Count;
            bool overlapWidth = totalWidth > fieldWidth;
            double startX = overlapWidth ? 0 : (fieldWidth - totalWidth) / 2;
            double overlapAmountWidth = overlapWidth ? (totalWidth - fieldWidth) / (player.HoverCards.Cards.Count - 1) : 0;

            double cardHeight = 40;
            double fieldHeight = canvas.ActualHeight - 50;
            double totalHeight = cardHeight * player.HoverCards.Cards.Count;
            bool overlapHeight = totalHeight > fieldHeight;
            double startY1 = overlapHeight ? 0 : (fieldHeight - totalHeight) / 2;
            double overlapAmountHeight = overlapHeight ? (totalHeight - fieldHeight) / (player.HoverCards.Cards.Count - 1) : 0;

            for (int i = 0; i < player.HoverCards.Cards.Count; i++)
            {
                var card = player.HoverCards.Cards[i];
                if (card.Image == null)
                {
                    card.Image = new Image
                    {
                        Source = card.SourseCardImage,
                        Width = card.Width,
                        Height = card.Height,
                        Margin = new Thickness(5)
                    };
                    canvas.Children.Add(card.Image);
                    InteractionCards(card, player);
                }

                double x = 0, y = 0;
                if (player == playerManager.Players[0]) // нижній
                {
                    x = startX - 10 + i * (cardWidth - overlapAmountWidth);
                    y = (canvas.Height - card.Height) / 2 + 5;
                }
                else if (player == playerManager.Players[1]) // лівий
                {
                    x = (canvas.Width - card.Width) / 2 - 13;
                    y = startY1 - 10 + i * (cardHeight - overlapAmountHeight);
                    card.Image.RenderTransform = new RotateTransform(90);
                    card.Image.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                else if (player == playerManager.Players[2]) // верхній
                {
                    x = startX - 10 + i * (cardWidth - overlapAmountWidth);
                    y = (canvas.Height - card.Height) / 2 - 15;
                    card.Image.RenderTransform = new RotateTransform(180);
                    card.Image.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                else if (player == playerManager.Players[3]) // правий
                {
                    x = (canvas.Width - card.Width) / 2 + 7;
                    y = startY1 - 10 + i * (cardHeight - overlapAmountHeight);
                    card.Image.RenderTransform = new RotateTransform(270);
                    card.Image.RenderTransformOrigin = new Point(0.5, 0.5);
                }

                Canvas.SetLeft(card.Image, x);
                Canvas.SetTop(card.Image, y);
                Panel.SetZIndex(card.Image, i);
            }
        }
        public void IfReverseCard(Card card)
        {
            if (card.Suit == 11)
            {
                playerManager.ToggleClockwise();
            }
        }
        public void IfBlockCard(Card card)
        {
            if (card.Suit == 10)
            {
                uiManager.ChangePlayer();
            }
        }
        public void IfAdd2Card(Card card)
        {
            if (card.Suit == 12)
            {
                int tempTurn;
                if (playerManager.Clockwise) tempTurn = playerManager.CheckCorrectTurn(playerManager.Turn + 1);
                else tempTurn = playerManager.CheckCorrectTurn(playerManager.Turn - 1);
                AddCardToPlayerFromDeck(playerManager.Players[tempTurn], deckCards, 2);
                SortCards(playerManager.Players[tempTurn]);
                uiManager.ChangePlayer();
            }
        }
        public void IfAdd4Card(Card card)
        {
            if (card.Suit == 13)
            {
                uiManager.CreateCanvasForChangeColor(card);
                uiManager.shouldAdd4Cards = true;
                uiManager.ChangePlayer();
            }
        }
        public void UpdateActualColor(Card card)
        {
            if (card.Color != 5)
            {
                gameZone.ActualColorCard = card.Color;
            }
        }
        public void IfChangeColorCard(Card card)
        {
            if (card.Suit == 14)
            {
                uiManager.CreateCanvasForChangeColor(card);
            }
        }
        private void InteractionCards(Card card, Player player)
        {
            card.Image.MouseEnter += (sender, e) => Card_MouseEnter(sender, e, player);
            card.Image.MouseLeave += (sender, e) => Card_MouseLeave(sender, e, player);
            card.Image.MouseLeftButtonDown += Card_MouseLeftButtonDown;
        }
        private void Card_MouseEnter(object sender, MouseEventArgs e, Player player)
        {
            Image cardImage = sender as Image;
            if (cardImage != null && player.HoverCards.Canvas.Children.Contains(cardImage))
            {
                if (player == playerManager.Players[0])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.1, 1.1);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentTop = Canvas.GetTop(cardImage);
                    Canvas.SetTop(cardImage, currentTop - 23);
                }
                else if (player == playerManager.Players[1])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.1, 1.1);
                    cardImage.RenderTransform = new RotateTransform(90);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentLeft = Canvas.GetLeft(cardImage);
                    Canvas.SetLeft(cardImage, currentLeft + 23);
                }
                else if (player == playerManager.Players[2])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.1, 1.1);
                    cardImage.RenderTransform = new RotateTransform(180);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentTop = Canvas.GetTop(cardImage);
                    Canvas.SetTop(cardImage, currentTop + 27);
                }
                else if (player == playerManager.Players[3])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.1, 1.1);
                    cardImage.RenderTransform = new RotateTransform(270);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentLeft = Canvas.GetLeft(cardImage);
                    Canvas.SetLeft(cardImage, currentLeft - 27);
                }
            }
        }
        private void Card_MouseLeave(object sender, MouseEventArgs e, Player player)
        {
            Image cardImage = sender as Image;
            if (cardImage != null && player.HoverCards.Canvas.Children.Contains(cardImage))
            {
                if (player == playerManager.Players[0])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.0, 1.0);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentTop = Canvas.GetTop(cardImage);
                    Canvas.SetTop(cardImage, currentTop + 23);
                }
                else if (player == playerManager.Players[1])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.0, 1.0);
                    cardImage.RenderTransform = new RotateTransform(90);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentLeft = Canvas.GetLeft(cardImage);
                    Canvas.SetLeft(cardImage, currentLeft - 23);
                }
                else if (player == playerManager.Players[2])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.0, 1.0);
                    cardImage.RenderTransform = new RotateTransform(180);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentTop = Canvas.GetTop(cardImage);
                    Canvas.SetTop(cardImage, currentTop - 27);
                }
                else if (player == playerManager.Players[3])
                {
                    cardImage.RenderTransform = new ScaleTransform(1.0, 1.0);
                    cardImage.RenderTransform = new RotateTransform(270);
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);
                    double currentLeft = Canvas.GetLeft(cardImage);
                    Canvas.SetLeft(cardImage, currentLeft + 27);
                }
            }
        }
        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image cardImage = sender as Image;
            if (cardImage != null)
            {
                Card card = playerManager.CurrentPlayer.HoverCards.Cards.First(c => c.Image == cardImage);
                Card lastCardInZona = gameZone.Cards.LastOrDefault();

                bool canMoveCard = card != null &&
                                   (gameZone.Cards.Count == 0 ||
                                   (card.Color == lastCardInZona.Color || card.Color == 5) ||
                                   (card.Color == gameZone.ActualColorCard) ||
                                   (card.Suit == lastCardInZona.Suit || card.Suit == 13 || card.Suit == 14));

                if (canMoveCard)
                {
                    double previousTop = Canvas.GetTop(cardImage);

                    if (LastThrowCard(cardImage, previousTop))
                    {
                        uiManager.ChangePlayer();
                        uiManager.ToggleInteractivityForPlayers();
                        return;
                    }

                    // Видаляємо карту з HoverCards.Canvas
                    playerManager.CurrentPlayer.HoverCards.Canvas.Children.Remove(cardImage);

                    // Встановлюємо випадкове обертання для картки
                    gameZone.Canvas.Children.Add(cardImage);
                    RotateTransform rotateTransform = new RotateTransform(random.Next(-15, 16));
                    cardImage.RenderTransform = rotateTransform;
                    cardImage.RenderTransformOrigin = new Point(0.5, 0.5);

                    Canvas.SetLeft(cardImage, (gameZone.Canvas.ActualWidth - cardImage.Width) / 2 - random.Next(0, 50));
                    Canvas.SetTop(cardImage, (gameZone.Canvas.ActualHeight - cardImage.Height) / 2 - random.Next(0, 50));

                    // Додаємо карту в GameZone
                    gameZone.Cards.Add(card);
                    playerManager.CurrentPlayer.HoverCards.removeCard(card);

                    // Оновлюємо позиції карток у ховері
                    UpdateCardPositions(playerManager.CurrentPlayer);

                    // Встановлюємо zIndex на максимальне значення
                    int maxZIndex = gameZone.Canvas.Children.OfType<UIElement>().Select(Panel.GetZIndex).DefaultIfEmpty(0).Max();
                    Panel.SetZIndex(cardImage, maxZIndex + 1);

                    // Перевірка перемоги
                    if (uiManager.CheckWin())
                    {
                        IfChangeColorCard(card);
                        IfReverseCard(card);
                        IfBlockCard(card);
                        IfAdd2Card(card);
                        IfAdd4Card(card);
                        UpdateActualColor(card);

                        uiManager.ChangePlayer();

                        if (card.Suit != 13 && card.Suit != 14)
                        {
                            LastThrowCard(cardImage, previousTop);
                            uiManager.IfCountCardsOne(playerManager.CurrentPlayer);
                            uiManager.ToggleInteractivityForPlayers();
                        }

                        UpdateCardPositions(playerManager.CurrentPlayer);
                    }

                    Debug.WriteLine("\n---\n\nКарти в ховері:\n");
                    foreach (var card1 in playerManager.CurrentPlayer.HoverCards.Cards)
                    {
                        Debug.WriteLine($"Color: {card1.Color}, Suit: {card1.Suit}, Image Coords: ({card1.X}, {card1.Y}), Size: ({card1.Width}x{card1.Height}), z-Index: {card1.zIndex}");
                    }

                    Debug.WriteLine("\nКарти в зоні:\n");
                    foreach (var card2 in gameZone.Cards)
                    {
                        Debug.WriteLine($"Color: {card2.Color}, Suit: {card2.Suit}, Image Coords: ({card2.X}, {card2.Y}), Size: ({card2.Width}x{card2.Height}), z-Index: {card2.zIndex}");
                    }

                    Debug.WriteLine("\nКарти в колоді:\n");
                    foreach (var card3 in deckCards.Cards)
                    {
                        Debug.WriteLine($"Color: {card3.Color}, Suit: {card3.Suit}, Image Coords: ({card3.X}, {card3.Y}), Size: ({card3.Width}x{card3.Height}), z-Index: {card3.zIndex}");
                    }

                    Debug.WriteLine($"\nActualColor - {gameZone.ActualColorCard}");
                    Debug.WriteLine($"TurnItPlayer - {playerManager.Turn}\n");
                }
            }
        }

        private bool LastThrowCard(Image cardImage, double previousTop)
        {
            if (playerManager.CurrentPlayer.HoverCards.Cards.Count == 1 && !uiManager.UnoButtonClicked)
            {
                Debug.WriteLine("Додано 2 карти");
                AddCardToPlayerFromDeck(playerManager.CurrentPlayer, deckCards, 2);
                uiManager.DeleteUnoCard();
                ReturnCardPosition(cardImage, previousTop);
                return true;
            }
            return false;
        }

        private void ReturnCardPosition(Image cardImage, double previousTop)
        {
            Canvas.SetTop(cardImage, previousTop);
        }

    }
}