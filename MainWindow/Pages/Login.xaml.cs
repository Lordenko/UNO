using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainWindow
{
    public partial class Login : Page
    {
        private Frame mainFrame;

        public Login(Frame frame)
        {
            InitializeComponent();
            mainFrame = frame;
            LoginManager loginManager = new LoginManager(CanvasLogin, mainFrame);
        }
    }
}
