using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ChessAI.Models;
using ChessAI.Services;

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
            // Luetaan nykyinen pelitilanne käyttöliittymästä
            BoardState position = CreateBoardStateFromUI();

            int depth = 3;

            if (cmbDepth.SelectedItem is ComboBoxItem depthItem &&
                int.TryParse(depthItem.Content?.ToString(), out int selectedDepth))
            {
                depth = selectedDepth;
            }

            ChessSearchService searchService = new ChessSearchService();

            // Suoritetaan haku Alpha-Beta-karsintaa käyttäen
            SearchResult result = searchService.FindBestMove(
                position,
                depth,
                true);

            if (result.BestMoveData == null)
            {
                MessageBox.Show("Tekoäly ei löytänyt mahdollista siirtoa.");
                return;
            }

            // Näytetään tekoälyn valitsema siirto laudalla
            HighlightAiMove(result.BestMoveData);

            MessageBox.Show(
                $"Paras siirto: {result.BestMove}\n" +
                $"Arvio: {result.BestScore}\n" +
                $"Tutkitut asemat: {result.NodesVisited}");
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

        // Muodostetaan käyttöliittymän nappuloista pelitilanne tekoälylle
        private BoardState CreateBoardStateFromUI()
        {
            BoardState position = new BoardState();

            double squareSize = cnvBoard.Width / 8;

            foreach (FrameworkElement piece in cnvPieces.Children)
            {
                double left = Canvas.GetLeft(piece);
                double top = Canvas.GetTop(piece);

                if (double.IsNaN(left) || double.IsNaN(top))
                    continue;

                int col = (int)((left + piece.Width / 2) / squareSize);
                int row = (int)((top + piece.Height / 2) / squareSize);

                if (row < 0 || row >= 8 || col < 0 || col >= 8)
                    continue;

                char symbol = GetPieceSymbol(piece.Name);

                if (symbol != '.')
                {
                    position.Squares[row, col] = symbol;
                }
            }

            if (cmbActivePlayer.SelectedItem is ComboBoxItem playerItem)
            {
                string player =
                    playerItem.Content?.ToString() ?? "";

                position.WhiteToMove =
                    !player.Contains(
                        "musta",
                        StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                position.WhiteToMove = true;
            }

            return position;
        }

        // Muutetaan käyttöliittymän nappulan nimi tekoälyn käyttämäksi merkiksi
        private char GetPieceSymbol(string name)
        {
            bool white =
                name.StartsWith(
                    "white",
                    StringComparison.OrdinalIgnoreCase);

            char piece;

            if (name.Contains("Pawn", StringComparison.OrdinalIgnoreCase))
                piece = 'P';
            else if (name.Contains("Knight", StringComparison.OrdinalIgnoreCase))
                piece = 'N';
            else if (name.Contains("Bishop", StringComparison.OrdinalIgnoreCase))
                piece = 'B';
            else if (name.Contains("Rook", StringComparison.OrdinalIgnoreCase))
                piece = 'R';
            else if (name.Contains("Queen", StringComparison.OrdinalIgnoreCase))
                piece = 'Q';
            else if (name.Contains("King", StringComparison.OrdinalIgnoreCase))
                piece = 'K';
            else
                return '.';

            return white
                ? piece
                : char.ToLowerInvariant(piece);
        }

        // Korostetaan tekoälyn valitseman siirron lähtö- ja kohderuutu
        private void HighlightAiMove(Move move)
        {
            List<Rectangle> oldHighlights =
                cnvBoard.Children
                    .OfType<Rectangle>()
                    .Where(r => Equals(r.Tag, "AiHighlight"))
                    .ToList();

            foreach (Rectangle highlight in oldHighlights)
            {
                cnvBoard.Children.Remove(highlight);
            }

            double squareSize = cnvBoard.Width / 8;

            AddAiHighlight(
                move.FromRow,
                move.FromCol,
                squareSize,
                Brushes.DeepSkyBlue);

            AddAiHighlight(
                move.ToRow,
                move.ToCol,
                squareSize,
                Brushes.Gold);
        }

        // Korostetaan tekoälyn valitseman siirron ruutu laudalla
        private void AddAiHighlight(
             int row,
             int col,
             double squareSize,
             Brush color)
        {
            Rectangle highlight = new Rectangle
            {
                Width = squareSize,
                Height = squareSize,
                Stroke = color,
                StrokeThickness = 5,
                Fill = Brushes.Transparent,
                Tag = "AiHighlight"
            };

            // Asetetaan korostus oikean ruudun kohdalle
            Canvas.SetLeft(highlight, col * squareSize);
            Canvas.SetTop(highlight, row * squareSize);

            cnvBoard.Children.Add(highlight);
        }

    }

}