using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MainWindow.Classes
{
    internal class HubManager
    {
        public Canvas CanvasHub { get; set; }
        private Frame frame;
        
        private Canvas LoginAndReg;
        private Canvas Play;
        private Canvas Settings;
        private Canvas MainMenu;
        private string PathToImage = "pack://application:,,,/Data/UnoLogo.png";
        private string PathToBackground = "pack://application:,,,/Data/gai109.jpg";

        public HubManager(Canvas canvasHub, Frame frame)
        {
            this.CanvasHub = canvasHub;
            this.frame = frame;
            InitialiseHub();
        }

        private void InitialiseHub()
        {

            Image backgroundImage = new Image()
            {
                Width = 1600,
                Height = 900,
                Source = new BitmapImage(new Uri(PathToBackground)),
                Stretch = Stretch.UniformToFill,
            };
            BlurEffect blurEffect = new BlurEffect { Radius = 15 };
            backgroundImage.Effect = blurEffect;
            CanvasHub.Children.Add(backgroundImage);
            Canvas.SetLeft(backgroundImage, -10);
            Canvas.SetTop(backgroundImage, -19);

            // Полотно в центрі екрану з прозорим фоном
            Border mainMenuBackground = new Border
            {
                Width = 400,
                Height = 600,
                Background = new SolidColorBrush(Colors.Gray) { Opacity = 0.9 },
                CornerRadius = new CornerRadius(20),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 0,
                    Opacity = 0.5
                }
            };
            Canvas.SetTop(mainMenuBackground, 140);
            Canvas.SetLeft(mainMenuBackground, 560);
            CanvasHub.Children.Add(mainMenuBackground);

            MainMenu = new Canvas();
            Canvas.SetTop(MainMenu, 140);
            Canvas.SetLeft(MainMenu, 560);
            CanvasHub.Children.Add(MainMenu);

            // Картинка зверху
            Image imageUno = new Image()
            {
                Source = new BitmapImage(new Uri(PathToImage, UriKind.Absolute)),
                Width = 300,
                Opacity = 1.0 // Встановлюємо непрозорість
            };
            Canvas.SetLeft(imageUno, (mainMenuBackground.Width - imageUno.Width) / 2);
            Canvas.SetTop(imageUno, -20);
            MainMenu.Children.Add(imageUno);

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
            Border playBorder = new Border
            {
                Width = 220,
                Height = 70,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Play = new Canvas();
            playBorder.Child = Play;
            TextBlock textPlay = new TextBlock()
            {
                Text = "Грати",
                FontSize = 24,
                Width = playBorder.Width,
                Height = playBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(playBorder, (mainMenuBackground.Width - playBorder.Width) / 2);
            Canvas.SetTop(playBorder, 290);
            Canvas.SetTop(textPlay, 15);
            Play.Children.Add(textPlay);
            MainMenu.Children.Add(playBorder);

            // Canvas з Border для Settings
            Border settingsBorder = new Border
            {
                Width = 220,
                Height = 70,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Settings = new Canvas();
            settingsBorder.Child = Settings;
            TextBlock textSettings = new TextBlock()
            {
                Text = "Налаштування",
                FontSize = 24,
                Width = settingsBorder.Width,
                Height = settingsBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(settingsBorder, (mainMenuBackground.Width - settingsBorder.Width) / 2);
            Canvas.SetTop(settingsBorder, 370);
            Canvas.SetTop(textSettings, 15);
            Settings.Children.Add(textSettings);
            MainMenu.Children.Add(settingsBorder);

            if (UserSession.NickName == null)
            {
                // Canvas для Login / Registration
                LoginAndReg = new Canvas()
                {
                    Width = 200,
                    Height = 50,
                    Background = new SolidColorBrush(Colors.Transparent)
                };

                // Стиль для тексту з зміною при наведенні
                Style textStyle = new Style(typeof(TextBlock));
                textStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Black)));
                Trigger textMouseOverTrigger = new Trigger
                {
                    Property = UIElement.IsMouseOverProperty,
                    Value = true
                };
                textMouseOverTrigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.White)));
                textStyle.Triggers.Add(textMouseOverTrigger);

                TextBlock loginText = new TextBlock()
                {
                    Text = "Увійти",
                    FontSize = 16,
                    Width = 60,
                    Height = 30,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Style = textStyle
                };
                Canvas.SetLeft(loginText, 0);
                Canvas.SetTop(loginText, 10);

                TextBlock decorText = new TextBlock()
                {
                    Text = "/",
                    FontSize = 16,
                    Width = 20,
                    Height = 30,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                Canvas.SetLeft(decorText, 55);
                Canvas.SetTop(decorText, 10);

                TextBlock registrationText = new TextBlock()
                {
                    Text = "Зареєструватися",
                    FontSize = 16,
                    Width = 130,
                    Height = 30,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Style = textStyle
                };
                Canvas.SetLeft(registrationText, 70);
                Canvas.SetTop(registrationText, 10);

                LoginAndReg.Children.Add(loginText);
                LoginAndReg.Children.Add(decorText);
                LoginAndReg.Children.Add(registrationText);

                Canvas.SetLeft(LoginAndReg, (mainMenuBackground.Width - LoginAndReg.Width) / 2 + 3);
                Canvas.SetTop(LoginAndReg, 440);
                MainMenu.Children.Add(LoginAndReg);

                loginText.MouseLeftButtonDown += Login_MouseLeftButtonDown;
                registrationText.MouseLeftButtonDown += Registation_MouseLeftButtonDown;
            }
            else
            {
                TextBlock userNameText = new TextBlock()
                {
                    Text = "Вітаємо, " + UserSession.NickName,
                    FontSize = 18,
                    Width = 300,
                    Foreground = new SolidColorBrush(Colors.White),
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Canvas.SetLeft(userNameText, (mainMenuBackground.Width - userNameText.Width) / 2);
                Canvas.SetTop(userNameText, 250);
                MainMenu.Children.Add(userNameText);
            }

            Border exitBorder = new Border
            {
                Width = 40,
                Height = 40,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Canvas Exit = new Canvas();
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
            Canvas.SetLeft(exitBorder, (mainMenuBackground.Width - exitBorder.Width) - 10);
            Canvas.SetTop(exitBorder, 10);
            Canvas.SetTop(textExit, 1);
            Exit.Children.Add(textExit);
            MainMenu.Children.Add(exitBorder);

            Exit.MouseLeftButtonDown += Exit_MouseLeftButtonDown;

            Play.MouseLeftButtonDown += Play_MouseLeftButtonDown;
            Settings.MouseLeftButtonDown += Settings_MouseLeftButtonDown;

        }
        private void Play_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Pages.Game(frame));
        }
        
        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void Settings_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Settings(frame));
        }
        
        private void Login_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Login(frame));
        }
        
        private void Registation_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Registration(frame));
        }


    }
}
