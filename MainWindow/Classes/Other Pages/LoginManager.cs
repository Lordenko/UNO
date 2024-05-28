using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Shapes;

namespace MainWindow
{
    internal class LoginManager
    {
        public Canvas CanvasLogin { get; set; }
        private Frame frame;
        private string PathToBackground = "pack://application:,,,/Data/gai109.jpg";
        private SQLiteConnection connection = new SQLiteConnection("Data Source=UnoAccount.db;Version=3;");
        private Ellipse redCircleNickName;
        private Ellipse redCirclePassword;
        private TextBox NickNameInput;
        private TextBox PasswordInput;

        public LoginManager(Canvas canvasHub, Frame frame)
        {
            this.CanvasLogin = canvasHub;
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
            CanvasLogin.Children.Add(backgroundImage);
            Canvas.SetLeft(backgroundImage, -10);
            Canvas.SetTop(backgroundImage, -19);

            // Полотно в центрі екрану з прозорим фоном
            Border mainLoginBackground = new Border
            {
                Width = 400,
                Height = 400,
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
            Canvas.SetTop(mainLoginBackground, 200);
            Canvas.SetLeft(mainLoginBackground, 560);
            CanvasLogin.Children.Add(mainLoginBackground);
            Canvas MainLogin = new Canvas();
            Canvas.SetTop(MainLogin, 200);
            Canvas.SetLeft(MainLogin, 560);
            CanvasLogin.Children.Add(MainLogin);

            Canvas canvasTextLogin = new Canvas()
            {
                Width = 100
            };
            TextBlock textLogin = new TextBlock()
            {
                Text = "Вхід",
                FontSize = 42,
                Width = 100,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            Canvas.SetLeft(canvasTextLogin, (mainLoginBackground.Width - canvasTextLogin.Width) / 2);
            Canvas.SetTop(canvasTextLogin, 20);
            canvasTextLogin.Children.Add(textLogin);
            MainLogin.Children.Add(canvasTextLogin);

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
            Canvas.SetLeft(exitBorder, (mainLoginBackground.Width - exitBorder.Width) - 10);
            Canvas.SetTop(exitBorder, 10);
            Canvas.SetTop(textExit, 1);
            Exit.Children.Add(textExit);
            MainLogin.Children.Add(exitBorder);

            Exit.MouseLeftButtonDown += Exit_MouseLeftButtonDown;



            // -- NickName --
            Canvas NickName = new Canvas();
            TextBlock NickNameText = new TextBlock()
            {
                Text = "Ім'я",
                FontSize = 20,
                Width = 150,
                Height = 50,
            };
            NickNameInput = new TextBox()
            {
                Width = 300,
                Height = 35,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center
            };
            redCircleNickName = new Ellipse
            {
                Width = 20,
                Height = 20,
                Visibility = Visibility.Collapsed,
                Fill = new SolidColorBrush(Colors.Red),
            };
            Canvas.SetTop(redCircleNickName, 35);
            Canvas.SetLeft(redCircleNickName, -25);
            NickName.Children.Add(redCircleNickName);
            // Створюємо ControlTemplate для заокруглення кутів
            ControlTemplate template = new ControlTemplate(typeof(TextBox));
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            border.SetValue(Border.PaddingProperty, new Thickness(5, 3, 5, 0)); // Встановлюємо Padding справа та зверху

            FrameworkElementFactory scrollViewer = new FrameworkElementFactory(typeof(ScrollViewer));
            scrollViewer.Name = "PART_ContentHost";
            border.AppendChild(scrollViewer);
            template.VisualTree = border;

            // Застосовуємо шаблон до TextBox
            NickNameInput.Template = template;

            Canvas.SetTop(NickNameInput, 30);
            Canvas.SetLeft(NickNameInput, 7);
            Canvas.SetTop(NickName, 120);
            Canvas.SetLeft(NickName, 45);
            Canvas.SetLeft(NickNameText, 15);
            NickName.Children.Add(NickNameText);
            NickName.Children.Add(NickNameInput);
            MainLogin.Children.Add(NickName);


            // -- Password --
            Canvas Password = new Canvas();
            TextBlock PasswordText = new TextBlock()
            {
                Text = "Пароль",
                FontSize = 20,
                Width = 150,
                Height = 50,
            };
            PasswordInput = new TextBox()
            {
                Width = 300,
                Height = 35,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center

            };
            redCirclePassword = new Ellipse
            {
                Width = 20,
                Height = 20,
                Visibility = Visibility.Collapsed,
                Fill = new SolidColorBrush(Colors.Red),
            };
            Canvas.SetLeft(redCirclePassword, -25);
            Canvas.SetTop(redCirclePassword, 35);
            Password.Children.Add(redCirclePassword);

            // Застосовуємо шаблон до TextBox
            PasswordInput.Template = template;

            Canvas.SetTop(PasswordInput, 30);
            Canvas.SetLeft(PasswordInput, 7);
            Canvas.SetLeft(PasswordText, 15);
            Canvas.SetTop(Password, 200);
            Canvas.SetLeft(Password, 45);
            Password.Children.Add(PasswordText);
            Password.Children.Add(PasswordInput);
            MainLogin.Children.Add(Password);


            Border loginButtonBorder = new Border
            {
                Width = 120,
                Height = 50,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Canvas LoginButton = new Canvas();
            loginButtonBorder.Child = LoginButton;
            TextBlock textLoginButton = new TextBlock()
            {
                Text = "Увійти",
                FontSize = 28,
                Width = loginButtonBorder.Width,
                Height = loginButtonBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(loginButtonBorder, (mainLoginBackground.Width - loginButtonBorder.Width) / 2);
            Canvas.SetTop(loginButtonBorder, 300);
            Canvas.SetTop(textLoginButton, 4);
            LoginButton.Children.Add(textLoginButton);
            MainLogin.Children.Add(loginButtonBorder);


            LoginButton.MouseLeftButtonDown += Login_MouseLeftButtonDown;
        }

        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Hub(frame));
        }
        private void Login_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            redCircleNickName.Visibility = Visibility.Collapsed;
            redCirclePassword.Visibility = Visibility.Collapsed;


            if (NickNameInput.Text == "" || PasswordInput.Text == "")
            {
                redCircleNickName.Visibility = Visibility.Visible;
                redCirclePassword.Visibility = Visibility.Visible;
            }
            else
            {
                connection.Open();
                string storedPassword = GetUserPassword(NickNameInput.Text);
                connection.Close();

                if (storedPassword == null)
                {
                    redCircleNickName.Visibility = Visibility.Visible;
                    redCirclePassword.Visibility = Visibility.Visible;
                    Debug.WriteLine("User not found.");
                }
                else if (storedPassword == PasswordInput.Text)
                {
                    Debug.WriteLine("Login successful.");
                    Debug.WriteLine($"NickName = {NickNameInput.Text}");
                    UserSession.NickName = NickNameInput.Text;
                    frame.Navigate(new Hub(frame));
                }
                else
                {
                    redCirclePassword.Visibility = Visibility.Visible;
                    Debug.WriteLine("Invalid password.");
                }
            }
        }

        private string GetUserPassword(string nickName)
        {
            string password = null;
            string selectQuery = "SELECT Password FROM Users WHERE NickName = @NickName";
            using (SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, connection))
            {
                selectCmd.Parameters.AddWithValue("@NickName", nickName);
                using (SQLiteDataReader reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        password = reader["Password"].ToString();
                    }
                }
            }
            return password;
        }

       
    }
}
