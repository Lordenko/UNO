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
using MainWindow.Classes;
using System.Data.SQLite;
using System.Diagnostics;

namespace MainWindow
{
    internal class SettingsManager
    {
        public Canvas CanvasSettings { get; set; }
        private Frame frame;

        private string PathToBackground = "pack://application:,,,/Data/gai109.jpg";
        private Canvas MainSettings;
        private Canvas SettingsName;
        private Canvas SettingsSectionName;
        private Canvas SettingsSectionOption;
        private Canvas Apply;
        private Canvas Exit;
        private Slider volumeSlider;
        private SQLiteConnection connection = new SQLiteConnection("Data Source=UnoAccount.db;Version=3;");



        public SettingsManager(Canvas canvasHub, Frame frame)
        { 
            this.CanvasSettings = canvasHub;
            this.frame = frame;
            Initialise();
        }
        private void Initialise()
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
            CanvasSettings.Children.Add(backgroundImage);
            Canvas.SetLeft(backgroundImage, -10);
            Canvas.SetTop(backgroundImage, -19);

            // Полотно в центрі екрану з прозорим фоном
            Border mainSettingsBackground = new Border
            {
                Width = 600,
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
            Canvas.SetTop(mainSettingsBackground, 250);
            Canvas.SetLeft(mainSettingsBackground, 450);
            CanvasSettings.Children.Add(mainSettingsBackground);

            MainSettings = new Canvas();
            Canvas.SetTop(MainSettings, 250);
            Canvas.SetLeft(MainSettings, 450);
            CanvasSettings.Children.Add(MainSettings);

            TextBlock TextSettingsName = new TextBlock()
            {
                Text = "Налаштування",
                FontSize = 42,
                Width = 300,
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            SettingsName = new Canvas()
            {
                Width = 200,
                Height = 60,
            };
            Canvas.SetLeft(SettingsName, (mainSettingsBackground.Width - SettingsName.Width) / 2 - 35);
            Canvas.SetTop(SettingsName, 10);
            MainSettings.Children.Add(SettingsName);
            SettingsName.Children.Add(TextSettingsName);

            TextBlock SettingsSectionName1 = new TextBlock()
            {
                Text = "Гучність звуку",
                FontSize = 24,
                Width = 240,
                Foreground = new SolidColorBrush(Colors.Black)
            };

            SettingsSectionName = new Canvas()
            {
                Width = 250,
                Height = 270,
            };
            Canvas.SetTop(SettingsSectionName, 80);
            Canvas.SetLeft(SettingsSectionName, (mainSettingsBackground.Width - SettingsSectionName.Width) / 2 - SettingsSectionName.Width / 2 - 10);
            MainSettings.Children.Add(SettingsSectionName);
            SettingsSectionName.Children.Add(SettingsSectionName1);


            volumeSlider = new Slider
            {
                Width = 240,
                Height = 24,
                Minimum = 0,
                Maximum = 100,
                Value = GetUserVolume(UserSession.NickName),
                Margin = new Thickness(0, 10, 0, 10),
                Focusable = false,
            };
            Canvas.SetLeft(volumeSlider, -110);

            

            SettingsSectionOption = new Canvas()
            {
                Width = 250,
                Height = 270,
                //Background = new SolidColorBrush(Colors.White)
            };
            Canvas.SetTop(SettingsSectionOption, 80);
            Canvas.SetLeft(SettingsSectionOption, (mainSettingsBackground.Width - SettingsSectionOption.Width) / 2 + SettingsSectionOption.Width / 2 + 20);
            SettingsSectionOption.Children.Add(volumeSlider);
            MainSettings.Children.Add(SettingsSectionOption);

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




            Border applyBorder = new Border
            {
                Width = 200,
                Height = 40,
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Style = buttonStyle
            };
            Apply = new Canvas();
            applyBorder.Child = Apply;
            TextBlock textApply = new TextBlock()
            {
                Text = "Застосувати",
                FontSize = 24,
                Width = applyBorder.Width,
                Height = applyBorder.Height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(applyBorder, (mainSettingsBackground.Width - applyBorder.Width) / 2);
            Canvas.SetTop(applyBorder, 330);
            Canvas.SetTop(textApply, 1);
            Apply.Children.Add(textApply);
            MainSettings.Children.Add(applyBorder);     
            Border exitBorder = new Border
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
            Canvas.SetLeft(exitBorder, (mainSettingsBackground.Width - exitBorder.Width) - 10);
            Canvas.SetTop(exitBorder, 10);
            Canvas.SetTop(textExit, 1);
            Exit.Children.Add(textExit);
            MainSettings.Children.Add(exitBorder);

            Exit.MouseLeftButtonDown += Exit_MouseLeftButtonDown;
            Apply.MouseLeftButtonDown += Apply_MouseLeftButtonDown;

        }

        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frame.Navigate(new Hub(frame));
        }
        private void Apply_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetUserVolume(UserSession.NickName, (int)volumeSlider.Value);
            Debug.WriteLine($"Value = {(int)volumeSlider.Value}");
            MusicPlayer.Instance.SetVolume(volumeSlider.Value);
            frame.Navigate(new Hub(frame));
        }

        private void SetUserVolume(string nickName, int volume)
        {
            string updateQuery = "UPDATE Users SET Volume = @Volume WHERE NickName = @NickName";
            using (connection)
            {
                using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, connection))
                {
                    updateCmd.Parameters.AddWithValue("@Volume", volume);
                    updateCmd.Parameters.AddWithValue("@NickName", nickName);

                    connection.Open();
                    updateCmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        private int GetUserVolume(string nickName)
        {
            int volume = -1; // Значення за замовчуванням, якщо користувача не знайдено
            string selectQuery = "SELECT Volume FROM Users WHERE NickName = @NickName";
            using (SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, connection))
            {
                selectCmd.Parameters.AddWithValue("@NickName", nickName);
                connection.Open();
                using (SQLiteDataReader reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        volume = Convert.ToInt32(reader["Volume"]);
                    }
                }
                connection.Close();

            }
            return volume;
        }


    }
}
