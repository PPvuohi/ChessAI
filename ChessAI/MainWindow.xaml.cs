using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Think_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            SetupBoard();
            // TODO
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void Depth_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }
        private void cmbDepth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // piirrä taustaruudukko
            int squareSize = (int)cnvBoard.Width / 8;
            Brush? lightSquareBrush = (Brush?)Application.Current.FindResource("LightSquareBrush");
            Brush? darkSquareBrush = (Brush?)Application.Current.FindResource("DarkSquareBrush");
            if (lightSquareBrush == null || darkSquareBrush == null)
            {
                MessageBox.Show("Taustaväriä LightSquareBrush ja/tai DarkSquareBrush ei ole määritelty.");
                return;
            }
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Rectangle square = new Rectangle
                    {
                        Width = squareSize,
                        Height = squareSize,
                        Fill = (row + col) % 2 == 0 ? lightSquareBrush : darkSquareBrush
                    };
                    Canvas.SetLeft(square, col * squareSize);
                    Canvas.SetTop(square, row * squareSize);
                    cnvBoard.Children.Add(square);
                }
            }
        }
        private void FromTrayToBoard(Canvas tray)
        {
            List<FrameworkElement> pieces = [.. tray.Children.Cast<FrameworkElement>()];
            foreach (FrameworkElement piece in pieces)
            {
                tray.Children.Remove(piece);
                cnvPieces.Children.Add(piece);
            }
        }
        private void SetupBoard()
        {
            FromTrayToBoard(cnvWhitePieces);
            FromTrayToBoard(cnvBlackPieces);
            // asettele nappulat aloituspaikoilleen
            int squareSize = (int)cnvBoard.Width / 8;
            double squareCenter = squareSize / 2.0;
            // keksitään tähän parempi rutiini myöhemmin
            for (int col = 1; col <= 8; col++)
            {
                // valkoiset sotilaat
                PlacePiece($"whitePawn{col}", 2, col);
                // mustat sotilaat
                PlacePiece($"blackPawn{col}", 7, col);
            }
            PlacePiece("whiteRook1", 1, 1);
            PlacePiece("whiteKnight1", 1, 2);
            PlacePiece("whiteBishop1", 1, 3);
            PlacePiece("whiteQueen", 1, 4);
            PlacePiece("whiteKing", 1, 5);
            PlacePiece("whiteBishop2", 1, 6);
            PlacePiece("whiteKnight2", 1, 7);
            PlacePiece("whiteRook2", 1, 8);
            PlacePiece("blackRook1", 8, 1);
            PlacePiece("blackKnight1", 8, 2);
            PlacePiece("blackBishop1", 8, 3);
            PlacePiece("blackQueen", 8, 4);
            PlacePiece("blackKing", 8, 5);
            PlacePiece("blackBishop2", 8, 6);
            PlacePiece("blackKnight2", 8, 7);
            PlacePiece("blackRook2", 8, 8);
        }
        private void PlacePiece(string pieceName, int row, int col)
        {
            double boardWidth = cnvBoard.Width;
            double squareSize = boardWidth / 8;
            double halfSquare = squareSize / 2.0;
            FrameworkElement? piece = FindPieceByName(pieceName, cnvPieces);
            if (piece == null)
                return;
            double halfWidth = piece.Width / 2.0;
            double halfHeight = piece.Height / 2.0;
            // oletetaan että rivi 1 on alareunassa
            Canvas.SetLeft(piece, (col - 1) * squareSize + (halfSquare - halfWidth));
            Canvas.SetTop(piece, boardWidth - row * squareSize + (halfSquare - halfHeight));
            piece.Visibility = Visibility.Visible;
        }
        private FrameworkElement? FindPieceByName(string name, Canvas container)
        {
            foreach (FrameworkElement piece in container.Children)
                if (piece.Name == name)
                    return piece;
            return null;
        }
        private void ClearBoard_Click(object sender, RoutedEventArgs e)
        {
            List<FrameworkElement> toRemove = [.. cnvPieces.Children.Cast<FrameworkElement>()];
            foreach (FrameworkElement piece in toRemove)
            {
                cnvPieces.Children.Remove(piece);
                if (piece.Name.StartsWith("white"))
                {
                    AddToTray(cnvWhitePieces, piece);
                }
                else if (piece.Name.StartsWith("black"))
                {
                    AddToTray(cnvBlackPieces, piece);
                }
            }
        }
        private void AddToTray(Canvas tray, FrameworkElement piece)
        {
            if (tray.Children.Contains(piece))
                return;
            tray.Children.Add(piece);
            int fillCount = tray.Children.Count;
            double trayWidth = tray.Width;
            double squareSize = trayWidth / 2;
            double halfSquare = squareSize / 2.0;
            double halfWidth = piece.Width / 2.0;
            double halfHeight = piece.Height / 2.0;
            int col = fillCount > 8 ? 0 : 1;
            Canvas.SetLeft(piece, col * squareSize + (halfSquare - halfWidth));
            Canvas.SetTop(piece, (fillCount % 8) * squareSize + (halfSquare - halfWidth));
            piece.Visibility = Visibility.Visible;
        }

        private void cmbActivePlayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}