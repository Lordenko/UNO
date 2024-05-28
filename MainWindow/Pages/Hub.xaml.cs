using MainWindow.Classes;
using System.Windows.Controls;

namespace MainWindow
{
    public partial class Hub : Page
    {
        private Frame mainFrame;
        public Hub(Frame frame)
        {
            InitializeComponent();
            mainFrame = frame;
            HubManager hubManager = new HubManager(CanvasHub, mainFrame);
        }
    }
}
