using MainWindow;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;

internal class PlayerManager
{
    public List<Player> Players { get; private set; }
    public UiManager UiManager { get; private set; }
    public Player CurrentPlayer => Players[Turn];

    public int Turn { get; private set; }
    public bool Clockwise { get; private set; } = true;

    public PlayerManager(UiManager uiManager, params Player[] players)
    {
        UiManager = uiManager;
        Players = new List<Player>(players);
        Turn = 0;
    }

    public void ChangePlayer()
    {
        Debug.WriteLine("PlayerChanged");
        if (Clockwise)
            Turn = (Turn + 1) % Players.Count;
        else
            Turn = (Turn - 1 + Players.Count) % Players.Count;
    }

    public void ToggleClockwise()
    {
        Clockwise = !Clockwise;
    }

    public int CheckCorrectTurn(int turn)
    {
        if (turn >= Players.Count) return 0;
        if (turn < 0) return Players.Count - 1;
        return turn;
    }

    public void ToggleInteractivityForPlayers()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            var player = Players[i];
            bool isEnabled = player == CurrentPlayer;
            foreach (var card in player.HoverCards.Cards)
            {
                if (card.Image != null)
                {
                    card.Image.IsEnabled = isEnabled;
                    if (isEnabled)
                    {
                        card.Image.Source = card.SourseCardImage;
                        UiManager.ChangeColorBorder(UiManager.HelpBorderForCanvas[i], Colors.LightGray);
                        UiManager.ChangeOpacityBorder(UiManager.HelpBorderForCanvas[i], 1);
                    }
                    else
                    {
                        card.Image.Source = new BitmapImage(new Uri(UiManager.PathForUnoDeckCardImage, UriKind.RelativeOrAbsolute));
                        UiManager.ChangeColorBorder(UiManager.HelpBorderForCanvas[i], Colors.Gray);
                        UiManager.ChangeOpacityBorder(UiManager.HelpBorderForCanvas[i], 0.7);
                    }
                }
            }
        }
    }
}