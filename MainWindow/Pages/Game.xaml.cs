using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MainWindow.Pages
{
    public partial class Game : Page
    {
        private Frame mainFrame;

        public Game(Frame frame)
        {
            InitializeComponent();
            mainFrame = frame;
            Player player1 = new Player();
            Player player2 = new Player();
            Player player3 = new Player();
            Player player4 = new Player();
            UiManager uiManager = new UiManager(mainFrame, CanvasGame, player1, player2, player3, player4, 0);

            CanvasGame.Width = 1920;
            CanvasGame.Height = 1080;

        }
    }
}
