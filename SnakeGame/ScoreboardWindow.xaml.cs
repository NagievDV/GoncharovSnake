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
using System.Windows.Shapes;

namespace SnakeGame
{

    public partial class ScoreboardWindow : Window
    {
        public ScoreboardWindow()
        {
            InitializeComponent();
            MainWindow.ChangeGameState();
            dGridScore.ItemsSource = Entities.GetContext().Scoreboard.ToList().OrderByDescending(x => x.Score).Take(10);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.ChangeGameState();
        }
    }
}
