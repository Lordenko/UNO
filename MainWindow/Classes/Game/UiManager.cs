using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace MainWindow
{
    internal class UiManager
    {
        public Canvas gameCanvas { get; set; }
        public DeckCards deckCards;
        public int gameMode { get; set; }
        public List<Border> HelpBorderForCanvas { get; set; }
        public bool UnoButtonClicked => unoButtonClicked;
        private Frame frame;

        private int countCardInStart = 7;
        private PlayerManager playerManager;
        private GameZone gameZone;
        private Canvas changeColorCanvas;
        private Image unoCardImage;
        internal string PathForUnoDeckCardImage = "pack://application:,,,/Data/Cards/backCard.png";
        internal string PathForUnoCardImage = "pack://application:,,,/Data/Cards/UNO_Logo.png";
        internal string PathForBackGround = "pack://application:,,,/Data/gai109.jpg";
        private bool unoButtonClicked = false;
        public bool shouldAdd4Cards = false;
        private Canvas Exit;
        private Border exitBorder;
        Canvas blurCanvas;
        Border shadowBorder;

        public UiManager(Frame frame, Canvas gameCanvas, Player player1, Player player2, Player player3, Player player4, int gameMode)
        {
            this.frame = frame;
            this.gameCanvas = gameCanvas;
            this.gameMode = gameMode;

            HelpBorderForCanvas = new List<Border>()
            {
                new Border(), new Border(), new Border(), new Border()
            };

            Image backgroundImage = new Image()
            {
                Width = 1600,
                Height = 900,
                Source = new BitmapImage(new Uri(PathForBackGround)),
                Stretch = Stretch.UniformToFill,
            };
            BlurEffect blurEffect = new BlurEffect { Radius = 15 };
            backgroundImage.Effect = blurEffect;
            gameCanvas.Children.Add(backgroundImage);
            Canvas.SetLeft(backgroundImage, -10);
            Canvas.SetTop(backgroundImage, -19);

            playerManager = new PlayerManager(this, player1, player2, player3, player4);
            this.gameZone = new GameZone();
            this.deckCards = new DeckCards(gameZone, playerManager, this);

            Initial();
            ToggleInteractivityForPlayers();



            player1.HoverCards.Canvas.SizeChanged += (s, e) => deckCards.UpdateCardPositions(player1);
            player2.HoverCards.Canvas.SizeChanged += (s, e) => deckCards.UpdateCardPositions(player2);
            player3.HoverCards.Canvas.SizeChanged += (s, e) => deckCards.UpdateCardPositions(player3);
            player4.HoverCards.Canvas.SizeChanged += (s, e) => deckCards.UpdateCardPositions(player4);


        }

        private void Initial()
        {

            InitializePlayerCanvas(HelpBorderForCanvas[0], playerManager.Players[0], 565, 670, 400, 150);
            InitializePlayerCanvas(HelpBorderForCanvas[1], playerManager.Players[1], 50, 225, 150, 400);
            InitializePlayerCanvas(HelpBorderForCanvas[2], playerManager.Players[2], 565, 25, 400, 150);
            InitializePlayerCanvas(HelpBorderForCanvas[3], playerManager.Players[3], 1340, 225, 150, 400);


            // Створення DropShadowEffect
            DropShadowEffect shadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 20,
                ShadowDepth = 0, // Нульове зміщення, щоб тінь була з усіх сторін
                Opacity = 0.5
            };

            // Створення зовнішнього Border для тіні
            Border shadowBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Colors.Transparent),
                Width = 800,
                Height = 400,
                Effect = shadowEffect // Застосування ефекту тіні
            };

            // Створення внутрішнього Border для вмісту
            Border GameZoneCanvasBorder = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Colors.Gray),
                Width = 800,
                Height = 400,
                ClipToBounds = true
            };

            // Додавання Canvas до внутрішнього Border
            gameZone.Canvas = new Canvas()
            {
                Width = 800,
                Height = 400
            };

            GameZoneCanvasBorder.Child = gameZone.Canvas;

            // Додавання внутрішнього Border до зовнішнього
            shadowBorder.Child = GameZoneCanvasBorder;

            // Додавання зовнішнього Border до Canvas
            Canvas.SetLeft(shadowBorder, 375);
            Canvas.SetTop(shadowBorder, 225);
            gameCanvas.Children.Add(shadowBorder);

            InitializeDeckCardCanvas();

            deckCards.GenerateCards();

            foreach (var player in playerManager.Players)
            {
                deckCards.AddCardToPlayerFromDeck(player, deckCards, countCardInStart);
            }

            deckCards.CardDeckCanvas.MouseLeftButtonDown += DeckCards_MouseLeftButtonDown;

            /////////////////////////

            // Стиль для кнопок з прозорістю
            Style buttonStyle = new Style(typeof(Border));
            buttonStyle.Setters.Add(new Setter(Border.OpacityProperty, 0.5));
            buttonStyle.Setters.Add(new Setter(Border.EffectProperty, new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 10,
                ShadowDepth = 0,
                Opacity = 0.5
            }));
            Trigger mouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            mouseOverTrigger.Setters.Add(new Setter(Border.OpacityProperty, 1.0));
            buttonStyle.Triggers.Add(mouseOverTrigger);


            exitBorder = new Border
            {
                Width = 40,
                Height = 40,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Exit = new Canvas();
            exitBorder.Child = Exit;
            TextBlock textExit = new TextBlock()
            {
                Text = "x",
                FontSize = 24,
                Width = exitBorder.Width,
                Height = exitBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(exitBorder, (gameZone.Canvas.Width - exitBorder.Width) - 10);
            Canvas.SetTop(exitBorder, 10);
            Canvas.SetTop(textExit, 1);
            Exit.Children.Add(textExit);
            gameZone.Canvas.Children.Add(exitBorder);

            Exit.MouseLeftButtonDown += Exit_MouseLeftButtonDown;
        }
        private void InitializePlayerCanvas(Border border, Player player, double left, double top, double width, double height)
        {


            // Створення DropShadowEffect
            DropShadowEffect shadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 20,
                ShadowDepth = 0, // Нульове зміщення, щоб тінь була з усіх сторін
                Opacity = 0.5
            };

            // Створення зовнішнього Border для тіні
            Border shadowBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Colors.Transparent),
                Width = width,
                Height = height,
                Effect = shadowEffect // Застосування ефекту тіні
            };

            // Створення внутрішнього Border для вмісту
            border = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Colors.Gray),
                Width = width,
                Height = height,
                ClipToBounds = true,
                Opacity = 0.8
            };

            // Створення Canvas для гравця
            player.HoverCards.Canvas = new Canvas()
            {
                Width = width,
                Height = height
            };

            // Додавання Canvas до внутрішнього Border
            border.Child = player.HoverCards.Canvas;

            // Додавання внутрішнього Border до зовнішнього
            shadowBorder.Child = border;

            // Додавання зовнішнього Border до Canvas
            Canvas.SetLeft(shadowBorder, left);
            Canvas.SetTop(shadowBorder, top);
            gameCanvas.Children.Add(shadowBorder);



            int playerIndex = Array.IndexOf(playerManager.Players.ToArray(), player);
            if (playerIndex >= 0 && playerIndex < HelpBorderForCanvas.Count)
            {
                HelpBorderForCanvas[playerIndex] = border;
            }
        }
        public void ChangePlayer()
        {
            playerManager.ChangePlayer();
        }
        public bool CheckWin()
        {
            if (playerManager.CurrentPlayer.HoverCards.Cards.Count == 0 && unoButtonClicked)
            {
                string message = playerManager.Turn == 0 ? "Ви виграли!" : $"Гравець {playerManager.Turn} виграв!";
                MessageBoxResult result = MessageBox.Show(message, "Кінець гри!", MessageBoxButton.OK);

                if (result == MessageBoxResult.OK)
                {
                    frame.Navigate(new Hub(frame)); // Навігація на іншу сторінку, змініть на потрібну сторінку
                }   

                return false;
            }
            return true;
        }

        public void DeleteUnoCard()
        {
            if (unoCardImage != null)
            {
                gameCanvas.Children.Remove(unoCardImage);
                unoButtonClicked = true;
            }
        }
        public void UpdateActualColor()
        {
            if (gameZone.Cards.Last().Color != 5)
            {
                gameZone.ActualColorCard = gameZone.Cards.Last().Color;
            }
        }
        public void IfCountCardsOne(Player player)
        {
            if (player.HoverCards.Cards.Count == 1 && HasPlayableCard(player))
            {
                unoCardImage = new Image()
                {
                    Source = new BitmapImage(new Uri(PathForUnoCardImage, UriKind.RelativeOrAbsolute)),
                    Width = 100,
                    Height = 100
                };
                gameCanvas.Children.Add(unoCardImage);
                Canvas.SetLeft(unoCardImage, 1000);
                Canvas.SetTop(unoCardImage, 500);
                unoButtonClicked = false;

                unoCardImage.MouseLeftButtonDown += UnoCard_MouseLeftButtonDown;
            }
        }
        private bool HasPlayableCard(Player player)
        {
            Card lastCardInZona = gameZone.Cards.LastOrDefault();
            int actualColor = gameZone.ActualColorCard;

            return player.HoverCards.Cards.Any(card =>
                card.Color == lastCardInZona.Color ||
                card.Color == actualColor ||
                card.Color == 5 ||
                card.Suit == lastCardInZona.Suit ||
                card.Suit == 13 ||
                card.Suit == 14);
        }
        public void ToggleInteractivityForPlayers()
        {
            playerManager.ToggleInteractivityForPlayers();
        }
        private void UnoCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeleteUnoCard();
            unoButtonClicked = true;
        }
        private void DeckCards_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            deckCards.AddCardToPlayerFromDeck(playerManager.CurrentPlayer, deckCards, 1);

            if (!unoButtonClicked)
            {
                DeleteUnoCard();
                unoButtonClicked = true;
            }
        }
        public void CreateCanvasForChangeColor(Card card)
        {
            ToggleInteractivityForChangeColor(false);

            changeColorCanvas = new Canvas()
            {
                Width = 200,
                Height = 200,
                Background = new SolidColorBrush(Colors.Black),
            };

            gameZone.Canvas.Children.Add(changeColorCanvas);

            double centerX = (gameZone.Canvas.ActualWidth - changeColorCanvas.Width) / 2;
            double centerY = (gameZone.Canvas.ActualHeight - changeColorCanvas.Height) / 2;

            Canvas.SetLeft(changeColorCanvas, centerX);
            Canvas.SetTop(changeColorCanvas, centerY);

            int maxZIndex = gameZone.Canvas.Children.OfType<UIElement>().Select(Panel.GetZIndex).DefaultIfEmpty(0).Max();
            Panel.SetZIndex(changeColorCanvas, maxZIndex + 1);

            AddColorCanvas("red", Colors.Red, 0, 0);
            AddColorCanvas("yellow", Colors.Yellow, changeColorCanvas.Width / 2, 0);
            AddColorCanvas("green", Colors.Green, 0, changeColorCanvas.Height / 2);
            AddColorCanvas("blue", Colors.Blue, changeColorCanvas.Width / 2, changeColorCanvas.Height / 2);
        }
        private void AddColorCanvas(string name, Color color, double left, double top)
        {
            Canvas colorCanvas = new Canvas()
            {
                Width = changeColorCanvas.Width / 2,
                Height = changeColorCanvas.Height / 2,
                Background = new SolidColorBrush(color),
                Name = name
            };
            changeColorCanvas.Children.Add(colorCanvas);
            Canvas.SetLeft(colorCanvas, left);
            Canvas.SetTop(colorCanvas, top);

            colorCanvas.MouseLeftButtonDown += ColorCanvas_MouseLeftButtonDown;
            colorCanvas.MouseEnter += ColorCanvas_MouseEnter;
            colorCanvas.MouseLeave += ColorCanvas_MouseLeave;
        }
        private void ToggleInteractivityForChangeColor(bool isEnabled)
        {
            deckCards.CardDeckCanvas.IsEnabled = isEnabled;

            foreach (var card in playerManager.CurrentPlayer.HoverCards.Cards)
            {
                if (card.Image != null)
                {
                    card.Image.IsEnabled = isEnabled;
                }
            }
        }

        private void ToggleInteractivityForSureMenu(bool isEnabled)
        {
            deckCards.CardDeckCanvas.IsEnabled = isEnabled;
            exitBorder.IsEnabled = isEnabled;
            foreach (var card in playerManager.CurrentPlayer.HoverCards.Cards)
            {
                if (card.Image != null)
                {
                    card.Image.IsEnabled = isEnabled;
                }
            }
        }
        private void ColorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas clickedCanvas = sender as Canvas;
            if (clickedCanvas != null)
            {
                switch (clickedCanvas.Name)
                {
                    case "red":
                        gameZone.ActualColorCard = 1;
                        break;
                    case "yellow":
                        gameZone.ActualColorCard = 2;
                        break;
                    case "green":
                        gameZone.ActualColorCard = 3;
                        break;
                    case "blue":
                        gameZone.ActualColorCard = 4;
                        break;
                }

                gameZone.Canvas.Children.Remove(changeColorCanvas);

                ToggleInteractivityForChangeColor(true);

                if (shouldAdd4Cards)
                {
                    int tempTurn;
                    if (playerManager.Clockwise)
                    {
                        tempTurn = playerManager.CheckCorrectTurn(playerManager.Turn - 1);
                    }
                    else
                    {
                        tempTurn = playerManager.CheckCorrectTurn(playerManager.Turn + 1);
                    }
                    deckCards.AddCardToPlayerFromDeck(playerManager.Players[tempTurn], deckCards, 4);
                    deckCards.SortCards(playerManager.Players[tempTurn]);
                    shouldAdd4Cards = false;
                }

                IfCountCardsOne(playerManager.CurrentPlayer);
                playerManager.ToggleInteractivityForPlayers();
            }
        }
        private void ColorCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Canvas hoverCanvas = sender as Canvas;
            if (hoverCanvas != null)
            {
                int maxZIndex = changeColorCanvas.Children.OfType<UIElement>().Select(Panel.GetZIndex).DefaultIfEmpty(0).Max();
                Panel.SetZIndex(hoverCanvas, maxZIndex + 1);
                hoverCanvas.RenderTransform = new ScaleTransform(1.4, 1.4);
                hoverCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }
        private void ColorCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Canvas hoverCanvas = sender as Canvas;
            if (hoverCanvas != null)
            {
                hoverCanvas.RenderTransform = new ScaleTransform(1.0, 1.0);
                hoverCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }
        public void ChangeColorBorder(Border border, Color color)
        {
            border.Background = new SolidColorBrush(color);
        }
        public void ChangeOpacityBorder(Border border, double opacity)
        {
            border.Opacity = opacity;
        }
        private void InitializeDeckCardCanvas()
        {


            deckCards.CardDeckCanvas = new Canvas()
            {
                Width = 300,
                Height = 300,

            };
            gameZone.Canvas.Children.Add(deckCards.CardDeckCanvas);


            List<Image> imageDeckCard = new List<Image>() { deckCards.CardDeckImage, deckCards.CardDeckImage, deckCards.CardDeckImage };

            // Визначаємо стиль для Image з тригерами
            // Визначаємо стиль для Canvas з тригерами
            Style canvasStyle = new Style(typeof(Canvas));
            canvasStyle.Setters.Add(new Setter(Canvas.OpacityProperty, 0.5));
            Trigger mouseOverTriggerForCanvas = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            mouseOverTriggerForCanvas.Setters.Add(new Setter(Canvas.OpacityProperty, 1.0));
            canvasStyle.Triggers.Add(mouseOverTriggerForCanvas);

            // Створюємо Canvas для зберігання карт
            Canvas cardsCanvas = new Canvas
            {
                Width = 200, // Задайте відповідну ширину
                Height = 200, // Задайте відповідну висоту
                Style = canvasStyle
            };

            // Створюємо масив Image з використанням визначеного стилю для карт

            imageDeckCard[0] = new Image()
            {
                Source = new BitmapImage(new Uri(PathForUnoDeckCardImage, UriKind.RelativeOrAbsolute)),
                Width = 100,
                Height = 100
            };
            cardsCanvas.Children.Add(imageDeckCard[0]);
            Canvas.SetTop(imageDeckCard[0], 30);

            imageDeckCard[1] = new Image()
            {
                Source = new BitmapImage(new Uri(PathForUnoDeckCardImage, UriKind.RelativeOrAbsolute)),
                Width = 100,
                Height = 100
            };
            cardsCanvas.Children.Add(imageDeckCard[1]);
            Canvas.SetTop(imageDeckCard[1], 25);
            Canvas.SetLeft(imageDeckCard[1], 5);

            imageDeckCard[2] = new Image()
            {
                Source = new BitmapImage(new Uri(PathForUnoDeckCardImage, UriKind.RelativeOrAbsolute)),
                Width = 100,
                Height = 100
            };
            cardsCanvas.Children.Add(imageDeckCard[2]);
            Canvas.SetTop(imageDeckCard[2], 20);
            Canvas.SetLeft(imageDeckCard[2], 10);

            // Додаємо cardsCanvas до основного Canvas або іншого контейнера
            deckCards.CardDeckCanvas.Children.Add(cardsCanvas);
        }
        public void CreateCanvasForSureBackToMenu()
        {
            ToggleInteractivityForSureMenu(false);

            // Створення блюр ефекту
            blurCanvas = new Canvas()
            {
                Width = 1600,
                Height = 900,
                Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))
            };
            BlurEffect blurEffect = new BlurEffect { Radius = 25 };
            blurCanvas.Effect = blurEffect;

            DropShadowEffect shadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 20,
                ShadowDepth = 0, // Нульове зміщення, щоб тінь була з усіх сторін
                Opacity = 0.5
            };

            // Створення зовнішнього Border для тіні
            shadowBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Colors.Transparent),
                Width = 400,
                Height = 200,
                Effect = shadowEffect // Застосування ефекту тіні
            };

            // Створення внутрішнього Border для вмісту
            Border borderSureBlock = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Colors.LightGray),
                Width = 400,
                Height = 200,
                ClipToBounds = true,
                Opacity = 0.8
            };

            Canvas canvasSureBlock = new Canvas()
            {
                Width = 400,
                Height = 200
            };

            // Додавання Canvas до внутрішнього Border
            borderSureBlock.Child = canvasSureBlock;

            // Додавання внутрішнього Border до зовнішнього
            shadowBorder.Child = borderSureBlock;


            TextBlock textSure1 = new TextBlock()
            {
                Text = "Повернутися до меню?",
                FontSize = 30,
                Width = 350,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
            };
            canvasSureBlock.Children.Add(textSure1);
            Canvas.SetLeft(textSure1, (canvasSureBlock.Width - textSure1.Width) / 2);
            Canvas.SetTop(textSure1, 30);

            TextBlock textSure2 = new TextBlock()
            {
                Text = "Ваша гра не буде збережена",
                FontSize = 16,
                Width = 250,
                TextAlignment = TextAlignment.Center,
            };
            canvasSureBlock.Children.Add(textSure2);
            Canvas.SetLeft(textSure2, (canvasSureBlock.Width - textSure2.Width) / 2);
            Canvas.SetTop(textSure2, 70);

            // Додавання зовнішнього Border до Canvas
            Canvas.SetTop(shadowBorder, (900 - shadowBorder.Height) / 2 - 20);
            Canvas.SetLeft(shadowBorder, 565);
            Panel.SetZIndex(blurCanvas, 1);
            Panel.SetZIndex(shadowBorder, 2);
            gameCanvas.Children.Add(blurCanvas);
            gameCanvas.Children.Add(shadowBorder);


            /////

            // Стиль для кнопок з прозорістю
            Style buttonStyle = new Style(typeof(Border));
            buttonStyle.Setters.Add(new Setter(Border.OpacityProperty, 0.5));
            buttonStyle.Setters.Add(new Setter(Border.EffectProperty, new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 10,
                ShadowDepth = 0,
                Opacity = 0.5
            }));
            Trigger mouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            mouseOverTrigger.Setters.Add(new Setter(Border.OpacityProperty, 1.0));
            buttonStyle.Triggers.Add(mouseOverTrigger);

            // Canvas з Border для Play
            Border ReturnBorder = new Border
            {
                Width = 160,
                Height = 40,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Canvas Return = new Canvas();
            ReturnBorder.Child = Return;
            TextBlock textReturn = new TextBlock()
            {
                Text = "Повернутися",
                FontSize = 24,
                Width = ReturnBorder.Width,
                Height = ReturnBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(ReturnBorder, 210);
            Canvas.SetTop(ReturnBorder, 110);
            Canvas.SetTop(textReturn, 2);
            Return.Children.Add(textReturn);
            canvasSureBlock.Children.Add(ReturnBorder);


            // Canvas з Border для Play
            Border ContinueBorder = new Border
            {
                Width = 160,
                Height = 40,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Canvas Continue = new Canvas();
            ContinueBorder.Child = Continue;
            TextBlock textContinue = new TextBlock()
            {
                Text = "Продовжити",
                FontSize = 24,
                Width = ContinueBorder.Width,
                Height = ContinueBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(ContinueBorder, 30);
            Canvas.SetTop(ContinueBorder, 110);
            Canvas.SetTop(textContinue, 2);
            Continue.Children.Add(textContinue);
            canvasSureBlock.Children.Add(ContinueBorder);

            Return.MouseLeftButtonDown += Return_MouseLeftButtonDown;

            Continue.MouseLeftButtonDown += Continue_MouseLeftButtonDown;


        }

        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CreateCanvasForSureBackToMenu();
        }
        private void Return_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Hub(frame));
        }
        private void Continue_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            gameCanvas.Children.Remove(blurCanvas);
            gameCanvas.Children.Remove(shadowBorder);
            ToggleInteractivityForSureMenu(true);
        }
    }
}  