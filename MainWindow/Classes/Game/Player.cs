using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MainWindow
{
    internal class Player
    {
        public bool Hide { get; set; } 
        public string Name { get; set; }
        public HoverCards HoverCards { get; set; }

        public Player()
        {
            HoverCards = new HoverCards();
        }
        public Player(string name, HoverCards hoverCards)
        {
            Name = name;
            HoverCards = hoverCards;
        }
    }
}
