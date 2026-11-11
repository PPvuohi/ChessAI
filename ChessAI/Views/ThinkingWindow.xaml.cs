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

namespace ChessAI.Views
{
    /// <summary>
    /// Interaction logic for ThinkingWindow.xaml
    /// </summary>
    public partial class ThinkingWindow : Window
    {
        private MainWindow mainWindow;
        public ThinkingWindow(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
        }
    }
}
