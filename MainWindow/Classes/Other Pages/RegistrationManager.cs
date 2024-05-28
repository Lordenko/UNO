using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Data.SQLite;

namespace MainWindow
{
    internal class RegistrationManager
    {
        public Canvas CanvasRegistration { get; set; }
        private Frame frame;
        private string PathToBackground = "pack://application:,,,/Data/gai109.jpg";
        private Ellipse redCircleNickName;
        private Ellipse redCirclePassword;
        private Ellipse redCircleRepeatPassword;
        private TextBlock NickNameText;
        private TextBlock PasswordText;
        private TextBlock RepeatPasswordText;
        private TextBox NickNameInput;
        private TextBox PasswordInput;
        private TextBox RepeatPasswordInput;
        private SQLiteConnection connection = new SQLiteConnection("Data Source=UnoAccount.db;Version=3;");

        public RegistrationManager(Canvas canvasHub, Frame frame)
        {
            this.CanvasRegistration = canvasHub;
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
            CanvasRegistration.Children.Add(backgroundImage);
            Canvas.SetLeft(backgroundImage, -10);
            Canvas.SetTop(backgroundImage, -19);

            // Полотно в центрі екрану з прозорим фоном
            Border mainRegistrationBackground = new Border
            {
                Width = 400,
                Height = 460,
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
            Canvas.SetTop(mainRegistrationBackground, 200);
            Canvas.SetLeft(mainRegistrationBackground, 560);
            CanvasRegistration.Children.Add(mainRegistrationBackground);
            Canvas MainRegistration = new Canvas();
            Canvas.SetTop(MainRegistration, 200);
            Canvas.SetLeft(MainRegistration, 560);
            CanvasRegistration.Children.Add(MainRegistration);

            Canvas canvasTextRegistration = new Canvas()
            {
                Width = 220
            };
            TextBlock textLogin = new TextBlock()
            {
                Text = "Реєстрація",
                FontSize = 42,
                Width = 220,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            Canvas.SetLeft(canvasTextRegistration, (mainRegistrationBackground.Width - canvasTextRegistration.Width) / 2);
            Canvas.SetTop(canvasTextRegistration, 20);
            canvasTextRegistration.Children.Add(textLogin);
            MainRegistration.Children.Add(canvasTextRegistration);

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
            Canvas.SetLeft(exitBorder, (mainRegistrationBackground.Width - exitBorder.Width) - 10);
            Canvas.SetTop(exitBorder, 10);
            Canvas.SetTop(textExit, 1);
            Exit.Children.Add(textExit);
            MainRegistration.Children.Add(exitBorder);

            Exit.MouseLeftButtonDown += Exit_MouseLeftButtonDown;

            // -- NickName --
            Canvas NickName = new Canvas();
            NickNameText = new TextBlock()
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

            redCircleNickName = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Red),
                Visibility = Visibility.Collapsed,
            };

            Canvas.SetTop(NickNameInput, 30);
            Canvas.SetLeft(NickNameInput, 5);
            Canvas.SetTop(redCircleNickName, 40);
            Canvas.SetLeft(redCircleNickName, -25);
            Canvas.SetTop(NickName, 120);
            Canvas.SetLeft(NickName, 45);
            Canvas.SetLeft(NickNameText, 15);
            NickName.Children.Add(NickNameText);
            NickName.Children.Add(NickNameInput);
            NickName.Children.Add(redCircleNickName);
            MainRegistration.Children.Add(NickName);

            // -- Password --
            Canvas Password = new Canvas();
            PasswordText = new TextBlock()
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

            // Застосовуємо шаблон до TextBox
            PasswordInput.Template = template;

            redCirclePassword = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Red),
                Visibility = Visibility.Collapsed,
            };

            Canvas.SetTop(PasswordInput, 30);
            Canvas.SetLeft(PasswordInput, 5);
            Canvas.SetTop(redCirclePassword, 40);
            Canvas.SetLeft(redCirclePassword, -25);
            Canvas.SetLeft(PasswordText, 15);
            Canvas.SetTop(Password, 200);
            Canvas.SetLeft(Password, 45);
            Password.Children.Add(PasswordText);
            Password.Children.Add(PasswordInput);
            Password.Children.Add(redCirclePassword);
            MainRegistration.Children.Add(Password);

            // -- Repeat Password --
            Canvas RepeatPassword = new Canvas();
            RepeatPasswordText = new TextBlock()
            {
                Text = "Повторіть пароль",
                FontSize = 20,
                Width = 250,
                Height = 50,
            };
            RepeatPasswordInput = new TextBox()
            {
                Width = 300,
                Height = 35,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Застосовуємо шаблон до TextBox
            RepeatPasswordInput.Template = template;

            redCircleRepeatPassword = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Red),
                Visibility = Visibility.Collapsed,
            };

            Canvas.SetTop(RepeatPasswordInput, 30);
            Canvas.SetLeft(RepeatPasswordInput, 5);
            Canvas.SetTop(redCircleRepeatPassword, 37);
            Canvas.SetLeft(redCircleRepeatPassword, -25);
            Canvas.SetLeft(RepeatPasswordText, 15);
            Canvas.SetTop(RepeatPassword, 280);
            Canvas.SetLeft(RepeatPassword, 45);
            RepeatPassword.Children.Add(RepeatPasswordText);
            RepeatPassword.Children.Add(RepeatPasswordInput);
            RepeatPassword.Children.Add(redCircleRepeatPassword);
            MainRegistration.Children.Add(RepeatPassword);

            Border SignUpBorder = new Border
            {
                Width = 40,
                Height = 40,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Canvas SingUpCanvas = new Canvas();
            SignUpBorder.Child = SingUpCanvas;
            TextBlock SignUpTextBlock = new TextBlock()
            {
                Text = "x",
                FontSize = 24,
                Width = SignUpBorder.Width,
                Height = SignUpBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(SignUpBorder, (mainRegistrationBackground.Width - SignUpBorder.Width) - 10);
            Canvas.SetTop(SignUpBorder, 10);
            Canvas.SetTop(SignUpTextBlock, 1);
            SingUpCanvas.Children.Add(SignUpTextBlock);
            MainRegistration.Children.Add(SignUpBorder);

            SingUpCanvas.MouseLeftButtonDown += Exit_MouseLeftButtonDown;

            Border regButtonBorder = new Border
            {
                Width = 250,
                Height = 50,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Canvas RegButton = new Canvas();
            regButtonBorder.Child = RegButton;
            TextBlock textRegButton = new TextBlock()
            {
                Text = "Зареєструватися",
                FontSize = 28,
                Width = regButtonBorder.Width,
                Height = regButtonBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(regButtonBorder, (mainRegistrationBackground.Width - regButtonBorder.Width) / 2);
            Canvas.SetTop(regButtonBorder, 380);
            Canvas.SetTop(textRegButton, 4);
            RegButton.Children.Add(textRegButton);
            MainRegistration.Children.Add(regButtonBorder);


            RegButton.MouseLeftButtonDown += LoginButton_MouseLeftButtonDown;
        }

        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Hub(frame));
        }

        private void LoginButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            redCirclePassword.Visibility = Visibility.Collapsed;
            redCircleRepeatPassword.Visibility = Visibility.Collapsed;
            redCircleNickName.Visibility = Visibility.Collapsed;

            bool dostup = true;

            if (PasswordInput.Text != RepeatPasswordInput.Text || PasswordInput.Text == "" || RepeatPasswordInput.Text == "")
            {
                redCirclePassword.Visibility = Visibility.Visible;
                redCircleRepeatPassword.Visibility = Visibility.Visible;
                dostup = false;
            }

            connection.Open();
            if (GetAllNames(connection).Contains(NickNameInput.Text) || NickNameInput.Text == "")
            {
                redCircleNickName.Visibility = Visibility.Visible;
                dostup = false;
            }

            if (dostup)
            {
                AddUser(connection, NickNameInput.Text, PasswordInput.Text, 0, 100);
                frame.Navigate(new Hub(frame));
            }
            connection.Close();
        }

        private void AddUser(SQLiteConnection connection, string nickName, string password, int score, int volume)
        {
            string insertQuery = "INSERT INTO Users (NickName, Password, Volume) VALUES (@NickName, @Password, @Volume)";
            using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, connection))
            {
                insertCmd.Parameters.AddWithValue("@NickName", nickName);
                insertCmd.Parameters.AddWithValue("@Password", password);
                insertCmd.Parameters.AddWithValue("@Score", score);
                insertCmd.Parameters.AddWithValue("@Volume", volume);
                insertCmd.ExecuteNonQuery();
            }
        }
        private List<string> GetAllNames(SQLiteConnection connection)
        {
            List<string> names = new List<string>();
            string selectQuery = "SELECT NickName FROM Users";
            using (SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, connection))
            using (SQLiteDataReader reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    names.Add(reader["NickName"].ToString());
                }
            }
            return names;
        
        }
    }
}
